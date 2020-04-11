using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiRite
{
    public class MultiDemodulatorWrapper : IDisposable
    {
        public static int MAX_MULTIPROC = 8;
        // the decoder seems to find signals right up to the nfa/nfb limit, but we'll overlap anyway
        private const int FREQ_OVERLAP_HZ = 8;
        private const double MIN_RANGE_HZ = 150;
        
        private XDft.Demodulator[] demodulators;
        private XDft.WsjtSharedMemory[] wsjtSharedMems;
        private XDft.WsjtExe[] wsjtExes;
        private XDft.RxSinkRepeater rxSinkRepeater;
        private bool[] enabled;

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

                string sharedMemoryKey = "DigiRite-" + instanceNumber.ToString();
                if (i != 0)
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
        }

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

        private void AllocateFrequencyBands()
        {
            if (nfa >= nfb)
                return;
            int nDemodsMinusOne = demodulators.Count() - 1;
            demodulator.nfa = m_nfa;
            demodulators[nDemodsMinusOne].nfb = m_nfb;
            if (nDemodsMinusOne == 0)
                return;
            // iterate over all but first and last
            double range = Math.Max(((double)(m_nfb - m_nfa) / (nDemodsMinusOne+1)), MIN_RANGE_HZ);
            bool enable = true;
            for (int i = 1; i <= nDemodsMinusOne; i++)
            {
                if (enable)
                {
                    int lowBound = (int)(m_nfa + i * range);
                    if (lowBound >= m_nfb)
                        enable = false;
                    demodulators[i - 1].nfb = lowBound + FREQ_OVERLAP_HZ;
                    demodulators[i].nfa = lowBound - FREQ_OVERLAP_HZ;
                }
                enabled[i] = enable;
           }
        }


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

        public uint Clock(uint tenthToTriggerDecode,  ref bool invokedDecode, ref int cycleNumber)
        {
            bool invDecodeAtEnd = false;
            uint ret = 0;
            int i = 0;
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
            return ret;
        }

        public bool DecodeAgain(int cycleNumber, ushort msecOffset)
        {
            bool ret = false;
            int i = 0;
            foreach (var a in demodulators)
                if (a.DecodeAgain(wsjtExes[i++], cycleNumber, msecOffset))
                    ret = true;
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
