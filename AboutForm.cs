using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiRite
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion; 
            textBoxIntro.Text = "DigiRite" + version  +
                " Copyright (c) 2020 WriteLog Contesting Software, LLC\r\n\r\n" +
                "This program is based on wsjtx-2.1.0 which is licensed software.\r\n" +
                "See the file COPYING.\r\n\r\n" +

"The algorithms, source code, look-and-feel of WSJT-X and related programs, and" +
" protocol specifications for the modes FSK441, FT8, JT4, JT6M JT9, JT65, JTMS, QRA64," +
" ISCAT, MSK144 are Copyright (C) 2001-2018 by one or more of the following authors:" +
" Joseph Taylor, K1JT; Bill Somerville, G4WJS; Steven Franke, K9AN; Nico Palermo," +
" IV3NWV; Grea Bream, KI7MT; Michael Black, W9MDB; Edson Pereira, PY2SDR; Philip Karn," +
" KA9Q; and other members of the WSJT Development Group.\r\n\r\n" ;

            BackColor = CustomColors.CommonBackgroundColor;
            textBoxIntro.BackColor = CustomColors.TxBackgroundColor;

        }
    }
}
