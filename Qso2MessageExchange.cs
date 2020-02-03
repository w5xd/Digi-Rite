using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiRite
{
    // Sequencing for the case where a QSO consists of sending a grid square and then a signal
    // report, in that order.
    #region Queue
    class Qso2MessageExchange : QueueCommon
    {
        public interface IQsoQueueCallBacks
        {
            string GetExchangeMessage(QsoInProgress q, bool addAck, ExchangeTypes exc);
            string GetAckMessage(QsoInProgress q, bool ofAnAck);
            void SendMessage(string toSend, QsoInProgress q, QsoSequencer.MessageSent ms);
            void LogQso(QsoInProgress q);
            void SendOnLoggedAck(QsoInProgress q, QsoSequencer.MessageSent ms);
        };

        protected IQsoQueueCallBacks callbacks;

        public Qso2MessageExchange(QsosPanel qsosPanel, IQsoQueueCallBacks cb) : base(qsosPanel)
        {  callbacks = cb;   }

        public override void MessageForMycall(RecentMessage recentMessage, 
            bool directlyToMe, string callQsled, 
            short band, bool autoStart, IsConversationMessage onUsed)
        {
            XDpack77.Pack77Message.ReceivedMessage rm = recentMessage.Message;
            var inProgList = qsosPanel.QsosInProgressDictionary;
            QsoInProgress inProgress = null;
            bool used = false;
            if (inProgList.TryGetValue(QsoInProgress.GetKey(rm, band), out inProgress))
                used = inProgress.AddMessageOnMatch(rm, directlyToMe, callQsled);
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
            } else if (null != inProgress)
            {
                if ((null != inProgress.Sequencer) && !inProgress.Sequencer.IsFinished && !inProgress.InLoggedInactiveState)
                    onUsed(Conversation.Origin.TO_OTHER); // make it show up in the conversation history
            }
        }

        // connect the Qso2MessageExchange with QsoInProgress on callbacks from the Qso2MessageSequencer
        class QsoSequencerCbImpl : Qso2MessageSequencer.IQsoSequencerCallbacks
        {
            public QsoSequencerCbImpl(Qso2MessageExchange queue, QsoInProgress q)
            { qsoQueue = queue; qso = q; }
            public void LogQso()
            { qsoQueue.LogQso(qso); }
            public void SendAck(QsoSequencer.MessageSent ms)
            { qsoQueue.SendAck(qso, ms); }
            public void SendExchange(ExchangeTypes ext, bool withAck, QsoSequencer.MessageSent ms)
            { qsoQueue.SendExchange(qso, ext, withAck, ms); }
            public void SendOnLoggedAck(QsoSequencer.MessageSent ms) { qsoQueue.callbacks.SendOnLoggedAck(qso, ms); } 
            public override String ToString()
            { return qso.ToString(); }

            private Qso2MessageExchange qsoQueue;
            private QsoInProgress qso;
        }

        protected bool isMe(string s)
        { return String.Equals(s, myCall) || String.Equals(s, myBaseCall);  }

        protected override void StartQso(QsoInProgress q)
        {
            Qso2MessageSequencer qs = new Qso2MessageSequencer(new QsoSequencerCbImpl(this, q));
            q.Sequencer = qs;
            bool directlyToMe = false;
            XDpack77.Pack77Message.ToFromCall toFromCall = q.Message.Pack77Message as XDpack77.Pack77Message.ToFromCall;
            if (null != toFromCall)
                directlyToMe = isMe(toFromCall.ToCall);
            qs.OnReceived(directlyToMe, q.Message.Pack77Message);
        }

        public void SendExchange(QsoInProgress q, ExchangeTypes exc, bool withAck, QsoSequencer.MessageSent ms)
        {  callbacks.SendMessage(callbacks.GetExchangeMessage(q, withAck, exc), q, ms);      }

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

        public void SendAck(QsoInProgress q, QsoSequencer.MessageSent ms)
        { callbacks.SendMessage(callbacks.GetAckMessage(q, false), q, ms);  }
    }
    #endregion

    #region Sequencer
    class Qso2MessageSequencer : IQsoSequencer
    {
        public interface IQsoSequencerCallbacks
        {
            void SendExchange(ExchangeTypes exc, bool withAck, QsoSequencer.MessageSent ms);
            void LogQso();
            void SendAck(QsoSequencer.MessageSent ms);
            void SendOnLoggedAck(QsoSequencer.MessageSent ms);
        }
        private const uint MAXIMUM_ACK_OF_ACK = 3;
        private bool haveGrid  = false;
        private bool haveReport  = false;
        private bool haveLoggedGrid = false;
        private bool haveLoggedReport = false;
        private bool amLeader = false;
        private bool amLeaderSet = false;
        private bool haveSentReport = false;
        private bool haveSentGrid = false;
        private bool haveReceivedWrongExchange = false;
        private bool haveAckOfGrid = false; // compiler correctly says we never read this
        private string ackOfAckGrid;
        private bool haveAckOfReport = false;
        private uint AckMoreAcks = 0;
        private bool onLoggedAckEnabled = false;
        private IQsoSequencerCallbacks cb;
        private delegate void ExchangeSent();
        private ExchangeSent lastSent;
        public Qso2MessageSequencer(IQsoSequencerCallbacks cb)
        { this.cb = cb;   }

        public bool IsFinished { get { return haveLoggedGrid || haveLoggedReport; } }

        public string DisplayState {
            get {
                int v = 0;
                if (haveGrid)
                    v += 1;
                if (haveReport)
                    v += 2;
                if (haveLoggedGrid || haveLoggedReport)
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
            XDpack77.Pack77Message.Roger roger = msg as XDpack77.Pack77Message.Roger;
            bool ack = (null != roger) && (roger.Roger);
            if (null != exc && (!haveGrid || directlyToMe))
            {
                string gs = exc.GridSquare;
                int rp = exc.SignaldB;
                var qslm = msg as XDpack77.Pack77Message.StandardMessage;
                if (!String.IsNullOrEmpty(gs))
                {   // received a grid
                    haveGrid = true;
                    ExchangeSent es;
                    if (ack)
                        haveAckOfGrid = true;
                    if (amLeader || ack)
                        es = () => cb.SendExchange(ExchangeTypes.DB_REPORT, haveReport, () => { haveSentReport = true; });
                    else
                        es = () => cb.SendExchange(ExchangeTypes.GRID_SQUARE, directlyToMe && haveGrid, () => { haveSentGrid = true; });
                    lastSent = es;
                    es();
                    return;
                }
                else if (rp > XDpack77.Pack77Message.Message.NO_DB)
                {   // received a dB report
                    haveReport = true;
                    lastSent = null;
                    if (ack && haveSentReport)
                        haveAckOfReport = true;
                    if (haveAckOfReport)
                    {
                        ExchangeSent es = () => cb.SendAck(() =>
                        {
                            if (!haveLoggedReport)
                            {
                                haveLoggedReport = true;
                                if (haveGrid)
                                    haveLoggedGrid = true;
                                cb.LogQso();
                            }
                        });
                        lastSent = es;
                        es();
                    }
                    else
                    {
                        ExchangeSent es = () => cb.SendExchange(ExchangeTypes.DB_REPORT, true, () => { haveSentReport = true; });
                        lastSent = es;
                        es();
                    }
                    return;
                }
                else if (null == msg as XDpack77.Pack77Message.StandardMessage)
                {   // message has an exchange, but for some contest we don't know about
                    if (!haveReceivedWrongExchange)
                    {
                        haveReceivedWrongExchange = true;
                        cb.LogQso();
                    }
                    cb.SendAck(null); // send a 73, log it, and get going
                    lastSent = null;
                    return;
                }
            }
            if (!haveReceivedWrongExchange && !haveGrid && !haveReport)
            {
                ExchangeSent es = null;
                if (haveSentGrid)
                    es = () => cb.SendExchange( ExchangeTypes.DB_REPORT , false, () => { haveSentReport = true; });
                else
                    es = () => cb.SendExchange(ExchangeTypes.GRID_SQUARE, false, () => { haveSentGrid = true; });
                lastSent = es;
                es();
                return;
            }
            XDpack77.Pack77Message.QSL qsl = msg as XDpack77.Pack77Message.QSL;
            if ((qsl != null) && String.Equals(qsl.CallQSLed, "ALL") || directlyToMe)
            {
                if (!haveLoggedGrid && (haveReport || (directlyToMe && haveGrid)))
                {
                    haveLoggedGrid = true;
                    if (haveReport)
                        haveLoggedReport = true;
                    lastSent = null;
                    cb.LogQso();
                    ackOfAckGrid = qsl.QslText; // see if they repeat exact message
                    AckMoreAcks = MAXIMUM_ACK_OF_ACK;
                    cb.SendOnLoggedAck(() =>
                    { onLoggedAckEnabled = true;}); 
                    return;
                }
                if (AckMoreAcks > 0 &&  directlyToMe && String.Equals(qsl.QslText, ackOfAckGrid))
                {   // only repeat this if they send exact same message
                    lastSent = null;
                    AckMoreAcks -= 1;
                    if (onLoggedAckEnabled)
                        cb.SendOnLoggedAck(null);
                    else
                        cb.SendAck(null);
                    return;
                }
                return;
            }
            OnReceivedNothing(); // didn't get what I wanted
         }

        public bool OnReceivedNothing()
        {
            if (null != lastSent)
            {
                lastSent();
                return true;
            }
            return false;
        }

    }
    #endregion
}
