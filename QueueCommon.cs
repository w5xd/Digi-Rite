﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiRite
{
    // There are multiple queing implementations. One is optimized for
    // contest-style everything-in-one-exchange and the other
    // for casual two-messages-for-a-qso. This class is what they both do.
    abstract class QueueCommon : IQsoQueue
    {
        protected QsosPanel qsosPanel;
        protected string myCall;
        protected string myBaseCall;

        protected QueueCommon(QsosPanel qp)
        { qsosPanel = qp; }

        public string MyCall { set { myCall = value; } }
        public string MyBaseCall { set { myBaseCall = value; } }

        public bool InitiateQso(RecentMessage rm, short band, bool onHisFrequency, System.Action onUsed=null)
        {
            var inProgList = qsosPanel.QsosInProgressDictionary;
            QsoInProgress q = null;
            if (inProgList.TryGetValue(QsoInProgress.GetKey(rm.Message, band), out q))
            {
                if (!q.Active)
                {
                    q.Active = true;
                    StartQso(q);
                }
                return false; // we're already trying to work this guy
            }
            if (null != onUsed) onUsed();
            QsoInProgress newStn = new QsoInProgress(rm, band);
            if (onHisFrequency)
                newStn.TransmitFrequency = rm.Message.Hz;
            qsosPanel.Add(newStn);
            StartQso(newStn);// transmit to him.
            return true;
        }

        public void OnCycleBeginning(int cycleNumber)
        {
            var inProgList = qsosPanel.QsosInProgressDictionary;
            foreach (var qp in inProgList)
            {
                var q = qp.Value;
                bool wasReceiveCycle = (q.Message.CycleNumber & 1) != (cycleNumber & 1);
                bool messagedThisCycle = q.OnCycleBegin(wasReceiveCycle);
                if (!q.InLoggedInactiveState && wasReceiveCycle && q.Sequencer != null )
                {
                    q.Sequencer.OnReceiveCycleEnd(messagedThisCycle, q.OnHold);
                    if (!messagedThisCycle && q.Sequencer.IsFinished && q.IsLogged)
                        q.Active = false;
                }
            }
        }

        // interface method present but we don't implement. pass it down to subclass
        public abstract void MessageForMycall(RecentMessage recentMessage, bool directlyToMe, string callQsled, 
            short band, bool autoStart, IsConversationMessage onUsed);

        protected abstract void StartQso(QsoInProgress qp);
    }
}
