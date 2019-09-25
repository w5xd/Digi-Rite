
using System;
using System.Runtime.InteropServices;

namespace WriteLogDigiRite
{
    [ComVisible(true), 
	    // FIXME
	    // These are the GUID and ProgId that the DigiRite Installer msi at
        // at writelog.com place in the Windows registry. If you use these, you
        // replace the installed one when invoked from WriteLog
        Guid("4CC973B6-9BFB-3E4B-8157-E55774DD50C0"), 
        ProgId("WriteLog.Ft8Auto")  
        ] 
    public class Ft8Auto
    {
        [ComVisible(true)]
        public void Show()
        {
            Program.applicationContext.CreateMainForm(); 
        }

        [ComVisible(true)]
        public void CloseWindow()
        {
            Program.applicationContext.CloseMainWindow();
        }

        [ComVisible(true)]
        public void AbortMessage()
        {
            Program.applicationContext.AbortMessage();
        }

        [ComVisible(true)]
        public int SendMessage(String s)
        {
            Program.applicationContext.SendRttyMessage(s);
            return 0;
        }

        [ComVisible(true)]
        public void SetWlEntry(object wl, int idx)
        {
            Program.applicationContext.SetWlEntry(wl, idx);
        }
    }
}
