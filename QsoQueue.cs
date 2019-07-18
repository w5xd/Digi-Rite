using System;
using System.Linq;

namespace WriteLogDigiRite
{
    /* class QsoQueue
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
    class QsoQueue : QueueCommon
    {
        private IQsoQueueCallBacks callbacks;

        public interface IQsoQueueCallBacks
        {
            string GetExchangeMessage(QsoInProgress q, bool addAck);
            string GetAckMessage(QsoInProgress q, bool ofAnAck);
            void SendMessage(string toSend, QsoInProgress q);
            void LogQso(QsoInProgress q);
        };

        // connect the QsoQueue with QsoInProgress on callbacks from the QsoSequencer
        protected class QsoSequencerImpl : QsoSequencer.IQsoSequencerCallbacks
        {
            public QsoSequencerImpl(QsoQueue queue, QsoInProgress q)
            {   qsoQueue = queue; qso = q;   }

            public void LogQso()  
                {  qsoQueue.logQso(qso);   }
            public void SendAck(bool ofAnAck)  
                { qsoQueue.sendAck(qso, ofAnAck);  }
            public void SendExchange(bool withAck)  
                {  qsoQueue.sendExchange(qso, withAck); }
            public override String ToString()         
                { return qso.ToString(); }

            private QsoQueue qsoQueue;
            private QsoInProgress qso;
        }

        public QsoQueue(QsosPanel listBox, IQsoQueueCallBacks cb, bool startWithAck = false) : base(listBox)
        {   callbacks = cb;  
            gridSquareAck = startWithAck;
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
                XDpack77.Pack77Message.Exchange exc = rm.Pack77Message as XDpack77.Pack77Message.Exchange;
                bool hasExchange = (exc != null) && !String.IsNullOrEmpty(exc.Exchange);
                XDpack77.Pack77Message.Roger roger = rm.Pack77Message as XDpack77.Pack77Message.Roger;
                bool ack = false;
                if (roger != null)
                    ack = roger.Roger; // if the message has a roger bit, use it
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
                    sequencer.OnReceivedAck();
            } else if (autoStart && directlyToMe)
            {
                onUsed(Conversation.Origin.TO_ME);
                // wasn't one we already had. but we autostart with any call
                InitiateQso(recentMessage, band, false);
            } else if (null != inProgress) 
            {
                if ((null != inProgress.Sequencer) && !inProgress.Sequencer.IsFinished)
                    onUsed(Conversation.Origin.TO_OTHER); // make it show up in the conversation history
            }
        }

        protected override void StartQso(QsoInProgress q)
        {   // q needs to already be in our qsosPanel list
            QsoSequencer qs = new QsoSequencer(new QsoSequencerImpl(this, q));
            q.Sequencer = qs;
            // very first message directed from other to me
            // can be a CQ I chose to answer, or can be an exchange
            XDpack77.Pack77Message.Exchange exc = q.Message.Pack77Message as XDpack77.Pack77Message.Exchange;
            if ((exc != null) && !String.IsNullOrEmpty(exc.Exchange))
                qs.OnReceivedExchange(gridSquareAck);
            else
                qs.Initiate(gridSquareAck && exc != null && exc.GridSquare != null && exc.GridSquare.Length >= 4);
        }

        private void sendExchange(QsoInProgress q, bool withAck)
        {    callbacks.SendMessage(exchangeMessage(q,withAck), q);    }

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

        private void sendAck(QsoInProgress q, bool ofAnAck) /* ofAnAck can be used to send CQ*/
        {  callbacks.SendMessage(ackMessage(q,ofAnAck), q); }

        private string ackMessage(QsoInProgress q, bool ofAnAck)
        { return callbacks.GetAckMessage(q, ofAnAck); }

        protected bool gridSquareAck;
    }
       
}
