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

        private static int SortOrder(RecentMessage left, RecentMessage right)
        {
            if (left.Dupe && !right.Dupe)
                return 1;
            if (!left.Dupe && right.Dupe)
                return -1;
            return right.Message.SignalDB - left.Message.SignalDB;
        }
        
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
                SortControls();
                SizeChanged(null,null);
            } }

        private class SortPanelOrder : System.Collections.Generic.IComparer<int>
        {   // sort by position to draw
            public SortPanelOrder(Control[] ar)
            {  m_ar = ar; }
            public int Compare(int x, int y)
            {
                CqLabel left = m_ar[x] as CqLabel;
                CqLabel right = m_ar[y] as CqLabel;
                return SortOrder(left.rm, right.rm);
            }
            private Control[] m_ar;
        }

        void SortControls()
        {
            Control []controls = new Control[tlp.Controls.Count];
            tlp.Controls.CopyTo(controls,0);        
            
            // sort the integer array without messing with tlp
            int []panelOrder = new int[controls.Length/2];
            for (int i = 0; i < panelOrder.Length; i++)
                panelOrder[i] = i *2;
            Array.Sort<int>(panelOrder, new SortPanelOrder(controls));

            // install the new order
            for (int i = 0; i < panelOrder.Length; i++)
            {
                int idx = panelOrder[i];
                Control cq = controls[idx];
                Control cbc = controls[idx+1];
                int TwoI = i * 2;
                tlp.Controls.SetChildIndex(cq, TwoI);
                tlp.Controls.SetChildIndex(cbc, 1 + TwoI);
            }
        }

        public void Add(RecentMessage rm, EnableCb enableCb)
        {
            CqLabel lb = new CqLabel(rm, colors);
            lb.Font = lblFont;
            lb.BackColor = lblBackColor;
            lb.ForeColor = lblForeColor;
            lb.AutoSize = false;
            lb.enableCb = enableCb;
            CheckBox cb = new CheckBox();
            cb.GotFocus += lb.OnGetFocus;
            cb.LostFocus += lb.OnLostFocus;

            tlp.Controls.Add(lb);
            tlp.Controls.Add(cb);
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
            SortControls();
            SizeChanged(null,null);
        }

        private void OnCheck(CheckBox cb, RecentMessage rm, bool onHisFreq)
        {
            if (cb.Checked)
                initiateQso(rm, onHisFreq);
        }
    }

    class CqLabel : FocusDrawingHelper
    {
        public RecentMessage rm;
        public CallPresentation.EnableCb enableCb;
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
