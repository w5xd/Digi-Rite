using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiRite
{
    public partial class SetupForm : Form
    {
       /* DigiRite requires some installation-specific setup parameters.
       */

        private int instanceNumber;
        private bool maySelectDevices;
        private bool maySelectLR;
        private bool maySelectCallUsed;
        private int contestExchangeOrig;
        private MainForm.DigiMode digiModeOrig;
        private bool mustResetState = false;
        public bool MustResetState { get { return mustResetState; } }

        public int whichTxDevice;
        public int whichRxDevice;
        public VfoControl controlSplit;
        public int txHighLimit;
        public bool forceRigUsb;
        public MainForm.DigiMode digiMode = MainForm.DigiMode.FT8;
        public int VfoSplitToPtt;
        public int PttToSound;
 
        public SetupForm(int instanceNumber, bool maySelectDevices, bool maySelectLR, bool maySelectCallUsed)
        {
            this.instanceNumber = instanceNumber; // some of the parameters are for multiple instances on a PC
            this.maySelectDevices = maySelectDevices;
            this.maySelectLR = maySelectLR;
            this.maySelectCallUsed = maySelectCallUsed;
            InitializeComponent();
        }

        private void SetupForm_Load(object sender, EventArgs e)
        {
            digiModeOrig = digiMode;
            BackColor = CustomColors.CommonBackgroundColor;
            groupBox2.BackColor =
            groupBox3.BackColor =
            groupBox1.BackColor = CustomColors.TxBackgroundColor;
            radioButtonInput = (int)(uint)Properties.Settings.Default["AudioInputChannel_" + instanceNumber.ToString()];
            switch (radioButtonInput)
            {
                case 0:
                    radioButtonInputLeft.Checked = true;
                    break;
                case 1:
                default:
                    radioButtonInputRight.Checked = true;
                    break;
            }

            radioButtonOutput = (int)(uint)Properties.Settings.Default["AudioOutputChannel_" + instanceNumber.ToString()];
            switch (radioButtonOutput)
            {
                case 0:
                    radioButtonOutputLeft.Checked = true;
                    break;
                case 1:
                    radioButtonOutputRight.Checked = true;
                    break;
                case 2:
                default:
                    radioButtonOutputMono.Checked = true;
                    break;
            }

            radioButtonInputLeft.Enabled =
                radioButtonInputRight.Enabled =
                radioButtonOutputLeft.Enabled =
                radioButtonOutputRight.Enabled =
                radioButtonOutputMono.Enabled = maySelectLR;

            textBoxCallUsed.Enabled = maySelectCallUsed;
            textBoxCallUsed.Text = Properties.Settings.Default.CallUsed.ToUpper();

            var inDevices = XD.WaveDeviceEnumerator.waveInDevices();
            foreach (string d in inDevices)
                comboBoxWaveIn.Items.Add(d);
            var outDevices = XD.WaveDeviceEnumerator.waveOutDevices();
            foreach (string d in outDevices)
                comboBoxWaveOut.Items.Add(d);
            comboBoxWaveIn.Enabled = maySelectDevices;
            comboBoxWaveOut.Enabled = maySelectDevices;
            labelWriteLogLR.Visible = !maySelectDevices;
            if (maySelectDevices)
            {
                uint rx = MainForm.StringToIndex(Properties.Settings.Default[
                    "AudioInputDevice_" + instanceNumber.ToString()].ToString(), inDevices);
                if (rx >= comboBoxWaveIn.Items.Count)
                    rx = 0;
                comboBoxWaveIn.SelectedIndex = (int)rx;
                uint tx = MainForm.StringToIndex(Properties.Settings.Default[
                    "AudioOutputDevice_" + instanceNumber.ToString()].ToString(), outDevices);
                if (tx >= comboBoxWaveOut.Items.Count)
                    tx = 0;
                comboBoxWaveOut.SelectedIndex = (int)tx;
            }

            textBoxMyGrid.Text = Properties.Settings.Default.MyGrid;

            comboBoxContest.SelectedIndex = Properties.Settings.Default.ContestExchange;
            contestExchangeOrig = Properties.Settings.Default.ContestExchange;

            foreach (String s in MainForm.DefaultAcknowledgements)
                comboBoxAckMsg.Items.Add(s);
            comboBoxAckMsg.SelectedIndex = Properties.Settings.Default.DefaultAcknowlegement;

            // these parameters are per-instance
            switch (controlSplit)
            {
                case VfoControl.VFO_NONE:
                    radioButtonNoVfo.Checked = true;
                    break;
                case VfoControl.VFO_SHIFT:
                    radioButtonShiftTX.Checked = true;
                    break;
                case VfoControl.VFO_SPLIT:
                    radioButtonSplitTX.Checked = true;
                    break;
            }
            if (txHighLimit >= numericUpDownTxMaxHz.Minimum &&
                txHighLimit <= numericUpDownTxMaxHz.Maximum)
                numericUpDownTxMaxHz.Value = txHighLimit;
            checkBoxUSB.Checked = forceRigUsb;

            if (digiMode == MainForm.DigiMode.FT8)
                radioButtonFt8.Checked = true;
            else 
                radioButtonFt4.Checked = true;
            digiModeOrig = digiMode;

            if ((PttToSound >= numericUpDownPttDelay.Minimum) &&
                (PttToSound <= numericUpDownPttDelay.Maximum))
                numericUpDownPttDelay.Value = PttToSound;
            if ((VfoSplitToPtt >= numericUpDownVfoToPtt.Minimum) &&
                (VfoSplitToPtt <= numericUpDownVfoToPtt.Maximum))
                numericUpDownVfoToPtt.Value = VfoSplitToPtt;

            if (Properties.Settings.Default.LeftClickIsMyTx)
                radioButtonR.Checked = true;
            else
                radioButtonL.Checked = true;

            textBoxCQ.Text = Properties.Settings.Default.CQmessage;
            if (textBoxCQ.Text.Length == 0)
                textBoxCQ.Text = "CQ";
            textBoxExchangeToSend.Text = Properties.Settings.Default.ContestMessageToSend;

            mustResetState = false;
            tabPageExchange.BackColor = BackColor;
            tabPageRestarts.BackColor = BackColor;
            tabPageRigControl.BackColor = BackColor;
            tabPageAudioDevices.BackColor = BackColor;
            tabPageOther.BackColor = BackColor;

            comboBoxContest_SelectedIndexChanged(null,null);

            numericUpDownMultiProc.Maximum = Math.Min(Math.Max(System.Environment.ProcessorCount,1),
                                                        MultiDemodulatorWrapper.MAX_MULTIPROC);
            try
            {
                numericUpDownMultiProc.Value = Properties.Settings.Default.MultiProcessCount;
            }
            catch (System.Exception )
            {
                numericUpDownMultiProc.Value = 1;
            }
        }

        // make the user type one in that can be parsed
        public static bool validateGridSquare(String grid)
        {
            grid = grid.ToUpper();
            if (grid.Length < 4)
                return false;
            else
            {
                if (!grid.Substring(0,2).All((char c) => { return c >= 'A' && c <= 'R';}))
                    return false;
                else
                {
                    if (!grid.Substring(2, 2).All(Char.IsDigit))
                        return false;
                    else
                    {
                        if (grid.Length == 4)
                        { return true; } 
                        else if (grid.Length == 6)
                        {
                            if (grid.Substring(4, 2).All((char c) => { return c >= 'A' && c <= 'X'; }))
                                return true;
                            else
                                return false;
                        }
                    }
                }
            }
            return false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (maySelectLR)
            {
                Properties.Settings.Default["AudioInputChannel_" + instanceNumber.ToString()] = (uint)radioButtonInput;
                Properties.Settings.Default["AudioOutputChannel_" + instanceNumber.ToString()] = (uint)radioButtonOutput;
            }
            if (maySelectDevices)
            {
                object selRx = comboBoxWaveIn.SelectedItem;
                if (null != selRx)
                    Properties.Settings.Default["AudioInputDevice_" + instanceNumber.ToString()] = selRx.ToString();
                object selTx = comboBoxWaveOut.SelectedItem;
                if (null != selTx)
                    Properties.Settings.Default["AudioOutputDevice_" + instanceNumber.ToString()] = selTx.ToString();
                whichRxDevice = comboBoxWaveIn.SelectedIndex;
                whichTxDevice = comboBoxWaveOut.SelectedIndex;
            }
            if (maySelectCallUsed)
                Properties.Settings.Default.CallUsed = textBoxCallUsed.Text;

            if (validateGridSquare(textBoxMyGrid.Text))
            {
                string gs = textBoxMyGrid.Text;
                gs = gs.Substring(0,2).ToUpper() + gs.Substring(2);
                if (gs.Length>4)
                    gs = gs.Substring(0,4) + gs.Substring(4).ToLower();
                Properties.Settings.Default.MyGrid = gs;
            }
            
            Properties.Settings.Default.ContestExchange = comboBoxContest.SelectedIndex;
            Properties.Settings.Default.DefaultAcknowlegement = comboBoxAckMsg.SelectedIndex;
            Properties.Settings.Default.LeftClickIsMyTx = radioButtonR.Checked;
            Properties.Settings.Default.CQmessage = textBoxCQ.Text.ToUpper();
            Properties.Settings.Default.ContestMessageToSend = textBoxExchangeToSend.Text.ToUpper();
            Properties.Settings.Default.MultiProcessCount = (ushort)numericUpDownMultiProc.Value;

            DialogResult = DialogResult.OK;
            controlSplit = radioButtonSplitTX.Checked ? VfoControl.VFO_SPLIT : 
                            (radioButtonShiftTX.Checked ? VfoControl.VFO_SHIFT : VfoControl.VFO_NONE);
            if (controlSplit != VfoControl.VFO_NONE)
                txHighLimit = (int)numericUpDownTxMaxHz.Value;
            forceRigUsb = checkBoxUSB.Checked;

            digiMode = radioButtonFt8.Checked ? MainForm.DigiMode.FT8 : MainForm.DigiMode.FT4;
            PttToSound = (int)numericUpDownPttDelay.Value;
            VfoSplitToPtt = (int)numericUpDownVfoToPtt.Value;


            if (digiMode != digiModeOrig)
                mustResetState = true;
            if (contestExchangeOrig != comboBoxContest.SelectedIndex)
                mustResetState = true;

            Close();
        }

        private int radioButtonInput;
        private void radioButtonInput_CheckedChanged(object sender, EventArgs e)
        {  radioButtonInput = ((RadioButton)(sender)).TabIndex;   }

        private int radioButtonOutput;
        private void radioButtonOutput_CheckedChanged(object sender, EventArgs e)
        {  radioButtonOutput = ((RadioButton)(sender)).TabIndex; }

        private void checkBoxSplit_CheckedChanged(object sender, EventArgs e)
        { numericUpDownTxMaxHz.Enabled = !radioButtonNoVfo.Checked;   }

        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
               if (textBoxCallUsed.Enabled && textBoxCallUsed.Text.Length == 0)
                {
                    tabControlTabs.SelectedTab = tabPageExchange;
                    textBoxCallUsed.Focus();
                    MessageBox.Show("Fill in callsign");
                    e.Cancel = true;
                    return;
                }

                if (!validateGridSquare(textBoxMyGrid.Text))
                {
                    tabControlTabs.SelectedTab = tabPageExchange;
                    textBoxMyGrid.Focus();
                    MessageBox.Show("Invalid Grid Square");
                    e.Cancel = true;
                    return;
                }

                 var split = textBoxCQ.Text.ToUpper().Split();
                if ((split.Length < 1) || (split[0] != "CQ"))
                {
                    tabControlTabs.SelectedTab = tabPageExchange;
                    MessageBox.Show("CQ message must start with CQ");
                    textBoxCQ.Focus();
                    e.Cancel = true;
                    return;
                }

                if ((split.Length > 2) || 
                    ((split.Length == 2) &&
                        ((split[1].Length < 2) ||
                        (split[1].Length > 4) ||
                        (split[1].Any((char x) => { return !Char.IsLetter(x); })))))
                {
                    tabControlTabs.SelectedTab = tabPageExchange;
                    MessageBox.Show("Directed CQ must be one word of two, three or four letters");
                    textBoxCQ.Focus();
                    e.Cancel = true;
                    return;
                }

                switch ((ExchangeTypes)comboBoxContest.SelectedIndex)
                {
                    case ExchangeTypes.ARRL_FIELD_DAY:
                    case ExchangeTypes.ARRL_RTTY:
                        if (String.IsNullOrEmpty(textBoxExchangeToSend.Text))
                        {
                            tabControlTabs.SelectedTab = tabPageExchange;
                            MessageBox.Show("The exchange cannot be empty");
                            textBoxExchangeToSend.Focus();
                            e.Cancel = true;
                            return;
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        private void textBoxCQ_TextChanged(object sender, EventArgs e)
        {
            string call = textBoxCallUsed.Text;
            call.ToUpper();
            if ((call.Length >= 4) && (textBoxCQ.Text.ToUpper().Contains(call)))
                MessageBox.Show("Enter CQ and, optionally, a directed CQ string of two to four letters.\r\n\r\nYou call is taken from the box above.");
        }

        private void comboBoxContest_SelectedIndexChanged(object sender, EventArgs e)
        {
            var et = (ExchangeTypes)comboBoxContest.SelectedIndex;
            textBoxExchangeToSend.Enabled = (et == ExchangeTypes.ARRL_FIELD_DAY) || (et == ExchangeTypes.ARRL_RTTY);

            switch ((ExchangeTypes)comboBoxContest.SelectedIndex)
            {
                case ExchangeTypes.ARRL_FIELD_DAY:
                    labelExchangeHint.Text = "number,class and section\r\ne.g. 1A STX";
                    break;
                case ExchangeTypes.ARRL_RTTY:
                    labelExchangeHint.Text = "State, Province, or %";
                    break;
                default:
                    labelExchangeHint.Text = "Grid from above and/or\r\ncomputed dB report";
                    break;
            }

            var hintLocation = labelExchangeHint.Location;
            hintLocation.Y = textBoxExchangeToSend.Location.Y + textBoxExchangeToSend.Size.Height /2 - labelExchangeHint.Size.Height/2;
            labelExchangeHint.Location = hintLocation;
        }
    }
}
