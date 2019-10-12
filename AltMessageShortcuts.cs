using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiRite
{
    class AltMessageShortcuts
    {   
        /* Class to present an array of labels, each with a keyboard
         * accelerator that is a digit. Pressing the accelerator checks
         * the corresponding item in the checked list box.
         */
        class MnemonicLabel : System.Windows.Forms.Label
        {
            public delegate void OnMnemonic();
            private OnMnemonic onMnemonic;
            public MnemonicLabel(OnMnemonic onMnemonic)
            {
                this.onMnemonic = onMnemonic;
            }
            protected override bool ProcessMnemonic(char charCode)
            {
                bool ret = base.ProcessMnemonic(charCode);
                if (ret)
                    onMnemonic();
                return ret;
            }
        };

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.CheckedListBox listBox;
        private System.Windows.Forms.Label[] buttons;
        public AltMessageShortcuts(System.Windows.Forms.Panel p, System.Windows.Forms.CheckedListBox lb)
        {
            panel = p;
            listBox = lb;
            setup();
        }

        public void setup()
        {
            int h = listBox.GetItemHeight(0);
            var sz = panel.Size;

            int btnCount = sz.Height / h;
            if (null != buttons)
            {
                foreach (var b in buttons)
                    b.Dispose();
            }
            buttons = new MnemonicLabel[btnCount];

            for (int i = 0; i < btnCount; i++)
            {
                int ii = i;
                buttons[i] = new MnemonicLabel(() =>
                {
                    if (listBox.Items.Count > ii)
                        listBox.SetItemChecked(ii, true);
                });
                panel.Controls.Add(buttons[i]);
                buttons[i].Size = new System.Drawing.Size(h - 1, h - 1);
                var lbFont = listBox.Font;
                System.Drawing.Font f = new System.Drawing.Font(lbFont.FontFamily, lbFont.Height * 2 / 3);
                buttons[i].Font = f;
                buttons[i].Location = new System.Drawing.Point(0, h * i);
                buttons[i].Text = String.Format("&{0}", i + 1);
                buttons[i].Margin = new System.Windows.Forms.Padding(0);
                buttons[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                buttons[i].UseMnemonic = true;
                buttons[i].TabStop = true;
                buttons[i].TabIndex = 1 + i;
                buttons[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                buttons[i].Visible = false;
            }
            listBox.TabIndex = btnCount + 1;
            listBox.Size = new System.Drawing.Size(sz.Width - h, sz.Height);
        }

        public void Populate()
        {   // show only those accelerators that correspond to entries in the list box.
            int i = 0;
            for (; i < listBox.Items.Count && i < buttons.Length; i++)
                buttons[i].Visible = true;
            for (; i < buttons.Length; i++)
                buttons[i].Visible = false;
        }
    }
}
