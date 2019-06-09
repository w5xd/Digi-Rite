using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteLogDigiRite
{
    public class QsoInProgress
    {
        private class QsoInProgressKey
        {
            public QsoInProgressKey(string baseCall, short band)
            {
                this.band = band;
                this.baseCall = baseCall;
            }
            public override int GetHashCode()
            {   // 
                const int bandHashWidth = 32 - 1; // hash with 32 bands
                return (band & bandHashWidth) | (baseCall.GetHashCode() & ~bandHashWidth);
            }
            public override bool Equals(object obj)
            {
                return Equals(obj as QsoInProgressKey);
            }

            public bool Equals(QsoInProgressKey obj)
            {
                return obj != null && obj.band == band && obj.baseCall == baseCall;
            }
            private short band;
            private string baseCall;
        }

        private RecentMessage originatingMessage;
        private int ackMessage;
        private IQsoSequencer qsoSequencer=null;
        private bool messagedThisCycle = true; // when created, we're not starved
        private bool holdingForAnotherQso = false;
        private bool active=true;
        private short band = 0;
        private DateTime timeOfLastReceived;
        private bool markedAsLogged = false;
        private uint transmitFrequency = 0;
        private List<XDpack77.Pack77Message.ReceivedMessage> messages = new List<XDpack77.Pack77Message.ReceivedMessage>();
        private const int MAX_CYCLES_WITHOUT_ANSWER = 5;
        public delegate void OnChanged();
        public OnChanged OnChangedCb { get; set; }
        public QsoInProgress(RecentMessage rm, short band)
        {            
            this.band = band;
            originatingMessage = rm;
            messages.Add(rm.Message);      
            this.AckMessage = Properties.Settings.Default.DefaultAcknowlegement;
            timeOfLastReceived = DateTime.UtcNow;
        }

        public XDpack77.Pack77Message.ReceivedMessage Message { get { return messages.First(); } }
        
        public String HisCall {
            get { 
                return ((XDpack77.Pack77Message.ToFromCall)(Message.Pack77Message)).FromCall;
            }
        }

        public int AckMessage { // FT8 supports 3 acknowledgement messages
            set { ackMessage = value; // RRR 73 RR73
                if (ackMessage < 0)
                    ackMessage = 0;
                if (ackMessage >= MainForm.DefaultAcknowledgements.Length)
                    ackMessage = 0;
                }
            get { return ackMessage; }
        }

        public override String ToString()
        {
            XDpack77.Pack77Message.ToFromCall ft = (XDpack77.Pack77Message.ToFromCall)Message.Pack77Message;
            String seq = "";
            if (null != qsoSequencer)
            {
                if (qsoSequencer.IsFinished)
                    seq = "L";
                else
                {
                    if (holdingForAnotherQso)
                        seq = "H";
                    else
                        seq = qsoSequencer.DisplayState;
                }
            }
            if (markedAsLogged)
                seq = "L";
            else if (AmTimedOut)
                seq = "T";
            return String.Format("{0,1} {1,6} {2,4} {3:####;;}", seq, ft.FromCall, Message.Hz, TransmitFrequency);
        }        

        public IQsoSequencer Sequencer { get { return qsoSequencer; } set { qsoSequencer = value; } }

        public uint SentSerialNumber { get; set; } = 0;

        public String SentGrid { get; set; }

        public bool IsLogged {
            get {
                if ((null != qsoSequencer) && qsoSequencer.IsFinished)
                    return true;
                return markedAsLogged;
            }
        }

        public int CyclesSinceMessaged { get; private set; } = 0;

        public bool CanAcceptAckNotToMe { get; private set; } = true;

        private bool MessagedThisCycle { get { 
                return messagedThisCycle || holdingForAnotherQso; 
                } }

        public DateTime TimeOfLastReceived { get { return timeOfLastReceived; } }

        public bool Active { get { return active; } set { 
                active = value;
                if (value)
                {
                    CyclesSinceMessaged = 0;
                    holdingForAnotherQso = false;
                }
                if (null != OnChangedCb) OnChangedCb();
                } }
        public bool Dupe { get => originatingMessage.Dupe;  }
        public bool Mult { get => originatingMessage.Mult;  }
        public bool MarkedAsLogged { get => markedAsLogged; set { 
                markedAsLogged = value;
                if (null != OnChangedCb) OnChangedCb();
            }
        }
        public uint TransmitFrequency { get => transmitFrequency; 
            set { transmitFrequency = value; 
                holdingForAnotherQso = false;
                if (null != OnChangedCb) OnChangedCb();
            }
        }

        private bool AmTimedOut { get { return !messagedThisCycle && 
                    CyclesSinceMessaged >= MAX_CYCLES_WITHOUT_ANSWER; } }

        public bool OnCycleBegin(bool wasReceiveCycle)
        {
            bool ret = MessagedThisCycle;
            if (ret)
                CyclesSinceMessaged = 0;
            else if (!holdingForAnotherQso && wasReceiveCycle)
                CyclesSinceMessaged += 1;
            if (AmTimedOut)
            {
                CanAcceptAckNotToMe = false;
                if (active)
                {
                    active = false;
                    if (null != OnChangedCb) OnChangedCb();
                }
            }
            messagedThisCycle = false;
            return ret;
        }

        static private String toBaseCall(string call)
        {
            string toBaseCall = null;
            if (XDft.Generator.checkCall(call, ref toBaseCall))
                return toBaseCall;
            else
                return call;
        }

        public object GetKey()
        { return GetKey(Message, band);  }

        public static object GetKey(XDpack77.Pack77Message.ReceivedMessage rm, short band)
        {   // separate QsoInProgress for each BaseCall on each band
            XDpack77.Pack77Message.ToFromCall newcall = rm.Pack77Message as XDpack77.Pack77Message.ToFromCall;
            if ((null != newcall) && !String.IsNullOrEmpty(newcall.FromCall))
                return new QsoInProgressKey(toBaseCall(newcall.FromCall), band);
            return null;
        }

        // if the message is to our same CALL...
        public bool AddMessageOnMatch(XDpack77.Pack77Message.ReceivedMessage rm, 
            bool directlyToMe, string callsQsled)
        {
            if (null == qsoSequencer)
                return false;
            try
            {
                if (directlyToMe)
                    CanAcceptAckNotToMe = true;
                if (!directlyToMe)
                {
                    if (String.IsNullOrEmpty(callsQsled))
                    {
                        // if he sends multiple messages in the same cycle...
                        // ...then we need to "hold" only if this is the only one
                        if (!messagedThisCycle && !markedAsLogged)
                        {
                            // he sent to someone else
                            int freqDif = (int)transmitFrequency;
                            freqDif -= (int)Message.Hz;
                            if (freqDif < 0)
                                freqDif = -freqDif;
                            if (freqDif <= 60)
                                holdingForAnotherQso = true;
                        }
                    }
                    else
                        holdingForAnotherQso = false;
                    return false;
                }
                holdingForAnotherQso = false;
                if (AmTimedOut || IsLogged)
                    active = true;
                messagedThisCycle = true;
                messages.Add(rm);
                timeOfLastReceived = DateTime.UtcNow;
                return true;
            }
            finally
            {
                if (null != OnChangedCb) OnChangedCb();
            }
        }

        public List<XDpack77.Pack77Message.ReceivedMessage> MessageList { get { return messages; } }
    }
}
