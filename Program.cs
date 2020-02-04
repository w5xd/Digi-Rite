using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DigiRite
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            const int UpgradedVersion = 62;  // increment every release
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
            MessageBox.Show("Debug me", "DigiRite.exe");
#endif
            int settingsVersion = Properties.Settings.Default.SavedVersion;
            if (settingsVersion < UpgradedVersion)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.SavedVersion = UpgradedVersion;
            }
            CustomColors.CommonBackgroundColor = Properties.Settings.Default.Background;
            CustomColors.TxBackgroundColor = Properties.Settings.Default.TxBackground;
            if ((args.Length >= 1) && args[0].ToUpper() == "-EMBEDDING")
            {
                var regServices = new RegistrationServices();
                int cookie = regServices.RegisterTypeForComClients(
                    typeof(Ft8Auto),
                    RegistrationClassContext.LocalServer ,
                    RegistrationConnectionType.SingleUse);
                applicationContext = new NoShowFormAppContext();
                Application.Run(applicationContext);
                regServices.UnregisterTypeForComClients(cookie);
            }
            else
            {   Application.Run(new MainForm(1));  }
        }

        public static NoShowFormAppContext applicationContext;

        public class NoShowFormAppContext : ApplicationContext
        {
            // as an automation server, we must immediately
            // have an HWND so the automation class can
            // get methods back on the Program's STA
            class NoShowControl : Control
            {
                public void Create()
                {   CreateHandle();     }
            }
            private NoShowControl toDispatch = new NoShowControl();

            private MainForm mainForm = null;
            private bool haveShownMain = false;

            public NoShowFormAppContext()
            {   // arrange for dispatch in ctor
                toDispatch.Visible = false;
                toDispatch.Create();
            }

            public void CreateMainForm()
            {
                toDispatch.BeginInvoke(new Action(() =>
                {
                    if (!haveShownMain)
                    {
                        haveShownMain = true;
                        mainForm.FormClosed += OnFormClosed;
                        mainForm.Show();
                    }
                }));
            }

            public void CloseMainWindow()
            {
                if (null != mainForm)
                    toDispatch.BeginInvoke(new Action(() => {
                        mainForm.Close();
                    }));
            }

            public void AbortMessage()
            {
                if (null != mainForm)
                    toDispatch.BeginInvoke(new Action(() => {
                        mainForm.AbortMessage();
                    }));
            }

            public void SetWlEntry(object wl, int instanceNumber)
            {
                // only do this once
                if (null == mainForm)
                {
                    toDispatch.BeginInvoke(new Action<object>((object w) =>
                    {
                        mainForm = new MainForm(instanceNumber);
                        mainForm.SetWlEntry(w);
                    }), wl);
                }
            }

            public void SendRttyMessage(String toSend)
            {
                if (null != mainForm)
                    toDispatch.BeginInvoke(new Action(() => {
                        mainForm.SendRttyMessage(toSend);
                    }));
            }

            public string GetCurrentMode()
            {
                /* THREAD SAFETY AND SYNCHRONIZATION ISSUE HERE.
                ** A BeginInvoke followed by AsyncWaitHandle.WaitOne
                ** WOULD BE a way to synchronize with the main thread.
                ** But it deadlocks with WriteLog. How?
                ** Our main thread calls into WL to do something (anything)
                ** to the rig and WL calls back here to check whether the
                ** mode should be overridden. 
                ** but WaitOne() blocks forever because the .NET
                ** dispatcher for thread Main doesn't dispatch
                ** our action cause its in an outbound COM call.
                **
                ** .NET probably has some mechanism for this code here
                ** to somehow inform the STA dispatcher for the main
                ** thread that we really are on the same COM causality
                ** as the outgoing COM call. But I couldn't find any
                ** .NET+COM magic to do that. 
                */
#if false
                // Thread safe, but deadlocks
                string ret = "";
                if (null != mainForm)
                {
                    var action = new Action(() =>
                    {
                        ret = mainForm.CurrentMode.ToString();
                    });
                    var result = toDispatch.BeginInvoke(action);
                    result.AsyncWaitHandle.WaitOne();
                    result.AsyncWaitHandle.Close();
                }
                return ret;
#else
                // Beware thread safety...but doesn't deadlock
                // and the enum itself can safely be accessed from outside
                // its own thread.
                var form = mainForm;
                if (null != form)
                    return form.CurrentMode.ToString();
                else
                    return "";
#endif
            }

            void OnFormClosed(object sender, EventArgs e)
            {   // the restaurant at the end of the universe
                mainForm = null;
                ExitThread();
            }
        }
    }
}
