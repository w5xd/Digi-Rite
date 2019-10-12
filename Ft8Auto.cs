
using System;
using System.Runtime.InteropServices;

namespace DigiRite
{
    [ComVisible(true), 
	/* FIXME
        ** If you want to modify DigiRite and simply replace the executable WriteLog invokes,
        ** you need do NOTHING here.
        **
        ** If you want to add an alternative exe that WriteLog invokes, you must change
        ** the GUID and ProgId below and arrange for your new exe to be COM registered.
        **
        ** If you don't want to integrate with WriteLog at all, you may ignore this
        ** class altogether from this project and figure out how to get DigiRite connected
        ** to your application your own way. Leave this class unchanged (and leave the code that
        ** registers it as a COM server in Program.cs) and your customized DigiRite retains
        ** binary compatibility such that WriteLog will find it.
        ** 
	    ** These are the GUID and ProgId that the DigiRite Installer msi at
        ** at writelog.com place in the Windows registry. If you use these, you
        ** replace the installed one when invoked from WriteLog 
        */
        Guid("4CC973B6-9BFB-3E4B-8157-E55774DD50C0"), 
        ProgId("WriteLog.Ft8Auto")  
        /* For reference: 
        ** WriteLog uses the registry key HKEY_LOCAL_MACHINE\SOFTWARE\W5XD\WriteLog\DigitalProductIds
        ** (on 64b Windows, do the drill with WOW6432Node)
        ** Name/Value pairs under DigitialProductIds are used by WriteLog to populate its
        ** right mouse context in its Entry Window. The value is the COM ProgId above.
        ** the name populates the context menu.
        ** The executable invoked by using the standard COM ProgId activation process
        ** WriteLog expects to implement the inteface of the "ComVisible" methods below. 
        */
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
