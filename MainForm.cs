using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DigiRite
{
    public enum ExchangeTypes { GRID_SQUARE, DB_REPORT, ARRL_FIELD_DAY, ARRL_RTTY, GRID_SQUARE_PLUS_REPORT};

    public enum VfoControl { VFO_NONE, VFO_SPLIT, VFO_SHIFT };

    public partial class MainForm : Form, QsoQueue.IQsoQueueCallBacks, Qso2MessageExchange.IQsoQueueCallBacks
    {
        public static String[] DefaultAcknowledgements = { "73", "RR73", "RRR" };
        public enum DigiMode { FT8, FT4 };

        // These are the objects needed to receive and send FT8.
        MultiDemodulatorWrapper demodulatorWrapper;
        private XD.WaveDevicePlayer waveDevicePlayer;
        private XD.WaveDeviceTx deviceTx = null; // to modulate
        private XDft.GeneratorContext genContext;
        private DigiMode digiMode = DigiMode.FT8;

        private RcvrForm rxForm;
        private String myCall;
        private String myBaseCall; // in case myCall is nonstandard
        private int instanceNumber;
        private String instanceRegKeyName;

        private uint RxInDevice = 0;
        private uint TxOutDevice = 0;
        private bool SetupMaySelectDevices = true;
        private bool SetupMaySelectLR = true;

        private IQsoQueue qsoQueue;
        private LogFile logFile;
        private LogFile conversationLogFile;
        private bool sendInProgress = false;
        private bool SendInProgress {
            get { return sendInProgress; }
            set { sendInProgress = value;
                if (!value)
                    CqMessageInProgress = false;}
        }
        private AltMessageShortcuts altMessageShortcuts;

        // what we put in listToMe and cqlist
        private CallPresentation cqListOdd;
        private CallPresentation cqListEven;
        private CallPresentation toMe;
        private QsosPanel qsosPanel;

        private VfoControl controlVFOsplit = VfoControl.VFO_NONE;
        private bool forceRigUsb = false;
        private int TxHighFreqLimit = 0;
        private uint noLoggerSerialNumber = 0;

#if DEBUG
        List<string> simulatorLines;
        int simulatorTimeOrigin = -1;
        DateTime simulatorStart;
        int simulatorNext = 0;
        bool autoAnswerAllCQsFromSimulator = false;
#endif

        public MainForm(int instanceNumber)
        {
            this.instanceNumber = instanceNumber;
            instanceRegKeyName = String.Format("Software\\W5XD\\WriteLog\\DigiRite-{0}", instanceNumber);
            InitializeComponent();
            labelPtt.Text = "";
        }

        #region Logger customization

        private DigiRiteLogger.IDigiRiteLogger logger;

        // currentBand is only used to distinguish messages from a CALL
        // that are part of different QSOs because they are on a different band.
        // Even that distinction rarely happens because DigiRite typically
        // discards old messages from old QSOs over time. But if you happen to
        // work a CALL on one band, changes bands, and immediately work the same
        // CALL, you want currentBand to be different when you change bands, else
        // the new messages from CALL will be assigned to the QSO on the old band.
        private short currentBand = 0;
        public DigiMode CurrentMode { get { return digiMode; } }
        public void SetWlEntry(object e)
        {   // WriteLog is the only logger that calls here.
            var wl = new DigiRiteLogger.WriteLog(instanceNumber);
            labelPtt.Text = wl.SetWlEntry(e);
            logger = wl;
        }
        #endregion

        public static uint StringToIndex(string MySetting, List<string> available)
        {
            uint ret = 0;
            string cmp = MySetting.ToUpper();
            for (ret = 0; ret < available.Count; ret++)
            {
                if (available[(int)ret].ToUpper().Contains(cmp))
                    break;
            }
            return ret;
        }

        private string LogFilePath { get { return demodulatorWrapper.AppDirectoryPath + "DigiRite.log"; } }

        const double AUDIO_SLIDER_SCALE = 12;
        private bool InitSoundInAndOut()
        {
            // The objects implement IDisposable. Failing to
            // dispose of one after quitting using it 
            // leaves its Windows resources
            // allocated until garbage collection.
            rxForm.demodParams = null;
            timerFt8Clock.Enabled = false;
            timerSpectrum.Enabled = false;
            timerCleanup.Enabled = false;
            if (null != demodulatorWrapper)
                demodulatorWrapper.Dispose();
            if (null != waveDevicePlayer)
                waveDevicePlayer.Dispose();
            waveDevicePlayer = null;
            if (null != deviceTx)
                deviceTx.Dispose();
            deviceTx = null;
            if (null != logFile)
                logFile.Dispose();
            logFile = null;
            if (null != conversationLogFile)
                conversationLogFile.Dispose();
            conversationLogFile = null;

            ushort multiProc = Properties.Settings.Default.MultiProcessCount;
            if (multiProc > System.Environment.ProcessorCount)
                multiProc = (ushort)System.Environment.ProcessorCount;
            demodulatorWrapper = new MultiDemodulatorWrapper(instanceNumber, multiProc);

            // The demodulator invokes the wsjtx decoder
            // the names of its parameters are verbatim from the wsjt-x source code.
            // Don't ask this author what they mean.
            demodulatorWrapper.nftx = 1500;
            demodulatorWrapper.nfqso = 1500;
            demodulatorWrapper.nfa = 200;
            demodulatorWrapper.nfb = 6000;

            if (Properties.Settings.Default.Decode_ndepth < 1)
                Properties.Settings.Default.Decode_ndepth = 1;
            if (Properties.Settings.Default.Decode_ndepth > 3)
                Properties.Settings.Default.Decode_ndepth = 1;
            demodulatorWrapper.ndepth = Properties.Settings.Default.Decode_ndepth;
            demodulatorWrapper.lft8apon = Properties.Settings.Default.Decode_lft8apon;
            demodulatorWrapper.nQSOProgress = 5;
            demodulatorWrapper.digiMode = digiMode == DigiMode.FT8 ? XDft.DigiMode.DIGI_FT8 : XDft.DigiMode.DIGI_FT4;

            // When the decoder finds an FT8 message, it calls us back...
            // ...on a foreign thread. Call BeginInvoke to get back on this one. See below.
            demodulatorWrapper.DemodulatorResultCallback = new XDft.DemodResult(Decoded);

            logFile = new LogFile(LogFilePath);
            String conversationLog = demodulatorWrapper.AppDirectoryPath + "Conversation.log";
            conversationLogFile = new LogFile(conversationLog, false);

            rxForm.logFile = logFile;

            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(instanceRegKeyName);
            if (null == rk)
                rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(instanceRegKeyName);
            using (rk)
            {
                uint channel = (uint)Properties.Settings.Default["AudioInputChannel_" + instanceNumber.ToString()];
                if (waveDevicePlayer != null)
                    waveDevicePlayer.Dispose();
                waveDevicePlayer = new XD.WaveDevicePlayer();
                if (!waveDevicePlayer.Open(RxInDevice, channel, demodulatorWrapper.GetRealTimeRxSink()))
                {
                    MessageBox.Show("Failed to open wave input");
                    waveDevicePlayer.Dispose();
                    waveDevicePlayer = null;
                    return false;
                }
                else
                {
                    if (fromRegistryValue(rk, "RxInputGain", out float x) && x >= 0 && x <= 1)
                        waveDevicePlayer.Gain = x;
                    rxForm.demodParams = demodulatorWrapper;
                    waveDevicePlayer.Resume();
                    rxForm.Player = waveDevicePlayer;
                }

                deviceTx = new XD.WaveDeviceTx();
                channel = (uint)Properties.Settings.Default["AudioOutputChannel_" + instanceNumber.ToString()];
                if (!deviceTx.Open(TxOutDevice, channel))
                {
                    MessageBox.Show("Failed to open wave output");
                    deviceTx.Dispose();
                    deviceTx = null;
                    return false;
                }
                deviceTx.SoundSyncCallback = new XD.SoundBeginEnd(AudioBeginEnd);
                if (fromRegistryValue(rk, "TxOutputGain", out float txg) && txg >= 0 && txg <= 1)
                    deviceTx.Gain = txg;
                float gain = deviceTx.Gain;
                bool gainOK = gain >= 0;
                labelTxValue.Text = "";
                if (gainOK)
                {   // not sure why the windows volume slider don't
                    // really work with linear commands, but here we go:
                    int v = trackValueFromGain(gain);
                    trackBarTxGain.Value = v;
                    labelTxValue.Text = trackBarTxGain.Value.ToString();
                }
                trackBarTxGain.Enabled = gainOK;
                timerFt8Clock.Enabled = true;
                timerSpectrum.Enabled = true;
                timerCleanup.Enabled = true;
                return true;
            }
        }
        #region TX output gain
        int trackValueFromGain(float gain)
        {
            double g = trackBarTxGain.Maximum + Math.Log(gain) * AUDIO_SLIDER_SCALE / Math.Log(2);
            int v = (int)g;
            if (v < trackBarTxGain.Minimum)
                v = trackBarTxGain.Minimum;
            return v;
        }
        float gainFromTrackValue(int v)
        {
            return (float)Math.Pow(2.0, (v - trackBarTxGain.Maximum) / AUDIO_SLIDER_SCALE);
        }
        #endregion

        #region received message interactions

        private List<XDpack77.Pack77Message.ReceivedMessage> recentMessages =
            new List<XDpack77.Pack77Message.ReceivedMessage>();

        private DateTime watchDogTime; // the dog sleeps only for so long
        private const int MAX_NUMBER_OF_PACK77_CHARS = 36; // truncate decoder strings
        private string DECODE_SEPARATOR = "~  ";
        private char MESSAGE_SEPARATOR;
        private void OnReceived(String s, int cycle)
        {   // When the FT8 decoder is invoked, it may find 
            // multiple signals in the stream. Each is notified by
            // a separate string here. An empty string is sent
            // at the end of the decoding session.
            if (!String.IsNullOrEmpty(s))
            {
                OneAtATime(new OneAtATimeDel(() =>
                {
                    int v = s.IndexOf(DECODE_SEPARATOR);
                    // "020000  -9  0.4  500 ~  CQ RU W5XD EM10                         "
                    if (v >= 0)
                    {
                        logFile.SendToLog(s);
                        string msg = s.Substring(v + 3);
                        if (msg.Length > MAX_NUMBER_OF_PACK77_CHARS)
                            msg = msg.Substring(0, MAX_NUMBER_OF_PACK77_CHARS);
                        int i3 = 0; int n3 = 0;
                        bool[] c77 = null;
                        XDft.Generator.pack77(msg, ref i3, ref n3, ref c77);
                        // kludge...see if pack failed cuz of hashed call
                        // This works around a bug in wsjtx's pack77 routine.
                        // If/when that bug is fixed, this code may be removed
                        if ((i3 == 0) && (n3 == 0))
                        {   // free text...see if removing <> makes it parse
                            string changedMessage = msg;
                            bool changed = false;
                            for (; ; )
                            {
                                int idx = changedMessage.IndexOfAny(new char[] { '<', '>' });
                                if (idx >= 0)
                                {
                                    changed = true;
                                    changedMessage = changedMessage.Substring(0, idx)
                                        + changedMessage.Substring(idx + 1);
                                }
                                else
                                    break;
                            }
                            if (changed)
                                XDft.Generator.pack77(changedMessage, ref i3, ref n3, ref c77);
                            // END kludge.
                        }
                        // have a look at the packing type. i3 and n3
                        XDpack77.Pack77Message.ReceivedMessage rm =
                            XDpack77.Pack77Message.ReceivedMessage.CreateFromReceived(i3, n3, s.Substring(0, v), msg, cycle, MESSAGE_SEPARATOR, ft4MsecOffset);
                        if (rm == null)
                            return; // FIXME. some messages we can't parse

                        // recentMessages retains only matching TimeTag's
                        if (recentMessages.Any() && recentMessages.First().TimeTag != rm.TimeTag)
                            recentMessages.Clear();

                        // discard message decodes that we already have
                        foreach (var m in recentMessages)
                            if (m.Match(rm)) return;

                        rxForm.OnReceived(rm);
                        recentMessages.Add(rm);

                        // certain kinds of messages are promoted to the checkbox lists
                        XDpack77.Pack77Message.ToFromCall toFromCall = rm.Pack77Message as XDpack77.Pack77Message.ToFromCall;
                        String toCall = toFromCall?.ToCall;
                        bool directlyToMe = (toCall != null) && ((toCall == myCall) || (toCall == myBaseCall));

                        if (directlyToMe)
                            watchDogTime = DateTime.UtcNow;

                        short mult = 0;
                        bool dupe = false;
                        String fromCall = toFromCall?.FromCall;
                        RecentMessage recentMessage;
                        if (fromCall != null && null != logger)
                        {   // dupe check if we can
                            logger.CheckDupeAndMult(fromCall, digiMode == DigiMode.FT8 ? "FT8" : "FT4", rm.Pack77Message, out dupe, out mult);
                        }
                        recentMessage = new RecentMessage(rm, dupe, mult > 0);

                        bool isConversation = false;
                        string callQsled = (rm.Pack77Message as XDpack77.Pack77Message.QSL)?.CallQSLed;
                        if (!String.IsNullOrEmpty(toCall))
                            qsoQueue.MessageForMycall(recentMessage, directlyToMe,
                                    callQsled, currentBand,
                                    checkBoxRespondAny.Checked || (checkBoxRespondNonDupe.Checked && !dupe),
                                    new IsConversationMessage((Conversation.Origin origin) =>
                                        {   // qsoQueue liked this message. log it
                                            isConversation = true;
                                            string toLog = s.Substring(0, v + 3) + msg;
                                            listBoxConversation.Items.Add(new ListBoxConversationItem(toLog, origin));
                                            conversationLogFile.SendToLog(toLog);
                                            ScrollListBoxToBottom(listBoxConversation);
                                        }));
                        if (!isConversation)
                        {   // nobody above claimed this message
                            if (directlyToMe)
                                toMe.Add(new RecentMessage(rm, dupe, mult > 0), (CheckState x) => { return true; });
                            else if (!String.Equals(fromCall, myCall) &&  // decoder is hearing our own
                                        !String.Equals(fromCall, myBaseCall) &&  // transmissions
                                    ((rm.Pack77Message as XDpack77.Pack77Message.IsCQ)?.SolicitsAnswers ?? false))
                            {
                                CallPresentation cqList = (cycle & 1) == 0 ? cqListEven : cqListOdd;
                                cqList.Add(recentMessage, (CheckState cqOnly) => {
                                    // enable the checkbox if: its a CQ, or if CheckState is Unchecked
                                    if (cqOnly == CheckState.Unchecked) return true; // everything shows in this mode
                                    // else if its not a CQ , return false
                                    else return null != toCall && toCall.Length >= 2 && toCall.Substring(0, 2) == "CQ"; }
                                    );
#if DEBUG
                                if (autoAnswerAllCQsFromSimulator)
                                    cqList.InitiateQsoCb(recentMessage);
#endif
                            }
                        }
                    }
                }));
            }
            else
            {
                if (digiMode == DigiMode.FT4)
                {
                    ft4DecodeOffsetIdx += 1;
                    if (ft4DecodeOffsetIdx < ft4DecodeOffsetMsec.Length)
                    {
                        ft4MsecOffset = 1e-3f * (float)((int)ft4DecodeOffsetMsec[ft4DecodeOffsetIdx] - (int)FT4_DECODER_CENTER_OFFSET_MSEC);
                        bool started = demodulatorWrapper.DecodeAgain(cycleNumber, ft4DecodeOffsetMsec[ft4DecodeOffsetIdx]);
                    }
                }
            }
        }

        private void ScrollListBoxToBottom(ListBox lb)
        {
            int visibleItems = lb.ClientSize.Height / lb.ItemHeight;
            lb.TopIndex = Math.Max(1 + lb.Items.Count - visibleItems, 0);
        }

        #endregion

        #region transmit management

        private int MAX_MESSAGES_PER_CYCLE { get { 
                int ret = (int)numericUpDownStreams.Value; 
                if ((decimal)ret != numericUpDownStreams.Value)
                    ret += 1;
                return ret;
                } }
        private float AMP_REDUCE_PER_STREAM {
            get {
                return numericUpDownStreams.Value == 1.1m ? 0.15f : 1.0f;
            }}
        private bool IS_AMP_REDUCED_PER_STREAM {
            get {
                return numericUpDownStreams.Value == 1.1m ? true : false;
            }
        }

        // empirically determined to "center" in the time slot
        private const int FT8_TX_AFTER_ZERO_MSEC = 210;
        private const int FT4_TX_AFTER_ZERO_MSEC = 210;
        private const ushort FT4_MULTIPLE_DECODE_OFFSET_MSEC = 400;
        private const int FT4_MULTIPLE_DECODE_COUNT = 1; // don't bother with sliding decode window with wsjtx-2.1.0-rc5
        private const short DEFAULT_DECODER_LOOKBACK_MSEC = 100;
        private const ushort FT4_DECODER_CENTER_OFFSET_MSEC = (FT4_MULTIPLE_DECODE_OFFSET_MSEC * (FT4_MULTIPLE_DECODE_COUNT / 2));
        private int UserVfoSplitToPtt = 550;
        private int UserPttToSound = 20;
        private int VfoSetToTxMsec = 550;
        private int FT_CYCLE_TENTHS = 150;
        private int FT_GAP_HZ = 60;
        private const int MAX_MULTI_STREAM_INCREMENT = 3;

        private void AfterNmsec(Action d, int msec)
        {
            var timer = new Timer { Interval = msec };
            timer.Tick += new EventHandler((o, e) =>
                {
                    timer.Enabled = false;
                    d();
                    timer.Dispose();
                });
            timer.Enabled = true;
        }

        private delegate void GenMessage(string msg, ref string msgSent, ref int[] itone, ref bool[] ftbits);
        private GenMessage genMessage;
        private bool[] transmittedForQSOLastCycle = new bool[2];
        private delegate DateTime GetNowTime();
        private int consecutiveTransmitCycles = 0;
        private bool CqMessageInProgress = false;
        private void transmitAtZero(bool allowLate = false, GetNowTime getNowTime = null)
        {   // right now we're at zero second in the cycle.
            if ((digiMode == DigiMode.FT4) && allowLate)
                return;
            if (null == genMessage)
                return;
            DateTime toSend = getNowTime == null ? DateTime.UtcNow : getNowTime();
            int nowTenths = toSend.Second * 10 + toSend.Millisecond / 100;
            int cyclePosTenths = nowTenths % FT_CYCLE_TENTHS;
            bool nowOdd = ((nowTenths / FT_CYCLE_TENTHS) & 1) != 0;
            int seconds = nowTenths;
            seconds /= FT_CYCLE_TENTHS;
            seconds *= FT_CYCLE_TENTHS; // round back to nearest 
            seconds /= TENTHS_IN_SECOND;
            int lastCycleIndex = nowOdd ? 0 : 1;
            // can't transmit two consecutive cycles, one odd and one even
            bool onUserSelectedCycle = nowOdd == radioButtonOdd.Checked;
            if (consecutiveTransmitCycles >= 2)
            {
                consecutiveTransmitCycles = 0;
                return;
            }
            List<QueuedToSendListItem> toSendList = new List<QueuedToSendListItem>();
            // scan the checkboxes and decide what to send
            if (checkBoxManualEntry.Checked && onUserSelectedCycle)
            {   // the manual entry is not associated with a QSO
                String ts = textBoxMessageEdit.Text.ToUpper();
                if (!String.IsNullOrEmpty(ts))
                {   // if there is typed in text, send it
                    checkBoxManualEntry.Checked = false;
                    toSendList.Add(new QueuedToSendListItem(ts, null));
                }
            }

            for (int i = 0; i < listBoxAlternatives.Items.Count; i++)
            {   // alternative messages are next on priority list after manual
                if (toSendList.Count >= MAX_MESSAGES_PER_CYCLE)
                    break;
                if (listBoxAlternatives.GetItemChecked(i))
                {
                    QueuedToSendListItem li = listBoxAlternatives.Items[i] as QueuedToSendListItem;
                    bool sendOdd = ((li.q.Message.CycleNumber + 1) & 1) != 0;
                    if (sendOdd == nowOdd)
                    {
                        toSendList.Add(li);
                        listBoxAlternatives.SetItemChecked(i, false);
                        li.q.OnSentAlternativeMessage();
                    }
                    break;
                }
            }

            // double list search is to maintain the priority order in listBoxInProgress.
            // the order things appear in checkedListBoxToSend is irrelevant.
            Dictionary<int, QueuedToSendListItem> inToSend = new Dictionary<int, QueuedToSendListItem>();
            int k = 0;
            var inProgress = qsosPanel.QsosInProgress;
            for (int i = 0; i < inProgress.Count; i++)
            {   // first entries are highest priority
                QsoInProgress q = inProgress[i];
                if (null == q)
                    continue;   // manual entry goes this way
                if (!q.Active)
                    continue;
                bool sendOdd = ((q.Message.CycleNumber + 1) & 1) != 0;
                if (sendOdd != nowOdd)
                    continue; // can't send this one cuz we're on wrong cycle
                for (int j = 0; j < checkedlbNextToSend.Items.Count; j++)
                {
                    if (!checkedlbNextToSend.GetItemChecked(j))
                        continue;   // present, but marked to skip
                    QueuedToSendListItem qli = checkedlbNextToSend.Items[j] as QueuedToSendListItem;
                    if ((null != qli) && Object.ReferenceEquals(qli.q, q))
                        inToSend.Add(k++, qli);
                }
            }
            
            // push those we haven't heard from to end of list, recent calls to beginning
            var sortedByRecentActivity = inToSend.OrderBy(item => item, new QueuedToSendListItemComparer());
            foreach (var qli in sortedByRecentActivity)
            {
                if (toSendList.Count >= MAX_MESSAGES_PER_CYCLE)
                    break;
                checkedlbNextToSend.Items.Remove(qli);
                if (toSendList.Any((qalready) => {
                    if (null != qalready && null != qalready.q && null != qli.Value.q)
                        return String.Equals(qli.Value.q.HisCall, qalready.q.HisCall);
                    return false; }
                    ))
                    continue; // already a send to this callsign. don't allow another
                toSendList.Add(qli.Value);
            }

            bool anyToSend = toSendList.Any();
            int thisCycleIndex = nowOdd ? 1 : 0;
            transmittedForQSOLastCycle[thisCycleIndex] = anyToSend;
            if (anyToSend)
            {
                consecutiveTransmitCycles += 1;
                if (consecutiveTransmitCycles >= 2)
                {
                    // force all Qsos in progress on "other" cycle to inactive
                    for (int i = 0; i < inProgress.Count; i++)
                    {   // first entries are highest priority
                        QsoInProgress q = inProgress[i];
                        if (null == q)
                            continue;   // manual entry goes this way
                        if (!q.Active)
                            continue;
                        bool sendOdd = ((q.Message.CycleNumber + 1) & 1) != 0;
                        if (sendOdd != nowOdd)
                            q.Active = false;
                    }
                }
            }
            else
                consecutiveTransmitCycles = 0;

            int cqMode = comboBoxCQ.SelectedIndex;
            bool onlyCQ = cqMode == 1 && !toSendList.Any();
            if (onUserSelectedCycle && toSendList.Count < MAX_MESSAGES_PER_CYCLE &&
                    (onlyCQ || cqMode == 2))
            {   // only CQ if we have nothing else to send
                string cq = "CQ";
                /* 77-bit pack is special w.r.t. CQ. can't sent directed CQ 
                ** with call that won't fit in 28 of those bits. */
                bool fullcallok = (myBaseCall == myCall);
                if (fullcallok)
                    cq = Properties.Settings.Default.CQmessage;
                cq += " " + myCall;
                if (fullcallok)
                    cq += " " + MyGrid4;
                toSendList.Add(new QueuedToSendListItem(cq, null));
                if (!checkBoxAutoXmit.Checked)
                    comboBoxCQ.SelectedIndex = 0;
                if (onlyCQ)
                    CqMessageInProgress = true;
            }

            List<XDft.Tone> itonesToSend = new List<XDft.Tone>();
            List<int> freqsUsed = new List<int>();
            int freqRange = FT_GAP_HZ + 1;
            int freqIncrement = freqRange + 1;
            bool doingMultiStream = toSendList.Count > 1;
            Conversation.Origin origin = Conversation.Origin.TRANSMIT;
            foreach (var item in toSendList)
            {
                QsoInProgress q = item.q;
                int freq = TxFrequency;
                if (null != q)
                {
                    uint assigned = q.TransmitFrequency;
                    if (assigned != 0)
                    {
                        freq = (int)assigned;
                        if (doingMultiStream)
                        {
                            int fdiff = freq - TxFrequency;
                            if (fdiff < 0)
                                fdiff = -fdiff;
                            if (fdiff > freqIncrement * MAX_MULTI_STREAM_INCREMENT)
                                freq = TxFrequency; // don't allow multi-streaming that far apart
                        }
                    }
                }

                // prohibit overlapping send frequencies
                for (int i = 0; i < freqsUsed.Count;)
                {
                    int f = freqsUsed[i];
                    if ((freq <= f + freqRange) && (freq >= f - freqRange))
                    {   // overlaps
                        freq += freqIncrement;
                        i = 0;
                        if (freqIncrement > 0)
                            freqIncrement = -freqIncrement;
                        else
                        {
                            freqIncrement = -freqIncrement;
                            freqIncrement += freqRange + 1;
                        }
                    }
                    else
                        i++;
                }
                if ((null != q) && q.TransmitFrequency != (uint)freq)
                {   // always transmit to this guy on one frequency
                    q.TransmitFrequency = (uint)freq;
                }
                freqsUsed.Add(freq);
                String asSent = null;
                int[] itones = null;
                bool[] ftbits = null;
                genMessage(item.MessageText, ref asSent, ref itones, ref ftbits);
                itonesToSend.Add(new XDft.Tone(itones, 1, freq, 0));
                string conversationItem = String.Format("{2:00}{3:00}{4:00} transmit {1,4}    {0}",
                        asSent,
                        freq, toSend.Hour, toSend.Minute, seconds);
                listBoxConversation.Items.Add(new ListBoxConversationItem(conversationItem, origin));
                conversationLogFile.SendToLog(conversationItem);
                ScrollListBoxToBottom(listBoxConversation);
                logFile.SendToLog("TX: " + item.MessageText);
                asSent = asSent.Trim();
                if (asSent != item.MessageText)
                    logFile.SendToLog("TX error sent \"" + asSent + "\" instead of \"" + item.MessageText + "\"");
                if (IS_AMP_REDUCED_PER_STREAM)
                    origin = Conversation.Origin.TRANSMIT_REDUCED;
            }

            const int MAX_CONVERSATION_LISTBOX_ITEMS = 1000;
            if (listBoxConversation.Items.Count >= MAX_CONVERSATION_LISTBOX_ITEMS)
            {
                while (listBoxConversation.Items.Count >= MAX_CONVERSATION_LISTBOX_ITEMS - 100)
                    listBoxConversation.Items.RemoveAt(0);
                ScrollListBoxToBottom(listBoxConversation);
            }

            const int ALLOW_LATE_MSEC = 1800; // ft8 decoder only allows so much lateness.This is OK unless our clock is slow.

            if (itonesToSend.Any())
            {
                SetTxCycle(nowOdd ? 1 : 0);
                deviceTx.TransmitCycle = XD.Transmit_Cycle.PLAY_NOW;
                if (itonesToSend.Count == 1)
                {   // single set of tones is sent slightly differently than multiple
                    int[] itones = itonesToSend[0].itone;
                    if (allowLate)
                    {
                        if (cyclePosTenths > 0)
                        {
                            int msecToTruncate = toSend.Millisecond + 100 * cyclePosTenths; // how late we are
                            msecToTruncate -= ALLOW_LATE_MSEC; // full itones don't last a full 15 seconds
                            int itonesToLose = msecToTruncate / 160;
                            if (itonesToLose > 0)
                            {
                                int[] truncated = new int[itones.Length - itonesToLose];
                                Array.Copy(itones, itonesToLose, truncated, 0, truncated.Length);
                                itones = truncated;
                            }
                        }
                    }
                    int freq = itonesToSend[0].frequency;
                    freq = RigVfoSplitForTx(freq, freq + FT_GAP_HZ);
                    SendInProgress = true;
                    AfterNmsec(new Action(() =>
                        XDft.Generator.Play(genContext, itones,
                            freq, deviceTx.GetRealTimeAudioSink())), VfoSetToTxMsec);
                }
                else
                {   // multiple to send
                    int minFreq = 99999;
                    int maxFreq = 0;
                    foreach (var itones in itonesToSend)
                    {
                        if (itones.frequency > maxFreq)
                            maxFreq = itones.frequency;
                        if (itones.frequency < minFreq)
                            minFreq = itones.frequency;
                    }
                    int deltaFreq = 0;
                    if (minFreq < rxForm.MinDecodeFrequency)
                        deltaFreq = rxForm.MinDecodeFrequency - minFreq;
                    else if (maxFreq > rxForm.MaxDecodeFrequency + FT_GAP_HZ)
                        deltaFreq = rxForm.MaxDecodeFrequency + FT_GAP_HZ - maxFreq; // negative
                    List<XDft.Tone> tones = new List<XDft.Tone>();
                    float amplitude = 1;
                    float relativeAmplitudeSubsequentQsos = AMP_REDUCE_PER_STREAM;
                    foreach (var itones in itonesToSend)
                    {
                        int[] nextTones = itones.itone;
                        if (allowLate)
                        {
                            if (cyclePosTenths > 0)
                            {
                                int msecToTruncate = toSend.Millisecond + 100 * cyclePosTenths; // how late we are
                                msecToTruncate -= ALLOW_LATE_MSEC; // full itones don't last a full 15 seconds
                                int itonesToLose = msecToTruncate / 160;
                                if (itonesToLose > 0)
                                {
                                    int[] truncated = new int[nextTones.Length - itonesToLose];
                                    Array.Copy(nextTones, itonesToLose, truncated, 0, truncated.Length);
                                    nextTones = truncated;
                                }
                            }
                        }

                        XDft.Tone thisSignal = new XDft.Tone(nextTones, amplitude, itones.frequency + deltaFreq, 0);
                        tones.Add(thisSignal);
                        amplitude *= relativeAmplitudeSubsequentQsos;
                    }
                    RigVfoSplitForTx(minFreq, maxFreq + FT_GAP_HZ, tones);
                    SendInProgress = true;
                    AfterNmsec(new Action(() =>
                        XDft.Generator.Play(genContext, tones.ToArray(), deviceTx.GetRealTimeAudioSink())), VfoSetToTxMsec);
                }
            }

            // clear out checkedlbNextToSend of anything from a QSO no longer in QSO(s) in Progress
            for (int j = 0; j < checkedlbNextToSend.Items.Count;)
            {
                QueuedToSendListItem qli = checkedlbNextToSend.Items[j] as QueuedToSendListItem;
                QsoInProgress qp;
                if (null != qli && 
                    (null != (qp = qli.q)) && 
                    inProgress.Any((q) => Object.ReferenceEquals(q, qp)))
                {   // if the QSO remains active in progress, but still in this list, it didn't get sent,
                    // mark it unchecked so user can see that.
                    if (nowOdd == (((qp.Message.CycleNumber + 1) & 1) != 0))
                        checkedlbNextToSend.SetItemChecked(j, false);
                    j += 1;
                }
                else
                    checkedlbNextToSend.Items.RemoveAt(j);
            }

            foreach (var item in inToSend)
                item.Value.q.TransmitedLastOpportunity = false;

            foreach (var item in toSendList)
            {
                var cb = item.MessageSent;
                if (null != cb)
                    cb();
                if (null != item.q)
                    item.q.TransmitedLastOpportunity = true;
            }
        }

        delegate void VfoOnTxEnd();
        private VfoOnTxEnd vfoOnTxEnd;

        private int RigVfoSplitForTx(int minAudioTx, int maxAudioTx, List<XDft.Tone> tones = null)
        {
            logger?.SetTransmitFocus();

            if (controlVFOsplit == VfoControl.VFO_NONE)
                return minAudioTx;

            // want all outputs below TxHighFreqLimit
            // ...and, more importantly, above half that.
            int minFreq = TxHighFreqLimit / 2;
            int maxFreq = TxHighFreqLimit;

            int offset = 0;   // try to offset
            if (minAudioTx < minFreq)
            {
                offset = minFreq - minAudioTx; // positive. always
                offset /= 100;
                offset += 1;
                offset *= 100;
            }
            else if (maxAudioTx > maxFreq)
            {
                offset = maxFreq - maxAudioTx; // negative 
                offset /= 100;
                offset -= 1;
                offset *= 100;
            }

            if (logger != null)
            {
                bool split;
                double txKHz; double rxKHz;
                logger.GetRigFrequency(out rxKHz, out txKHz, out split);

                // proposed split in offset
                if (((maxAudioTx - minAudioTx) >= (maxFreq - minFreq))
                    || (offset == 0))
                {   //un-split the rig if the needed range is beyond
                    // the setup parameters, or no offset is needed
                    if (split)
                        logger.SetRigFrequency(rxKHz, rxKHz, false);
                    return minAudioTx;
                }

                bool rigIsAlreadyOk = false;  // check if the rig has an acceptable split already
                if (split)
                {
                    int currentOffset = (int)(1000 * (rxKHz - txKHz));
                    if ((minAudioTx + currentOffset >= minFreq) &&
                        maxAudioTx + currentOffset <= maxFreq)
                    {   // the rig's state is OK already
                        offset = currentOffset;
                        rigIsAlreadyOk = // OK only if we're really supposed to be in split mode
                            controlVFOsplit != VfoControl.VFO_SHIFT;
                    }
                }
                // offset is what we'll set
                if (!rigIsAlreadyOk)
                {
                    double rxDuringTx = rxKHz;
                    double txDuringTx = rxKHz - .001f * offset;
                    if (controlVFOsplit == VfoControl.VFO_SPLIT)
                        logger.SetRigFrequency(rxDuringTx, txDuringTx, true);
                    else if (controlVFOsplit == VfoControl.VFO_SHIFT)
                    {
                        rxDuringTx = txDuringTx;
                        logger.SetRigFrequency(rxDuringTx, txDuringTx, false);
                        vfoOnTxEnd = () =>
                        {
                            logger.SetRigFrequency(rxKHz, rxKHz, false);
                        };
                    }
                }
            }
            if (null != tones)
                foreach (var t in tones)
                    t.frequency += offset;
            return minAudioTx + offset;
        }

        private void RigVfoSplitForTxOff()
        {
            if (null != vfoOnTxEnd)
                vfoOnTxEnd();
            vfoOnTxEnd = null; // only run it once
        }

        private void SetTxCycle(int cycle)
        {
            if ((cycle & 1) == 0)
                radioButtonEven.Checked = true;
            else
                radioButtonOdd.Checked = true;
        }

        private void InitiateQsoFromMessage(RecentMessage rm, bool onHisFrequency)
        {
            if (!qsoQueue.InitiateQso(rm, currentBand, onHisFrequency,
                    () =>
                    {
                        string s = rm.Message.ToString();
                        // log the message
                        listBoxConversation.Items.Add(new ListBoxConversationItem(s, Conversation.Origin.INITIATE));
                        conversationLogFile.SendToLog(s);
                        ScrollListBoxToBottom(listBoxConversation);
                    })
                )
                return;

            // turn its check box ON since this is an interactive click
            for (int i = 0; i < checkedlbNextToSend.Items.Count; i++)
            {
                QueuedToSendListItem qli = checkedlbNextToSend.Items[i] as QueuedToSendListItem;
                if (null != qli)
                {
                    if (Object.ReferenceEquals(qli.q.Message, rm.Message))
                    {
                        checkedlbNextToSend.SetItemChecked(i, true);
                        break;
                    }
                }
            }

            RxFrequency = (int)rm.Message.Hz;
            watchDogTime = DateTime.UtcNow;
            if (((rm.Message.CycleNumber & 1) != (cycleNumber & 1))
                && intervalTenths <= START_LATE_MESSAGES_THROUGH_TENTHS)
            {   // start late if we can
                if (CqMessageInProgress)
                    AbortMessage();
                if (!SendInProgress)
                    transmitAtZero(true);
            }
            checkBoxAutoXmit.Checked = true;
        }

        int MAX_UNANSWERED_MINUTES = 5;
        #endregion

        #region IQsoQueueCallBacks
        private const string BracketFormat = "<{0}>";
        
        private bool needBrackets(string call)
        {
            int n22=0;
            return XDft.Generator.pack28(call, ref n22);
        }

        public string GetExchangeMessage(QsoInProgress q, bool addAck)
        {
            var excSet = Properties.Settings.Default.ContestExchange;
            return GetExchangeMessage(q, addAck, (ExchangeTypes)excSet);
        }

        public string MyGrid4 {
            get {
                string ret = Properties.Settings.Default.MyGrid;
                if (ret.Length > 4)
                    ret = ret.Substring(0, 4);
                return ret;
            }
        }

        public string GetExchangeMessage(QsoInProgress q, bool addAck, ExchangeTypes excSet)
        {
            string hiscall = q.HisCall;
            bool hiscallNeedsBrackets = needBrackets(hiscall);
            string mycall = myCall;
            bool mycallNeedsBrackets = !String.Equals(myCall, myBaseCall);
            // 77-bit pack allows only one callsign to be bracketed.
            if (hiscallNeedsBrackets && mycallNeedsBrackets)
            {
                // reduce his call to his base cuz both cannot be hashed
                XDft.Generator.checkCall(hiscall, ref hiscall);
                mycall = String.Format(BracketFormat, myCall);
            }
            else if (hiscallNeedsBrackets)
                hiscall = String.Format(BracketFormat, hiscall);
            else if (mycallNeedsBrackets)
                mycall = String.Format(BracketFormat, mycall);

            // fill in from logger if we can
            if (q.SentSerialNumber == 0)
            {   // assign a serial number even if contest doesn't need it
                uint serialToSend = ++noLoggerSerialNumber;
                if (null != logger)
                    serialToSend = logger.GetSendSerialNumber(q.HisCall);
                // logger may give us the same serial number since we're just one radio
                Dictionary<uint, uint> serialsInProgress = new Dictionary<uint, uint>();
                var inProgress = qsosPanel.QsosInProgress;
                for (int i = 0; i < inProgress.Count; i++)
                {   // first entries are highest priority
                    QsoInProgress qp = inProgress[i];
                    if (null == qp)
                        continue;   // manual entry goes this way
                    uint qps = qp.SentSerialNumber;
                    if (qps != 0)
                        serialsInProgress[qps] = qps;
                }
                uint ignore;
                while (serialsInProgress.TryGetValue(serialToSend, out ignore))
                    serialToSend += 1;
                q.SentSerialNumber = serialToSend;
            }
            switch (excSet)
            {
                case ExchangeTypes.ARRL_FIELD_DAY:
                    string entryclass = "";
                    var fdsplit = Properties.Settings.Default.ContestMessageToSend.Split((char[])null,
                        StringSplitOptions.RemoveEmptyEntries);
                    bool founddigit = false;
                    foreach (string w in fdsplit)
                    {
                        if (!founddigit)
                        {
                            if (Char.IsDigit(w[0]))
                            {
                                founddigit = true;
                                entryclass = w;
                            }
                        }
                        else if (w.All(Char.IsLetter))
                            return String.Format("{0} {1} {2}{3} {4}", q.HisCall, myCall,
                                addAck ? "R " : "", entryclass, w);
                    }
                    break;

                case ExchangeTypes.ARRL_RTTY:
                    string part = null;
                    String percentSearch = Properties.Settings.Default.ContestMessageToSend;
                    String percentsRemoved = "";
                    for (; ; )
                    {   // is there a serial number in the message?
                        int percentPos = percentSearch.IndexOf('%');
                        if (percentPos < 0)
                        {
                            percentsRemoved += percentSearch;
                            break;  // no serial number
                        }
                        if ((percentPos < percentSearch.Length - 1) &&
                            Char.IsLetter(percentSearch[percentPos + 1]))
                        {
                            percentsRemoved += percentSearch.Substring(0, percentPos);
                            percentSearch = percentSearch.Substring(percentPos + 2);
                            continue; // this one is not a serial number
                        }
                        part = String.Format("{0:0000}", q.SentSerialNumber);
                        break;
                    }
                    if (String.IsNullOrEmpty(part))
                    {
                        var rttysplit = percentsRemoved.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string w in rttysplit)
                            if (w.All(Char.IsLetter))
                            {
                                part = w.ToUpper();
                                break;
                            }
                    }
                    return String.Format("{0} {1} {2}{3} {4}", q.HisCall, myCall,
                        addAck ? "R " : "", q.Message.RST, part);

                case ExchangeTypes.GRID_SQUARE:
                    if (null != logger)
                    {   // logger can override sent grid
                        string sentgrid = logger.GridSquareSendingOverride();
                        if (sentgrid.Length >= 4)
                        {
                            q.SentGrid = sentgrid;
                            return String.Format("{0} {1} {2} {3}",
                                hiscall, mycall,
                                addAck ? "R" : "", sentgrid);
                        }
                    }
                    break;

                case ExchangeTypes.DB_REPORT:
                    break; // handle below
            }
            // if logger is not running, or doesn't handle the exchange.
            switch (excSet)
            {
                case ExchangeTypes.DB_REPORT:
                    int dB = q.Message.SignalDB;
                    return String.Format("{0} {1} {2}{3:+00;-00;+00}",
                        hiscall,
                        mycall,
                        addAck ? "R" : "", dB);
            }

            return String.Format("{0} {1} {2}{3}",
                hiscall,
                mycall,
                addAck ? "R " : "", MyGrid4);
        }

        private string GetAckMessage(QsoInProgress q, bool ofAnAck, int whichAck)
        {
            // this one does non standard calls backwards from above. 
            // The standard call is the one that gets hashed.
            string hiscall = q.HisCall;
            bool hiscallNeedsBrackets = needBrackets(hiscall);
            string mycall = myCall;
            bool mycallNeedsBrackets = !String.Equals(myCall, myBaseCall);
            if (hiscallNeedsBrackets && mycallNeedsBrackets)
                hiscall = String.Format(BracketFormat, hiscall);
            else if (hiscallNeedsBrackets)
                mycall = String.Format(BracketFormat, myBaseCall);
            else if (mycallNeedsBrackets)
                hiscall = String.Format(BracketFormat, hiscall);
            return hiscall + " " + mycall + " " +
                DefaultAcknowledgements[whichAck];
        }

        public string GetAckMessage(QsoInProgress q, bool ofAnAck)
        {  return GetAckMessage(q, ofAnAck, q.AckMessage); }

        public void SendOnLoggedAck(QsoInProgress q, QsoSequencer.MessageSent ms)
        {
            int which = comboBoxOnLoggedMessage.SelectedIndex;
            if (which <= 0) // none
                return;
            var toSend = GetAckMessage(q, true, which - 1);
            SendMessage(toSend, q, ms);
        }

        public void SendMessage(string s, QsoInProgress q, QsoSequencer.MessageSent ms)
        {
            for (int i = 0; i < checkedlbNextToSend.Items.Count; i++)
            {   // if there is a message on this QSO already, remove it.
                QueuedToSendListItem qli = checkedlbNextToSend.Items[i] as QueuedToSendListItem;
                if (null != qli)
                {
                    if (Object.ReferenceEquals(qli.q, q))
                    {
                        if (!checkedlbNextToSend.GetItemChecked(i))
                        {
                            checkedlbNextToSend.Items.RemoveAt(i);
                            break;
                        }
                        else
                            return;
                    }
                }
            }
            int idx = checkedlbNextToSend.Items.Add(new SortedQueuedToSendListItem(s, q, qsosPanel, ms));
            checkedlbNextToSend.SetItemChecked(idx, checkBoxAutoXmit.Checked && q.Active);
            checkedlbNextToSend.Sort();
        }

        private void fillAlternativeMessages(QsoInProgress q)
        {
            listBoxAlternatives.Items.Clear();
            Dictionary<string, int> alreadyEntered = new Dictionary<string, int>();
            string msg = GetExchangeMessage(q, false);
            listBoxAlternatives.Items.Add(new QueuedToSendListItem(msg, q));
            alreadyEntered.Add(msg,0);
            int v;
            msg = GetExchangeMessage(q, true);
            if (!alreadyEntered.TryGetValue(msg, out v))
            {
                listBoxAlternatives.Items.Add(new QueuedToSendListItem(msg, q));
                alreadyEntered.Add(msg,0);
            }

            ExchangeTypes excSet = (ExchangeTypes)Properties.Settings.Default.ContestExchange;
            if (excSet != ExchangeTypes.DB_REPORT)
            {
                msg = GetExchangeMessage(q, false, ExchangeTypes.DB_REPORT);
                if (!alreadyEntered.TryGetValue(msg, out v))
                {
                    listBoxAlternatives.Items.Add(new QueuedToSendListItem(msg, q));
                    alreadyEntered.Add(msg,0);
                }
                msg = GetExchangeMessage(q, true, ExchangeTypes.DB_REPORT);
                if (!alreadyEntered.TryGetValue(msg, out v))
                {
                    listBoxAlternatives.Items.Add(new QueuedToSendListItem(msg, q));
                    alreadyEntered.Add(msg,0);
                }
            }
            if (excSet != ExchangeTypes.GRID_SQUARE)
            {
                msg = GetExchangeMessage(q, false, ExchangeTypes.GRID_SQUARE);
                if (!alreadyEntered.TryGetValue(msg, out v))
                {
                    listBoxAlternatives.Items.Add(new QueuedToSendListItem(msg, q));
                    alreadyEntered.Add(msg, 0);
                }
                msg = GetExchangeMessage(q, true, ExchangeTypes.GRID_SQUARE);
                if (!alreadyEntered.TryGetValue(msg, out v))
                {
                    listBoxAlternatives.Items.Add(new QueuedToSendListItem(msg, q));
                    alreadyEntered.Add(msg, 0);
                }
            }

            for (int i = 0; i < DefaultAcknowledgements.Length; i++)
            {
                msg = GetAckMessage(q, false, i);
                listBoxAlternatives.Items.Add(new QueuedToSendListItem(msg, q));
            }

            altMessageShortcuts.Populate();
        }

        public void LogQso(QsoInProgress q)
        {
            if (null != logger)
            {
                var excSet = Properties.Settings.Default.ContestExchange;
                String date = String.Format("{0:yyyyMMdd}", q.TimeOfLastReceived);
                String time = String.Format("{0:HHmmss}", q.TimeOfLastReceived);
                logger.SetQsoItemsToLog(q.HisCall, q.SentSerialNumber, date, time, q.SentGrid, digiMode == DigiMode.FT8 ? "FT8" : "FT4");
                switch ((ExchangeTypes)excSet)
                {
                    case ExchangeTypes.ARRL_FIELD_DAY:
                        LogFdQso(q);
                        break;
                    case ExchangeTypes.ARRL_RTTY:
                        LogRttyRoundUpQso(q);
                        break;

                    case ExchangeTypes.DB_REPORT:
                    case ExchangeTypes.GRID_SQUARE:
                    case ExchangeTypes.GRID_SQUARE_PLUS_REPORT:
                        LogGridSquareQso(q);
                        break;
                }
            }
            q.MarkedAsLogged = true;
            CurrentActiveQsoCalltoLogger();
        }

        private void LogFdQso(QsoInProgress q)
        {
            // start at latest and work backwards
            string cat = "", sec = "";
            for (int i = q.MessageList.Count - 1; i >= 0; i -= 1)
            {
                XDpack77.Pack77Message.ReceivedMessage rm = q.MessageList[i];
                XDpack77.Pack77Message.ArrlFieldDayMessage iexc = 
                    rm.Pack77Message as XDpack77.Pack77Message.ArrlFieldDayMessage;
                if (iexc == null)
                    continue;
                var fields = iexc.Exchange.Split((char[])null);
                cat = fields[0];
                sec = fields[1];
                break;
            }
            logger.LogFieldDayQso(cat,sec);
        }

        private void LogRttyRoundUpQso(QsoInProgress q)
        {
            string recRst = "";
            string recStateOrSer = "";
            // RCV RST and "QTH" which might be serial number
            for (int i = q.MessageList.Count - 1; i >= 0; i -= 1)
            {   // from most recent back to oldest
                XDpack77.Pack77Message.ReceivedMessage rm = q.MessageList[i];
                XDpack77.Pack77Message.RttyRoundUpMessage iexc = 
                    rm.Pack77Message as XDpack77.Pack77Message.RttyRoundUpMessage;
                if (iexc == null)
                    continue;
                // found last RTTY Roundup message
                string exc = iexc.Exchange;
                var fields = exc.Split((char[])null);
                recRst = fields[0];
                recStateOrSer = fields[1];
                break;
            }
            logger.LogRoundUpQso(q.Message.RST, recRst, recStateOrSer);
        }

        private void LogGridSquareQso(QsoInProgress q)
        {
            /* This function works for both grid square and
             * signal report exchange. Search the list of
             * messages in the QSO for both and add what we find to the log.
             */
            int incomingDB = q.Message.SignalDB;
            string sentdB = incomingDB.ToString("D2");
            if (incomingDB>=0)
                sentdB = "+" + sentdB;
            string dbReport = null;
            string gridsquare="";
            foreach (var m in q.MessageList)
            {
                XDpack77.Pack77Message.Exchange iExc = m.Pack77Message as XDpack77.Pack77Message.Exchange;
                if (iExc == null)
                    continue;
                // find received message with a grid square in it
                string gs = iExc.GridSquare;
                if (!String.IsNullOrEmpty(gs))
                    gridsquare = gs;
                // find received message with an RST
                int db = iExc.SignaldB;
                if (db > XDpack77.Pack77Message.Message.NO_DB)
                    dbReport = String.Format("{0:+00;-00;+00}", db);
            }
            logger.LogGridSquareQso(sentdB, gridsquare, dbReport);
        }
        
        #endregion

        private String MyCall {
            set { 
                myCall = value.ToUpper().Trim();
                myBaseCall = myCall;
                if (!String.IsNullOrEmpty(value) && !XDft.Generator.checkCall(myCall, ref myBaseCall))
                    MessageBox.Show("Callsign " + value + " is not a valid callsign for FT8");
            }
        }

        const int TENTHS_IN_SECOND = 10;

#if DEBUG // for the simulator
        const int INVALID_TIME_TENTHS = -1 - (10 * 60 * 60);
        bool simulatedThisInterval;
        int timeStampTenths(String s, out bool isOdd)
        {
            isOdd = false;
            if (String.IsNullOrEmpty(s))
                return -1;
            if (s.Length < 6)
                return -1;
            try
            {
                int seconds = Int32.Parse(s.Substring(4, 2));
                int secondsMaybePlusHalf = seconds + 1;
                isOdd = 0 != (1 & ((secondsMaybePlusHalf * TENTHS_IN_SECOND) / FT_CYCLE_TENTHS));
                return (isOdd ? FT_CYCLE_TENTHS % 10 : 0) + 10 * (seconds +
                    60 * Int32.Parse(s.Substring(2,2)));
            }
            catch (System.Exception )
            {  return INVALID_TIME_TENTHS;   }
        }
        uint SIMULATOR_POPS_OUT_DECODED_MESSAGES_AT_TENTHS = 130;
#endif

        #region Form events
        private static bool fromRegistryValue(Microsoft.Win32.RegistryKey rk, string valueName, out int v)
        {
            v = 0;
            if (null == rk)
                return false;
            object rv = rk.GetValue(valueName);
            if (rv == null)
                return false;
            return Int32.TryParse(rv.ToString(), out v);
        }

        private static bool fromRegistryValue(Microsoft.Win32.RegistryKey rk, string valueName, out float v)
        {
            v = 0;
            if (null == rk)
                return false;
            object rv = rk.GetValue(valueName);
            if (rv == null)
                return false;
            return float.TryParse(rv.ToString(), out v);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
#if !DEBUG
            if (null == logger)
            {
                if (MessageBox.Show(
                    "Running without a logging program has limited functionality!\r\n\r\n" +
                    "QSOs will NOT be logged, even when so indicated, and contest exchange contents are (mostly) not valid.\r\n\r\n" +
                    "Do you understand and accept these restrictions?\r\nYou must click YES to continue.", 
                    "DigiRite", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    Close();
                    return;
                }
            }
#endif
            rxForm = new RcvrForm(this, instanceNumber);
            this.Text = String.Format("{0}-{1}", this.Text, instanceNumber);

            ClockLabel cl = new ClockLabel();
            cl.Location = labelClockAnimation.Location;
            cl.Size = labelClockAnimation.Size;
            panel3.Controls.Remove(labelClockAnimation);
            panel3.Controls.Add(cl);
            labelClockAnimation.Dispose();
            labelClockAnimation = cl;

            // Apply color scheme
            label4.BackColor =
            label5.BackColor =
            label3.BackColor =
            label11.BackColor =
            panel5.BackColor = 
            panel6.BackColor =
            panel3.BackColor =
            label7.BackColor =
            menuStrip.BackColor =
            panel1.BackColor = CustomColors.CommonBackgroundColor;
            groupBox3.BackColor =
            trackBarTxGain.BackColor = CustomColors.TxBackgroundColor;

            if (!SetupTxAndRxDeviceIndicies())
                MyCall = Properties.Settings.Default.CallUsed;

            MAX_UNANSWERED_MINUTES = Properties.Settings.Default.MaxUnansweredMinutes;

            while (true)
            {
                string myGrid = Properties.Settings.Default.MyGrid;
                if (SetupForm.validateGridSquare(myGrid) && InitSoundInAndOut())
                    break;
                var sf = new SetupForm(
                    instanceNumber,
                    SetupMaySelectDevices, SetupMaySelectLR,
                    null == logger);
                sf.controlSplit = controlVFOsplit;
                sf.forceRigUsb = forceRigUsb;
                sf.txHighLimit = TxHighFreqLimit;
                sf.PttToSound = UserPttToSound;
                sf.VfoSplitToPtt = UserVfoSplitToPtt;
                if (sf.ShowDialog() != DialogResult.OK)
                    {
                        Close();
                        return;
                    }
                controlVFOsplit = sf.controlSplit;
                forceRigUsb = sf.forceRigUsb;
                TxHighFreqLimit = sf.txHighLimit;
                digiMode = sf.digiMode;
                MyCall = Properties.Settings.Default.CallUsed;
                UserPttToSound = sf.PttToSound;
                UserVfoSplitToPtt = sf.VfoSplitToPtt;
            }

            cqListEven = new CallPresentation(panelEvenCQs, labelCqTable, checkBoxCqTable);
            cqListEven.InitiateQsoCb += new CallPresentation.InitiateQso(InitiateQsoFromMessage);
            cqListOdd = new CallPresentation(panelOddCQs, labelCqTable, checkBoxCqTable);
            cqListOdd.InitiateQsoCb += new CallPresentation.InitiateQso(InitiateQsoFromMessage);
            toMe = new CallPresentation(listToMe, labelCqTable, checkBoxCqTable);
            toMe.InitiateQsoCb += new CallPresentation.InitiateQso(InitiateQsoFromMessage);
            qsosPanel = new QsosPanel(panelInProgress, labelInProgress, checkBoxInProgress);
            qsosPanel.fillAlternatives += new QsosPanel.FillAlternatives(fillAlternativeMessages);
            qsosPanel.qsoActiveChanged += new QsosPanel.QsoActiveChanged(OnQsoActiveChanged);
            qsosPanel.onRemovedQso += new QsosPanel.OnRemovedQso(quitQso);
            qsosPanel.logAsIs += new QsosPanel.LogAsIs(LogQso);
            qsosPanel.isCurrentCycle += new QsosPanel.IsCurrentCycle((QsoInProgress q) =>
                { return ((q.Message.CycleNumber & 1) != 0) == radioButtonEven.Checked; } );
            qsosPanel.orderChanged += new QsosPanel.OrderChanged(OnQsoInProgressOrderChanged);
            cqListEven.Reset();
            cqListOdd.Reset();
            toMe.Reset();
            cqListEven.SizeChanged(null, null);
            cqListOdd.SizeChanged(null, null);
            toMe.SizeChanged(null, null);
            qsosPanel.SizeChanged(null, null);

            foreach (var s in DefaultAcknowledgements)
                comboBoxOnLoggedMessage.Items.Add(s);
            comboBoxOnLoggedMessage.SelectedIndex = Properties.Settings.Default.OnLoggedAcknowedgeMessage;

            initQsoQueue();
            
            rxForm.logFile = logFile;
            rxForm.Show();

            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(instanceRegKeyName);
            int decodeMin = 100;
            int decodeMax = 5000;
            if (null != rk)
                using (rk)
                {   // used saved settings
                    {
                        int x;
                        if (fromRegistryValue(rk, "ControlVfoSplit", out x) && x > 0)
                        {
                            int m;
                            if (fromRegistryValue(rk, "MaxTxAudioFrequency", out m) && m > 0)
                            {
                                if (Enum.IsDefined(typeof(VfoControl), x))
                                {
                                    controlVFOsplit = (VfoControl)x;
                                    TxHighFreqLimit = m;
                                }
                            }
                        }
                    }
                    {
                        int x;
                        if (fromRegistryValue(rk, "ForceRigUSB", out x) && x > 0)
                            forceRigUsb = true;
                    }
                    {
                        int x, y;
                        if (fromRegistryValue(rk, "MainX", out x) && fromRegistryValue(rk, "MainY", out y))
                        {
                            System.Drawing.Point cornerMe = new System.Drawing.Point(x, y);
                            foreach (Screen s in Screen.AllScreens)
                            {
                                if (s.WorkingArea.Contains(cornerMe))
                                {
                                    StartPosition = FormStartPosition.Manual;
                                    Location = cornerMe;
                                    break;
                                }
                            }
                        }
                    }
                    {
                        int w, h;
                        if (fromRegistryValue(rk, "MainW", out w) && fromRegistryValue(rk, "MainH", out h))
                        {
                            if (w < 280)
                                w = 280;
                            if (h < 280)
                                h = 280;
                            Size = new System.Drawing.Size(w, h);
                        }
                    }

                    int mainSplitterDistance;
                    if (fromRegistryValue(rk, "MainSplit", out mainSplitterDistance))
                    {
                        if (mainSplitterDistance >= splitContainerCqLeft.Panel1MinSize &&
                            mainSplitterDistance <= splitContainerCqLeft.Width - splitContainerCqLeft.Panel2MinSize)
                            splitContainerCqLeft.SplitterDistance = mainSplitterDistance;
                    }

                    int leftSplitterDistance;
                    if (fromRegistryValue(rk, "LeftVerticalSplit", out leftSplitterDistance))
                    {
                        if (leftSplitterDistance >= splitContainerAnswerUpCqsDown.Panel1MinSize &&
                            leftSplitterDistance <= splitContainerAnswerUpCqsDown.Height - splitContainerAnswerUpCqsDown.Panel2MinSize)
                            splitContainerAnswerUpCqsDown.SplitterDistance = leftSplitterDistance;
                    }

                    {
                        int x, y;
                        if (fromRegistryValue(rk, "XcvrX", out x) && fromRegistryValue(rk, "XcvrY", out y))
                        {
                            System.Drawing.Point cornerXcvr = new System.Drawing.Point(x, y);
                            foreach (Screen s in Screen.AllScreens)
                            {
                                if (s.WorkingArea.Contains(cornerXcvr))
                                {
                                    rxForm.Location = cornerXcvr;
                                    break;
                                }
                            }
                        }
                    }
                    {
                        int w, h;
                        if (fromRegistryValue(rk, "XcvrW", out w) && fromRegistryValue(rk, "XcvrH", out h))
                        {
                            if (w < 280)
                                w = 280;
                            if (h < 280)
                                h = 280;
                            rxForm.Size = new System.Drawing.Size(w, h);
                        }
                    }

                    int xcvrSplitterDistance;
                    if (fromRegistryValue(rk, "XcvrSplit", out xcvrSplitterDistance))
                        try
                        { rxForm.SplitterDistance = xcvrSplitterDistance; }
                        finally { }
                    int txEven;
                    if (fromRegistryValue(rk, "TxEven", out txEven))
                    {
                        if (txEven == 0)
                            radioButtonOdd.Checked = true;
                        else
                            radioButtonEven.Checked = true;
                    }
                    int cqBoth;
                    if (fromRegistryValue(rk, "BothCQsShow", out cqBoth))
                        checkBoxCQboth.Checked = cqBoth != 0;
                    int cqsOnly;
                    if (fromRegistryValue(rk, "OnlyCQsShow", out cqsOnly))
                    {
                        switch (cqsOnly)
                        {
                            case 0: checkBoxOnlyCQs.CheckState = CheckState.Unchecked; break;
                            case 1: checkBoxOnlyCQs.CheckState = CheckState.Indeterminate; break;
                            case 2: checkBoxOnlyCQs.CheckState = CheckState.Checked; break;
                        }
                    }
                    int txFreq;
                    if (fromRegistryValue(rk, "TXfrequency", out txFreq))
                        TxFrequency = txFreq;
                    int temp;
                    if (fromRegistryValue(rk, "DecodeMinHz", out temp))
                    {
                        decodeMin = temp;
                        if (decodeMin < 100)
                            decodeMin = 100;
                        if (decodeMin > 4500)
                            decodeMin = 4500;
                    }
                    if (fromRegistryValue(rk, "DecodeMaxHz", out temp))
                    {
                        decodeMax = temp;
                        if (decodeMax < 100)
                            decodeMax = 100;
                        if (decodeMax > 5000)
                            decodeMax = 5000;
                        if (decodeMax <= decodeMin)
                            decodeMax = decodeMin + FT_GAP_HZ;
                    }

                    if (fromRegistryValue(rk, "DigiMode", out temp))
                    {
                        if (temp > 0)
                            digiMode = DigiMode.FT4;
                        else
                            digiMode = DigiMode.FT8;
                    }

                    if (fromRegistryValue(rk, "VfoSplitToPtt", out temp))
                        UserVfoSplitToPtt = temp;
                    if (fromRegistryValue(rk, "PttToSound", out temp))
                        UserPttToSound = temp;
                }

            changeDigiMode();

#if DEBUG
            try
            {
                using (var simContents = System.IO.File.OpenText(@"C:\temp\decoded.txt"))
                {
                    string s = "";
                    while ((s = simContents.ReadLine()) != null)
                    {
                        if (s.Length > 9)
                        {
                            s = s.Substring(9);
                            var split = s.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                            int dummy;
                            if ((split.Length > 5) && Int32.TryParse(split[0], out dummy))
                            {
                                if (simulatorLines == null)
                                    simulatorLines = new List<string>();
                                simulatorLines.Add(s);
                            }
                        }
                    }

                    bool isOdd;
                    if (simulatorLines != null && simulatorLines.Count > 0)
                        simulatorTimeOrigin = timeStampTenths(simulatorLines[0], out isOdd);
                }
            }
            catch (System.Exception)
            { /* do nothing */}
            simulatorStart = DateTime.UtcNow;
#endif

            locationToSave = Location;
            sizeToSave = Size;
            checkBoxShowMenu.Checked = Properties.Settings.Default.ShowMenu;
            checkBoxCalcNextToSend.Checked = checkedlbNextToSend.Visible = Properties.Settings.Default.ShowCalcNextToSend;
            rxForm.RxHz = (int)numericUpDownRxFrequency.Value;
            rxForm.MinDecodeFrequency = decodeMin;
            rxForm.MaxDecodeFrequency = decodeMax;
            comboBoxCQ.SelectedIndex = 0;
            cqListEven.FilterCqs = cqListOdd.FilterCqs = checkBoxOnlyCQs.CheckState;
            listBoxConversation.DrawMode = DrawMode.OwnerDrawFixed;
            altMessageShortcuts = new AltMessageShortcuts(listBoxAlternativesPanel, listBoxAlternatives);
            logFile.SendToLog("Started");
            ApplyFontControls();
#if DEBUG
            {
                Microsoft.Win32.RegistryKey sim = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\W5XD\\WriteLog\\DigiRite\\Simulator");
                if (null != sim)
                    using (sim)
                    {
                        int x;
                        if (fromRegistryValue(sim, "AnswerAllCQs", out x) && x > 0)
                            autoAnswerAllCQsFromSimulator = true;
                        if (fromRegistryValue(sim, "AutoAnswerNonDupes", out x) && x > 0)
                            checkBoxRespondNonDupe.Checked = true;
                        if (fromRegistryValue(sim, "CqSetting", out x) && x > 0)
                            comboBoxCQ.SelectedIndex = x;
                        if (fromRegistryValue(sim, "EnableSimulation", out x) && x == 0)
                            simulatorLines = null;
                    }
            }
#endif

            finishedLoad = true;
        }

        private void changeDigiMode()
        {
            ClockLabel cl = labelClockAnimation as ClockLabel;
            switch (digiMode)
            {
                case DigiMode.FT4:
                    genContext = XDft.GeneratorContext.getFt4Context((ushort)UserPttToSound);
                    genMessage = new GenMessage(XDft.Generator.genft4);
                    checkTransmitAtZero = new CheckTransmitAtZero(Ft4CheckTransmitAtZero);
                    demodulatorWrapper.digiMode = XDft.DigiMode.DIGI_FT4;
                    demodulatorWrapper.DemodulateSoundPreUtcZeroMsec = (short)( DEFAULT_DECODER_LOOKBACK_MSEC + FT4_DECODER_CENTER_OFFSET_MSEC);
                    demodulatorWrapper.nzhsym = 18;
#if DEBUG
                    if ((1 & FT4_MULTIPLE_DECODE_COUNT) != 1)
                        throw new System.Exception("FT4_MULTIPLE_DECODE_COUNT must be an odd number");
                    if (demodulatorWrapper.DemodulateSoundPreUtcZeroMsec > 1000)
                        throw new System.Exception("Demodulator cannot look back further than 1000msec");
                    SIMULATOR_POPS_OUT_DECODED_MESSAGES_AT_TENTHS = 50;
#endif
                    demodulatorWrapper.DemodulateDefaultSoundShiftMsec = FT4_DECODER_CENTER_OFFSET_MSEC;
                    ft4DecodeOffsetMsec = new ushort[FT4_MULTIPLE_DECODE_COUNT];
                    ft4DecodeOffsetMsec[0] = FT4_DECODER_CENTER_OFFSET_MSEC; // zero idx is run by OnClock
                    int middleIdx = FT4_MULTIPLE_DECODE_COUNT / 2;
                    for (int i = 0; i < middleIdx; i++)
                    {   // work the time shift outwards from the nominally correct one.
                        ft4DecodeOffsetMsec[i * 2 + 1] = (ushort)(FT4_DECODER_CENTER_OFFSET_MSEC + (i + 1) * FT4_MULTIPLE_DECODE_OFFSET_MSEC);
                        ft4DecodeOffsetMsec[i * 2 + 2] = (ushort)(FT4_DECODER_CENTER_OFFSET_MSEC - (i + 1) * FT4_MULTIPLE_DECODE_OFFSET_MSEC);
                    }
                    TRIGGER_DECODE_TENTHS = 20;
                    CLEAR_OLD_MESSAGES_AT_TENTHS = 10;
                    cl.CYCLE = 75;
                    rxForm.CYCLE = 75;
                    FT_CYCLE_TENTHS = 75;
                    FT_GAP_HZ = 90;
                    MESSAGE_SEPARATOR = '+';
                    TUNE_LEN = 64;
                    break;

                case DigiMode.FT8:
                    genContext = XDft.GeneratorContext.getFt8Context((ushort)UserPttToSound);
                    genMessage = new GenMessage(XDft.Generator.genft8);
                    checkTransmitAtZero = new CheckTransmitAtZero(Ft8CheckTransmitAtZero);
                    demodulatorWrapper.digiMode = XDft.DigiMode.DIGI_FT8;
                    demodulatorWrapper.DemodulateSoundPreUtcZeroMsec = DEFAULT_DECODER_LOOKBACK_MSEC;
                    demodulatorWrapper.nzhsym = 50;
                    TRIGGER_DECODE_TENTHS = 50;
                    CLEAR_OLD_MESSAGES_AT_TENTHS = 50;
                    cl.CYCLE = 150;
                    FT_CYCLE_TENTHS = 150;
                    rxForm.CYCLE = 150;
                    FT_GAP_HZ = 60;
                    MESSAGE_SEPARATOR = '~';
                    TUNE_LEN =19;
#if DEBUG
                    SIMULATOR_POPS_OUT_DECODED_MESSAGES_AT_TENTHS = 130;
#endif
                    break;
            }
            DECODE_SEPARATOR = String.Format("{0}  ", MESSAGE_SEPARATOR);
        }

        private void initQsoQueue()
        {
            ExchangeTypes contest = (ExchangeTypes)Properties.Settings.Default.ContestExchange;
            switch (contest)
            {
                case ExchangeTypes.GRID_SQUARE_PLUS_REPORT:
                    qsoQueue = new Qso2MessageExchange(qsosPanel, this);
                    break;

                case ExchangeTypes.GRID_SQUARE:
                    qsoQueue =  new QsoQueue(qsosPanel, this, (XDpack77.Pack77Message.Message m) => {
                                var sm = m as XDpack77.Pack77Message.StandardMessage;
                                return (null != sm) && (!String.IsNullOrEmpty(sm.GridSquare) && sm.GridSquare.Length >= 4);
                            });
                    break;

                case ExchangeTypes.ARRL_FIELD_DAY:
                    qsoQueue = new QsoQueue(qsosPanel, this, (XDpack77.Pack77Message.Message m) => {
                        // select our own contest messages
                        return null != m as XDpack77.Pack77Message.ArrlFieldDayMessage;
                    });
                    break;

                case ExchangeTypes.ARRL_RTTY:
                    qsoQueue = new QsoQueue(qsosPanel, this, (XDpack77.Pack77Message.Message m) => {
                        // select our own contest messages
                        return null != m as XDpack77.Pack77Message.RttyRoundUpMessage;
                    });
                    break;

                case ExchangeTypes.DB_REPORT:
                    qsoQueue = new QsoQueue(qsosPanel, this, (XDpack77.Pack77Message.Message m) => {
                        var sm = m as XDpack77.Pack77Message.StandardMessage;
                        return (null != sm) && sm.SignaldB > XDpack77.Pack77Message.Message.NO_DB;
                    });
                    break;
            }
            qsoQueue.MyCall = myCall;
            qsoQueue.MyBaseCall = myBaseCall;
            qsosPanel.Reset(); // no QSOs in progress can survive switching queue handling
        }
        private bool finishedLoad = false;
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (finishedLoad)
            {
                Properties.Settings.Default.OnLoggedAcknowedgeMessage = (ushort)comboBoxOnLoggedMessage.SelectedIndex;
                Properties.Settings.Default.ShowMenu = checkBoxShowMenu.Checked;
                Properties.Settings.Default.ShowCalcNextToSend = checkBoxCalcNextToSend.Checked;
                Properties.Settings.Default.Save();
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(instanceRegKeyName);
                if (null != rk)
                {   // save windows positions, etc.
                    rk.SetValue("MainX", locationToSave.X.ToString());
                    rk.SetValue("MainY", locationToSave.Y.ToString());
                    rk.SetValue("MainW", sizeToSave.Width.ToString());
                    rk.SetValue("MainH", sizeToSave.Height.ToString());
                    rk.SetValue("MainSplit", splitContainerCqLeft.SplitterDistance.ToString());
                    rk.SetValue("LeftVerticalSplit", splitContainerAnswerUpCqsDown.SplitterDistance.ToString());
                    rk.SetValue("XcvrX", rxForm.LocationToSave.X.ToString());
                    rk.SetValue("XcvrY", rxForm.LocationToSave.Y.ToString());
                    rk.SetValue("XcvrW", rxForm.SizeToSave.Width.ToString());
                    rk.SetValue("XcvrH", rxForm.SizeToSave.Height.ToString());
                    rk.SetValue("XcvrSplit", rxForm.SplitterDistance.ToString());
                    if (controlVFOsplit == VfoControl.VFO_NONE)
                            rk.SetValue("ControlVfoSplit", "0");
                    else
                    {
                        rk.SetValue("MaxTxAudioFrequency", TxHighFreqLimit.ToString());
                        rk.SetValue("ControlVfoSplit", ((int) controlVFOsplit).ToString());
                    }
                    rk.SetValue("ForceRigUSB", forceRigUsb ? "1" : "0");
                    rk.SetValue("TxEven", radioButtonEven.Checked ? "1" : "0");
                    string onlyCqsVal="0";
                    switch (checkBoxOnlyCQs.CheckState)
                    {
                        case CheckState.Unchecked:  onlyCqsVal = "0"; break;
                        case CheckState.Indeterminate: onlyCqsVal = "1"; break;
                        case CheckState.Checked:   onlyCqsVal = "2";  break;
                    }
                    rk.SetValue("OnlyCQsShow", onlyCqsVal);
                    rk.SetValue("BothCQsShow", checkBoxCQboth.Checked ? "1" : "0");
                    rk.SetValue("TXfrequency", numericUpDownFrequency.Value.ToString());
                    rk.SetValue("DecodeMinHz", rxForm.MinDecodeFrequency.ToString());
                    rk.SetValue("DecodeMaxHz", rxForm.MaxDecodeFrequency.ToString());
                    rk.SetValue("DigiMode", (digiMode == DigiMode.FT8 ? 0 : 1).ToString());
                    rk.SetValue("VfoSplitToPtt", UserVfoSplitToPtt.ToString());
                    rk.SetValue("PttToSound", UserPttToSound.ToString());
                    if (null != deviceTx)
                        rk.SetValue("TxOutputGain", deviceTx.Gain.ToString());
                    if (null != waveDevicePlayer)
                        rk.SetValue("RxInputGain", waveDevicePlayer.Gain.ToString());
                }
            }
            if (demodulatorWrapper != null)
                demodulatorWrapper.Dispose();
            demodulatorWrapper = null;

            if (waveDevicePlayer != null)
                waveDevicePlayer.Dispose();
            waveDevicePlayer = null;

            if (logFile != null)
            {
                logFile.SendToLog("Closed");
                logFile.Dispose();
            }
            logFile = null;

            if (conversationLogFile != null)
                conversationLogFile.Dispose();
            conversationLogFile = null;
        }

        private bool SetupTxAndRxDeviceIndicies()
        {
            // default device select to what's in settings
            TxOutDevice = StringToIndex(Properties.Settings.Default["AudioOutputDevice_" + instanceNumber.ToString()].ToString(),
                XD.WaveDeviceEnumerator.waveOutDevices());
            RxInDevice = StringToIndex(Properties.Settings.Default["AudioInputDevice_" + instanceNumber.ToString()].ToString(),
               XD.WaveDeviceEnumerator.waveInDevices());
            if (null != logger)
            {
                MyCall = logger.CallUsed;
                if (!String.IsNullOrEmpty(myCall))
                    Properties.Settings.Default.CallUsed = myCall;
                var wlSetup = logger as DigiRiteLogger.WriteLog;
                if (null != wlSetup)// we are connected to WriteLog's automation interface
                     return wlSetup.SetupTxAndRxDeviceIndicies(ref SetupMaySelectDevices, ref RxInDevice, ref TxOutDevice,
                         (short lr) =>
                         {
                             Properties.Settings.Default["AudioInputChannel_" + instanceNumber.ToString()] = (uint)lr;
                             Properties.Settings.Default["AudioOutputChannel_" + instanceNumber.ToString()] = (uint)lr;
                             SetupMaySelectLR = false;
                         });
            }
            else
                return false;
            return true;
        }

        private void timerSpectrum_Tick(object sender, EventArgs e)
        {
            if ((null != rxForm) && (null != demodulatorWrapper))
                rxForm.DisplaySpectrum(demodulatorWrapper.demodulator);
        }
        
        private void timerCleanup_Tick(object sender, EventArgs e)
        {
            DateTime removalTime = DateTime.UtcNow - TimeSpan.FromMinutes(Properties.Settings.Default.ReactivateQSOTimerMinutes);
            qsosPanel.PurgeOldLoggedQsos(removalTime);
        }

        private System.Drawing.Point locationToSave;
        private System.Drawing.Size sizeToSave;
        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                locationToSave = Location;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                sizeToSave = Size;
        }

        private delegate void OneAtATimeDel();
        private bool inOneAtATime = false;
        private List<OneAtATimeDel> oneAtATimeList = new List<OneAtATimeDel>();
        void OneAtATime(OneAtATimeDel d)
        {
            /* Calls out on COM automation are opportunities to
             * reenter this class. That reentrancy is is handled here 
             * (i.e. prevented) by keeping a queue in oneAtATimeList */
            if (inOneAtATime)
            {
                oneAtATimeList.Add(d);
                return;
            }
            inOneAtATime = true;
            d();
            while (oneAtATimeList.Any())
            {
                var deferred = oneAtATimeList.First();
                oneAtATimeList.RemoveAt(0);
                deferred();
            }
            inOneAtATime = false;
        }

        private bool inClockTick = false;
        private bool zeroIntervalCalled = false;
        private bool cqClearCalled = false;
        private bool transmitAtZeroCalled = false;

        private uint intervalTenths = 0;
        private int cycleNumber = 0;
        uint CLEAR_OLD_MESSAGES_AT_TENTHS = 50;
        uint TRIGGER_DECODE_TENTHS = 50; // At this second and later, see if we see any messages

        const uint START_LATE_MESSAGES_THROUGH_TENTHS = 60;
        private delegate void CheckTransmitAtZero(int cycleNumber, int msecIntoCycle);
        private CheckTransmitAtZero checkTransmitAtZero;

        private void Ft8CheckTransmitAtZero(int cycleNumber, int msecIntoCycle)
        {
            if (msecIntoCycle < 0)
                return;
            if (intervalTenths >= FT_CYCLE_TENTHS - TENTHS_IN_SECOND)
            {   // during final second 
                const int PREP_TRANSMIT_MSEC = 1050;
                int FT_CYCLE_MSEC = FT_CYCLE_TENTHS * 100;
                if (!transmitAtZeroCalled && (msecIntoCycle >= FT_CYCLE_MSEC - PREP_TRANSMIT_MSEC))
                {
                    int preTransmitCycleNumber = cycleNumber + 1; // pretransmit for NEXT cycle
                    bool IsTransmit = ((cycleNumber & 1) != 0) == radioButtonOdd.Checked;
                    int nowToCycleStartMsec = FT_CYCLE_MSEC + 1 - msecIntoCycle; // now to start of next cycle
                    var timeToReport = DateTime.UtcNow + TimeSpan.FromMilliseconds(nowToCycleStartMsec);
                    int nowToCallXmit = nowToCycleStartMsec + FT8_TX_AFTER_ZERO_MSEC - UserPttToSound - UserVfoSplitToPtt;
                    int delayMsec = 0;
                    if (nowToCallXmit > 0)
                    {
                        VfoSetToTxMsec = UserVfoSplitToPtt; // normal case--we have time
                        delayMsec = nowToCallXmit;
                    }
                    else
                    {
                        VfoSetToTxMsec = UserVfoSplitToPtt; // timer callback came too late--just will go out late
                        delayMsec = 1;
                    }
                    var getTimeToReport = new GetNowTime(() => timeToReport);

                    AfterNmsec(
                        new Action(() =>
                        {
                            cycleNumber = preTransmitCycleNumber;
                            qsoQueue.OnCycleBeginning(cycleNumber);
                            transmitAtZero(false, getTimeToReport);
                        }), delayMsec);
                    transmitAtZeroCalled = true;
                }
            }
            else
                transmitAtZeroCalled = false;
        }

        private void Ft4CheckTransmitAtZero(int cycleNumber, int msecIntoCycle)
        {
            if (msecIntoCycle < 0)
                return;
            if (intervalTenths >= FT_CYCLE_TENTHS - TENTHS_IN_SECOND)
            {   // during final second 
                const int PREP_TRANSMIT_MSEC = 1050;
                int FT_CYCLE_MSEC = FT_CYCLE_TENTHS * 100;
                if (!transmitAtZeroCalled && (msecIntoCycle >= FT_CYCLE_MSEC - PREP_TRANSMIT_MSEC))
                {
                    int preTransmitCycleNumber = cycleNumber + 1; // pretransmit for NEXT cycle
                    bool IsTransmit = ((cycleNumber & 1) != 0) == radioButtonOdd.Checked;
                    // FT4 is MUCH less tolerant of clock sync issues than FT8.
                    int nowToCycleStartMsec = FT_CYCLE_MSEC + 1 - msecIntoCycle; // now to start of next cycle
                    var timeToReport = DateTime.UtcNow + TimeSpan.FromMilliseconds(nowToCycleStartMsec);
                    int nowToCallXmit = nowToCycleStartMsec + FT4_TX_AFTER_ZERO_MSEC - UserPttToSound - UserVfoSplitToPtt;
                    int delayMsec = 0;
                    if (nowToCallXmit > 0)
                    {
                        VfoSetToTxMsec = UserVfoSplitToPtt; // normal case--we have time
                        delayMsec = nowToCallXmit;
                    }
                    else
                    {
                        VfoSetToTxMsec = UserVfoSplitToPtt; // timer callback came too late--just will go out late
                        delayMsec = 1;
                    }
                    var getTimeToReport = new GetNowTime(() => timeToReport);
                        
                    AfterNmsec(
                        new Action(() =>
                        {
                            cycleNumber = preTransmitCycleNumber;
                            qsoQueue.OnCycleBeginning(cycleNumber);
                            transmitAtZero(false, getTimeToReport); 
                        }), delayMsec);
                    transmitAtZeroCalled = true;
                }
            }
            else
                transmitAtZeroCalled = false;
        }
        private ushort[] ft4DecodeOffsetMsec;
        private int ft4DecodeOffsetIdx;
        private float ft4MsecOffset = 0;

        /* having a clock to call the decoder simplifies
        ** keeping the demodulator on this gui thread.
        ** The timing of the clock is not important with
        ** the exception that the decoder needs to be called close
        ** to the beginning of a cycle second...so call here 
        ** a "few" times per second. */
        private void timerFt8Clock_Tick(object sender, EventArgs e)
        {
            if (inClockTick) // don't recurse
                return; // didn't need to be here anyway
            inClockTick = true;
            var nowutc = DateTime.UtcNow;
            if ((nowutc - watchDogTime).TotalMinutes > MAX_UNANSWERED_MINUTES)
                checkBoxAutoXmit.Checked = false;
            OneAtATime(new OneAtATimeDel(() =>
            {
                try
                {
                    labelClock.Text = "";
                    int msecIntoCycle = -1;
                    if ((null != demodulatorWrapper) && (null != waveDevicePlayer))
                    {
                        // TRIGGER_DECODE tells the demodulator whether to actually demodulate
                        bool invokedDecode = false;
                        String hiscall = qsosPanel.FirstActive?.HisCall;
                        demodulatorWrapper.mycall = myCall;
                        demodulatorWrapper.hiscall = hiscall;
                        intervalTenths = demodulatorWrapper.Clock(TRIGGER_DECODE_TENTHS, ref invokedDecode, ref cycleNumber);
                        // invokedDecode tells us whether it actually was able to invoke the wsjtx decoder.
                        // Some reasons it might not: interval is less than TRIGGER_DECODE.
                        // We have recently called into Clock which did invoke a decode, and that one isn't finished yet.
                        if (invokedDecode)
                        {
                            ft4DecodeOffsetIdx = 0;
                            if (null != ft4DecodeOffsetMsec)
                                ft4MsecOffset = 1e-3f * (float)((int)ft4DecodeOffsetMsec[0] - (int)FT4_DECODER_CENTER_OFFSET_MSEC);
                        }

                        labelClock.Text = (intervalTenths/ TENTHS_IN_SECOND).ToString();
                        // twice per second, and synced to the utc second
                        var nowTime = DateTime.UtcNow;
                        int nowmsec = nowTime.Millisecond;
                        if (nowmsec > 500)
                            nowmsec -= 500;
                        // stay close to the begin/middle of the UTC second
                        timerFt8Clock.Interval = 501 - nowmsec; 

                        int msecInMinute = 1000 * nowTime.Second + nowTime.Millisecond;
                        msecIntoCycle = msecInMinute % ( FT_CYCLE_TENTHS * 100);
                    }
                    bool isOddCycle = (cycleNumber & 1) != 0;
                    bool isTransmitCycle = radioButtonOdd.Checked == isOddCycle;
                    rxForm.OnClock(intervalTenths, isTransmitCycle);

#if DEBUG
                    if (intervalTenths >= SIMULATOR_POPS_OUT_DECODED_MESSAGES_AT_TENTHS)
                    {   // invoke simulator in second #13 for FT8
                        if (!simulatedThisInterval)
                        {
                            simulatedThisInterval = true;
                            while (simulatorLines != null && simulatorLines.Count > simulatorNext)
                            {
                                var now = DateTime.UtcNow;
                                bool isOdd;
                                int simNextTenths = timeStampTenths(simulatorLines[simulatorNext], out isOdd);
                                if (simNextTenths <= INVALID_TIME_TENTHS)
                                    simulatorNext += 1; // skip it
                                else
                                {
                                    int simTimeTenths = (simNextTenths - simulatorTimeOrigin);
                                    if (isOdd != isOddCycle) 
                                        simTimeTenths += FT_CYCLE_TENTHS; // delay simulation to match odd/even w.r.t. real time
                                    if (simTimeTenths < 0)
                                        simTimeTenths += 10 * 60 * 60;
                                    if (simTimeTenths > 10 * 60 * 60)
                                        simTimeTenths -= 10 * 60 * 60;
                                    int tenthsSinceOrigin = (int)(now - simulatorStart).TotalMilliseconds / 100;
                                    if (simTimeTenths <= tenthsSinceOrigin)
                                        OnReceived(simulatorLines[simulatorNext++], (simNextTenths / FT_CYCLE_TENTHS) % (600 / FT_CYCLE_TENTHS));
                                    else
                                        break;
                                }
                            }
                        }
                    }
                    else
                        simulatedThisInterval = false;
#endif

                    if (null != checkTransmitAtZero)
                        checkTransmitAtZero(cycleNumber, msecIntoCycle);

                    if (intervalTenths < TENTHS_IN_SECOND)
                    {   // FT8 cycle second zero is a special time
                        if (!zeroIntervalCalled)
                        {   // only once per cycle
                            zeroIntervalCalled = true;
                            if (null != logger)
                                currentBand = logger.GetCurrentBand();
                        }
                     }
                    else
                        zeroIntervalCalled = false;

                    if (intervalTenths >= CLEAR_OLD_MESSAGES_AT_TENTHS)
                    {
                        if (!cqClearCalled)
                        {
                            if (isOddCycle)
                                cqListOdd.Reset();
                            else
                                cqListEven.Reset();
                            if (radioButtonEven.Checked == isOddCycle)
                                toMe.Reset();
                            if (isTransmitCycle)
                            {  
                                // if there is an active QSO and
                                // if there is not an already-checked alternative message
                                // then fillAlternativeMessages
                                var q = qsosPanel.FirstActive;
                                if (null != q)
                                {
                                    bool isChecked = false;
                                    for (int i = 0; i < listBoxAlternatives.Items.Count; i++)
                                        if (listBoxAlternatives.GetItemChecked(i))
                                        {
                                            isChecked = true;
                                            break;
                                        }
                                    if (!isChecked)
                                        fillAlternativeMessages(q);
                                }
                            }
                        }
                        cqClearCalled = true;
                    }
                    else
                        cqClearCalled = false;

                    ClockLabel cl = labelClockAnimation as ClockLabel;
                    if (null != cl)
                    {
                        cl.Tenths = intervalTenths;
                        cl.AmTransmit = isTransmitCycle;
                    }
                    if (forceRigUsb && (null != logger))
                        logger.ForceRigToUsb();
                }
                finally
                { inClockTick = false; }
            }));
        }

        private string m_previousCallToWriteLog; // optimization to reduce out-of-process calls
        private void CurrentActiveQsoCalltoLogger()
        {
            if (null == logger)
                return;
            var inProgress = qsosPanel.QsosInProgress;
            for (int i = 0; i < inProgress.Count; i++)
            {
                QsoInProgress qp = inProgress[i];
                if (null == qp)
                    continue;
                if (!qp.Active)
                    continue;
                if (qp.IsLogged)
                    continue;
                if (m_previousCallToWriteLog != qp.HisCall)
                {
                    m_previousCallToWriteLog = qp.HisCall;
                    string grid = "";
                    foreach (var m in qp.MessageList)
                    {
                        XDpack77.Pack77Message.Exchange hisGrid = m.Pack77Message as XDpack77.Pack77Message.Exchange;
                        if (hisGrid != null && !String.IsNullOrEmpty(hisGrid.GridSquare))
                        {
                            grid = hisGrid.GridSquare;
                            break;
                        }
                    }
                    logger.SetCurrentCallAndGrid(qp.HisCall, grid);
                }
                return;
            }
            m_previousCallToWriteLog = "";
            logger.SetCurrentCallAndGrid("", "");
        }

#endregion

#region foreign threads
        // The XDft8 assembly invokes our delegate on a foreign thread.
        private void Decoded(String s, int cycle)
        {   // BeginInvoke back onto form's thread
                BeginInvoke(new Action<String, int>((String x, int c) => OnReceived(x, c)), s, cycle);
        }

        private void AudioBeginEnd(bool isBeginning)
        {   // get back on the form's thread
            BeginInvoke(new Action<bool>(OnAudioComplete), isBeginning);
        }
#endregion

        private void OnAudioComplete(bool isBegin)
        {
            // tell writelog to turn on/off the PTT
            if (null != logger)
            {
                logger.SetPtt(isBegin);
                if (!isBegin)
                    RigVfoSplitForTxOff();
            }
            SendInProgress = isBegin;
        }

        public void SendRttyMessage(String toSend) // WriteLog pressed an F-key
        {   // automation call from WriteLog....
        }

        public void AbortMessage()
        {
            if (null != deviceTx)
                deviceTx.Abort();
            SendInProgress = false;
        }
     
        private void quitQso(QsoInProgress q)
        {
            qsosPanel.Remove(q);
            for (int i = 0; i < checkedlbNextToSend.Items.Count;)
            {
                QueuedToSendListItem qli = checkedlbNextToSend.Items[i] as QueuedToSendListItem;
                if ((null != qli) && Object.ReferenceEquals(qli.q, q))
                        checkedlbNextToSend.Items.RemoveAt(i);
                else
                    i += 1;
            }
        }

#region form control events

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            AbortMessage();
            for (int i = 0; i < checkedlbNextToSend.Items.Count; i++)
                checkedlbNextToSend.SetItemChecked(i, false);
            for (int i = 0; i < listBoxAlternatives.Items.Count; i++)
                listBoxAlternatives.SetItemChecked(i, false);
            checkBoxAutoXmit.Checked = false;
            checkBoxManualEntry.Checked = false;
            comboBoxCQ.SelectedIndex = 0;
        }

        private void radioOddEven_CheckedChanged(object sender, EventArgs e)
        {
            deviceTx.TransmitCycle = radioButtonEven.Checked ? 
                XD.Transmit_Cycle.PLAY_EVEN_15S : XD.Transmit_Cycle.PLAY_ODD_15S;
            if (!checkBoxCQboth.Checked)
            {
                splitContainerCQ.Panel1Collapsed = radioButtonEven.Checked ^ false;
                splitContainerCQ.Panel2Collapsed = radioButtonEven.Checked ^ true;
            }
            qsosPanel.RefreshOnScreen();
        }

        private void checkedlbNextToSend_ItemCheck(object sender, ItemCheckEventArgs e)
        {   // ItemCheck event preceeds checking the check box. 
            // ..but I want the check box true before calling transmitAtZero...
            // ..so have to work around recursion issues
            bool checkState = e.NewValue == CheckState.Checked;
            if (checkState)
                BeginInvoke(new Action(() =>
                {
                    if (!SendInProgress && intervalTenths <= START_LATE_MESSAGES_THROUGH_TENTHS)
                        transmitAtZero(true);
                }));
        }

        private void checkBoxAutoXmit_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoXmit.Checked)
                    watchDogTime = DateTime.UtcNow;
            for (int i = 0; i < checkedlbNextToSend.Items.Count; i++)
                checkedlbNextToSend.SetItemChecked(i, checkBoxAutoXmit.Checked);
        }

        private void checkBoxCalcNextToSend_CheckedChanged(object sender, EventArgs e)
        {
            checkedlbNextToSend.Visible = checkBoxCalcNextToSend.Checked;
        }

        private void listBoxAlternatives_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {   // turn all the others off
                for (int i = 0; i < listBoxAlternatives.Items.Count; i++)
                {
                    if (i == e.Index)
                        continue;
                    listBoxAlternatives.SetItemChecked(i, false);
                }
                QueuedToSendListItem qli = listBoxAlternatives.Items[e.Index] as QueuedToSendListItem;
                if (null != qli)
                    textBoxMessageEdit.Text = qli.MessageText;
            }
        }

        private void listBoxAlternatives_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueuedToSendListItem qli = listBoxAlternatives.SelectedItem as QueuedToSendListItem;
            if (null != qli)
                textBoxMessageEdit.Text = qli.MessageText;
        }

        private void textBoxMessageEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {   // user typed CR with focus in manual entry text box
                e.Handled = true;
                checkBoxManualEntry.Checked = true;
            }
        }
     
        private void checkBoxRespondNonDupe_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRespondNonDupe.Checked)
            {
                if (checkBoxRespondAny.Checked)
                    checkBoxRespondAny.Checked = false;
                watchDogTime = DateTime.UtcNow;
                checkBoxAutoXmit.Checked = true;
            }
            splitContainerAnswerUpCqsDown.Panel1Collapsed = checkBoxRespondNonDupe.Checked;
        }

        private void checkBoxRespondAny_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRespondAny.Checked)
            {
                if (checkBoxRespondNonDupe.Checked)
                    checkBoxRespondNonDupe.Checked = false;
                watchDogTime = DateTime.UtcNow;
                checkBoxAutoXmit.Checked = true;
            }
            splitContainerAnswerUpCqsDown.Panel1Collapsed = checkBoxRespondAny.Checked;

        }

        private void OnQsoActiveChanged(QsoInProgress q)
        {
            for (int i = 0; i < checkedlbNextToSend.Items.Count; i++)
            {
                QueuedToSendListItem qli = checkedlbNextToSend.Items[i] as QueuedToSendListItem;
                if ((null != qli) && Object.ReferenceEquals(qli.q, q))
                    checkedlbNextToSend.SetItemChecked(i, q.Active && checkBoxAutoXmit.Checked);
            }
            CurrentActiveQsoCalltoLogger();
        }

        private void OnQsoInProgressOrderChanged()
        {
            checkedlbNextToSend.Sort();
            CurrentActiveQsoCalltoLogger();
        }

        private void checkBoxShowMenu_CheckedChanged(object sender, EventArgs e)
        { menuStrip.Visible = checkBoxShowMenu.Checked;  }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {  Close(); }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool maySelectCallUsed = null == logger;
            if (!maySelectCallUsed)
            {
                String curCall = logger.CallUsed;
                if (!String.IsNullOrEmpty(curCall))
                {
                    Properties.Settings.Default.CallUsed = curCall.ToUpper().Trim();
                    MyCall = curCall;
                    qsoQueue.MyCall = myCall;
                    qsoQueue.MyBaseCall = myBaseCall;
                }
            }
            var form = new SetupForm(instanceNumber,
                SetupMaySelectDevices, SetupMaySelectLR,
                maySelectCallUsed);
            form.controlSplit = controlVFOsplit;
            form.forceRigUsb = forceRigUsb;
            form.txHighLimit = TxHighFreqLimit;
            form.digiMode = digiMode;
            form.PttToSound = UserPttToSound;
            form.VfoSplitToPtt = UserVfoSplitToPtt;
            var res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                controlVFOsplit = form.controlSplit;
                forceRigUsb = form.forceRigUsb;
                TxHighFreqLimit = form.txHighLimit;
                digiMode = form.digiMode;
                UserPttToSound = form.PttToSound;
                UserVfoSplitToPtt = form.VfoSplitToPtt;
                if (form.MustResetState)
                    changeDigiMode();
                if (SetupMaySelectDevices)
                {
                    if (form.whichRxDevice >= 0)
                        RxInDevice = (uint)form.whichRxDevice;
                    if (form.whichTxDevice >= 0)
                        TxOutDevice = (uint)form.whichTxDevice;
                }
                if (maySelectCallUsed)
                {
                    MyCall = Properties.Settings.Default.CallUsed;
                    qsoQueue.MyCall = myCall;
                    qsoQueue.MyBaseCall = myBaseCall;
                }
                InitSoundInAndOut();
                if (form.MustResetState)
                    initQsoQueue();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {  new AboutForm().ShowDialog(); }

        private void checkBoxCQboth_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCQboth.Checked)
            {
                splitContainerCQ.Panel1Collapsed = false;
                splitContainerCQ.Panel2Collapsed = false;
            }
            else
            {
                splitContainerCQ.Panel1Collapsed = radioButtonEven.Checked ^ false;
                splitContainerCQ.Panel2Collapsed = radioButtonEven.Checked ^ true;
            }
        }

        private void checkBoxOnlyCQs_CheckedChanged(object sender, EventArgs e)
        {
            cqListEven.FilterCqs = cqListOdd.FilterCqs = checkBoxOnlyCQs.CheckState;
        }

        private void viewLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logFile.Flush();
            System.Diagnostics.Process.Start(LogFilePath);
        }

        private void viewReadMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string readme = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                + "\\ReadMe.htm";
            try
            { System.Diagnostics.Process.Start(readme); }
            catch (System.Exception) { }
        }
        
        private void helpToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            if (null != tsmi)
            {
                var logLength = LogFile.LogFileLength(LogFilePath);
                if (logLength != 0)
                    logFileLengthToolStripMenuItem.Text = 
                        String.Format("Log file length: {0:#,##0.0} MB", logLength/(1024.0*1024.0));
            }
        }

        private void resetLogFileToEmpyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to clear your log file?", 
                "DigiRite", MessageBoxButtons.YesNo) == DialogResult.Yes)
                logFile.ResetToEmpty(LogFilePath);
        }

        private void removeAllInactiveToolStripMenuItem_Click(object sender, EventArgs e)
        { qsosPanel.removeAllInactive();  }

        private void trackBarTxGain_Scroll(object sender, EventArgs e)
        {
            deviceTx.Gain = gainFromTrackValue(trackBarTxGain.Value);
            labelTxValue.Text = trackBarTxGain.Value.ToString();
        }
        
        private void comboBoxCQ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCQ.SelectedIndex != 0)
                checkBoxAutoXmit.Checked = true;
        }
        int TUNE_LEN;

        private void ApplyFontControls()
        {
            var font = Properties.Settings.Default.FixedFont;
            panelInProgress.Font = font;
            listToMe.Font = font;
            panelEvenCQs.Font = font;
            panelOddCQs.Font = font;
            listBoxAlternatives.Font = font;
            altMessageShortcuts.setup();
            altMessageShortcuts.Populate();
            listBoxConversation.Font = font;
            listBoxConversation.ItemHeight = font.Height;
            textBoxMessageEdit.Font = font;
            rxForm.SetFixedFont(font);
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FontDialog fd = new FontDialog();
            fd.FixedPitchOnly = true;
            fd.Font = Properties.Settings.Default.FixedFont;
            if (fd.ShowDialog() != DialogResult.Cancel)
            {
                Properties.Settings.Default.FixedFont = fd.Font;
                ApplyFontControls();
            }
        }

        private decimal numericUpDownStreamsPrevious = 1m;
        private void numericUpDownStreams_ValueChanged(object sender, EventArgs e)
        {
            int v = (int)numericUpDownStreams.Value;
            if ((v == 2 && numericUpDownStreamsPrevious == 1m) ||
                (v == 1 && numericUpDownStreamsPrevious == 2m))
                numericUpDownStreams.Value = 1.1m;

            if ((decimal)v != numericUpDownStreams.Value)
            {
                if (numericUpDownStreams.Value == 1.1m)
                    numericUpDownStreams.DecimalPlaces = 1;
                else
                    numericUpDownStreams.Value = v;
            }
            else
                numericUpDownStreams.DecimalPlaces = 0;
            numericUpDownStreamsPrevious = numericUpDownStreams.Value;
        }

        private void numericUpDownStreams_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        #endregion

        #region TX RX frequency
        private void buttonTune_Click(object sender, EventArgs e)
        {
            if (SendInProgress)
                return;
            deviceTx.TransmitCycle = XD.Transmit_Cycle.PLAY_NOW;
            int tuneFrequency = TxFrequency;
            RigVfoSplitForTx(tuneFrequency, tuneFrequency + FT_GAP_HZ);
            int[] it = new int[TUNE_LEN];
            AfterNmsec(new Action(() =>
                XDft.Generator.Play(genContext, it, tuneFrequency, deviceTx.GetRealTimeAudioSink())), UserVfoSplitToPtt);
        }

        public int TxFrequency {
            get {
                return (int)numericUpDownFrequency.Value;
            }
            set {
                if ((value <= 0) || (value > 6000))
                    return;
                numericUpDownFrequency.Value = value;
            }
        }

        public int RxFrequency {
            get {
                if (null != demodulatorWrapper)
                    return (int)demodulatorWrapper.nfqso;
                return 0;
            }
            set {
                if ((value <= 0) || (value > 6000))
                    return;
                int v = value;
                if (null != demodulatorWrapper)
                    demodulatorWrapper.nfqso = v;
                rxForm.RxHz = v;
                if (v != (int)numericUpDownRxFrequency.Value)
                {
                    if (v >= numericUpDownFrequency.Minimum && (v <= numericUpDownFrequency.Maximum))
                        numericUpDownRxFrequency.Value = v;
                }
            }
        }
        
        private void numericUpDownFrequency_ValueChanged(object sender, EventArgs e)
        { rxForm.TxHz = TxFrequency; }

        private void numericUpDownRxFrequency_ValueChanged(object sender, EventArgs e)
        { RxFrequency = (int)numericUpDownRxFrequency.Value;}

        private void buttonEqTx_Click(object sender, EventArgs e)
        { numericUpDownRxFrequency.Value = numericUpDownFrequency.Value; }

        private void buttonEqRx_Click(object sender, EventArgs e)
        { numericUpDownFrequency.Value = numericUpDownRxFrequency.Value; }

        private void buttonTxToQip_Click(object sender, EventArgs e)
        {
            var inProgress = qsosPanel.QsosInProgress;
            for (int i = 0; i < inProgress.Count; i++)
            {
                QsoInProgress qp = inProgress[i];
                if (null == qp)
                    continue;   
                if (qp.TransmitFrequency != qp.Message.Hz || ModifierKeys.HasFlag(Keys.Control))
                    qp.TransmitFrequency = 0;
            }
        }

        #endregion

 
     
    }
}
