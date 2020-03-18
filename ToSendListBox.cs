using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DigiRite
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
                                var qsi1Check = GetItemChecked(counter);
                                var qsi0Check = GetItemChecked(counter-1);
                                object temp = Items[counter];
                                this.Items[counter] = this.Items[counter - 1];
                                this.Items[counter - 1] = temp;
                                swapped = true;
                                if (qsi1Check != qsi0Check)
                                {
                                    // The CheckedListBox ItemChecked stayed
                                    // at the old item index (surprise!)
                                    // fix that
                                    SetItemChecked(counter, qsi0Check);
                                    SetItemChecked(counter - 1, qsi1Check);
                                }
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
        public QueuedToSendListItem(string s, QsoInProgress q, QsoSequencer.MessageSent ms = null)
        {
            this.s = s;
            this.q = q;
            this.messageSentCb = ms;
        }
        public override string ToString()
        { return s; }
        public String MessageText { get { return s; } set { s = value; } }
        public QsoSequencer.MessageSent MessageSent { get { return messageSentCb; } }
        public QsoInProgress q;
        private string s;
        private QsoSequencer.MessageSent messageSentCb;
    }

    // priority goes to Qsos in progress with fewer CyclesSinceMessaged
    class QueuedToSendListItemComparer : IComparer<KeyValuePair<int, QueuedToSendListItem>>
    {
        public int Compare(KeyValuePair<int, QueuedToSendListItem> x, KeyValuePair<int, QueuedToSendListItem> y)
        {
            // more recently heard from sort to smaller numbers 
            if (x.Value.q.CyclesSinceMessaged < y.Value.q.CyclesSinceMessaged)
                return -1;
            else if (x.Value.q.CyclesSinceMessaged > y.Value.q.CyclesSinceMessaged)
                return 1;
            else // same age. return sort of original keys
                return x.Key < y.Key ? -1 : (x.Key == y.Key ? 0 : 1);
        }
    }

    class SortedQueuedToSendListItem : QueuedToSendListItem
    {
        public QsosPanel qp;
        public SortedQueuedToSendListItem(string s, QsoInProgress q, QsosPanel qp, QsoSequencer.MessageSent ms) : base(s,q, ms)
        { this.qp = qp; }
    }
}
