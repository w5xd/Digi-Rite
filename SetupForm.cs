using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WriteLogDigiRite
{
    public partial class SetupForm : Form
    {
       /* DigiRite requires some installation-specific setup parameters.
       */

        private int instanceNumber;
        private bool maySelectDevices;
        private bool maySelectLR;
        private bool maySelectCallUsed;

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

            DialogResult = DialogResult.OK;
            controlSplit = radioButtonSplitTX.Checked ? VfoControl.VFO_SPLIT : 
                            (radioButtonShiftTX.Checked ? VfoControl.VFO_SHIFT : VfoControl.VFO_NONE);
            if (controlSplit != VfoControl.VFO_NONE)
                txHighLimit = (int)numericUpDownTxMaxHz.Value;
            forceRigUsb = checkBoxUSB.Checked;

            digiMode = radioButtonFt8.Checked ? MainForm.DigiMode.FT8 : MainForm.DigiMode.FT4;
            PttToSound = (int)numericUpDownPttDelay.Value;
            VfoSplitToPtt = (int)numericUpDownVfoToPtt.Value;

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
                if (!validateGridSquare(textBoxMyGrid.Text))
                {
                    MessageBox.Show("Invalid Grid Square");
                    e.Cancel = true;
                }
            }
        }
    }
}
