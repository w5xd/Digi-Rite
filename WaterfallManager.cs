using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DigiRite
{
    /* Without this class, DigiRite has only a spectrum line display. 
     * This class, in its OnLoad() method, detects if WriteLog's waterfall component is installed. 
     * If its found, it substitutes that waterfall for the spectrum chart control.
     */
    class WaterfallManager
    {
        public WriteLog.IWaterfallV3 iwaterfall { get; private set; } = null;
        public WriteLog.IAnnotations annotations { get; private set; } = null;
        public WriteLog.IPeakRx peak { get; private set; } = null;

        public bool OnLoad(LogFile logfile, Control chartSpectrum, Label labelWaterfall, Control.ControlCollection collection, MainForm mainForm)
        {
            Control waterfall = null;
            // registry goop to look unconditionally in 32bit registry for WriteLog, whether we are x86 or x64
            Microsoft.Win32.RegistryKey rk32 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            {
                if (null != rk32)
                {
                    Microsoft.Win32.RegistryKey wlrk32 = rk32.OpenSubKey(@"Software\\W5XD\\WriteLog\\Install");
                    if (null != wlrk32)
                    {
                        object wlDir = wlrk32.GetValue("Directory");
                        /* SetCurrentDirectory
                         * Why?
                         * The WriteLog waterfall depends on the d3dcsx.dll (Microsoft's DirectX2D). That dll must be installed
                         * on a per-application basis. Win32's dll lookup for the case of loading for a .net assembly in the GAC
                         * needs to find d3dcxs. WL installs a copy, so we "cd" this exe there so it will be found.
                         */
                        if (null != wlDir)  
                        {
                            try
                            {
                                System.IO.Directory.SetCurrentDirectory(wlDir.ToString() +
#if BUILD_X86
                    "Programs"
#elif BUILD_X64
                    "DigiRite X64 Waterfall"
#endif
                                );
                            }
                            catch (System.Exception) { } // ignore failure
                        }
                        wlrk32.Close();
                    }
                    rk32.Close();
                }
            }
            try
            {
                // if WriteLog's DigiRiteWaterfall assembly loads, use it.
                System.Reflection.Assembly waterfallAssembly = System.Reflection.Assembly.Load(
                    "WriteLogWaterfallDigiRite, Version=12.0.52.5, Culture=neutral, PublicKeyToken=e34bde9f0678e8b6");
                System.Type t = waterfallAssembly.GetType("WriteLog.WaterfallFactory");
                if (t == null)
                    throw new System.Exception("WaterfallFactory type not found");

                wfFactory = (WriteLog.IWaterfallFactory)System.Activator.CreateInstance(t);
                waterfall = (Control)wfFactory.GetWaterfall();
                waterfallEditor = wfFactory.GetEditor() as Form;
                iwaterfall = (WriteLog.IWaterfallV3)waterfall;
            }
            catch (System.Exception ex)
            {   // if WriteLog is not installed, this is "normal successful completion" 
                logfile.SendToLog("Load waterfall exception " + ex.ToString()); // not normal
            }

            if (null != waterfall)
            {   // waterfall loaded. it goes where chartSpectrum was.

                ((System.ComponentModel.ISupportInitialize)waterfall).BeginInit();
                waterfall.Location = chartSpectrum.Location;
                waterfall.Dock = chartSpectrum.Dock;
                waterfall.Size = chartSpectrum.Size;
                waterfall.TabIndex = chartSpectrum.TabIndex;
                waterfall.Name = "waterfall";
                waterfall.Font = labelWaterfall.Font; // labelWaterfall is placeholder for properties
                waterfall.BackColor = labelWaterfall.BackColor;
                waterfall.ForeColor = labelWaterfall.ForeColor;
                waterfall.Visible = true;
                iwaterfall.TxFreqHz = mainForm.TxFrequency;
                iwaterfall.RxFreqHz = mainForm.RxFrequency;
                iwaterfall.onTxFreqMove = new WriteLog.OnFreqMove((int hz) => { mainForm.TxFrequency = hz; });
                iwaterfall.onRxFreqMove = new WriteLog.OnFreqMove((int hz) => { mainForm.RxFrequency = hz; });
                ((System.ComponentModel.ISupportInitialize)waterfall).EndInit();

                collection.Remove(chartSpectrum);
                collection.Add(waterfall);
                collection.SetChildIndex(waterfall, 0);

                peak = iwaterfall as WriteLog.IPeakRx;
                annotations = iwaterfall as WriteLog.IAnnotations;
            }

            return iwaterfall != null;
        }

        public void StoreProperties()
        {
            if (null != wfFactory)
                wfFactory.StoreProperties();
        }

        public void ShowOptionsDialog(System.Windows.Forms.Form par)
        {
            if (waterfallEditor != null)
            {
                if (waterfallEditor.WindowState == FormWindowState.Minimized)
                    waterfallEditor.WindowState = FormWindowState.Normal;
                else if (!waterfallEditor.Visible)
                    waterfallEditor.Show(par);
            }
        }

        private Form waterfallEditor;
        private WriteLog.IWaterfallFactory wfFactory;
    }
}
