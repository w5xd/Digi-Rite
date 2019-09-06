using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WriteLogDigiRite
{
    class ConversationListBox : ListBox
    {   /* Custom draw the ConverstationHistory list box.   */
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if ((e.Index >= 0) && (e.Index < Items.Count))
            {
                ListBoxConversationItem lbci = Items[e.Index] as ListBoxConversationItem;
                if (null != lbci)
                {
                    string t = lbci.ToString();
                    Color backColor = BackColor;
                    switch (lbci.origin)
                    {
                        case Conversation.Origin.INITIATE:
                            backColor = Color.PaleGreen;
                            break;
                        case Conversation.Origin.TO_ME:
                            backColor = Color.Pink;
                            break;
                        case Conversation.Origin.TO_OTHER:
                            backColor = Color.White;
                            break;
                        case Conversation.Origin.TRANSMIT:
                            backColor = Color.Yellow;
                            break;
                        default:
                            break;
                    }
                    using (SolidBrush sb = new SolidBrush(backColor))
                        e.Graphics.FillRectangle(sb, e.Bounds);
                    using (SolidBrush sb = new SolidBrush(e.ForeColor))
                        e.Graphics.DrawString(t, Font, sb, e.Bounds.Left, e.Bounds.Top);
                    e.DrawFocusRectangle();
                    return;
                }
            }
            base.OnDrawItem(e);
        }

    }

    public static class Conversation
    { public enum Origin { TRANSMIT, TO_ME, TO_OTHER, INITIATE }; }

    class ListBoxConversationItem
    {
        string s;
        public Conversation.Origin origin;
        public ListBoxConversationItem(string s, Conversation.Origin o)
        {
            this.s = s;
            origin = o;
        }
        public override string ToString()
        {  return s;}
    }

}
