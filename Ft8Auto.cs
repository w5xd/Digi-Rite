
using System;
using System.Runtime.InteropServices;

namespace WriteLogDigiRite
{
    [ComVisible(true), 
        Guid("00000000-0000-0000-0000-000000000000"), // FIXME! 
        ProgId("YourApp.YourAuto")  // FIXME! 
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
