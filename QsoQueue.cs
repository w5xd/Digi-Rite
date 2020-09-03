using System;
using System.Linq;

namespace DigiRite
{
    /* class QsoQueue
     * This implementation is specialized to handle the contest exchange
     * where there is only one message each to and from the other station
     * that has what we need. See Qso2MessageExchange for the other implementation
     * 
     * DigiRite can have multiple QSOs in progress at a time. 
     * "in progress" means we have either sent or received a message with the
     * station, but have not received the required two acknowledgements we 
     * need. (one for our exchange, the second for "you're in my log")
     * 
     * A QsoInProgress goes into our list based simply on having received
     * a message (even unsolicited) or the user clicking on a message
     * from someone we want to work.
     * 
     * They are in ordered list of priority.
     * 
     * When we decide to originate a message to such a station, a QsoSequencer
     * gets created. It decides what message to send based on what
     * acknowledgements we have, so far.
     * 
     * Incoming messages are sent to us based on their ToCall matching myCall.
     * We forward each message to the appropriate QsoInProgress based on matching
     * hisCall.
     */

    public delegate bool ContestMessageSelector(XDpack77.Pack77Message.Message m);

    class QsoQueue : QueueCommon
    {
        private IQsoQueueCallBacks callbacks;

        public interface IQsoQueueCallBacks
        {
            string GetExchangeMessage(QsoInProgress q, bool addAck);
            string GetAckMessage(QsoInProgress q, bool ofAnAck);
            void SendMessage(string toSend, QsoInProgress q, QsoSequencer.MessageSent ms);
            void LogQso(QsoInProgress q);
            void SendOnLoggedAck(QsoInProgress q, QsoSequencer.MessageSent ms);
        };

        // connect the QsoQueue with QsoInProgress on callbacks from the QsoSequencer
        protected class QsoSequencerCbImpl : QsoSequencer.IQsoSequencerCallbacks
        {
            public QsoSequencerCbImpl(QsoQueue queue, QsoInProgress q)
            {   qsoQueue = queue; qso = q;   }

            public void LogQso()  
                {  qsoQueue.logQso(qso);   }
            public void SendAck(bool ofAnAck, QsoSequencer.MessageSent ms)  
                { qsoQueue.sendAck(qso, ofAnAck, ms);  }
            public void SendExchange(bool withAck, QsoSequencer.MessageSent ms)  
                {  
                    qsoQueue.sendExchange(qso, withAck, ms); 
                }
            public void SendOnLoggedAck(QsoSequencer.MessageSent ms) {  qsoQueue.callbacks.SendOnLoggedAck(qso, ms);} 
            public override String ToString()         
                { return qso.ToString(); }

            private QsoQueue qsoQueue;
            private QsoInProgress qso;
        }

        protected ContestMessageSelector messageSelector;

        public QsoQueue(QsosPanel listBox, IQsoQueueCallBacks cb, ContestMessageSelector selector) : base(listBox)
        {   
            callbacks = cb; 
            messageSelector = selector;
        }     

        // call here every for every incoming message that might be relevant to us
        public override void MessageForMycall(RecentMessage recentMessage,  
            bool directlyToMe, string callQsled, short band,
            bool autoStart, IsConversationMessage onUsed)
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
                QsoSequencer sequencer = inProgress.Sequencer as QsoSequencer;
                onUsed(directlyToMe ? Conversation.Origin.TO_ME : Conversation.Origin.TO_OTHER);
                // What's in the message? an exchange and/or an acknowledgement?
                bool ack = false;
                bool hasExchange = directlyToMe && ExchangeFromMessage(rm.Pack77Message) != null;
                if (hasExchange)
                {
                    XDpack77.Pack77Message.Roger roger = rm.Pack77Message as XDpack77.Pack77Message.Roger;
                    if (roger != null)
                        ack = roger.Roger; // if the message has a roger bit, use it
                }
                if (!hasExchange && !ack) // but if no exchange, allow QSL to also set ack
                {   // if the message can QSO prior, see if can apply to us
                        if ((String.Equals("ALL", callQsled) && inProgress.CanAcceptAckNotToMe) || 
                            String.Equals(myCall,callQsled)  || 
                            String.Equals(myBaseCall, callQsled))
                            ack = true;
                }

                if (hasExchange)
                    sequencer.OnReceivedExchange(ack);
                else if (ack)
                    sequencer.OnReceivedAck(directlyToMe);
                else 
                    sequencer.OnReceivedWrongExchange();
            } else if (autoStart && directlyToMe)
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

        protected virtual XDpack77.Pack77Message.Exchange ExchangeFromMessage(XDpack77.Pack77Message.Message m)
        {
            if (!messageSelector(m))
                return null;
            XDpack77.Pack77Message.Exchange exc = m as XDpack77.Pack77Message.Exchange;
            // The Pack77Message Exchange interface can be deceptive.
            // The Standard message CQ can have this interface but a null exc.Exchange with non-null GridSquare
            if (exc != null && !String.IsNullOrEmpty(exc.Exchange))
                return exc;
            return null;
        }

        protected override void StartQso(QsoInProgress q)
        {   // q needs to already be in our qsosPanel list
            QsoSequencer qs = new QsoSequencer(new QsoSequencerCbImpl(this, q), false);
            q.Sequencer = qs;
            // very first message directed from other to me
            // can be a CQ I chose to answer, or can be an exchange
            XDpack77.Pack77Message.Exchange exc =  ExchangeFromMessage(q.Message.Pack77Message);
            if (exc != null)
                qs.OnReceivedExchange(false);
            else
                qs.Initiate();
        }

        private void sendExchange(QsoInProgress q, bool withAck, QsoSequencer.MessageSent ms)
        {    callbacks.SendMessage(exchangeMessage(q,withAck), q, ms);    }

        private string exchangeMessage(QsoInProgress q, bool withAck)
        {  return callbacks.GetExchangeMessage(q, withAck);   }

        private void logQso(QsoInProgress q)
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

        private void sendAck(QsoInProgress q, bool ofAnAck, QsoSequencer.MessageSent ms) /* ofAnAck can be used to send CQ*/
        {  callbacks.SendMessage(ackMessage(q,ofAnAck), q, ms); }

        private string ackMessage(QsoInProgress q, bool ofAnAck)
        { return callbacks.GetAckMessage(q, ofAnAck); }
    }

    class QsoQueueGridSquare : QsoQueue
    {
        public QsoQueueGridSquare(QsosPanel listBox, IQsoQueueCallBacks cb, ContestMessageSelector selector) : base(listBox, cb, selector)
        {}

        protected override void StartQso(QsoInProgress q)
        {   // q needs to already be in our qsosPanel list
            QsoSequencer qs = new QsoSequencer(new QsoSequencerCbImpl(this, q), false);
            q.Sequencer = qs;
            XDpack77.Pack77Message.Exchange exc = ExchangeFromMessage(q.Message.Pack77Message);
            if (exc != null)
                qs.OnReceivedExchange(false, exc.Exchange != null); // like base class, but don't allow send of ack on initiating
            else
                qs.Initiate();
        }


        protected override XDpack77.Pack77Message.Exchange ExchangeFromMessage(XDpack77.Pack77Message.Message m)
        {
            if (!messageSelector(m))
                return null;
            XDpack77.Pack77Message.Exchange exc = m as XDpack77.Pack77Message.Exchange;
            return exc;
        }
    }
}
