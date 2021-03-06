﻿using System;
using System.Windows.Forms;
using System.Drawing;

namespace DigiRite
{
    class ClockLabel : Label
    {
        public ClockLabel()
        {
            AutoSize = false;
        }
        private uint tenths = 0;
        public uint Tenths { get => tenths; set { 
                if (tenths == value) return;
                tenths = value; 
                Invalidate();} }

        bool amTransmit = false;
        public bool AmTransmit {
            get => amTransmit; set {
                if (value == amTransmit) return;
                amTransmit = value;
                Invalidate();
            }
        }
        public uint CYCLE = 150;
        protected override void OnPaint(PaintEventArgs e)
        {
            Color fillColor = BackColor;
            System.Drawing.Brush fb = new SolidBrush(fillColor);
            using (fb)
                e.Graphics.FillRectangle(fb, ClientRectangle);
            uint s = tenths;
            s += 1; if (s >= CYCLE) s -= CYCLE;
            Brush circleBrush = new SolidBrush(amTransmit ? Color.Red : Color.Green);
            using (circleBrush)
            {
                Rectangle clockRect = new Rectangle(new Point(1, 1), new Size(this.Size.Width - 2, this.Size.Height - 2));
                Pen blackPen = new Pen(Color.Black);
                using (blackPen)
                {
                    if (s < CYCLE)
                    {
                        if (s == 0)
                            e.Graphics.FillEllipse(circleBrush, clockRect);
                        else
                        {
                            float startAngle = 270;
                            float sweepAngle = (float)(360.0 * s / CYCLE);
                            e.Graphics.FillPie(circleBrush, clockRect, startAngle, sweepAngle);
                            e.Graphics.DrawPie(blackPen, clockRect, startAngle, sweepAngle);
                        }
                    }
                    e.Graphics.DrawEllipse(blackPen, clockRect);
                }
            }
        }
    }
}
