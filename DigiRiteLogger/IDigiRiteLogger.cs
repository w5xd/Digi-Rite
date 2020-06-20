using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiRiteLogger
{
    public interface IDigiRiteLogger
    {
        string CallUsed { get; } // call we are transmitting
        // For a given call and message, return whether it is a dupe and/or a multiplier. mult < 0 means "unknown", ==0 means no, >0 means yes.
        void CheckDupeAndMult(string call, string digitalMode, XDpack77.Pack77Message.Message m, out bool dupe, out short mult);
        // we're about to transmit on our radio.
        void SetTransmitFocus();
        // get the radio's current frequency 
        void GetRigFrequency( out double rxKHz, out double txKHz, out bool split);
        // The absolute value of GetCurrentBand is not important. It just needs to be a different value for each band duped separately.  
        short GetCurrentBand();
        // When we're about to transmit, and our setup says the rig must be in USB mode
        void ForceRigToUsb();
        // Set the rig to a given frequency and split
        void SetRigFrequency(double rxKHz, double txKHz, bool split);
        // ptt is called at the beginning and end of each transmission
        void SetPtt(bool ptt);
        // when we're about to transmit an exchange, assign a serial number
        uint GetSendSerialNumber(string call);
        // If the logger is for a rover that sends a different grid 
        string GridSquareSendingOverride();
        // Logging a QSO is two steps. First SetQsoItemsToLog sets all the common values.
        void SetQsoItemsToLog(string call, uint SentSerialNumber, string AdifDate, string AdifTime, string sentGrid, string digitalMode);
        // ..then one of these three actually logs the QSO:
        void LogFieldDayQso(string category, string section);
        void LogRoundUpQso(string sentRst, string receivedRst, string stateOrSerial);
        void LogGridSquareQso(string sentRst, string receivedGrid, string receivedDbReport);
        // DigiRite tells the logger what call its trying to work now. and its grid, if known. and serial number, if assigned
        void SetCurrentCallAndGridAndSerial(string call, string grid, uint serialNumber);
    }
}
