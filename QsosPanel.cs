using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WriteLogDigiRite
{
    /*DigiRite has multiple QSOs "in progress" concurrently.
     * This is shown visually in a PanelOfCheckBoxes, specialized here
     * with some mouse event handling and on-screen drawing.
     */
    class QsosPanel : PanelOfCheckBoxes
    {
        public QsosPanel(Panel tp, Label modelLabel, CheckBox modelCheckbox) 
            : base(tp, modelLabel, modelCheckbox)
        {  
            tp.Controls.Clear(); 
            tp.MouseDown += PanelRightMouseDown;
        }

        #region callbacks
        public delegate void FillAlternatives(QsoInProgress qp);
        FillAlternatives fillAlternativesCb;
        public FillAlternatives fillAlternatives { set { fillAlternativesCb = value; } get => fillAlternativesCb; }

        public delegate void LogAsIs(QsoInProgress qp);
        LogAsIs logAsIsCb;
        public LogAsIs logAsIs { set { logAsIsCb = value; } get => logAsIsCb; }

        public delegate void QsoActiveChanged(QsoInProgress qp);
        QsoActiveChanged qsoActiveChangedCb;
        public QsoActiveChanged qsoActiveChanged { set { qsoActiveChangedCb = value; } get => qsoActiveChangedCb; }

        public delegate void OnRemovedQso(QsoInProgress qp);
        OnRemovedQso onRemovedQsoCb;
        public OnRemovedQso onRemovedQso { set { onRemovedQsoCb = value; } get => onRemovedQsoCb; }

        public delegate bool IsCurrentCycle(QsoInProgress qp);
        IsCurrentCycle isCurrentCycleCb;
        public IsCurrentCycle isCurrentCycle { set { isCurrentCycleCb = value; } get => isCurrentCycleCb; }

        public delegate void OrderChanged();
        OrderChanged orderChangedCb;
        public OrderChanged orderChanged { set { orderChangedCb = value; } get => orderChangedCb; }
        #endregion

        // the underlying data is in the Control array
        public List<QsoInProgress> QsosInProgress {
            get {
                List<QsoInProgress> ret = new List<QsoInProgress>();
                foreach (var v in tlp.Controls)
                {
                    QsoInProgressLabel ql = v as QsoInProgressLabel;
                    if (null != ql)
                        ret.Add(ql.qso);
                }
                return ret;
            }
        }

        public void Add(QsoInProgress qp)
        {  
            int addedIndex = tlp.Controls.Count / 2;
            QsoInProgressLabel lb = new QsoInProgressLabel(qp, colors);
            lb.Font = lblFont;
            lb.BackColor = lblBackColor;
            lb.ForeColor = lblForeColor;
            lb.AutoSize = false;
            lb.isCurrentCycleCb = isCurrentCycleCb;
            QsoCb cb = new QsoCb();
            cb.GotFocus += lb.OnGetFocus;
            cb.LostFocus += lb.OnLostFocus;

            // set active and checked on add
            cb.Checked = true;
            cb.TabStop = false;
            qp.Active = true;
            qp.OnChangedCb += new QsoInProgress.OnChanged(() =>
            {
                lb.Invalidate();
                if (cb.Checked != qp.Active)
                    cb.Checked = qp.Active;
            });

            cb.onRightMouse = new MouseEventHandler((object o, MouseEventArgs target) =>
            {
                RightMouseDown(o, qp, target);
            });
            lb.onRightMouse = new MouseEventHandler((object o, MouseEventArgs target) =>
            {
                RightMouseDown(o, qp, target);
            });

            tlp.Controls.Add(lb);
            tlp.Controls.Add(cb);

            lb.onClick += new EventHandler((object o, EventArgs e) =>
            {
                cb.Focus();
            });
            lb.onGetFocusCb += new FocusDrawingHelper.OnGetFocusCb(() => {
                if (null != fillAlternativesCb)fillAlternativesCb(qp);
                });
            cb.CheckedChanged += new EventHandler((object o, EventArgs e) =>
            {
                OnCheck(o as CheckBox, qp);
            });

            lb.Size = lblSize;
            cb.Size = cbSize;
            PositionEntry(cb, lb, addedIndex);
        }

        public QsoInProgress FirstActive {
            get {
                foreach (var v in tlp.Controls)
                {
                    QsoInProgressLabel ql = v as QsoInProgressLabel;
                    if ((null != ql) && ql.qso.Active)
                        return ql.qso;
                }
                return null;
            }
        }

        private void OnCheck(CheckBox cb, QsoInProgress qp)
        {
            SetQsoActive(qp, cb.Checked);
            fillAlternativesCb(qp);
        }

        public void Remove(QsoInProgress qp)
        {
            foreach (var c in tlp.Controls)
            {
                QsoInProgressLabel ql = c as QsoInProgressLabel;
                if (null == ql)
                    continue;
                if (Object.ReferenceEquals(qp, ql.qso))
                {
                    int idx = tlp.Controls.IndexOf(ql);
                    IDisposable d1 = tlp.Controls[idx];
                    tlp.Controls.RemoveAt(idx);
                    IDisposable d2 = tlp.Controls[idx];
                    tlp.Controls.RemoveAt(idx);
                    d1.Dispose();
                    d2.Dispose();
                    SizeChanged(null,null); // redraw
                    if (null != orderChangedCb)
                        orderChangedCb();
                    return;
                }
            }
        }

        public int QsoPriority(QsoInProgress qp)
        {   // linear search is not the fastest...
            // ...but we're just not going to have more
            // than a few dozen
            int ret = 0;
            for (int i = 0; i < tlp.Controls.Count; i++)
            {
                QsoInProgressLabel ql = tlp.Controls[i] as QsoInProgressLabel;
                if (null == ql)
                    continue;
                if (Object.ReferenceEquals(qp, ql.qso))
                    return 1 + ret;
                else
                    ret += 1;
            }
            return ret;
        }

        private void SetQsoActive(QsoInProgress qp, bool active)
        {
            if (qp.Active != active)
            {
                qp.Active = active;
                qsoActiveChangedCb(qp);
                for (int i = 0; i < tlp.Controls.Count; i += 1)
                {
                    QsoInProgressLabel lb = tlp.Controls[i] as QsoInProgressLabel;
                    if (null == lb)
                        continue;
                    if (!Object.ReferenceEquals(lb.qso, qp))
                        continue;
                    QsoCb cb = tlp.Controls[i+1] as QsoCb;
                    if (null != cb)
                        cb.Checked = active;
                    break;
                }
            }
        }

        public void RefreshOnScreen()
        { tlp.Invalidate(true); }

        private void PanelRightMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            ContextMenuStrip inProgressRightMouse;
            List<ToolStripItem> toolStripItems = new List<ToolStripItem>();
            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "&Inactive: remove all",
                    Tag = new Action(() => removeAllInactive())
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }
            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "&Logged: remove all",
                    Tag = new Action(() => removeAllLogged())
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }
            inProgressRightMouse = new ContextMenuStrip();
            inProgressRightMouse.Items.AddRange(toolStripItems.ToArray());
            System.Drawing.Point pos = Cursor.Position;
            inProgressRightMouse.Show(pos);
            inProgressRightMouse.Visible = true;
        }

        private void RightMouseDown(object sender, QsoInProgress q, MouseEventArgs e)
        {
            if (null != e // keyboard event forwards with null
                && e.Button != MouseButtons.Right)
                return;

            Control control = sender as Control;
            int index = tlp.Controls.IndexOf(control) / 2;

            ContextMenuStrip inProgressRightMouse;
            List<ToolStripItem> toolStripItems = new List<ToolStripItem>();

            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "&TX on their frequency",
                    Checked = q.Message.Hz == q.TransmitFrequency,
                    Tag = new Action(() =>
                    {
                        q.TransmitFrequency = q.TransmitFrequency == q.Message.Hz ?
                            0 : q.Message.Hz;
                    })
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }

            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "&Active",
                    Checked = q.Active,
                    Tag = new Action(() =>
                    {
                        SetQsoActive(q, !q.Active);
                    })
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }

            if (index > 0)
            {
                var tsi = new ToolStripMenuItem
                {
                    Text = "Move QSO &Up",
                    Tag = new Action(() =>
                    {
                        int i = index * 2;
                        Control o1 = tlp.Controls[i];
                        Control o2 = tlp.Controls[1 + i];
                        tlp.Controls.SetChildIndex(o1, i-2);
                        tlp.Controls.SetChildIndex(o2, i-1);
                        SizeChanged(null, null);
                        if (null != orderChangedCb)
                            orderChangedCb();
                    })
                };
                tsi.Click += inProgressContext_Click;
                toolStripItems.Add(tsi);
            }

            if (index < (tlp.Controls.Count / 2) - 1)
            {
                var tsi = new ToolStripMenuItem
                {
                    Text = "Move QSO &Down",
                    Tag = new Action(() =>
                    {
                        int i = index * 2;
                        Control o1 = tlp.Controls[i];
                        Control o2 = tlp.Controls[1 + i];
                        tlp.Controls.SetChildIndex(o2, 3 + i);
                        tlp.Controls.SetChildIndex(o1, 2 + i);
                        SizeChanged(null,null);
                        if (null != orderChangedCb)
                            orderChangedCb();
                    })
                };
                tsi.Click += inProgressContext_Click;
                toolStripItems.Add(tsi);
            }
            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "Log &QSO as-is",
                    Tag = new Action(() =>
                    {
                        logAsIsCb(q);
                        SetQsoActive(q, false);
                    })
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }

            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "Alternative &Messages",
                    Tag = new Action(() => fillAlternativesCb(q))
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }
            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "&Remove from List",
                    Tag = new Action(() => onRemovedQsoCb(q))
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }
            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "&Inactive: remove all",
                    Tag = new Action(() => removeAllInactive())
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }
            {
                var contextItem = new ToolStripMenuItem
                {
                    Text = "&Logged: remove all",
                    Tag = new Action(() => removeAllLogged())
                };
                contextItem.Click += inProgressContext_Click;
                toolStripItems.Add(contextItem);
            }
            inProgressRightMouse = new ContextMenuStrip();
            inProgressRightMouse.Items.AddRange(toolStripItems.ToArray());
            System.Drawing.Point pos = (e != null) ?
                Cursor.Position :  /* mouse */
                control.PointToScreen(control.Location) + control.Size; /* keyboard */
            inProgressRightMouse.Show(pos);
            inProgressRightMouse.Visible = true;
        }

        private void inProgressContext_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tmi = sender as ToolStripMenuItem;
            if (null != tmi)
            {
                var action = tmi.Tag as Action;
                if (null != action)
                    action();
            }
        }
        
        private delegate bool ChooseThisQso(QsoInProgress q);
        private void removeByCondition(ChooseThisQso choose)
        {
            bool foundOne = false;
            for (; ; )
            {
                foundOne = false;
                foreach (var c in tlp.Controls)
                {
                    QsoInProgressLabel ql = c as QsoInProgressLabel;
                    if (null == ql)
                        continue;
                    if (choose(ql.qso))
                    {
                        int idx = tlp.Controls.IndexOf(ql);
                        tlp.Controls.RemoveAt(idx);
                        tlp.Controls.RemoveAt(idx);
                        foundOne = true;
                        break;
                    }
                }
                if (!foundOne)
                    break;
            }
            if (foundOne && null != orderChangedCb)
                orderChangedCb();
            SizeChanged(null, null);
        }

        public void removeAllInactive()
        {  removeByCondition((QsoInProgress q) => !q.Active); }

        public void removeAllLogged()
        { removeByCondition((QsoInProgress q) => q.IsLogged); }
    }

    class QsoInProgressLabel : FocusDrawingHelper
    {
        RttyRiteColors colors;
        QsoInProgress qp;
        public QsoInProgressLabel(QsoInProgress qp, RttyRiteColors colors)
        {
            this.qp = qp;
            this.colors = colors;
        }

        public System.EventHandler onClick;
        public MouseEventHandler onRightMouse;
        public QsosPanel.IsCurrentCycle isCurrentCycleCb;
        public QsoInProgress qso { get { return qp; } }

        protected override void OnPaint(PaintEventArgs e)
        {
            bool isCurrent = true;
            if (null != isCurrentCycleCb)
                isCurrent = isCurrentCycleCb(qp);
            System.Drawing.Color fillColor = BackColor;
            if (!isCurrent)
                fillColor = System.Drawing.Color.Gray;
            using (System.Drawing.Brush fb = new System.Drawing.SolidBrush(fillColor))  
                e.Graphics.FillRectangle(fb, ClientRectangle);
            bool doBck = false;

            if (isCurrent)
            {
                doBck = qp.Dupe || qp.Mult; ;
                if (qp.Dupe)
                    fillColor = colors.DupeBackGround;
                else if (qp.Mult)
                    fillColor = colors.MultBackGround;
                if (doBck)
                {
                    String toRender = qp.ToString();
                    int toSkip = 2; // skip first two characters unconditionally
                    while (toSkip < toRender.Length && Char.IsWhiteSpace(toRender[toSkip]))
                        toSkip += 1;
                    string first = toRender.Substring(0, toSkip - 1);
                    string second = toRender.Substring(toSkip);
                    second = second.Trim();
                    var sze1 = TextRenderer.MeasureText(first, Font);
                    var sze2 = TextRenderer.MeasureText(second, Font);
                    System.Drawing.Rectangle toFill = ClientRectangle;
                    toFill.Size = sze2;
                    toFill.X += sze1.Width;
                    using (System.Drawing.Brush fcb = new System.Drawing.SolidBrush(fillColor))
                        e.Graphics.FillRectangle(fcb, toFill);
                }
            }

            TextRenderer.DrawText(e.Graphics,
                            qp.ToString(),
                            Font,
                            ClientRectangle,
                            ForeColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            DrawFocusRect(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
                onClick(this, mevent);
            else
                onRightMouse(this, mevent);
        }


        // label control doesn't do what we want on these events:
        protected override void OnMouseUp(MouseEventArgs mevent)
        { } // disable

        protected override void OnClick(EventArgs e)
        { } // disable
    }

    class QsoCb : CheckBox
    {
        public MouseEventHandler onRightMouse;
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
                base.OnMouseDown(mevent);
            else
                onRightMouse(this, mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
                base.OnMouseUp(mevent);
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.P)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                onRightMouse(this, null);
                return;
            }
            base.OnKeyDown(e);
        }
    }
}
