using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WriteLogDigiRite
{
    class ToSendListBox : System.Windows.Forms.CheckedListBox
    {   /* Custom sort the checkListBoxToSend */
        public ToSendListBox() : base()
        {        }

        public new void Sort()
        {
            // https://stackoverflow.com/questions/3012647/custom-listbox-sorting
            if (this.Items.Count > 1)
            {
                bool swapped;
                do
                {
                    // bubble sort
                    int counter = this.Items.Count - 1;
                    swapped = false;
                    while (counter > 0)
                    {
                        SortedQueuedToSendListItem qsi1 = this.Items[counter] as SortedQueuedToSendListItem;
                        SortedQueuedToSendListItem qsi0 = this.Items[counter-1] as SortedQueuedToSendListItem;
                        if ((null != qsi1) && (null != qsi0))
                        {
                            if (qsi1.qp.QsoPriority(qsi1.q) <
                                qsi0.qp.QsoPriority(qsi0.q))
                            {
                                object temp = Items[counter];
                                this.Items[counter] = this.Items[counter - 1];
                                this.Items[counter - 1] = temp;
                                swapped = true;
                            }
                        }
                        counter -= 1;
                    }
                }
                while (swapped);
            }
        }
    }

    class QueuedToSendListItem
    {
        public QueuedToSendListItem(string s, QsoInProgress q)
        {
            this.s = s;
            this.q = q;
        }
        public override string ToString()
        { return s; }
        public String MessageText { get { return s; } set { s = value; } }
        public QsoInProgress q;
        private string s;
    }

    class SortedQueuedToSendListItem : QueuedToSendListItem
    {
        public QsosPanel qp;
        public SortedQueuedToSendListItem(string s, QsoInProgress q, QsosPanel qp) : base(s,q)
        { this.qp = qp;  }
    }
}
