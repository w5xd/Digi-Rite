using System;
using System.Windows.Forms;
using System.Drawing;

namespace DigiRite
{
    class VerticalBarLabel : Label
    {
        private ushort m_Value = 0;
        public ushort Value
        {
            get { return m_Value; }
            set { m_Value = value; Invalidate(); }
        }
        protected const ushort RED_BOUNDARY = 0x4000;
        protected float fraction(ushort v)
        {
                 return ((float)v) / 0x7FFF;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Color fillColor = BackColor;
            System.Drawing.Brush fb = new SolidBrush(fillColor);
            using (fb)
                e.Graphics.FillRectangle(fb, ClientRectangle);
            Pen blackPen = new Pen(Color.Black);
            using (blackPen)
            {
                if (Value != 0)
                {
                    Brush greenBrush = new SolidBrush(Color.Green);
                    using (greenBrush)
                    {
                        var f = fraction(Value);
                        var Height = Size.Height - 2;
                        var Width = Size.Width - 2;
                        Rectangle barRect = new Rectangle(1, 1+ (int)((1 - f) * Height), Width, (int)(f * Height));
                        bool anyRed = Value > RED_BOUNDARY;
                        if (anyRed)
                        {
                            Brush redBrush = new SolidBrush(Color.Red);
                            using (redBrush)
                                e.Graphics.FillRectangle(redBrush, barRect);
                        }
                        ushort GreenHeight = anyRed ? RED_BOUNDARY : Value;
                        var greenF = fraction(GreenHeight);
                        Rectangle greenRect = new Rectangle(1, 1+(int)((1 - greenF) * Height), Width, (int)(greenF * Height));
                        e.Graphics.FillRectangle(greenBrush, greenRect);
                        {
                            e.Graphics.DrawRectangle(blackPen, barRect);
                        }
                    }
                }
                e.Graphics.DrawRectangle(blackPen, new Rectangle(0, 0, Size.Width-1, Size.Height-1));
            }
        }
    }
}
