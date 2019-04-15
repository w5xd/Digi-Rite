using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WriteLogDigiRite
{
    class CallPresentation : PanelOfCheckBoxes
    {   
        // draw list of messages onto a Windows.Forms.Panel, each with a checkbox
        private List<RecentMessage> cqList = new List<RecentMessage>();

        public delegate void InitiateQso(RecentMessage m, bool onHisFrequency=false);
        private InitiateQso initiateQso;
        public InitiateQso InitiateQsoCb { get => initiateQso; set => initiateQso = value; }

        public CallPresentation(Panel tp, Label modelLabel, CheckBox modelCheckbox) 
            : base(tp, modelLabel, modelCheckbox)
        {}

        public void Add(RecentMessage rm)
        {
            int addedIndex = tlp.Controls.Count / 2;
            CqLabel lb = new CqLabel(rm, colors);
            lb.Font = lblFont;
            lb.BackColor = lblBackColor;
            lb.ForeColor = lblForeColor;
            lb.AutoSize = false;
            CheckBox cb = new CheckBox();
            cb.GotFocus += lb.OnGetFocus;
            cb.LostFocus += lb.OnLostFocus;
            tlp.Controls.Add(lb);
            tlp.Controls.Add(cb);

            cb.TabStop = false;

            lb.Click += new EventHandler((object o, EventArgs e) => {
                MouseEventArgs me = e as MouseEventArgs;
                bool nextCheck = !cb.Checked;
                if ((null != me) && (me.Button == MouseButtons.Right) && nextCheck)
                    initiateQso(rm, false);
                cb.Checked = nextCheck;
            });

            bool onHisFreq = false;
            XDpack77.Pack77Message.ToFromCall toFromCall = rm.Message.Pack77Message as XDpack77.Pack77Message.ToFromCall;
            String toCall = toFromCall?.ToCall;
            if (null != toCall && (toCall=="CQ" || toCall.StartsWith("CQ ")))
                onHisFreq = true;

            cb.CheckedChanged += new EventHandler((object o, EventArgs e) =>
            {
                OnCheck(o as CheckBox, rm, onHisFreq);
            });

            lb.Size = lblSize;
            cb.Size = cbSize;
            PositionEntry(cb, lb, addedIndex);
        }

        private void OnCheck(CheckBox cb, RecentMessage rm, bool onHisFreq)
        {
            if (cb.Checked)
                initiateQso(rm, onHisFreq);
        }
    }

    class CqLabel : FocusDrawingHelper
    {
        RecentMessage rm;
        RttyRiteColors colors;
        public CqLabel(RecentMessage rm, RttyRiteColors colors)
        {            
            this.rm = rm; 
            this.colors = colors;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (System.Drawing.Brush bck = new System.Drawing.SolidBrush(BackColor))
                e.Graphics.FillRectangle(bck, ClientRectangle);
            System.Drawing.Color fillColor = BackColor;
            bool doBck = rm.Dupe || rm.Mult;;
            if (rm.Dupe)
                fillColor = colors.DupeBackGround;
            else if (rm.Mult)
                fillColor = colors.MultBackGround;
            if (doBck)
            {
                String toRender = rm.ToString();
                string first = toRender.Substring(0, 5);
                string second = toRender.Substring(6);
                second = second.Trim();
                var sze1 = TextRenderer.MeasureText(first, Font);
                var sze2 = TextRenderer.MeasureText(second, Font);
                System.Drawing.Rectangle toFill = ClientRectangle;
                toFill.Size = sze2;
                toFill.X += sze1.Width;
                using (System.Drawing.Brush fb = new System.Drawing.SolidBrush(fillColor))
                    e.Graphics.FillRectangle(fb, toFill);
            }
            TextRenderer.DrawText(e.Graphics,
                rm.ToString(), 
                Font, 
                ClientRectangle, 
                ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            DrawFocusRect(e);
        }  
    }  
}
