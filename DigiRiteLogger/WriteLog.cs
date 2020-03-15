using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiRiteLogger
{
    /* Implementation of IDigiRiteLogger for calling WriteLog through WriteLog's
     * COM APIs.
     */
    public class WriteLog : IDigiRiteLogger
    {
        private int instanceNumber;
        private WriteLogClrTypes.ISingleEntry iWlEntry = null;
        private WriteLogClrTypes.ISingleEntry iWlDupingEntry = null;
        private WriteLogClrTypes.IWriteL iWlDoc = null;
        private System.IO.Ports.SerialPort pttPort = null;

        public WriteLog(int instanceNumber)
        {
            this.instanceNumber = instanceNumber;
        }

        public string CallUsed { get { return iWlDoc.CallUsed; } }

        public string SetWlEntry(object wl)
        {
            string labelPttText = "";
            if (pttPort != null)
                pttPort.Dispose();
            pttPort = null;
            if (wl == null)
            {
                iWlEntry = null;
                iWlDoc = null;
                iWlDupingEntry = null;
            }
            else
            {
                iWlEntry = (WriteLogClrTypes.ISingleEntry)(wl);
                iWlDoc = (WriteLogClrTypes.IWriteL)iWlEntry.GetParent();
                iWlDupingEntry = iWlDoc.CreateEntry();
                SetupExchangeFieldNumbers();
                string RttyRegKeyName = "Software\\W5XD\\writelog.ini\\RttyRite";
                if (instanceNumber > 1)
                    RttyRegKeyName += instanceNumber.ToString();
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RttyRegKeyName);
                if (null != rk)
                {
                    object Port = rk.GetValue("Port");
                    if (null != Port)
                    {
                        int port;
                        if (Int32.TryParse(Port.ToString(), out port) && port > 0)
                        {
                            try
                            {
                                string portname = "COM" + port.ToString();
                                pttPort = new System.IO.Ports.SerialPort(portname);
                                pttPort.Handshake = System.IO.Ports.Handshake.None;
                                pttPort.RtsEnable = false;
                                pttPort.Open();
                                labelPttText = "ptt on " + portname;
                            }
                            catch (System.Exception)
                            {
                                pttPort = null;
                            }
                        }
                    }
                }
            }
            return labelPttText;
        }

        private int SentRstFieldNumber = -1;
        private int ReceivedRstFieldNumber = -1;
        private int GridSquareReceivedFieldNumber = -1;
        private int GridSquareSentFieldNumber = -1;
        private int DgtlFieldNumber = -1;

        private void SetupExchangeFieldNumbers()
        {
            if (null == iWlDoc)
                return;
            WriteLogClrTypes.IQsoCollection qsoc = iWlDoc.GetQsoCollection() as WriteLogClrTypes.IQsoCollection;
            string[] names = qsoc.GetColumnAdifNames();
            int appWriteLogGrid = -1;
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == "RST_SENT")
                    SentRstFieldNumber = i + 1;
                else if (names[i] == "RST_RCVD")
                    ReceivedRstFieldNumber = i + 1;
                else if (names[i] == "GRIDSQUARE")
                    GridSquareReceivedFieldNumber = i + 1;
                else if (names[i] == "APP_WRITELOG_MYGRID")
                    GridSquareSentFieldNumber = i + 1;
                else if (names[i] == "APP_WRITELOG_GRID")
                    appWriteLogGrid = i + 1;
            }
            if (GridSquareReceivedFieldNumber <= 0)
                GridSquareReceivedFieldNumber = appWriteLogGrid;
            string[] titles = qsoc.GetColumnTitles();
            for (int i = 0; i < titles.Length; i++)
            {
                if (titles[i].ToUpper().IndexOf("DGTL") >= 0)
                {
                    DgtlFieldNumber = i + 1;
                    break;
                }
            }
#if DEBUG
            if (DgtlFieldNumber > 0)
                iWlEntry.SetFieldN((short)DgtlFieldNumber, "FT8");
#endif
        }

        public delegate void ForceLeftRight(short lr);
        public bool SetupTxAndRxDeviceIndicies(ref bool SetupMaySelectDevices, ref uint RxInDevice, ref uint TxOutDevice, ForceLeftRight flr)
        {
            var lr = iWlEntry.GetLeftRight();
            if ((lr != 0) && (lr != 1))
            {
                // instance #1 is allowed to select neither L nor R
                if ((instanceNumber == 1) && (lr == (short)-1))
                    lr = 0; // put on left
                else if (lr > 1)
                { // Radio #3 or #4
                    return false; // use setup form
                }
                else
                {
                    MessageBox.Show("DigiRite requires the Entry Window to be set to either L or R");
                    return false;
                }
            }
            SetupMaySelectDevices = false;
            var RxInDevices = XD.WaveDeviceEnumerator.waveInDevices();
            var TxOutDevices = XD.WaveDeviceEnumerator.waveOutDevices();
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                "Software\\W5XD\\writelog.ini\\OneDevicePerRadio");
            if ((rk != null) && (rk.ValueCount > 1))
            {
                // WL Sound Mixer is set up one-device-per-radio
                object v = null;
                RxInDevice = UInt32.MaxValue;
                TxOutDevice = UInt32.MaxValue;
                if (rk != null)
                {
                    // RX side
                    v = rk.GetValue(lr == 0 ? "LeftReceiverAudioDeviceId" : "RightReceiverAudioDeviceId");
                    if (v != null)
                    {
                        string id = v.ToString().ToUpper();
                        for (int i = 0; i < RxInDevices.Count; i++)
                        {
                            if (String.Equals(XD.WaveDeviceEnumerator.waveInInstanceId(i).ToUpper(), id))
                            {
#if DEBUG
                                    // have a look at the user-friendly name of the device
                                    var waveIns = XD.WaveDeviceEnumerator.waveInDevices();
                                    if (i < waveIns.Count)
                                    {
                                        string name = waveIns[i];
                                    }
#endif
                                RxInDevice = (uint)i;
                                break;
                            }
                        }
                        if (RxInDevice < 0)
                            MessageBox.Show("Use WriteLog Sound Mixer to set the " + (lr == 0 ? "Left" : "Right") + " Rx audio in");
                    }
                    else
                        MessageBox.Show("WriteLog Sound Mixer control is not set up for " + (lr == 0 ? "Left" : "Right") + " RX audio in");
                    // TX side
                    v = rk.GetValue(lr == 0 ? "LeftTransmitterAudioDeviceId" : "RightTransmitterAudioDeviceId");
                    if (v != null)
                    {
                        string id = v.ToString().ToUpper();
                        for (int i = TxOutDevices.Count - 1; i >= 0; i -= 1)
                        {
                            if (XD.WaveDeviceEnumerator.waveOutInstanceId(i).ToUpper() == id)
                            {
#if DEBUG
                                // have a look at the user-friendly name of the device
                                var waveOuts = XD.WaveDeviceEnumerator.waveOutDevices();
                                if (i < waveOuts.Count)
                                {
                                    string name = waveOuts[i];
                                }
#endif
                                TxOutDevice = (uint)i;
                                break;
                            }
                        }
                        if (TxOutDevice < 0)
                            MessageBox.Show("Use WriteLog Sound Mixer to set the " + (lr == 0 ? "Left" : "Right") + " TX audio out");
                    }
                    else
                        MessageBox.Show("WriteLog Sound Mixer control is not set up for " + (lr == 0 ? "Left" : "Right") + " RX audio in");
                }
            }
            else
            {
                // WL Sound Mixer is set up for separate device per radio
                object v = null;
                rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\W5XD\\writelog.ini\\WlSound");
                if (rk != null)
                    v = rk.GetValue("RxInDevice");
                if (v != null)
                {
                    string RxInDeviceName = v.ToString().ToUpper();
                    for (int i = RxInDevices.Count - 1; i >= 0; i -= 1)
                    {
                        if (RxInDevices[i].ToUpper().Contains(RxInDeviceName))
                        {
                            RxInDevice = (uint)i;
                            break;
                        }
                    }
                }
                else
                    MessageBox.Show("WriteLog Sound Mixer control is not set up for RX audio in");
                v = rk.GetValue("TxOutDevice");
                if (v != null)
                {
                    string TxOutDeviceName = v.ToString().ToUpper();
                    for (int i = 0; i < TxOutDevices.Count; i++)
                    {
                        if (TxOutDevices[i].ToUpper().Contains(TxOutDeviceName))
                        {
                            TxOutDevice = (uint)i;
                            break;
                        }
                    }
                }
                else
                    MessageBox.Show("WriteLog Sound Mixer control is not set up for TX audio out");
                flr(lr);
            }
            return true;
        }

        public void CheckDupeAndMult(string fromCall, string digitalMode, XDpack77.Pack77Message.Message rm, out bool dupeout, out short mult)
        {
            int dupe = 0;
            mult = 0;
            iWlDupingEntry.ClearEntry();
            if (DgtlFieldNumber > 0)
                iWlDupingEntry.SetFieldN((short)DgtlFieldNumber, digitalMode);
            if (ReceivedRstFieldNumber > 0) // ClearEntry in WL defaults the RST. clear it out
                iWlDupingEntry.SetFieldN((short)ReceivedRstFieldNumber, "");
            if (GridSquareReceivedFieldNumber > 0)
            {
                XDpack77.Pack77Message.Exchange hisGrid = rm as XDpack77.Pack77Message.Exchange;
                if (hisGrid != null && !String.IsNullOrEmpty(hisGrid.GridSquare))
                    iWlDupingEntry.SetFieldN((short)GridSquareReceivedFieldNumber, hisGrid.GridSquare);
            }
            iWlDupingEntry.Callsign = fromCall;
            dupe = iWlDupingEntry.Dupe();
            if (dupe == 0)
            {
                mult = iWlDupingEntry.IsNewMultiplier((short)-1);
                if (mult <= 0 && GridSquareReceivedFieldNumber > 0)
                    mult = iWlDupingEntry.IsNewMultiplier((short)GridSquareReceivedFieldNumber);
            }
            dupeout = dupe != 0;
        }

        public void SetTransmitFocus()
        {
            iWlEntry.SetTransmitFocus();
        }

        private short wlmode;

        public void GetRigFrequency(out double rxKHz, out double txKHz, out bool split)
        {
            short wlsplit = 0; double tx = 0; double rx = 0;  // where is the rig now?
            iWlEntry.GetLogFrequency(ref wlmode, ref rx, ref tx, ref wlsplit);
            txKHz = tx;
            rxKHz = rx;
            split = wlsplit != 0;
        }

        public short GetCurrentBand()
        {
            short mode = 0; double tx = 0; double rx = 0; short split = 0;
            iWlEntry.GetLogFrequency(ref mode, ref rx, ref tx, ref split);
            mode = 6;
            iWlDupingEntry.SetLogFrequencyEx(mode, rx, tx, split);
            return iWlEntry.GetBand();
        }

        public void SetRigFrequency( double rxKHz, double txKHz, bool split)
        {
            iWlEntry.SetLogFrequencyEx(wlmode, rxKHz, txKHz, (short)(split ? 1 : 0));
        }

        public void SetPtt(bool ptt)
        {
            iWlEntry.SetXmitPtt((short)(ptt ? 1 : 0));
            if (null != pttPort)
                pttPort.RtsEnable = ptt;
        }

        public uint GetSendSerialNumber(string call)
        {
            iWlDupingEntry.Callsign = call;
            iWlDupingEntry.SerialNumber = 0; // get a fresh one
            var ret = iWlDupingEntry.SerialNumber;
            iWlDupingEntry.Callsign = "";
            return ret;
        }

        public string GridSquareSendingOverride()
        {
            if (GridSquareSentFieldNumber > 0)
            {
                string sentgrid = iWlEntry.GetFieldN((short)GridSquareSentFieldNumber).ToUpper();
                if (sentgrid.Length >= 4)
                    return sentgrid;
            }
            return "";
        }

        public void SetQsoItemsToLog(string HisCall, uint SentSerialNumber, string AdifDate, string AdifTime, string SentGrid, string digitalMode)
        {
            iWlDupingEntry.ClearEntry();
            if (ReceivedRstFieldNumber > 0) // ClearEntry in WL defaults the RST. clear it out
                iWlDupingEntry.SetFieldN((short)ReceivedRstFieldNumber, "");
            {
                short mode = 0; double tx = 0; double rx = 0; short split = 0;
                iWlEntry.GetLogFrequency(ref mode, ref rx, ref tx, ref split);
                mode = 6;
                iWlDupingEntry.SetLogFrequencyEx(mode, rx, tx, split);
            }
            // call, serial number,  RST and DGTL we set up front
            iWlDupingEntry.Callsign = HisCall;
            iWlDupingEntry.SerialNumber = SentSerialNumber;
            iWlDupingEntry.SetDateTimeFromADIF(AdifDate, AdifTime);
            if (null != SentGrid && GridSquareSentFieldNumber > 0)
                iWlDupingEntry.SetFieldN((short)GridSquareSentFieldNumber, SentGrid);
            if (DgtlFieldNumber > 0)
                iWlDupingEntry.SetFieldN((short)DgtlFieldNumber, digitalMode);
        }

        public void LogFieldDayQso(string cat, string sec)
        {
            iWlDupingEntry.SetFieldN(2, cat);
            iWlDupingEntry.SetFieldN(3, sec);
            iWlDupingEntry.EnterQso();
            iWlDupingEntry.ClearEntry();
        }

        public void LogRoundUpQso(string sentRst, string receivedRst, string stateOrSerial)
        {
            if (SentRstFieldNumber > 0)
                iWlDupingEntry.SetFieldN((short)SentRstFieldNumber, sentRst);
            iWlDupingEntry.SetFieldN(3, receivedRst);
            iWlDupingEntry.SetFieldN(4, stateOrSerial);
            iWlDupingEntry.EnterQso();
            iWlDupingEntry.ClearEntry();
        }

        public void LogGridSquareQso(string sentRst, string receivedGrid, string receivedDbReport)
        {
            if (SentRstFieldNumber > 0)
                iWlDupingEntry.SetFieldN((short)SentRstFieldNumber, sentRst);
            if (!String.IsNullOrEmpty(receivedGrid) && (GridSquareReceivedFieldNumber > 0))
                iWlDupingEntry.SetFieldN((short)GridSquareReceivedFieldNumber, receivedGrid);
            if ((ReceivedRstFieldNumber > 0) && !String.IsNullOrEmpty(receivedDbReport))
                try
                {
                    iWlDupingEntry.SetFieldNnoValidate((short)ReceivedRstFieldNumber, receivedDbReport); // novalidate cuz RTTY-enabled modules won't let this in
                }
                catch (System.Exception)
                {
                    iWlDupingEntry.SetFieldN((short)ReceivedRstFieldNumber, receivedDbReport); // old versions of WL don't have SetFieldNnoValidate
                }

            iWlDupingEntry.EnterQso();
            iWlDupingEntry.ClearEntry();
        }

        public void ForceRigToUsb()
        {
            short mode = 0; double tx = 0; double rx = 0; short split = 0;
            iWlEntry.GetLogFrequency(ref mode, ref rx, ref tx, ref split);
            if (mode != 2)
            {
                mode = 2;
                iWlEntry.SetLogFrequencyEx(mode, rx, tx, split);
            }
        }

        public void SetCurrentCallAndGrid(string call, string grid)
        {
            iWlEntry.ClearEntry();
            if (!String.IsNullOrEmpty(call))
                iWlEntry.Callsign = call;
            if (GridSquareReceivedFieldNumber >= 0)
                iWlEntry.SetFieldN((short)GridSquareReceivedFieldNumber, grid);
        }
    }
}
