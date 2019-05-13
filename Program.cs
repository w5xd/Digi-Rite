using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WriteLogDigiRite
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
            MessageBox.Show("Debug me", "DigiRite.exe");
#endif
            const int UpgradedVersion = 21;  // increment every release
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

            void OnFormClosed(object sender, EventArgs e)
            {   // the restaurant at the end of the universe
                mainForm = null;
                ExitThread();
            }
        }
    }
}
