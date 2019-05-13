using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteLogDigiRite
{
    // Sequencing for the case where a QSO consists of sending a grid square and then a signal
    // report, in that order.
    class Qso2MessageExchange : QueueCommon
    {
        public interface IQsoQueueCallBacks
        {
            string GetExchangeMessage(QsoInProgress q, bool addAck, ExchangeTypes exc);
            string GetAckMessage(QsoInProgress q, bool ofAnAck);
            void SendMessage(string toSend, QsoInProgress q);
            void LogQso(QsoInProgress q);
        };

        protected IQsoQueueCallBacks callbacks;

        public Qso2MessageExchange(QsosPanel qsosPanel, IQsoQueueCallBacks cb) : base(qsosPanel)
        {  callbacks = cb;   }

        public override void MessageForMycall(RecentMessage recentMessage, 
            bool directlyToMe, string callQsled, 
            short band, bool autoStart, IsConversationMessage onUsed)
        {
            XDpack77.Pack77Message.ReceivedMessage rm = recentMessage.Message;
            var inProgList = qsosPanel.QsosInProgress;
            QsoInProgress inProgress = null;
            bool isSameStation = false;
            bool used = false;
            foreach (QsoInProgress q in inProgList)
            {
                inProgress = q;
                used = q.AddMessageOnMatch(rm, directlyToMe,
                    callQsled, band, out isSameStation);
                if (isSameStation)
                    break;
            }
            if (used)
            {
                // we have an ongoing QSO for this message
                onUsed(Conversation.Origin.TO_ME);
                ((Qso2MessageSequencer)(inProgress.Sequencer)).OnReceived(directlyToMe, rm.Pack77Message);
            }  else if (autoStart && directlyToMe)
            {
                onUsed(Conversation.Origin.TO_ME);
                // wasn't one we already had. but we autostart with any call
                InitiateQso(recentMessage, band, false);
            } else if (isSameStation)
            {
                if ((null != inProgress.Sequencer) && !inProgress.Sequencer.IsFinished)
                    onUsed(Conversation.Origin.TO_OTHER); // make it show up in the conversation history
            }
        }

        // connect the Qso2MessageExchange with QsoInProgress on callbacks from the Qso2MessageSequencer
        class QsoSequencerImpl : Qso2MessageSequencer.IQsoSequencerCallbacks
        {
            public QsoSequencerImpl(Qso2MessageExchange queue, QsoInProgress q)
            { qsoQueue = queue; qso = q; }
            public void LogQso()
            { qsoQueue.LogQso(qso); }
            public void SendAck()
            { qsoQueue.SendAck(qso); }
            public void SendExchange(ExchangeTypes ext, bool withAck )
            { qsoQueue.SendExchange(qso, ext, withAck); }
            public override String ToString()
            { return qso.ToString(); }

            private Qso2MessageExchange qsoQueue;
            private QsoInProgress qso;
        }

        protected bool isMe(string s)
        { return String.Equals(s, myCall) || String.Equals(s, myBaseCall);  }

        protected override void StartQso(QsoInProgress q)
        {
            Qso2MessageSequencer qs = new Qso2MessageSequencer(new QsoSequencerImpl(this, q));
            q.Sequencer = qs;
            bool directlyToMe = false;
            XDpack77.Pack77Message.ToFromCall toFromCall = q.Message.Pack77Message as XDpack77.Pack77Message.ToFromCall;
            if (null != toFromCall)
                directlyToMe = isMe(toFromCall.ToCall);
            qs.OnReceived(directlyToMe, q.Message.Pack77Message);
        }

        public void SendExchange(QsoInProgress q, ExchangeTypes exc, bool withAck)
        {  callbacks.SendMessage(callbacks.GetExchangeMessage(q, withAck, exc), q);      }

        public void LogQso(QsoInProgress q)
        {
            callbacks.LogQso(q);
            var screenItems = qsosPanel.QsosInProgress;
            foreach (QsoInProgress qnext in screenItems)
            {
                if (qnext.Sequencer == null)
                {
                    StartQso(qnext);
                    return;
                }
            }
        }

        public void SendAck(QsoInProgress q)
        { callbacks.SendMessage(callbacks.GetAckMessage(q, false), q);  }
    }

    class Qso2MessageSequencer : IQsoSequencer
    {
        public interface IQsoSequencerCallbacks
        {
            void SendExchange(ExchangeTypes exc, bool withAck);
            void LogQso();
            void SendAck();
        }
        private bool haveGrid  = false;
        private bool haveReport  = false;
        private bool haveLogged = false;
        private bool amLeader = false;
        private bool amLeaderSet = false;
        private bool haveSentReport = false;
        private IQsoSequencerCallbacks cb;
        private delegate void ExchangeSent();
        private ExchangeSent lastSent;
        public Qso2MessageSequencer(IQsoSequencerCallbacks cb)
        { this.cb = cb;   }

        public bool IsFinished { get { return haveLogged; } }

        public string DisplayState {
            get {
                int v = 0;
                if (haveGrid)
                    v += 1;
                if (haveReport)
                    v += 2;
                if (haveLogged)
                    v += 4;
                return v.ToString();
            }
        }
        public delegate bool IsMe(string c);
        public void OnReceived(bool directlyToMe, XDpack77.Pack77Message.Message msg)
        {
            XDpack77.Pack77Message.Exchange exc = msg as XDpack77.Pack77Message.Exchange;
            if (!amLeaderSet)
                amLeader = directlyToMe;
            amLeaderSet = true;
            if (null != exc)
            {
                string gs = exc.GridSquare;
                int rp = exc.SignaldB;
                if (!String.IsNullOrEmpty(gs))
                {   // received a grid
                    haveGrid = true;
                    ExchangeSent es;
                    if (amLeader)
                    {
                        es = ()=> cb.SendExchange(ExchangeTypes.DB_REPORT, false); 
                        haveSentReport = true;
                    }
                    else
                        es = ()=>cb.SendExchange(ExchangeTypes.GRID_SQUARE, false);
                    lastSent = es;
                    es();
                    return;
                }
                else if (rp > XDpack77.Pack77Message.Message.NO_DB)
                {   // received a dB report
                    XDpack77.Pack77Message.Roger roger = msg as XDpack77.Pack77Message.Roger;
                    bool ack = (null != roger) && (roger.Roger);
                    haveReport = true;
                    lastSent = null;
                    if (amLeader && ack && haveSentReport)
                        cb.SendAck();
                    else
                    {
                        haveSentReport = true;
                        ExchangeSent es = ()=>cb.SendExchange(ExchangeTypes.DB_REPORT,true);
                        lastSent = es;
                        es();
                    }
                    return;
                }
            }
            XDpack77.Pack77Message.QSL qsl = msg as XDpack77.Pack77Message.QSL;
            if ((qsl != null) && String.Equals(qsl.CallQSLed, "ALL") || directlyToMe)
            {
                if (!haveLogged && !haveGrid && !haveReport)
                {
                    ExchangeSent es = () => cb.SendExchange(amLeader ? ExchangeTypes.DB_REPORT : ExchangeTypes.GRID_SQUARE, false);
                    lastSent = es;
                    es();
                    return;
                }
                if (!haveLogged && (haveReport || (directlyToMe && haveGrid)))
                {
                    haveLogged = true;
                    lastSent = null;
                    if (!amLeader)
                        cb.SendAck();
                    cb.LogQso();
                    return;
                }
                OnReceivedNothing(); // didn't get what I wanted
            }
         }

        public void OnReceivedNothing()
        {
            if (null != lastSent)
                lastSent();
        }
    }
}
