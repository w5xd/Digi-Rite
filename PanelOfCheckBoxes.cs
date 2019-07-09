using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WriteLogDigiRite
{
    class PanelOfCheckBoxes
    {
        protected PanelOfCheckBoxes(Panel tp, Label modelLabel, CheckBox modelCheckbox)
        {
            tlp = tp;
            // grab the presentation properties that we will use for copies
            Control cb = modelCheckbox;
            Control lb = modelLabel;
            cbSize = cb.Size;
            cbLocation0 = cb.Location;
            lblSize = lb.Size;
            lblFont = lb.Font;
            lblForeColor = lb.ForeColor;
            lblBackColor = lb.BackColor;
            lblLocation0 = lb.Location;
            tlp.SizeChanged += new System.EventHandler(SizeChanged);
        }
        
        protected RttyRiteColors colors = new RttyRiteColors();
        protected int rowsThatfit = 1;
        protected Panel tlp;
        protected System.Drawing.Size cbSize;
        protected System.Drawing.Size lblSize;
        protected System.Drawing.Font lblFont;
        protected System.Drawing.Point lblLocation0;
        protected System.Drawing.Point cbLocation0;

        protected System.Drawing.Color lblForeColor;
        protected System.Drawing.Color lblBackColor;

        protected int VerticalPitch { get { return lblSize.Height; } }
        protected int HorizontalPitch { get { return lblLocation0.X + lblSize.Width; } }

        protected void PositionEntry(Control cb, Control lb, int idx)
        {
            int rowNum = idx % rowsThatfit;
            int colNum = idx / rowsThatfit;
            System.Drawing.Point cbLocation = cbLocation0;
            System.Drawing.Point lblLocation = lblLocation0;
            cbLocation.Y += rowNum * VerticalPitch;
            lblLocation.Y += rowNum * VerticalPitch;
            cbLocation.X += colNum * HorizontalPitch;
            lblLocation.X += colNum * HorizontalPitch;
            lb.Location = lblLocation;
            cb.Location = cbLocation;
        }

        public void SizeChanged(object sender, EventArgs e)
        {
            rowsThatfit = (tlp.Size.Height - 1) / VerticalPitch;
            if (rowsThatfit < 1)
                rowsThatfit = 1;
            bool first = true;
            int which = 0;
            for (int i = 0; i < tlp.Controls.Count; i++)
            {
                Control lb = tlp.Controls[i];
                lb.TabIndex = i++;
                Control cb = tlp.Controls[i];
                if (cb.Enabled)
                {
                    cb.TabIndex = i;
                    cb.TabStop = first;
                    first = false;
                }
                else // !Enabled pushed to end of keyboard tab order. 
                    cb.TabIndex = tlp.Controls.Count + i; 
                if (cb.Visible)
                    PositionEntry(cb, lb, which++);
            }
        }

        public void Reset()
        {
            Control[] controls = new Control[tlp.Controls.Count];
            tlp.Controls.CopyTo(controls, 0);
            tlp.Controls.Clear();
            foreach (var c in controls)
               (c as IDisposable)?.Dispose();
        }

        public void UncheckAll()
        {
            foreach (Control c in tlp.Controls)
            {
                CheckBox cb = c as CheckBox;
                if (cb != null)
                    cb.Checked = false;
            }
        }
    }

    class RttyRiteColors
    {
        System.Drawing.Color multBackGround = System.Drawing.Color.FromArgb(255, 255, 255, 0);
        System.Drawing.Color dupeBackGround = System.Drawing.Color.FromArgb(255, 255, 192, 192);
        System.Drawing.Color rcvdText = System.Drawing.Color.FromArgb(255, 0, 0, 0);

        public RttyRiteColors()
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\W5XD\\writelog.ini\\RttyRite");
            if (null != rk)
            {
                fromWriteLogRegistry(rk, "MultBackgroundColor", ref multBackGround);
                fromWriteLogRegistry(rk, "DupeBackgroundColor", ref dupeBackGround);
                fromWriteLogRegistry(rk, "ReceivedTextColor", ref rcvdText);
            }
        }

        void fromWriteLogRegistry(Microsoft.Win32.RegistryKey rk, string valuename, ref System.Drawing.Color update)
        {
            object m = rk.GetValue(valuename);
            int rgb;
            if ((m != null) && Int32.TryParse(m.ToString(), out rgb))
            {
                int r; int g; int b;
                r = 255 & rgb;
                g = 255 & (rgb >> 8);
                b = 255 & (rgb >> 16);
                update = System.Drawing.Color.FromArgb(255, r, g, b);
            }
        }

        public System.Drawing.Color MultBackGround {
            get {    /* [rttyrite]MultBackgroundColor */
                return multBackGround;
            }
        }
        public System.Drawing.Color DupeBackGround {
            get {    /* [rttyrite]DupeBackgroundColor */
                return dupeBackGround;
            }
        }
        public System.Drawing.Color ReceivedTextColor {
            get {   /* [rttyrite]ReceivedTextColor */
                return rcvdText;
            }
        }
    };

    class FocusDrawingHelper : Label
    {
        bool haveFocus = false;
        public delegate void OnGetFocusCb();
        public OnGetFocusCb onGetFocusCb;
        public void OnGetFocus(object sender, EventArgs e)
        {
            haveFocus = true;
            Invalidate();
            if (null != onGetFocusCb) onGetFocusCb();
        }
        public void OnLostFocus(object sender, EventArgs e)
        {
            haveFocus = false;
            Invalidate();
        }
        protected void DrawFocusRect(PaintEventArgs e)
        {
            if (haveFocus)
            {
                using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1));
                }
            }
        }
    }
}
