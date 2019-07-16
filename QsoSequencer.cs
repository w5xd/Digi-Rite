namespace WriteLogDigiRite
{
    public class QsoSequencer : IQsoSequencer
    {
        /* A state machine that translates events( receipt of a message or a 
         * timeout when expecting one) into actions.    */
        public interface IQsoSequencerCallbacks
        {
             void SendExchange(bool withAck);
             void SendAck(bool ofAnAck);
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
        private bool HaveLoggedQso { get { return HaveTheirs & HaveAck; } }
        private uint State  = 0;
        private const uint FINISHED_STATE = 4;

        // call to answer a CQ or otherwise think the other station might answer
        public void Initiate(bool ack = false)
        {
            qsoSequencerCallbacks.SendExchange(ack);
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
            if (!HaveTheirs || !HaveAck)
            {
                qsoSequencerCallbacks.SendExchange(HaveTheirs);
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
                qsoSequencerCallbacks.SendExchange(true);
                State = 2;
            }
            else
            {   // if they do, then ack this one
                qsoSequencerCallbacks.SendAck(false);
                OnReceivedAck(); // state is such this call only logs--does not send
                State = System.Math.Max(3, State);
            }
        }

        public bool OnReceivedAck()
        {
            bool retval = true;
            if (HaveTheirs)
            {
                if (!haveLogged)
                {   // we only ack the ack once
                    haveLogged = true;
                    if (!HaveAck)
                        qsoSequencerCallbacks.SendAck(true);
                    HaveAck = true;
                    qsoSequencerCallbacks.LogQso();
                    State = 4;
                }
                else
                    retval = false;
            }
            else // this is the wierd transition. 
            {
                qsoSequencerCallbacks.SendExchange(false);
                State = 1;
            }
            return retval;
        }
    }
}
