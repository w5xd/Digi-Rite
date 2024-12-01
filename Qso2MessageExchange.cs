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
            string GetQslMessage(QsoInProgress q, bool ofAnAck);
            void SendMessage(string toSend, QsoInProgress q, QsoSequencer.MessageSent ms);
            void LogQso(QsoInProgress q);
            void SendOnLoggedQsl(QsoInProgress q, QsoSequencer.MessageSent ms);
        };

        protected IQsoQueueCallBacks callbacks;

        public Qso2MessageExchange(QsosPanel qsosPanel, IQsoQueueCallBacks cb) : base(qsosPanel)
        {  callbacks = cb;   }

        public override void MessageForMycall(RecentMessage recentMessage, 
            bool directlyToMe, CallQsled callQsled,
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
                ((Qso2MessageSequencer)(inProgress.Sequencer)).OnReceived(directlyToMe, callQsled, rm.Pack77Message);
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
            public void SendQsl(QsoSequencer.MessageSent ms)
            { qsoQueue.SendQsl(qso, ms); }
            public void SendExchange(ExchangeTypes ext, bool withAck, QsoSequencer.MessageSent ms)
            { qsoQueue.SendExchange(qso, ext, withAck, ms); }
            public void SendOnLoggedQsl(QsoSequencer.MessageSent ms) { qsoQueue.callbacks.SendOnLoggedQsl(qso, ms); } 
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
            qs.OnReceived(directlyToMe, CallQsled.None, q.Message.Pack77Message);
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

        public void SendQsl(QsoInProgress q, QsoSequencer.MessageSent ms)
        { callbacks.SendMessage(callbacks.GetQslMessage(q, false), q, ms);  }
    }
    #endregion

    #region Sequencer
    class Qso2MessageSequencer : IQsoSequencer
    {
        public interface IQsoSequencerCallbacks
        {
            void SendExchange(ExchangeTypes exc, bool withAck, QsoSequencer.MessageSent ms);
            void LogQso();
            void SendQsl(QsoSequencer.MessageSent ms);
            void SendOnLoggedQsl(QsoSequencer.MessageSent ms);
        }
        private const int MAXIMUM_ACK_OF_ACK = 3;
        private bool haveGrid  = false;
        private bool haveReport  = false;
        private bool haveLoggedGrid = false;
        private bool haveLoggedReport = false;
        private bool haveSentReport = false;
        private bool haveSentGrid = false;
        private bool haveReceivedQsl = false;
        private bool haveReceivedWrongExchange = false;
        private string qslTextReceived;
        private int AckMoreAcks = 0;
        private IQsoSequencerCallbacks cb;
        private delegate void ExchangeSent();
        private ExchangeSent lastSent;
#if DEBUG
        // make it easier to debug a specific QSO when there are multiple
        private static uint gId;
        private uint id;
#endif
        public Qso2MessageSequencer(IQsoSequencerCallbacks cb)
        { 
            this.cb = cb;
#if DEBUG
            id = ++gId;
#endif
        }

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

        private void LogQso()
        {
            if (haveLoggedReport != haveReport || haveLoggedGrid != haveGrid)
                cb.LogQso();
            haveLoggedReport |= haveReport;
            haveLoggedGrid |= haveGrid;
        }

        public void OnReceived(bool directlyToMe, CallQsled callQsled, XDpack77.Pack77Message.Message msg)
        {
            deferredToEndOfReceive = null;
            XDpack77.Pack77Message.Exchange exc = msg as XDpack77.Pack77Message.Exchange;
            XDpack77.Pack77Message.Roger roger = msg as XDpack77.Pack77Message.Roger;
            bool msgHasR = (null != roger) && (roger.Roger);
            ExchangeSent eToSend = null;
            QsoSequencer.MessageSent asTransmitted = () =>
            {
                AckMoreAcks = MAXIMUM_ACK_OF_ACK - 1;
            };
            if (null != exc)
            {
                string gs = exc.GridSquare;
                int rp = exc.SignaldB;
                var qslm = msg as XDpack77.Pack77Message.StandardMessage;
                if (!String.IsNullOrEmpty(gs))
                {   // received a grid
                    haveGrid = true;
                    if (!msgHasR && !directlyToMe && !haveReceivedQsl)
                        eToSend = () => cb.SendExchange(ExchangeTypes.GRID_SQUARE, haveGrid & haveReport, () =>
                            { haveSentGrid = true; });
                }
                else if (directlyToMe)
                {
                    if (rp > XDpack77.Pack77Message.Message.NO_DB)
                    {   // received a dB report
                        haveReport = true;
                        if (!haveSentReport)
                        {
                            msgHasR = false; // if I receive a report with an R, but have never sent one, ignore the R
                            haveReceivedQsl = false;
                        }
                        if (!msgHasR && !haveReceivedQsl)
                            eToSend = () => cb.SendExchange(ExchangeTypes.DB_REPORT, haveReport & haveGrid, () =>
                                { haveSentReport = true; });
                        if (msgHasR)
                            haveReceivedQsl = true;
                    }
                    else if (null == msg as XDpack77.Pack77Message.StandardMessage)
                    {   // message has an exchange, but for some contest we don't know about
                        haveReceivedWrongExchange = true;
                        LogQso();
                        cb.SendQsl(null); // send a 73, log it, and get going
                        lastSent = null;
                        return;
                    }
                }
                if (eToSend == null)
                {
                    if (haveReport && haveGrid)
                        eToSend = () => cb.SendQsl(asTransmitted);
                }
            }
            if (eToSend == null && !haveReceivedWrongExchange)
            {
                if (!haveReport)
                    eToSend = () => cb.SendExchange(ExchangeTypes.DB_REPORT, haveReport & haveGrid, () =>
                        { haveSentReport = true; });
                else if (!haveGrid)
                    eToSend = () => cb.SendExchange(ExchangeTypes.GRID_SQUARE, haveReport & haveGrid, () =>
                        { haveSentGrid = true; });
                else if (directlyToMe && haveSentReport)
                {
                    eToSend = () =>
                    {
                        if (!haveLoggedGrid || !haveLoggedReport)
                        {
                            LogQso();
                            cb.SendOnLoggedQsl(asTransmitted);
                        }
                    };
                }
            }
            bool isMe = callQsled == CallQsled.IsMe || directlyToMe;
            XDpack77.Pack77Message.QSL qsl = msg as XDpack77.Pack77Message.QSL;
            bool qslTextMatchesLastTime = !String.IsNullOrEmpty(qslTextReceived) && String.Equals(qsl.QslText, qslTextReceived);
            // is this message a QSL to end the QSO?
            if (callQsled != CallQsled.None || haveReceivedQsl)
            {
                if (haveReceivedWrongExchange)
                    return;
                Action toDoOnAck = () =>
                {
                    lastSent = null;
                    if (AckMoreAcks >= 0 && directlyToMe && qslTextMatchesLastTime)
                    {   // only repeat this if they send exact same message
                        AckMoreAcks -= 1;
                        if (AckMoreAcks >= 0)
                            cb.SendQsl(null);
                        return;
                    }
                    if (AckMoreAcks == 0 && (haveReport || (isMe && haveGrid)))
                    {
                        LogQso();
                        if (!haveReport || !haveGrid || msgHasR)
                            cb.SendQsl(asTransmitted); // He terminated the QSO by sending us a QSL, but we were not finished.
                        else
                        {
                            AckMoreAcks = 1;
                            cb.SendOnLoggedQsl(asTransmitted);
                        }
                        return;
                    }
                    else if (directlyToMe && qslTextMatchesLastTime)
                        cb.SendQsl(asTransmitted);
                };
                // do it now? or wait to see if multi-streaming partner sends a message directlyToMe
                if (isMe)
                    toDoOnAck();
                else
                    deferredToEndOfReceive = toDoOnAck;
                qslTextReceived = qsl.QslText; // see if they repeat exact message                        
                haveReceivedQsl = true;
                return;
            }
            if (eToSend != null)
            {
                lastSent = eToSend;
                eToSend();
                return;
            }
            /* In the (unlikely) possibility I got a message directly to me but could do nothing with it,
             * I assume the other guy is not going to multi-stream multiple messages directly to me in the
             * same cycle and I know that OnReceivedNothing() will not be called at the end of this cycle
             * because I did get this message I am processing now, so behave as if I got nothing.
             */
            if (isMe)   
                OnReceivedNothing();
        }

        private Action deferredToEndOfReceive;
        public void OnReceiveCycleEnd(bool messagedThisCycle, bool onHold)
        {
            if (null != deferredToEndOfReceive)
            {
                deferredToEndOfReceive();
                deferredToEndOfReceive = null;
            }
            else if (!messagedThisCycle)
            {
                if (!IsFinished && !onHold)
                    OnReceivedNothing();
            }
        }
        private bool OnReceivedNothing()
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
