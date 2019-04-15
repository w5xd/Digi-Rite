using System;

namespace WriteLogDigiRite
{
    public class RecentMessage
    {
        private XDpack77.Pack77Message.ReceivedMessage msg;
        private bool dupe = false;
        private bool mult = false;

        public RecentMessage(XDpack77.Pack77Message.ReceivedMessage m, bool dupe, bool mult)
        { msg = m; this.dupe = dupe; this.mult = mult; }

        public override String ToString()
        {
            string letter = " ";
            if (dupe)
                letter = "D";
            else if (mult)
                letter = "M";
            return String.Format("{0} {1:+00;-0#} {2}", letter, msg.SignalDB, msg.Content);
        }
        public XDpack77.Pack77Message.ReceivedMessage Message { get { return msg; } }
        public bool Dupe { get => dupe; }
        public bool Mult { get => mult; }
    };

}
