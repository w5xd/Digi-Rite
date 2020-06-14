namespace DigiRite
{
    public class QsoSequencer : IQsoSequencer
    {
        /* A state machine that translates events( receipt of a message or a 
         * timeout when expecting one) into actions.    */

        public delegate void MessageSent();
        public interface IQsoSequencerCallbacks
        {
             void SendExchange(bool withAck, MessageSent ms);
             void SendAck(bool ofAnAck, MessageSent ms);
             void LogQso();
             void SendOnLoggedAck(MessageSent ms);
        }

        private bool haveLogged = false; // only invoke the LogQso callback once
        private bool haveLoggedExchange = false;

        private IQsoSequencerCallbacks qsoSequencerCallbacks;

        public QsoSequencer(IQsoSequencerCallbacks callbacks, bool haveSentExchange)
        {
            qsoSequencerCallbacks = callbacks;
            HaveSentExchange = haveSentExchange;
            State = 0;
        }
        private const uint MAXIMUM_ACK_OF_ACK = 3;
        private bool HaveTheirExchange  = false;
        private bool HaveAck  = false;
        private bool HaveSentAck = false;
        private bool HaveSentExchange = false;
        private uint AckMoreAcks = 0;
        private bool HaveLoggedQso { get { return HaveTheirExchange & HaveAck; } }
        private uint State  = 0;
        private const uint FINISHED_STATE = 4;
        private uint WrongExchangeCount = 0;
        private const uint MAX_WRONG_EXCHANGE = 3;

        // call to answer a CQ or otherwise think the other station might answer
        public void Initiate()
        {
            qsoSequencerCallbacks.SendExchange(false, () => { HaveSentExchange = true; }); // Beware--no guarantee actually sent
            State = 1;
        }

        public bool IsFinished {
            get {
                return State >= FINISHED_STATE;
            }
        }

        public string DisplayState { get { return State.ToString(); } }
        private System.Action deferredToEndOfReceive;
        public void OnReceiveCycleEnd(bool messagedThisCycle)
        {
            if (!messagedThisCycle)
            {
                if (!IsFinished)
                    OnReceivedNothing();
            }
            else if (null != deferredToEndOfReceive)
                deferredToEndOfReceive();
            deferredToEndOfReceive = null;
        }

        private bool OnReceivedNothing()
        {
            if (!HaveTheirExchange || !HaveAck || !HaveSentAck)
            {
                MessageSent ms = null;
                if (HaveTheirExchange)
                    ms = () => { HaveSentAck = true; HaveSentExchange = true;};
                qsoSequencerCallbacks.SendExchange(HaveTheirExchange, ms);
                return true;
            }
            return false;
        }

        public void OnReceivedExchange(bool withAck, bool allowSendAck = true)
        {
            deferredToEndOfReceive = null;
            HaveTheirExchange = allowSendAck;
            HaveAck |= withAck && HaveSentExchange;
            if (!withAck)
            {   // if they don't have ours, send it
                qsoSequencerCallbacks.SendExchange(allowSendAck,  () => { HaveSentAck |= allowSendAck; HaveSentExchange = true;  });
                State = 2;
            }
            else
            {   // if they do, then ack this one
                bool prevLogged = haveLoggedExchange; // redundant exchanges received only log once
                qsoSequencerCallbacks.SendAck(false,  () => {
                        HaveSentAck = true; 
                        if (!prevLogged)
                            LogQso();}
                );
                State = System.Math.Max(3, State);
            }
        }

        private void LogQso()
        {
            haveLogged = true;
            if (HaveTheirExchange)
                haveLoggedExchange = true;
            qsoSequencerCallbacks.LogQso();
            State = 4;
        }

        public void OnReceivedAck(bool directlyToMe)
        {
            deferredToEndOfReceive = null;
            System.Action toDo = () =>
            {
                HaveAck = true;
                if (HaveTheirExchange)
                {
                    if (!haveLoggedExchange)
                    {   // we only ack the ack once
                        if (!HaveSentAck)
                            qsoSequencerCallbacks.SendAck(true,
                                () =>
                                {
                                    HaveSentAck = true;
                                    LogQso();
                                });
                        else
                        {
                            AckMoreAcks = MAXIMUM_ACK_OF_ACK;
                            qsoSequencerCallbacks.SendOnLoggedAck(null);
                            LogQso();
                        }
                    }
                    else if (AckMoreAcks > 0)
                        qsoSequencerCallbacks.SendAck(true,
                            () => { AckMoreAcks -= 1; });
                }
                else if (WrongExchangeCount > 0)
                {
                    // give up.
                    if (!haveLogged)
                        AckMoreAcks = MAXIMUM_ACK_OF_ACK;
                    if (AckMoreAcks > 0)
                        qsoSequencerCallbacks.SendAck(true,
                            () =>
                            {
                                AckMoreAcks -= 1;
                                if (!haveLogged)
                                    LogQso();
                            });
                }
                else // ask them to try again
                {
                    qsoSequencerCallbacks.SendExchange(false, () => { HaveSentExchange = true; });
                    State = 1;
                }
            };
            // defer the action to end of receive cycle if not directly to me
            if (directlyToMe)
                toDo();
            else
                deferredToEndOfReceive = toDo;
        }

        public void OnReceivedWrongExchange()
        {
            deferredToEndOfReceive = null;
            if ((!HaveTheirExchange || !HaveSentExchange) && (WrongExchangeCount < MAX_WRONG_EXCHANGE))
            {
                qsoSequencerCallbacks.SendExchange(false, () => { HaveSentExchange = true; WrongExchangeCount += 1;});
                return;
            }
            qsoSequencerCallbacks.SendAck(false, () => { if (!haveLogged) LogQso(); });
        }
    }
}
