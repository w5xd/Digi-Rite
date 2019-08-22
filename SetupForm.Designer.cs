namespace WriteLogDigiRite
{
    partial class SetupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxWaveIn = new System.Windows.Forms.ComboBox();
            this.radioButtonInputRight = new System.Windows.Forms.RadioButton();
            this.radioButtonInputLeft = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxWaveOut = new System.Windows.Forms.ComboBox();
            this.radioButtonOutputMono = new System.Windows.Forms.RadioButton();
            this.radioButtonOutputRight = new System.Windows.Forms.RadioButton();
            this.radioButtonOutputLeft = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelWriteLogLR = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCallUsed = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMyGrid = new System.Windows.Forms.TextBox();
            this.comboBoxContest = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxAckMsg = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonShiftTX = new System.Windows.Forms.RadioButton();
            this.radioButtonNoVfo = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownTxMaxHz = new System.Windows.Forms.NumericUpDown();
            this.radioButtonSplitTX = new System.Windows.Forms.RadioButton();
            this.checkBoxUSB = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonFt4 = new System.Windows.Forms.RadioButton();
            this.radioButtonFt8 = new System.Windows.Forms.RadioButton();
            this.numericUpDownPttDelay = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownVfoToPtt = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTxMaxHz)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPttDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVfoToPtt)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxWaveIn);
            this.groupBox1.Controls.Add(this.radioButtonInputRight);
            this.groupBox1.Controls.Add(this.radioButtonInputLeft);
            this.groupBox1.Location = new System.Drawing.Point(12, 169);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 71);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&RX audio";
            // 
            // comboBoxWaveIn
            // 
            this.comboBoxWaveIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWaveIn.FormattingEnabled = true;
            this.comboBoxWaveIn.Location = new System.Drawing.Point(77, 30);
            this.comboBoxWaveIn.Name = "comboBoxWaveIn";
            this.comboBoxWaveIn.Size = new System.Drawing.Size(194, 21);
            this.comboBoxWaveIn.TabIndex = 2;
            // 
            // radioButtonInputRight
            // 
            this.radioButtonInputRight.AutoSize = true;
            this.radioButtonInputRight.Location = new System.Drawing.Point(15, 43);
            this.radioButtonInputRight.Name = "radioButtonInputRight";
            this.radioButtonInputRight.Size = new System.Drawing.Size(50, 17);
            this.radioButtonInputRight.TabIndex = 1;
            this.radioButtonInputRight.Text = "Right";
            this.radioButtonInputRight.UseVisualStyleBackColor = true;
            this.radioButtonInputRight.CheckedChanged += new System.EventHandler(this.radioButtonInput_CheckedChanged);
            // 
            // radioButtonInputLeft
            // 
            this.radioButtonInputLeft.AutoSize = true;
            this.radioButtonInputLeft.Checked = true;
            this.radioButtonInputLeft.Location = new System.Drawing.Point(15, 19);
            this.radioButtonInputLeft.Name = "radioButtonInputLeft";
            this.radioButtonInputLeft.Size = new System.Drawing.Size(43, 17);
            this.radioButtonInputLeft.TabIndex = 0;
            this.radioButtonInputLeft.TabStop = true;
            this.radioButtonInputLeft.Text = "Left";
            this.radioButtonInputLeft.UseVisualStyleBackColor = true;
            this.radioButtonInputLeft.CheckedChanged += new System.EventHandler(this.radioButtonInput_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxWaveOut);
            this.groupBox2.Controls.Add(this.radioButtonOutputMono);
            this.groupBox2.Controls.Add(this.radioButtonOutputRight);
            this.groupBox2.Controls.Add(this.radioButtonOutputLeft);
            this.groupBox2.Location = new System.Drawing.Point(12, 256);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(276, 107);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "&TX Audio";
            // 
            // comboBoxWaveOut
            // 
            this.comboBoxWaveOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWaveOut.FormattingEnabled = true;
            this.comboBoxWaveOut.Location = new System.Drawing.Point(77, 32);
            this.comboBoxWaveOut.Name = "comboBoxWaveOut";
            this.comboBoxWaveOut.Size = new System.Drawing.Size(194, 21);
            this.comboBoxWaveOut.TabIndex = 3;
            // 
            // radioButtonOutputMono
            // 
            this.radioButtonOutputMono.AutoSize = true;
            this.radioButtonOutputMono.Location = new System.Drawing.Point(21, 78);
            this.radioButtonOutputMono.Name = "radioButtonOutputMono";
            this.radioButtonOutputMono.Size = new System.Drawing.Size(52, 17);
            this.radioButtonOutputMono.TabIndex = 2;
            this.radioButtonOutputMono.Text = "Mono";
            this.radioButtonOutputMono.UseVisualStyleBackColor = true;
            this.radioButtonOutputMono.CheckedChanged += new System.EventHandler(this.radioButtonOutput_CheckedChanged);
            // 
            // radioButtonOutputRight
            // 
            this.radioButtonOutputRight.AutoSize = true;
            this.radioButtonOutputRight.Location = new System.Drawing.Point(21, 55);
            this.radioButtonOutputRight.Name = "radioButtonOutputRight";
            this.radioButtonOutputRight.Size = new System.Drawing.Size(50, 17);
            this.radioButtonOutputRight.TabIndex = 1;
            this.radioButtonOutputRight.Text = "Right";
            this.radioButtonOutputRight.UseVisualStyleBackColor = true;
            this.radioButtonOutputRight.CheckedChanged += new System.EventHandler(this.radioButtonOutput_CheckedChanged);
            // 
            // radioButtonOutputLeft
            // 
            this.radioButtonOutputLeft.AutoSize = true;
            this.radioButtonOutputLeft.Checked = true;
            this.radioButtonOutputLeft.Location = new System.Drawing.Point(21, 32);
            this.radioButtonOutputLeft.Name = "radioButtonOutputLeft";
            this.radioButtonOutputLeft.Size = new System.Drawing.Size(43, 17);
            this.radioButtonOutputLeft.TabIndex = 0;
            this.radioButtonOutputLeft.TabStop = true;
            this.radioButtonOutputLeft.Text = "Left";
            this.radioButtonOutputLeft.UseVisualStyleBackColor = true;
            this.radioButtonOutputLeft.CheckedChanged += new System.EventHandler(this.radioButtonOutput_CheckedChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(312, 414);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 18;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(409, 414);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 19;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelWriteLogLR
            // 
            this.labelWriteLogLR.Location = new System.Drawing.Point(40, 131);
            this.labelWriteLogLR.Name = "labelWriteLogLR";
            this.labelWriteLogLR.Size = new System.Drawing.Size(214, 35);
            this.labelWriteLogLR.TabIndex = 9;
            this.labelWriteLogLR.Text = "Device select disabled for WriteLog L&&R. Use Soundboard Mixer";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(171, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Call &used:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxCallUsed
            // 
            this.textBoxCallUsed.Enabled = false;
            this.textBoxCallUsed.Location = new System.Drawing.Point(228, 12);
            this.textBoxCallUsed.Name = "textBoxCallUsed";
            this.textBoxCallUsed.Size = new System.Drawing.Size(100, 20);
            this.textBoxCallUsed.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(360, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "My grid:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxMyGrid
            // 
            this.textBoxMyGrid.Location = new System.Drawing.Point(410, 12);
            this.textBoxMyGrid.Name = "textBoxMyGrid";
            this.textBoxMyGrid.Size = new System.Drawing.Size(61, 20);
            this.textBoxMyGrid.TabIndex = 4;
            // 
            // comboBoxContest
            // 
            this.comboBoxContest.AllowDrop = true;
            this.comboBoxContest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxContest.FormattingEnabled = true;
            this.comboBoxContest.Items.AddRange(new object[] {
            "Grid Square",
            "dB Report",
            "ARRL Field Day",
            "ARRL Rtty RoundUp",
            "Grid Square & dB Report"});
            this.comboBoxContest.Location = new System.Drawing.Point(228, 90);
            this.comboBoxContest.Name = "comboBoxContest";
            this.comboBoxContest.Size = new System.Drawing.Size(159, 21);
            this.comboBoxContest.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(166, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Exchange:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(104, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Acknowledge message:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxAckMsg
            // 
            this.comboBoxAckMsg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAckMsg.FormattingEnabled = true;
            this.comboBoxAckMsg.Location = new System.Drawing.Point(228, 51);
            this.comboBoxAckMsg.Name = "comboBoxAckMsg";
            this.comboBoxAckMsg.Size = new System.Drawing.Size(121, 21);
            this.comboBoxAckMsg.TabIndex = 6;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonShiftTX);
            this.groupBox3.Controls.Add(this.radioButtonNoVfo);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.numericUpDownTxMaxHz);
            this.groupBox3.Controls.Add(this.radioButtonSplitTX);
            this.groupBox3.Location = new System.Drawing.Point(325, 273);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(167, 135);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "TX frequency limits";
            // 
            // radioButtonShiftTX
            // 
            this.radioButtonShiftTX.AutoSize = true;
            this.radioButtonShiftTX.Location = new System.Drawing.Point(7, 63);
            this.radioButtonShiftTX.Name = "radioButtonShiftTX";
            this.radioButtonShiftTX.Size = new System.Drawing.Size(119, 17);
            this.radioButtonShiftTX.TabIndex = 2;
            this.radioButtonShiftTX.TabStop = true;
            this.radioButtonShiftTX.Text = "Shift VFO during TX";
            this.radioButtonShiftTX.UseVisualStyleBackColor = true;
            this.radioButtonShiftTX.CheckedChanged += new System.EventHandler(this.checkBoxSplit_CheckedChanged);
            // 
            // radioButtonNoVfo
            // 
            this.radioButtonNoVfo.AutoSize = true;
            this.radioButtonNoVfo.Location = new System.Drawing.Point(7, 19);
            this.radioButtonNoVfo.Name = "radioButtonNoVfo";
            this.radioButtonNoVfo.Size = new System.Drawing.Size(96, 17);
            this.radioButtonNoVfo.TabIndex = 0;
            this.radioButtonNoVfo.TabStop = true;
            this.radioButtonNoVfo.Text = "no VFO control";
            this.radioButtonNoVfo.UseVisualStyleBackColor = true;
            this.radioButtonNoVfo.CheckedChanged += new System.EventHandler(this.checkBoxSplit_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Maximum audio freq:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(106, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Hz";
            // 
            // numericUpDownTxMaxHz
            // 
            this.numericUpDownTxMaxHz.Enabled = false;
            this.numericUpDownTxMaxHz.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownTxMaxHz.Location = new System.Drawing.Point(51, 101);
            this.numericUpDownTxMaxHz.Maximum = new decimal(new int[] {
            3500,
            0,
            0,
            0});
            this.numericUpDownTxMaxHz.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTxMaxHz.Name = "numericUpDownTxMaxHz";
            this.numericUpDownTxMaxHz.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownTxMaxHz.TabIndex = 4;
            this.numericUpDownTxMaxHz.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // radioButtonSplitTX
            // 
            this.radioButtonSplitTX.AutoSize = true;
            this.radioButtonSplitTX.Location = new System.Drawing.Point(7, 41);
            this.radioButtonSplitTX.Name = "radioButtonSplitTX";
            this.radioButtonSplitTX.Size = new System.Drawing.Size(123, 17);
            this.radioButtonSplitTX.TabIndex = 1;
            this.radioButtonSplitTX.Text = "Split VFOs during TX";
            this.radioButtonSplitTX.UseVisualStyleBackColor = true;
            this.radioButtonSplitTX.CheckedChanged += new System.EventHandler(this.checkBoxSplit_CheckedChanged);
            // 
            // checkBoxUSB
            // 
            this.checkBoxUSB.Location = new System.Drawing.Point(330, 228);
            this.checkBoxUSB.Name = "checkBoxUSB";
            this.checkBoxUSB.Size = new System.Drawing.Size(117, 39);
            this.checkBoxUSB.TabIndex = 16;
            this.checkBoxUSB.Text = "Always force rig to &USB mode";
            this.checkBoxUSB.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonFt4);
            this.groupBox4.Controls.Add(this.radioButtonFt8);
            this.groupBox4.Location = new System.Drawing.Point(15, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(62, 79);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Mode";
            // 
            // radioButtonFt4
            // 
            this.radioButtonFt4.AutoSize = true;
            this.radioButtonFt4.Location = new System.Drawing.Point(7, 47);
            this.radioButtonFt4.Name = "radioButtonFt4";
            this.radioButtonFt4.Size = new System.Drawing.Size(44, 17);
            this.radioButtonFt4.TabIndex = 1;
            this.radioButtonFt4.TabStop = true;
            this.radioButtonFt4.Text = "FT&4";
            this.radioButtonFt4.UseVisualStyleBackColor = true;
            // 
            // radioButtonFt8
            // 
            this.radioButtonFt8.AutoSize = true;
            this.radioButtonFt8.Location = new System.Drawing.Point(7, 20);
            this.radioButtonFt8.Name = "radioButtonFt8";
            this.radioButtonFt8.Size = new System.Drawing.Size(44, 17);
            this.radioButtonFt8.TabIndex = 0;
            this.radioButtonFt8.TabStop = true;
            this.radioButtonFt8.Text = "FT&8";
            this.radioButtonFt8.UseVisualStyleBackColor = true;
            // 
            // numericUpDownPttDelay
            // 
            this.numericUpDownPttDelay.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPttDelay.Location = new System.Drawing.Point(454, 193);
            this.numericUpDownPttDelay.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPttDelay.Name = "numericUpDownPttDelay";
            this.numericUpDownPttDelay.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownPttDelay.TabIndex = 15;
            this.numericUpDownPttDelay.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // numericUpDownVfoToPtt
            // 
            this.numericUpDownVfoToPtt.Increment = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numericUpDownVfoToPtt.Location = new System.Drawing.Point(454, 159);
            this.numericUpDownVfoToPtt.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numericUpDownVfoToPtt.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownVfoToPtt.Name = "numericUpDownVfoToPtt";
            this.numericUpDownVfoToPtt.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownVfoToPtt.TabIndex = 13;
            this.numericUpDownVfoToPtt.Value = new decimal(new int[] {
            550,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(331, 195);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "&PTT to sound msec:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(331, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(116, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "&VFO split to PTT msec:";
            // 
            // SetupForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(519, 449);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericUpDownVfoToPtt);
            this.Controls.Add(this.numericUpDownPttDelay);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBoxUSB);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.comboBoxAckMsg);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxContest);
            this.Controls.Add(this.textBoxMyGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxCallUsed);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelWriteLogLR);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupForm";
            this.Text = "DigiRite Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.Load += new System.EventHandler(this.SetupForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTxMaxHz)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPttDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVfoToPtt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonInputRight;
        private System.Windows.Forms.RadioButton radioButtonInputLeft;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonOutputMono;
        private System.Windows.Forms.RadioButton radioButtonOutputRight;
        private System.Windows.Forms.RadioButton radioButtonOutputLeft;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxWaveIn;
        private System.Windows.Forms.ComboBox comboBoxWaveOut;
        private System.Windows.Forms.Label labelWriteLogLR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCallUsed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMyGrid;
        private System.Windows.Forms.ComboBox comboBoxContest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxAckMsg;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownTxMaxHz;
        private System.Windows.Forms.RadioButton radioButtonSplitTX;
        private System.Windows.Forms.CheckBox checkBoxUSB;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButtonFt4;
        private System.Windows.Forms.RadioButton radioButtonFt8;
        private System.Windows.Forms.NumericUpDown numericUpDownPttDelay;
        private System.Windows.Forms.NumericUpDown numericUpDownVfoToPtt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton radioButtonNoVfo;
        private System.Windows.Forms.RadioButton radioButtonShiftTX;
    }
}