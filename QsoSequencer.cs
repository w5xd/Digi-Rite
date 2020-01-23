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
        }

        private bool haveLogged = false; // only invoke the LogQso callback once

        private IQsoSequencerCallbacks qsoSequencerCallbacks;

        public QsoSequencer(IQsoSequencerCallbacks callbacks)
        {
            qsoSequencerCallbacks = callbacks;
            State = 0;
        }
        private bool HaveTheirs  = false;
        private bool HaveAck  = false;
        private bool HaveSentAck = false;
        private bool AckMoreAcks = false;
        private bool HaveLoggedQso { get { return HaveTheirs & HaveAck; } }
        private uint State  = 0;
        private const uint FINISHED_STATE = 4;

        // call to answer a CQ or otherwise think the other station might answer
        public void Initiate(bool ack = false)
        {
            qsoSequencerCallbacks.SendExchange(ack, null); // Beware--no guarantee actually sent
            HaveTheirs = ack;
            State = 1;
        }

        public bool IsFinished {
            get {
                return State >= FINISHED_STATE;
            }
        }

        public string DisplayState { get { return State.ToString(); } }
        
        public bool OnReceivedNothing()
        {
            if (!HaveTheirs || !HaveAck || !HaveSentAck)
            {
                MessageSent ms = null;
                if (HaveTheirs)
                    ms = () => { HaveSentAck = true; };
                qsoSequencerCallbacks.SendExchange(HaveTheirs, ms);
                return true;
            }
            return false;
        }

        public void OnReceivedExchange(bool withAck)
        {
            HaveTheirs = true;
            HaveAck |= withAck;
            if (!withAck)
            {   // if they don't have ours, send it
                qsoSequencerCallbacks.SendExchange(true,  () => { HaveSentAck = true;   });
                State = 2;
            }
            else
            {   // if they do, then ack this one
                bool prevLogged = haveLogged; // redundant exchanges received only log once
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
            qsoSequencerCallbacks.LogQso();
            State = 4;
        }

        public bool OnReceivedAck()
        {
            bool retval = true;
            HaveAck = true;
            if (HaveTheirs)
            {
                if (!haveLogged)
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
                        qsoSequencerCallbacks.SendAck(true, null);
                        AckMoreAcks = true;
                        LogQso();
                    }
                }
                else if (AckMoreAcks)
                    qsoSequencerCallbacks.SendAck(true, null);
                else
                    retval = false;
            }
            else // this is the wierd transition. 
            {
                qsoSequencerCallbacks.SendExchange(false, null);
                State = 1;
            }
            return retval;
        }
    }
}
