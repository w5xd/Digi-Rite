namespace DigiRite
{
    partial class RcvrForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RcvrForm));
            this.chartSpectrum = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.numericUpDownMaxFreq = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMinFreq = new System.Windows.Forms.NumericUpDown();
            this.checkBoxEnableAP = new System.Windows.Forms.CheckBox();
            this.comboBoxnDepth = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelPower = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelWaterfall = new System.Windows.Forms.Label();
            this.labelClockAnimation = new System.Windows.Forms.Label();
            this.labelClock = new System.Windows.Forms.Label();
            this.listBoxReceived = new System.Windows.Forms.ListBox();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.panelRightGain = new System.Windows.Forms.Panel();
            this.labelVUmeter = new System.Windows.Forms.Label();
            this.trackBarRxGain = new System.Windows.Forms.TrackBar();
            this.eventLog1 = new System.Diagnostics.EventLog();
            this.contextMenuStripOnReceived = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyItemToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerVU = new System.Windows.Forms.Timer(this.components);
            this.checkBoxAnnotate = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chartSpectrum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinFreq)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelRightGain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRxGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            this.contextMenuStripOnReceived.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartSpectrum
            // 
            this.chartSpectrum.BackColor = System.Drawing.Color.MistyRose;
            this.chartSpectrum.BorderlineColor = System.Drawing.Color.Black;
            chartArea1.AxisX.Interval = 1000D;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.Maximum = 3000D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.MinorGrid.Enabled = true;
            chartArea1.AxisY.IsLogarithmic = true;
            chartArea1.AxisY.LabelStyle.Enabled = false;
            chartArea1.AxisY.MajorGrid.Interval = 2D;
            chartArea1.AxisY.Maximum = 100000D;
            chartArea1.AxisY.Minimum = 0.01D;
            chartArea1.Name = "ChartArea1";
            this.chartSpectrum.ChartAreas.Add(chartArea1);
            this.chartSpectrum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartSpectrum.Location = new System.Drawing.Point(0, 0);
            this.chartSpectrum.Name = "chartSpectrum";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.IsVisibleInLegend = false;
            series1.Name = "Amp";
            series1.SmartLabelStyle.Enabled = false;
            this.chartSpectrum.Series.Add(series1);
            this.chartSpectrum.Size = new System.Drawing.Size(335, 340);
            this.chartSpectrum.TabIndex = 0;
            // 
            // numericUpDownMaxFreq
            // 
            this.numericUpDownMaxFreq.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownMaxFreq.Location = new System.Drawing.Point(204, 10);
            this.numericUpDownMaxFreq.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownMaxFreq.Minimum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.numericUpDownMaxFreq.Name = "numericUpDownMaxFreq";
            this.numericUpDownMaxFreq.Size = new System.Drawing.Size(63, 20);
            this.numericUpDownMaxFreq.TabIndex = 3;
            this.numericUpDownMaxFreq.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numericUpDownMaxFreq.ValueChanged += new System.EventHandler(this.DecodeFrequencyRangeChanged);
            // 
            // numericUpDownMinFreq
            // 
            this.numericUpDownMinFreq.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownMinFreq.Location = new System.Drawing.Point(78, 10);
            this.numericUpDownMinFreq.Maximum = new decimal(new int[] {
            4500,
            0,
            0,
            0});
            this.numericUpDownMinFreq.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownMinFreq.Name = "numericUpDownMinFreq";
            this.numericUpDownMinFreq.Size = new System.Drawing.Size(63, 20);
            this.numericUpDownMinFreq.TabIndex = 1;
            this.numericUpDownMinFreq.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownMinFreq.ValueChanged += new System.EventHandler(this.DecodeFrequencyRangeChanged);
            // 
            // checkBoxEnableAP
            // 
            this.checkBoxEnableAP.AutoSize = true;
            this.checkBoxEnableAP.Location = new System.Drawing.Point(401, 12);
            this.checkBoxEnableAP.Name = "checkBoxEnableAP";
            this.checkBoxEnableAP.Size = new System.Drawing.Size(76, 17);
            this.checkBoxEnableAP.TabIndex = 5;
            this.checkBoxEnableAP.Text = "&Enable AP";
            this.checkBoxEnableAP.UseVisualStyleBackColor = true;
            this.checkBoxEnableAP.CheckedChanged += new System.EventHandler(this.checkBoxEnableAP_CheckedChanged);
            // 
            // comboBoxnDepth
            // 
            this.comboBoxnDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxnDepth.FormattingEnabled = true;
            this.comboBoxnDepth.Items.AddRange(new object[] {
            "Fast",
            "Normal",
            "Deep"});
            this.comboBoxnDepth.Location = new System.Drawing.Point(285, 10);
            this.comboBoxnDepth.Name = "comboBoxnDepth";
            this.comboBoxnDepth.Size = new System.Drawing.Size(105, 21);
            this.comboBoxnDepth.TabIndex = 4;
            this.comboBoxnDepth.SelectedIndexChanged += new System.EventHandler(this.comboBoxnDepth_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "M&ax freq:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "M&in freq (Hz)";
            // 
            // labelPower
            // 
            this.labelPower.AutoSize = true;
            this.labelPower.Location = new System.Drawing.Point(580, 14);
            this.labelPower.Name = "labelPower";
            this.labelPower.Size = new System.Drawing.Size(36, 13);
            this.labelPower.TabIndex = 7;
            this.labelPower.Text = "power";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxAnnotate);
            this.panel1.Controls.Add(this.labelWaterfall);
            this.panel1.Controls.Add(this.comboBoxnDepth);
            this.panel1.Controls.Add(this.labelPower);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numericUpDownMaxFreq);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.numericUpDownMinFreq);
            this.panel1.Controls.Add(this.checkBoxEnableAP);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 340);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(729, 37);
            this.panel1.TabIndex = 0;
            // 
            // labelWaterfall
            // 
            this.labelWaterfall.AutoSize = true;
            this.labelWaterfall.BackColor = System.Drawing.Color.Black;
            this.labelWaterfall.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelWaterfall.ForeColor = System.Drawing.Color.White;
            this.labelWaterfall.Location = new System.Drawing.Point(729, 13);
            this.labelWaterfall.Name = "labelWaterfall";
            this.labelWaterfall.Size = new System.Drawing.Size(48, 15);
            this.labelWaterfall.TabIndex = 8;
            this.labelWaterfall.Text = "waterfall";
            this.labelWaterfall.Visible = false;
            // 
            // labelClockAnimation
            // 
            this.labelClockAnimation.Location = new System.Drawing.Point(7, 7);
            this.labelClockAnimation.Name = "labelClockAnimation";
            this.labelClockAnimation.Size = new System.Drawing.Size(24, 24);
            this.labelClockAnimation.TabIndex = 0;
            this.labelClockAnimation.Text = "label3";
            // 
            // labelClock
            // 
            this.labelClock.AutoSize = true;
            this.labelClock.Location = new System.Drawing.Point(10, 51);
            this.labelClock.Name = "labelClock";
            this.labelClock.Size = new System.Drawing.Size(19, 13);
            this.labelClock.TabIndex = 1;
            this.labelClock.Text = "14";
            this.labelClock.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // listBoxReceived
            // 
            this.listBoxReceived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxReceived.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxReceived.FormattingEnabled = true;
            this.listBoxReceived.IntegralHeight = false;
            this.listBoxReceived.ItemHeight = 12;
            this.listBoxReceived.Location = new System.Drawing.Point(0, 0);
            this.listBoxReceived.Name = "listBoxReceived";
            this.listBoxReceived.Size = new System.Drawing.Size(390, 340);
            this.listBoxReceived.TabIndex = 0;
            this.listBoxReceived.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBoxReceived_MouseUp);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.listBoxReceived);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.chartSpectrum);
            this.splitContainerMain.Size = new System.Drawing.Size(729, 340);
            this.splitContainerMain.SplitterDistance = 390;
            this.splitContainerMain.TabIndex = 24;
            this.splitContainerMain.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainerMain_SplitterMoving);
            this.splitContainerMain.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerMain_SplitterMoved);
            this.splitContainerMain.SizeChanged += new System.EventHandler(this.splitContainerMain_SizeChanged);
            // 
            // panelRightGain
            // 
            this.panelRightGain.Controls.Add(this.labelVUmeter);
            this.panelRightGain.Controls.Add(this.labelClockAnimation);
            this.panelRightGain.Controls.Add(this.labelClock);
            this.panelRightGain.Controls.Add(this.trackBarRxGain);
            this.panelRightGain.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRightGain.Location = new System.Drawing.Point(729, 0);
            this.panelRightGain.Name = "panelRightGain";
            this.panelRightGain.Size = new System.Drawing.Size(40, 377);
            this.panelRightGain.TabIndex = 1;
            // 
            // labelVUmeter
            // 
            this.labelVUmeter.Location = new System.Drawing.Point(14, 257);
            this.labelVUmeter.Name = "labelVUmeter";
            this.labelVUmeter.Size = new System.Drawing.Size(8, 44);
            this.labelVUmeter.TabIndex = 3;
            this.labelVUmeter.Text = "VU";
            // 
            // trackBarRxGain
            // 
            this.trackBarRxGain.AutoSize = false;
            this.trackBarRxGain.Location = new System.Drawing.Point(6, 90);
            this.trackBarRxGain.Maximum = 100;
            this.trackBarRxGain.Name = "trackBarRxGain";
            this.trackBarRxGain.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarRxGain.Size = new System.Drawing.Size(27, 160);
            this.trackBarRxGain.TabIndex = 2;
            this.trackBarRxGain.TickFrequency = 10;
            this.trackBarRxGain.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarRxGain.Scroll += new System.EventHandler(this.trackBarRxGain_Scroll);
            // 
            // eventLog1
            // 
            this.eventLog1.SynchronizingObject = this;
            // 
            // contextMenuStripOnReceived
            // 
            this.contextMenuStripOnReceived.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.copyItemToClipboardToolStripMenuItem});
            this.contextMenuStripOnReceived.Name = "contextMenuStripOnReceived";
            this.contextMenuStripOnReceived.Size = new System.Drawing.Size(197, 48);
            this.contextMenuStripOnReceived.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStripOnReceived_Closing);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.clearToolStripMenuItem.Text = "Clear list";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // copyItemToClipboardToolStripMenuItem
            // 
            this.copyItemToClipboardToolStripMenuItem.Name = "copyItemToClipboardToolStripMenuItem";
            this.copyItemToClipboardToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.copyItemToClipboardToolStripMenuItem.Text = "Copy item to clipboard";
            this.copyItemToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyItemToClipboardToolStripMenuItem_Click);
            // 
            // timerVU
            // 
            this.timerVU.Interval = 200;
            this.timerVU.Tick += new System.EventHandler(this.timerVU_Tick);
            // 
            // checkBoxAnnotate
            // 
            this.checkBoxAnnotate.AutoSize = true;
            this.checkBoxAnnotate.Enabled = false;
            this.checkBoxAnnotate.Location = new System.Drawing.Point(497, 12);
            this.checkBoxAnnotate.Name = "checkBoxAnnotate";
            this.checkBoxAnnotate.Size = new System.Drawing.Size(69, 17);
            this.checkBoxAnnotate.TabIndex = 6;
            this.checkBoxAnnotate.Text = "Annotate";
            this.checkBoxAnnotate.UseVisualStyleBackColor = true;
            this.checkBoxAnnotate.CheckedChanged += new System.EventHandler(this.checkBoxAnnotate_CheckedChanged);
            // 
            // RcvrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 377);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelRightGain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RcvrForm";
            this.Text = "DigiRite Receive";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XcvrForm_FormClosing);
            this.Load += new System.EventHandler(this.XcvrForm_Load);
            this.LocationChanged += new System.EventHandler(this.XcvrForm_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.XcvrForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.chartSpectrum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinFreq)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panelRightGain.ResumeLayout(false);
            this.panelRightGain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRxGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            this.contextMenuStripOnReceived.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartSpectrum;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxFreq;
        private System.Windows.Forms.NumericUpDown numericUpDownMinFreq;
        private System.Windows.Forms.CheckBox checkBoxEnableAP;
        private System.Windows.Forms.ComboBox comboBoxnDepth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelPower;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBoxReceived;
        private System.Windows.Forms.Label labelClock;
        private System.Windows.Forms.Label labelWaterfall;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Label labelClockAnimation;
        private System.Windows.Forms.Panel panelRightGain;
        private System.Windows.Forms.TrackBar trackBarRxGain;
        private System.Diagnostics.EventLog eventLog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOnReceived;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyItemToClipboardToolStripMenuItem;
        private System.Windows.Forms.Label labelVUmeter;
        private System.Windows.Forms.Timer timerVU;
        private System.Windows.Forms.CheckBox checkBoxAnnotate;
    }
}