using System;

namespace DigiRiteComLogger
{
    /* DigiRiteComLogger.Logger
     * This class uses COM IDispatch to forward all (except one) of the IDigiRiteLogger methods
     * and properties over COM. The exception is SetupTxAndRxDeviceIndicies, which is implemented as a no-op
     */
    public class Logger : DigiRiteLogger.IDigiRiteLogger
    {
        public Logger(int instance)
        {        }

        private object remoteAdif;
        private Type remoteAdifType;

        public string SetAutomation(object automate)
        {
            remoteAdif = automate;
            remoteAdifType = automate.GetType();
            return "";
        }

        public string CallUsed { get {
                return (string)remoteAdifType.InvokeMember("CallUsed", System.Reflection.BindingFlags.GetProperty, null, remoteAdif, new object[0]);
            }
            set {
                remoteAdifType.InvokeMember("CallUsed", System.Reflection.BindingFlags.SetProperty, null, remoteAdif, new object[] { value });
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
            var args = new object[] {call, digitalMode, m.AsReceived.Content, dupe, mult, m.i3, m.n3 };
            System.Reflection.ParameterModifier mods = new System.Reflection.ParameterModifier(args.Length);
            mods[3] = true;
            mods[4] = true;
            remoteAdifType.InvokeMember("CheckDupeAndMult", bf, null, remoteAdif, args, new System.Reflection.ParameterModifier[1] { mods }, null, null);
            dupe = (bool)args[3];
            mult = (short)args[4];
        }

        public void ForceRigToUsb()
        { remoteAdifType.InvokeMember("ForceRigToUsb", bf, null, remoteAdif, new object[0]);}

        public short GetCurrentBand()
        {  return (short)remoteAdifType.InvokeMember("GetCurrentBand", bf, null, remoteAdif, new object[0]); }

        public void GetRigFrequency(out double rxKHz, out double txKHz, out bool split)
        {
            var args = new object[] { (double)0, (double)0, (bool)false };
            System.Reflection.ParameterModifier mods = new System.Reflection.ParameterModifier(args.Length);
            mods[0] = true;
            mods[1] = true;
            mods[2] = true;
            remoteAdifType.InvokeMember("GetRigFrequency", bf, null, remoteAdif, args, 
                new System.Reflection.ParameterModifier[1] { mods }, null, null);
            rxKHz = (double)args[0];
            txKHz = (double)args[1];
            split = (bool)args[2];
        }

        public uint GetSendSerialNumber(string call)
        {
            return (uint)remoteAdifType.InvokeMember("GetSendSerialNumber", bf, null, remoteAdif, new object[] { call });
        }

        public string GridSquareSendingOverride()
        {
            return (string)remoteAdifType.InvokeMember("GridSquareSendingOverride", bf, null, remoteAdif, new object[0] );
        }

        public void LogFieldDayQso(string category, string section)
        {
            var args = new object[] { category, section };
            remoteAdifType.InvokeMember("LogFieldDayQso", bf, null, remoteAdif, args);
        }

        public void LogGridSquareQso(string sentRst, string receivedGrid, string receivedDbReport)
        {
            var args = new object[] { sentRst, receivedGrid, receivedDbReport };
            remoteAdifType.InvokeMember("LogGridSquareQso", bf, null, remoteAdif, args);
        }

        public void LogRoundUpQso(string sentRst, string receivedRst, string stateOrSerial)
        {
            var args = new object[] { sentRst, receivedRst, stateOrSerial };
            remoteAdifType.InvokeMember("LogRoundUpQso", bf, null, remoteAdif, args);
        }

        public void SetCurrentCallAndGridAndSerial(string call, string grid, uint serialNumber)
        {  remoteAdifType.InvokeMember("SetCurrentCallAndGridAndSerial", bf, null, remoteAdif, new object[] {call, grid, serialNumber}); }

        public void SetPtt(bool ptt)
        {  remoteAdifType.InvokeMember("SetPtt", bf, null, remoteAdif, new object[] { ptt });}

        public void SetQsoItemsToLog(string call, uint SentSerialNumber, string AdifDate, string AdifTime, string sentGrid, string digitalMode)
        {
            var args = new object[] { call,  SentSerialNumber,  AdifDate,  AdifTime,  sentGrid,  digitalMode };
            remoteAdifType.InvokeMember("SetQsoItemsToLog", bf, null, remoteAdif, args);
        }

        public void SetRigFrequency(double rxKHz, double txKHz, bool split)
        { remoteAdifType.InvokeMember("SetRigFrequency", bf, null, remoteAdif, new object[] { rxKHz, txKHz, split });  }

        public void SetTransmitFocus()
        {  remoteAdifType.InvokeMember("SetTransmitFocus", bf, null, remoteAdif, new object[0] );}

        public bool SetupTxAndRxDeviceIndicies(ref bool SetupMaySelectDevices, ref uint RxInDevice, ref uint TxOutDevice, DigiRiteLogger.ForceLeftRight flr)
        {   // the RxInDevice and TxOutDevice are relevant only in this process. This call cannot be forwarded out of process.
            return false;
        }
    }
}
