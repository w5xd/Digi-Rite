using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace DigiRite
{
    public partial class RcvrForm : Form
    {
        /* Show:
        ** a) incoming decoded messages. all of them.
        ** b) a spectrum (or waterfall if WriteLog is available)
        ** c) decoder parameter controls.
         */
        public RcvrForm(MainForm mf, int inst)
        {
            mainForm = mf;
            instanceNumber = inst;
            InitializeComponent();
        }

        MainForm mainForm;
        int instanceNumber;
        LogFile logfile;

        public uint CYCLE { set {
                ClockLabel cl = labelClockAnimation as ClockLabel;
                if (null != cl)
                    cl.CYCLE = value;
                } }

        public LogFile logFile { set { logfile = value; } }

        private WaterfallManager waterfallManager;
        private WriteLog.IWaterfallV3 iwaterfall
        {
            get
            {
                if (null != waterfallManager)
                    return waterfallManager.iwaterfall;
                return null;
            }
        }
        private WriteLog.IAnnotations annotations
        {
            get
            {
                if (null != waterfallManager)
                    return waterfallManager.annotations;
                return null;
            }
        }
        private WriteLog.IPeakRx peak { get {
                if (null != waterfallManager) return waterfallManager.peak;
                return null;
            } }
        private static uint AUDIO_BUFFER_SAMPLE_COUNT = 4096;
        private void XcvrForm_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("{0}-{1}", this.Text, instanceNumber);
            labelPower.Text = "";
            ClockLabel cl = new ClockLabel();
            cl.Location = labelClockAnimation.Location;
            cl.Size = labelClockAnimation.Size;
            panelRightGain.Controls.Remove(labelClockAnimation);
            panelRightGain.Controls.Add(cl);
            labelClockAnimation.Dispose();
            labelClockAnimation = cl;

            var verticalBarLabel = new VerticalBarLabel();
            verticalBarLabel.Location = labelVUmeter.Location;
            verticalBarLabel.Size = labelVUmeter.Size;
            panelRightGain.Controls.Remove(labelVUmeter);
            panelRightGain.Controls.Add(verticalBarLabel);
            labelVUmeter.Dispose();
            labelVUmeter = verticalBarLabel;

            comboBoxnDepth.SelectedIndex = Properties.Settings.Default.Decode_ndepth - 1;
            checkBoxEnableAP.Checked = Properties.Settings.Default.Decode_lft8apon;

            panelRightGain.BackColor =
            panel1.BackColor = CustomColors.CommonBackgroundColor;

            waterfallManager = new WaterfallManager();
            if (waterfallManager.OnLoad(logfile, chartSpectrum, labelWaterfall, splitContainerMain.Panel2.Controls, mainForm))
            {
                iwaterfall.MaxHz = MaxDecodeFrequency;
                iwaterfall.MinHz = MinDecodeFrequency;
                myDemod.SetAudioSamplesCallback(null, AUDIO_BUFFER_SAMPLE_COUNT,
                    AUDIO_BUFFER_SAMPLE_COUNT, iwaterfall.GetAudioProcessor());
                buttonOptions.Enabled = true;
                chartSpectrum.Dispose();
                chartSpectrum = null;
            }
            if (null != annotations)
            {
                checkBoxAnnotate.Checked = annotations.Visible;
                checkBoxAnnotate.Enabled = true;
            }
            if (null != peak)
                timerVU.Enabled = true;

            prevSplitter = splitContainerMain.SplitterDistance;
            locationToSave = Location;
            sizeToSave = Size;
        }

        public void SetFixedFont(System.Drawing.Font font)
        {
            listBoxReceived.Font = font;
        }

        MultiDemodulatorWrapper myDemod = null;
        public MultiDemodulatorWrapper demodParams {  set { 
                myDemod = value;
                if (null != value)
                {
                    if (null != iwaterfall)
                    {   //plumb the demodulator's audio stream to the waterfall
                        myDemod.SetAudioSamplesCallback(null,
                            4096,
                            4096,
                            iwaterfall.GetAudioProcessor());
                    }
                    DecodeFrequencyRangeChanged(null,null);
                }
            } }

        public void StoreProperties()
        {
            if (null != waterfallManager)
                waterfallManager.StoreProperties();
        }

        XD.WaveDevicePlayer waveDevicePlayer;
        public XD.WaveDevicePlayer Player { set { 
                waveDevicePlayer = value; 
                float gain = waveDevicePlayer.Gain;
                bool gainOK = gain >= 0;
                if (gainOK)
                    trackBarRxGain.Value = (int)(gain * 100);
                trackBarRxGain.Enabled = gainOK;
                } }

        public int MinDecodeFrequency { get {
                return (int)numericUpDownMinFreq.Value;}
            set { numericUpDownMinFreq.Value = value; }
        }
        public int MaxDecodeFrequency { get {
                return (int)numericUpDownMaxFreq.Value; }
            set { numericUpDownMaxFreq.Value = value; }
        }

        #region chart spectrum
        const int NUM_TO_AVERAGE = 5;
        // this magic number comes from deep inside the wsjtx sources.
        // Hint: stop at the bin for 5000Hz when doing a 2**14 bin FFT on 12000KHz data.
        const int NUM_FREQUENCY_BINS = 6827; // each bin is 12000/(2**14) Hz
        private float[] spectrum = new float[NUM_FREQUENCY_BINS];
        private float[][] averagedSpectrum = new float[NUM_TO_AVERAGE][];
        private int numAveragedSpectrum = 0;
        private int movingAverageIdx = 0;
        public void DisplaySpectrum(XDft.Demodulator demodulator)
        {
            if (IsDisposed)
                return;
            if (null == chartSpectrum)
                return;
            for (int j = 0; j < NUM_TO_AVERAGE; j++)
            {
                if (null == averagedSpectrum[j])
                    averagedSpectrum[j] = new float[NUM_FREQUENCY_BINS];
            }
            float power = 0;

            /* wsjtx-2.0.1 implements its spectrum calculation in a non-reentrant
            ** way. The result is that ONLY one instance of XDft.Decoder
            ** can call GetSignalSpectrum else you won't get results that are
            ** of any use. You may switch from one instance to another only
            ** if you're willing to put up with an undefined length of time
            ** it takes for the new one to properly synchronize (a full 15sec
            ** cycle is enough, but before that, you don't know what you're getting.) */
            uint samples = demodulator.GetSignalSpectrum(spectrum, ref power);

            for (int i = 0; i < NUM_FREQUENCY_BINS; i++)
                averagedSpectrum[movingAverageIdx][i] = spectrum[i];
            movingAverageIdx += 1;
            if (movingAverageIdx >= NUM_TO_AVERAGE)
                movingAverageIdx = 0;
            numAveragedSpectrum += 1;
            if (numAveragedSpectrum >= NUM_TO_AVERAGE)
                numAveragedSpectrum = NUM_TO_AVERAGE;

            var series = chartSpectrum.Series[0];
            series.Points.Clear();
            for (int i = 0; i < 4096; i += 1)
            {
                double sample = 0;
                for (int j = 0; j < numAveragedSpectrum; j++)
                    sample += averagedSpectrum[j][i];
                series.Points.AddXY((5000 * i) / NUM_FREQUENCY_BINS, sample / numAveragedSpectrum);
            }
            labelPower.Text = power.ToString();
        }
        #endregion

        private void DecodeFrequencyRangeChanged(object sender, EventArgs e)
        {
            if (null == myDemod)
                return;
            if (Object.ReferenceEquals(sender, numericUpDownMinFreq))
            {
                int max = (int)numericUpDownMaxFreq.Value - 60;
                if ((numericUpDownMinFreq.Value > max))
                {
                    if (max < numericUpDownMinFreq.Minimum)
                        max = (int)numericUpDownMinFreq.Minimum;
                    numericUpDownMinFreq.Value = max;
                    return;
                }
            }
            else if (Object.ReferenceEquals(sender, numericUpDownMaxFreq))
            {
                int min = (int)numericUpDownMinFreq.Value + 60;
                if (numericUpDownMaxFreq.Value < min)
                {
                    if (min > numericUpDownMaxFreq.Maximum)
                        min = (int)numericUpDownMaxFreq.Maximum;
                    numericUpDownMaxFreq.Value = min;
                    return;
                }
            }
            myDemod.nfa = (int)numericUpDownMinFreq.Value;
            myDemod.nfb = (int)numericUpDownMaxFreq.Value;

            if (null != iwaterfall)
            {
                iwaterfall.MaxHz = (int)numericUpDownMaxFreq.Value;
                iwaterfall.MinHz = (int)numericUpDownMinFreq.Value;
            }
        }
        
        #region waterfall
        public int TxHz {
            set {
                if (null != iwaterfall)
                    iwaterfall.TxFreqHz = value;
            }
        }
        public int RxHz {
            set {
                if (null != iwaterfall)
                    iwaterfall.RxFreqHz = value;
            }
        }
        #endregion

        private void comboBoxnDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Decode_ndepth = comboBoxnDepth.SelectedIndex + 1;
            if (null != myDemod)
                myDemod.ndepth = comboBoxnDepth.SelectedIndex + 1;
        }

        private void checkBoxEnableAP_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Decode_lft8apon = checkBoxEnableAP.Checked;
            if (null != myDemod)
                myDemod.lft8apon = checkBoxEnableAP.Checked;
        }

        class DisplayReceivedMessage
        {
            public DisplayReceivedMessage(XDpack77.Pack77Message.ReceivedMessage rm)
            {   m = rm;            }
            public override string ToString() { return m.ToString(); }
            private XDpack77.Pack77Message.ReceivedMessage m;
        }

        const int MAX_RECEIVED_LOOKBACK = 10000; // number of entries in the history listbox

        public void OnReceived(XDpack77.Pack77Message.ReceivedMessage m)
        {
            while (listBoxReceived.Items.Count > MAX_RECEIVED_LOOKBACK)
                listBoxReceived.Items.RemoveAt(0);
            listBoxReceived.Items.Add(new DisplayReceivedMessage(m));
            int visibleItems = listBoxReceived.ClientSize.Height / listBoxReceived.ItemHeight;
            listBoxReceived.TopIndex = Math.Max(listBoxReceived.Items.Count - visibleItems + 1, 0);

            if (null != iwaterfall)
            {
                var call = m.Pack77Message as XDpack77.Pack77Message.ToFromCall;
                if (null != call)
                {
                    int tag = 1;
                    var tgString = m.TimeTag;
                    if (!String.IsNullOrEmpty(tgString))
                        Int32.TryParse(tgString, out tag);
                    if (null != annotations)
                        annotations.AddAnnotation(tag, (int)m.Hz, call.FromCall, 0);
                }
            }

        }

        bool markedWaterfallThisCycle = false;
        public void OnClock(uint tick, bool isTransmit)
        {
            labelClock.Text = (tick/10).ToString();
            if (tick < 10)
            {
                if (!markedWaterfallThisCycle)
                {
                    if (null != iwaterfall)
                    {
                        var dt = System.DateTime.UtcNow;
                        int tag = dt.Second + dt.Minute * 100 + dt.Hour * 10000;
                        iwaterfall.MarkCurrentRaster(tag);
#if DEBUG
                        int f = (MinDecodeFrequency + MaxDecodeFrequency) / 2;
                        annotations.AddAnnotation(tag, f, "TEST " + f.ToString(), 0);
#endif

                    }
                }
                markedWaterfallThisCycle = true;
            }
            else
                markedWaterfallThisCycle = false;
            ClockLabel cl = labelClockAnimation as ClockLabel;
            if (null != cl)
            {
                cl.Tenths = tick;
                cl.AmTransmit = isTransmit;
            }
        }
        private object listBoxReceivedSelectedItem = null;
        private void listBoxReceived_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var item = listBoxReceived.IndexFromPoint(e.Location);
                if (item != ListBox.NoMatches)
                {
                    contextMenuStripOnReceived.Items[1].Enabled = true;
                    listBoxReceivedSelectedItem = listBoxReceived.Items[item];
                    listBoxReceived.SelectedIndex = item;
                }
                contextMenuStripOnReceived.Show(this, new System.Drawing.Point(e.X, e.Y));//place the menu at the pointer position
            }
        }
           
        private void XcvrForm_FormClosing(object sender, FormClosingEventArgs e)
        {   // don't "close" this form. Minimize it.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }
       
        private System.Drawing.Point locationToSave;
        public System.Drawing.Point LocationToSave {
            get { return locationToSave; }
        }
        private System.Drawing.Size sizeToSave;
        public System.Drawing.Size SizeToSave {
             get { return sizeToSave;} }

        public int SplitterDistance {
            get { return splitContainerMain.SplitterDistance; }
            set {
                if (value >= splitContainerMain.Panel1MinSize &&
                    value <= splitContainerMain.Width - splitContainerMain.Panel2MinSize)
                    {
                        splitContainerMain.SplitterDistance = value;
                        prevSplitter = value;
                    }
                }
        }

#region splitter handler kludge
        int prevSplitter = -1;
        bool armSplitterMoved = false;
        private void splitContainerMain_SizeChanged(object sender, EventArgs e)
        {
            if (prevSplitter > 0)
                splitContainerMain.SplitterDistance = prevSplitter;
        }

        private void splitContainerMain_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            armSplitterMoved = true;
        }

        private void splitContainerMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (armSplitterMoved)
            {
                armSplitterMoved = false;
                prevSplitter = splitContainerMain.SplitterDistance;
            }
        }
#endregion

        private void XcvrForm_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                locationToSave = Location;
        }

        private void XcvrForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                sizeToSave = Size;
        }

        private void trackBarRxGain_Scroll(object sender, EventArgs e)
        {
            waveDevicePlayer.Gain = (float)(trackBarRxGain.Value / 100.0);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBoxReceived.Items.Clear();
        }

        private void copyItemToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxReceivedSelectedItem != null)
                System.Windows.Forms.Clipboard.SetText(listBoxReceivedSelectedItem.ToString());
        }

        private void contextMenuStripOnReceived_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            listBoxReceived.SelectedIndex = ListBox.NoMatches;
        }

        private void timerVU_Tick(object sender, EventArgs e)
        {
            var p = peak;
            if (null != p)
            {
                var vu = (labelVUmeter as VerticalBarLabel);
                vu.Value = p.GetPeakRxAndReset();
            }
        }

        private void checkBoxAnnotate_CheckedChanged(object sender, EventArgs e)
        {
            if (null != annotations)
                annotations.Visible = checkBoxAnnotate.Checked;
        }

        private void buttonOptions_Click(object sender, EventArgs e)
        {
            if (waterfallManager != null)
                waterfallManager.ShowOptionsDialog(this);
        }

    }
    
}
