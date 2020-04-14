using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiRite
{
    /* MultiDemodulatorWrapper
     * Consolidate the various objects required to host a WSJT-X decoder into this one class
     * while also presenting (almost) the same interface to the rest of the DigiRite application.
     * And allow for the case of multiple decoders, each responsible for only part of the
     * total bandwidth being decoded. Why does that help? because it has been observed that
     * (a) limiting the bandwidth on the decoder causes it to finish presenting its messages
     * much sooner
     * (b) it runs on only one thread and therefore
     * (c) running multiple decoders on a multi-core CPU consumes more of them while getting
     * decoded messages sooner.
     */
    public class MultiDemodulatorWrapper : IDisposable
    {
        public static int MAX_MULTIPROC = 8;
        // the decoder seems to find signals right up to the nfa/nfb limit, but we'll overlap anyway
        private const int FREQ_OVERLAP_HZ = 8;
        private const double MIN_RANGE_HZ = 200; // give each decoder at least this much, even if that means using fewer
        
        // these are all parallel arrays
        private XDft.Demodulator[] demodulators;
        private XDft.WsjtSharedMemory[] wsjtSharedMems;
        private XDft.WsjtExe[] wsjtExes;
        private bool[] enabled;

        private XDft.RxSinkRepeater rxSinkRepeater; // need help repeating the RX audio to each decoder

        // have to know at construction time how many decoders
        public MultiDemodulatorWrapper(int instanceNumber, uint numDemodulators = 1)
        {
            if (numDemodulators > MAX_MULTIPROC)
                numDemodulators = (uint)MAX_MULTIPROC;
            if (numDemodulators == 0)
                numDemodulators = 1;

            demodulators = new XDft.Demodulator[numDemodulators];
            wsjtSharedMems = new XDft.WsjtSharedMemory[numDemodulators];
            wsjtExes = new XDft.WsjtExe[numDemodulators];
            enabled = new bool[numDemodulators];

            for (uint i = 0; i < numDemodulators; i++)
            {
                demodulators[i] = new XDft.Demodulator();
                enabled[i] = true;

                // The first decoder gets the same subdirectory name as a single decoder
                string sharedMemoryKey = "DigiRite-" + instanceNumber.ToString();
                if (i != 0) // subsequent ones get extra goop in their names
                    sharedMemoryKey += "-" + i.ToString();
                wsjtSharedMems[i] = new XDft.WsjtSharedMemory(sharedMemoryKey, false);
                if (!wsjtSharedMems[i].CreateWsjtSharedMem())
                    throw new System.Exception("Failed to create Shared Memory from " + sharedMemoryKey);

                // The subprocess itself is managed by the XDft
                wsjtExes[i] = new XDft.WsjtExe();
                wsjtExes[i].AppDataName = sharedMemoryKey;

                if (!wsjtExes[i].CreateWsjtProcess(wsjtSharedMems[i]))
                {
                    Dispose();
                    throw new System.Exception("Failed to launch wsjt exe");
                }
            }
            multibandManager = new MultibandManagerOneDemod(demodulators);
        }

        // There always is at least one, so provide a simple way to get at that one
        public XDft.Demodulator demodulator { get { return demodulators[0]; } }

        public XDft.DemodResult DemodulatorResultCallback {
            get { return demodulator.DemodulatorResultCallback; }
            set { 
                foreach (var a in demodulators) 
                    a.DemodulatorResultCallback = value; 
                } }

        // parallelization is done by frequency ranges
        private int m_nfa;     
        private int m_nfb;

        // the min and max decoder frequencies are handled in this special way
        public int nfa { get { return m_nfa; }
            set { 
                m_nfa = value;
                AllocateFrequencyBands(); 
                } 
            }

        public int nfb { get { return m_nfb; }
            set {
                m_nfb = value;
                AllocateFrequencyBands();
                }
            }

        private class FrequencyBand
        {
            public FrequencyBand(int nfa)
            {
                this.nfa = nfa;
                nfb = 0;
            }
            public int nfa;
            public int nfb;
        }

        private void AllocateFrequencyBands()
        {
            multibandManager = new MultibandManagerOneDemod(demodulators);
            if (nfa >= nfb)
                return;
            int nDemodsMinusOne = demodulators.Count() - 1;
            if (nDemodsMinusOne == 0)
            {
                demodulator.nfa = m_nfa;
                demodulator.nfb = m_nfb;
                return;
            }

            // defer assigning nfa and nfb until the demodulator starts
            List<FrequencyBand> bands = new List<FrequencyBand>();
            bands.Add(new FrequencyBand(m_nfa));

            // iterate over all but first and last
            double diff = m_nfb - m_nfa;
            double range = diff / (nDemodsMinusOne+1);
            int num = nDemodsMinusOne + 1;
            bool enable = true;
            if (range < MIN_RANGE_HZ)
            {   // limit multi-processing to wider than this
                num = (int)(diff / MIN_RANGE_HZ);
                if (num <= 1)
                {
                    // the best we can do is put the whole range in one decoder
                    num = 1;
                    enable = false;
                }
                range = diff / num;
            }
            for (int i = 1; i <= nDemodsMinusOne; i++)
            {
                if (enable)
                {
                    int lowBound = (int)(m_nfa + i * range);
                    if (lowBound >= m_nfb)
                        enable = false;
                    bands.Last().nfb = lowBound + FREQ_OVERLAP_HZ;
                    bands.Add(new FrequencyBand(lowBound - FREQ_OVERLAP_HZ));
                }
                enabled[i] = enable;
            }
            bands.Last().nfb = m_nfb;
            multibandManager = new MultibandManager(bands.ToArray(), demodulators);
        }

        // all properties except min and max are just duplicated among all decoders
        public int n2pass { get { return demodulator.n2pass; }
            set { foreach (var a in demodulators) a.n2pass = value; } }

        public int ndepth { get { return demodulator.ndepth; }
            set { foreach (var a in demodulators) a.ndepth = value; } }

        public int nfqso { get { return demodulator.nfqso; }
            set { foreach (var a in demodulators) a.nfqso = value; } }

        public int nftx { get { return demodulator.nftx; }
            set { foreach (var a in demodulators) a.nftx = value; } }

        public bool lft8apon { get { return demodulator.lft8apon; }
            set { foreach (var a in demodulators) a.lft8apon = value; } }

        public int nexp_decode { get { return demodulator.nexp_decode; }
            set { foreach (var a in demodulators) a.nexp_decode = value; } }

        public int nQSOProgress { get { return demodulator.nQSOProgress; }
            set { foreach (var a in demodulators) a.nQSOProgress = value; } }

        public int nzhsym { get { return demodulator.nzhsym; }
            set { foreach (var a in demodulators) a.nzhsym = value; } }

        public int npts8 { get { return demodulator.npts8; }
            set { foreach (var a in demodulators) a.npts8 = value; } }

        public string mycall { get { return demodulator.mycall; }
            set { foreach (var a in demodulators) a.mycall = value; } }

        public string hiscall { get { return demodulator.hiscall; }
            set { foreach (var a in demodulators) a.hiscall = value; } }

        public XDft.DigiMode digiMode { get { return demodulator.digiMode; }
            set { foreach (var a in demodulators) a.digiMode = value; } }

        public ushort DemodulateDefaultSoundShiftMsec { get { return demodulator.DemodulateDefaultSoundShiftMsec; }
            set { foreach (var a in demodulators) a.DemodulateDefaultSoundShiftMsec = value; } }

        public short DemodulateSoundPreUtcZeroMsec { get { return demodulator.DemodulateSoundPreUtcZeroMsec; }
            set { foreach (var a in demodulators) a.DemodulateSoundPreUtcZeroMsec = value; } }

        public string AppDirectoryPath { get { return wsjtExes[0].AppDirectoryPath; } }


        private interface IMultibandManager
        {
            bool ClockInProgress { get; set; }
        }
        
        private class MultibandManagerOneDemod : IMultibandManager
        {
            // constructor for manager that does nothing
            public MultibandManagerOneDemod(XDft.Demodulator[] demodulators)
            {
                foreach (var a in demodulators)
                    a.DecodeCallback = null;
            }

            public bool ClockInProgress { get; set ; } = false;
        }

        private class MultibandManager : IMultibandManager
        {
            FrequencyBand[] frequencyBands = null;
            XDft.Demodulator[] demodulators = null;
            int [] lastAllocatedBand = null;
            // constructor for manager that does something
            public MultibandManager(FrequencyBand[] frequencyBands, XDft.Demodulator[] demodulators)
            {
                this.frequencyBands = frequencyBands;
                this.demodulators = demodulators;
                int which = 0;
                foreach (var a in demodulators)
                {
                    int v = which++;
                    a.DecodeCallback = new XDft.StartDecodeCallback(() => AllocateBandwidth(v));
                }
                lastAllocatedBand = new int[frequencyBands.Count()];
                for (int i = 0; i < lastAllocatedBand.Length; i++)
                    lastAllocatedBand[i] = -1;
            }

            private Dictionary<int,int> assignedThisCycle;
            private bool clockInProgress = false;
            public bool ClockInProgress {
                get { return clockInProgress; }
                set {
                        if (value != clockInProgress)
                        {
                            clockInProgress = value;
                            if (value)
                                assignedThisCycle = new Dictionary<int, int>();
                        }
                    }
            }

            // decoder calls here right before it starts a decoding run.
            // set its nfa and nfb properties
            private void AllocateBandwidth(int which)
            {
                if (!ClockInProgress)
                    return; // DecodeAgain does NOT fiddle with the frequency assignments

                if ((which >= frequencyBands.Length) || (which < 0))
                    throw new System.Exception("Invalid decoder number"); // demodulator clr ignores this...

                lock (this)
                {
                // rotate the frequency band assignments through the demodulators.
                // Why? mostly to give them a chance to populate their hashed callsign tables.
                    int nextToAssign = lastAllocatedBand[which];
                    if (nextToAssign < 0)
                        nextToAssign = which;
                    else
                        nextToAssign += 1;

                    for (;;)
                    {
                        if (nextToAssign >= lastAllocatedBand.Length)
                            nextToAssign = 0;
                        if (!assignedThisCycle.TryGetValue(nextToAssign, out int val))
                        {
                            assignedThisCycle[nextToAssign] = which;
                            break;
                        }
                        else
                        {
                            if (val == which) // we got called multiple times in same cycle for same decoder...
                                return;
                            nextToAssign += 1;
                        }
                    }
                    demodulators[which].nfa = frequencyBands[nextToAssign].nfa;
                    demodulators[which].nfb = frequencyBands[nextToAssign].nfb;
                    lastAllocatedBand[which] = nextToAssign;
                }
            }
        }
        private IMultibandManager multibandManager;

        public uint Clock(uint tenthToTriggerDecode,  ref bool invokedDecode, ref int cycleNumber)
        {
            bool invDecodeAtEnd = false;
            uint ret = 0;
            int i = 0;
            multibandManager.ClockInProgress = true;
            foreach (var a in demodulators)
            {
                if (enabled[i])
                {
                    bool ivd = false;
                    ret = a.Clock(tenthToTriggerDecode, wsjtExes[i++], ref ivd, ref cycleNumber);
                    if (ivd)
                        invDecodeAtEnd = true;
                }
            }
            invokedDecode = invDecodeAtEnd;
            multibandManager.ClockInProgress = false;
            return ret;
        }

        public bool DecodeAgain(int cycleNumber, ushort msecOffset)
        {
            bool ret = false;
            int i = 0;
            foreach (var a in demodulators)
            {
                if (enabled[i])
                    if (a.DecodeAgain(wsjtExes[i], cycleNumber, msecOffset))
                        ret = true;
                i += 1;
            }
            return ret;
        }

        public void SetAudioSamplesCallback(XDft.AudioCallback callback, 
            uint sampleInterval, uint sampleCount, IntPtr nativeProcessor)
        {
            demodulator.SetAudioSamplesCallback(callback, sampleInterval,
                sampleCount, nativeProcessor);
        }

        public IntPtr GetRealTimeRxSink()
        {
            if (demodulators.Count() == 1)
                return demodulator.GetRealTimeRxSink();
            rxSinkRepeater = new XDft.RxSinkRepeater();
            foreach(var a in demodulators)
                rxSinkRepeater.AddSink(a.GetRealTimeRxSink());
            return rxSinkRepeater.GetRealTimeRxSink();
        }

        public void Dispose()
        {
            for (int i = 0; i < demodulators.Count(); i++)
            {
                if (null != demodulators[i])
                    demodulators[i].Dispose();
                demodulators[i] = null;
            }
            for (int i = 0; i < wsjtSharedMems.Count(); i++)
            {
                if (null != wsjtSharedMems[i])
                    wsjtSharedMems[i].Dispose();
                wsjtSharedMems[i] = null;
            }
            for (int i = 0; i < wsjtExes.Count(); i++)
            {
                if (null != wsjtExes[i])
                    wsjtExes[i].Dispose();
                wsjtExes[i] = null;
            }
            if (null != rxSinkRepeater)
                rxSinkRepeater.Dispose();
            rxSinkRepeater = null;
        }
    }
}
