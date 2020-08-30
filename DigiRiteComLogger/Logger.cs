using System;

namespace DigiRiteComLogger
{
    /* DigiRiteComLogger.Logger
     * DigiRite looks for this class named "Logger"
     * This class uses COM IDispatch to forward all (except one) of the IDigiRiteLogger methods
     * and properties over COM. 
     * The exception is SetupTxAndRxDeviceIndicies, which is implemented as a no-op because out-of-process 
     * does not have enough context.
     */
    public class Logger : DigiRiteLogger.IDigiRiteLogger
    {
        private int instanceNumber;
        public Logger(int instance) // required constructor
        {
            // The external program that asked for this Logger to be instanced passed the instance
            // ... so it already knows this...
            this.instanceNumber = instance;
        }

        private object remoteLogger;
        private Type remoteLoggerType;

        public string SetAutomation(object automate)
        {
            remoteLogger = automate;
            remoteLoggerType = automate.GetType();
            return "";
        }

        // most of the IDigiRiteLogger calls are forwarded via IDispatch to the remote process. 

        public string CallUsed
        {
            get
            {
                return (string)remoteLoggerType.InvokeMember("CallUsed",
                    System.Reflection.BindingFlags.GetProperty, null, remoteLogger, new object[0]);
            }
            set
            {
                remoteLoggerType.InvokeMember("CallUsed",
                    System.Reflection.BindingFlags.SetProperty, null, remoteLogger, new object[] { value });
            }
        }

        public string GridUsed
        {
            get
            {
                return (string)remoteLoggerType.InvokeMember("GridUsed",
                    System.Reflection.BindingFlags.GetProperty, null, remoteLogger, new object[0]);
            }
            set
            {
                remoteLoggerType.InvokeMember("GridUsed",
                    System.Reflection.BindingFlags.SetProperty, null, remoteLogger, new object[] { value });
            }
        }

        private const System.Reflection.BindingFlags bf = System.Reflection.BindingFlags.InvokeMethod;

        public void CheckDupeAndMult(string call, string digitalMode,
            XDpack77.Pack77Message.Message m, out bool dupe, out short mult)
        {
            // XDpack77.Pack77Message.Message can't be remoted as-is
            // but it can be reproduced on the server side with just its Content property and i3 and n3
            dupe = false;
            mult = 0;
            var args = new object[] { call, digitalMode, m.AsReceived.Content, dupe, mult, m.i3, m.n3 };
            System.Reflection.ParameterModifier mods = new System.Reflection.ParameterModifier(args.Length);
            mods[3] = true;
            mods[4] = true;
            remoteLoggerType.InvokeMember("CheckDupeAndMult", bf, null, remoteLogger, args, new System.Reflection.ParameterModifier[1] { mods }, null, null);
            dupe = (bool)args[3];
            mult = (short)args[4];
        }

        public void ForceRigToUsb()
        { remoteLoggerType.InvokeMember("ForceRigToUsb", bf, null, remoteLogger, new object[0]); }

        public short GetCurrentBand()
        { return (short)remoteLoggerType.InvokeMember("GetCurrentBand", bf, null, remoteLogger, new object[0]); }

        public void GetRigFrequency(out double rxKHz, out double txKHz, out bool split)
        {
            var args = new object[] { (double)0, (double)0, (bool)false };
            System.Reflection.ParameterModifier mods = new System.Reflection.ParameterModifier(args.Length);
            mods[0] = true;
            mods[1] = true;
            mods[2] = true;
            remoteLoggerType.InvokeMember("GetRigFrequency", bf, null, remoteLogger, args,
                new System.Reflection.ParameterModifier[1] { mods }, null, null);
            rxKHz = (double)args[0];
            txKHz = (double)args[1];
            split = (bool)args[2];
        }

        public uint GetSendSerialNumber(string call)
        {
            return (uint)remoteLoggerType.InvokeMember("GetSendSerialNumber", bf, null, remoteLogger, new object[] { call });
        }

        public string GridSquareSendingOverride()
        {
            return (string)remoteLoggerType.InvokeMember("GridSquareSendingOverride", bf, null, remoteLogger, new object[0]);
        }

        public void LogFieldDayQso(string category, string section)
        {
            var args = new object[] { category, section };
            remoteLoggerType.InvokeMember("LogFieldDayQso", bf, null, remoteLogger, args);
        }

        public void LogGridSquareQso(string sentRst, string receivedGrid, string receivedDbReport)
        {
            var args = new object[] { sentRst, receivedGrid, receivedDbReport };
            remoteLoggerType.InvokeMember("LogGridSquareQso", bf, null, remoteLogger, args);
        }

        public void LogRoundUpQso(string sentRst, string receivedRst, string stateOrSerial)
        {
            var args = new object[] { sentRst, receivedRst, stateOrSerial };
            remoteLoggerType.InvokeMember("LogRoundUpQso", bf, null, remoteLogger, args);
        }

        public void SetCurrentCallAndGridAndSerial(string call, string grid, uint serialNumber)
        { remoteLoggerType.InvokeMember("SetCurrentCallAndGridAndSerial", bf, null, remoteLogger, new object[] { call, grid, serialNumber }); }

        public void SetPtt(bool ptt)
        { remoteLoggerType.InvokeMember("SetPtt", bf, null, remoteLogger, new object[] { ptt }); }

        public void SetQsoItemsToLog(string call, uint SentSerialNumber, string AdifDate, string AdifTime, string sentGrid, string digitalMode)
        {
            var args = new object[] { call, SentSerialNumber, AdifDate, AdifTime, sentGrid, digitalMode };
            remoteLoggerType.InvokeMember("SetQsoItemsToLog", bf, null, remoteLogger, args);
        }

        public void SetRigFrequency(double rxKHz, double txKHz, bool split)
        { remoteLoggerType.InvokeMember("SetRigFrequency", bf, null, remoteLogger, new object[] { rxKHz, txKHz, split }); }

        public void SetTransmitFocus()
        { remoteLoggerType.InvokeMember("SetTransmitFocus", bf, null, remoteLogger, new object[0]); }

        public bool SetupTxAndRxDeviceIndicies(ref bool SetupMaySelectDevices, ref uint RxInDevice, ref uint TxOutDevice, DigiRiteLogger.ForceLeftRight flr)
        {   // the RxInDevice and TxOutDevice are relevant only in this process. This call cannot be forwarded out of process.
            return false;
        }
    }
}
