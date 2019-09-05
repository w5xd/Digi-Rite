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
       
        public delegate bool EnableCb(CheckState CqsOnly);

        private CheckState m_filterCqs;
        public CheckState FilterCqs { set {
                m_filterCqs = value;
                for (int i = 0; i < tlp.Controls.Count; i+=2)
                {
                    CqLabel cql = tlp.Controls[i] as CqLabel;
                    Control cb = tlp.Controls[i+1];
                    bool enable = cql.enableCb(m_filterCqs);
                    cql.Enabled = enable;
                    cb.Enabled = enable;
                    bool vis = true;
                    if (m_filterCqs == CheckState.Checked)
                        vis = enable;
                    cql.Visible = vis;
                    cb.Visible = vis;
                    if (vis)
                    {
                        cql.Invalidate();
                        cb.Invalidate();
                    }
                }
                SizeChanged(null,null);
            } }
        private int m_ctrlIdxOfFirstDupe;

        public void Add(RecentMessage rm, EnableCb enableCb)
        {
            int insertIdx = tlp.Controls.Count;
            if (insertIdx == 0)
                m_ctrlIdxOfFirstDupe = 0;

            // insert in tlp.Controls always at m_ctrlIdxOfFirstDupe
            // if it Is a dupe, then m_ctrlIdxOfFirstDupe is unchanged, otherwise it increments
            //
            // presentationIndex assignment. 
            // non-dupes start at zero and work up.
            // dupes start at LabelsThatFit-1 and work down
            //
            // On add when full....completely different logic.
            // when full, Dupe added at end ...or
            //  non-dupe takes the position of the newest Dupe on the screen, which is then bumped to the end

            CqLabel lb = new CqLabel(rm, colors);
            lb.Font = lblFont;
            lb.BackColor = lblBackColor;
            lb.ForeColor = lblForeColor;
            lb.AutoSize = false;
            lb.enableCb = enableCb;
            CheckBox cb = new CheckBox();
            cb.GotFocus += lb.OnGetFocus;
            cb.LostFocus += lb.OnLostFocus;

            bool isFull = tlp.Controls.Count >= LabelsThatFit * 2;
            tlp.Controls.Add(lb);
            tlp.Controls.Add(cb);
            if (m_ctrlIdxOfFirstDupe < tlp.Controls.Count - 2)
            {
                tlp.Controls.SetChildIndex(cb, m_ctrlIdxOfFirstDupe);
                tlp.Controls.SetChildIndex(lb, m_ctrlIdxOfFirstDupe);
            }
            if (!rm.Dupe)
                m_ctrlIdxOfFirstDupe += 2;

            int presentationIndex;
            int numNonDupes = m_ctrlIdxOfFirstDupe / 2;
            int numDupes = tlp.Controls.Count / 2 - numNonDupes;
            if (!isFull)
                presentationIndex = rm.Dupe ? LabelsThatFit - numDupes : numNonDupes - 1;
            else
            {
                if (rm.Dupe)
                    presentationIndex = numNonDupes + numDupes - 1;
                else
                {
                    presentationIndex = numNonDupes - 1;
                    // Dupe at this position--move to end
                    if (tlp.Controls.Count > m_ctrlIdxOfFirstDupe)
                        PositionEntry(tlp.Controls[m_ctrlIdxOfFirstDupe+1], tlp.Controls[m_ctrlIdxOfFirstDupe], numNonDupes + numDupes - 1);
                }
            }

            bool enabled = enableCb(m_filterCqs);
            lb.Enabled =  cb.Enabled = enabled;
            bool vis = true;
            if (m_filterCqs == CheckState.Checked)
                vis = cb.Enabled;
            lb.Visible = vis;
            cb.Visible = vis;

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
            PositionEntry(cb, lb, presentationIndex);
        }

        private void OnCheck(CheckBox cb, RecentMessage rm, bool onHisFreq)
        {
            if (cb.Checked)
                initiateQso(rm, onHisFreq);
        }
    }

    class CqLabel : FocusDrawingHelper, PanelOfCheckBoxes.PresentAtEnd
    {
        public RecentMessage rm;
        public CallPresentation.EnableCb enableCb;
        RttyRiteColors colors;
        public CqLabel(RecentMessage rm, RttyRiteColors colors)
        {            
            this.rm = rm; 
            this.colors = colors;
        }

        public bool AtEnd { get { return rm.Dupe; } }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (System.Drawing.Brush bck = new System.Drawing.SolidBrush(BackColor))
                e.Graphics.FillRectangle(bck, ClientRectangle);
            System.Drawing.Color fillColor = BackColor;
            bool doBck = rm.Dupe || rm.Mult || !Enabled;
            if (!Enabled)
                fillColor = System.Drawing.Color.LightGray;
            else
            {
                if (rm.Dupe)
                    fillColor = colors.DupeBackGround;
                else if (rm.Mult)
                    fillColor = colors.MultBackGround;
            }
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
