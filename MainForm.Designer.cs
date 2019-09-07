namespace WriteLogDigiRite
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonAbort = new System.Windows.Forms.Button();
            this.numericUpDownFrequency = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxOnlyCQs = new System.Windows.Forms.CheckBox();
            this.checkBoxCQboth = new System.Windows.Forms.CheckBox();
            this.checkBoxShowMenu = new System.Windows.Forms.CheckBox();
            this.labelClock = new System.Windows.Forms.Label();
            this.listToMe = new System.Windows.Forms.Panel();
            this.timerFt8Clock = new System.Windows.Forms.Timer(this.components);
            this.timerSpectrum = new System.Windows.Forms.Timer(this.components);
            this.panel5 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxRespondAny = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxInProgress = new System.Windows.Forms.CheckBox();
            this.labelInProgress = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxAutoXmit = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelClockAnimation = new System.Windows.Forms.Label();
            this.buttonEqTx = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownRxFrequency = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.listBoxAlternativesPanel = new System.Windows.Forms.Panel();
            this.listBoxAlternatives = new System.Windows.Forms.CheckedListBox();
            this.checkBoxManualEntry = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxMessageEdit = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonTxToQip = new System.Windows.Forms.Button();
            this.labelTxValue = new System.Windows.Forms.Label();
            this.trackBarTxGain = new System.Windows.Forms.TrackBar();
            this.label13 = new System.Windows.Forms.Label();
            this.buttonTune = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownStreams = new System.Windows.Forms.NumericUpDown();
            this.labelPtt = new System.Windows.Forms.Label();
            this.buttonEqRx = new System.Windows.Forms.Button();
            this.comboBoxCQ = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonOdd = new System.Windows.Forms.RadioButton();
            this.radioButtonEven = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abortTxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qsoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllInactiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewReadMeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFileLengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetLogFileToEmpyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerCqLeft = new System.Windows.Forms.SplitContainer();
            this.splitContainerAnswerUpCqsDown = new System.Windows.Forms.SplitContainer();
            this.splitContainerCQ = new System.Windows.Forms.SplitContainer();
            this.panelEvenCQs = new System.Windows.Forms.Panel();
            this.checkBoxCqTable = new System.Windows.Forms.CheckBox();
            this.labelCqTable = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelOddCQs = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.splitContainerCenter = new System.Windows.Forms.SplitContainer();
            this.panelInProgress = new System.Windows.Forms.Panel();
            this.panelQipLabel = new System.Windows.Forms.Panel();
            this.timerCleanup = new System.Windows.Forms.Timer(this.components);
            this.listBoxConversation = new WriteLogDigiRite.ConversationListBox();
            this.checkedlbNextToSend = new WriteLogDigiRite.ToSendListBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrequency)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRxFrequency)).BeginInit();
            this.listBoxAlternativesPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTxGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStreams)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCqLeft)).BeginInit();
            this.splitContainerCqLeft.Panel1.SuspendLayout();
            this.splitContainerCqLeft.Panel2.SuspendLayout();
            this.splitContainerCqLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAnswerUpCqsDown)).BeginInit();
            this.splitContainerAnswerUpCqsDown.Panel1.SuspendLayout();
            this.splitContainerAnswerUpCqsDown.Panel2.SuspendLayout();
            this.splitContainerAnswerUpCqsDown.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCQ)).BeginInit();
            this.splitContainerCQ.Panel1.SuspendLayout();
            this.splitContainerCQ.Panel2.SuspendLayout();
            this.splitContainerCQ.SuspendLayout();
            this.panelEvenCQs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCenter)).BeginInit();
            this.splitContainerCenter.Panel1.SuspendLayout();
            this.splitContainerCenter.Panel2.SuspendLayout();
            this.splitContainerCenter.SuspendLayout();
            this.panelInProgress.SuspendLayout();
            this.panelQipLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAbort
            // 
            this.buttonAbort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonAbort.Location = new System.Drawing.Point(8, 181);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(58, 23);
            this.buttonAbort.TabIndex = 14;
            this.buttonAbort.Text = "&Abort TX";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // numericUpDownFrequency
            // 
            this.numericUpDownFrequency.Increment = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownFrequency.Location = new System.Drawing.Point(6, 23);
            this.numericUpDownFrequency.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownFrequency.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownFrequency.Name = "numericUpDownFrequency";
            this.numericUpDownFrequency.Size = new System.Drawing.Size(66, 20);
            this.numericUpDownFrequency.TabIndex = 0;
            this.numericUpDownFrequency.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownFrequency.ValueChanged += new System.EventHandler(this.numericUpDownFrequency_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxOnlyCQs);
            this.panel1.Controls.Add(this.checkBoxCQboth);
            this.panel1.Controls.Add(this.checkBoxShowMenu);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 417);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 39);
            this.panel1.TabIndex = 3;
            // 
            // checkBoxOnlyCQs
            // 
            this.checkBoxOnlyCQs.AutoSize = true;
            this.checkBoxOnlyCQs.Location = new System.Drawing.Point(86, 11);
            this.checkBoxOnlyCQs.Name = "checkBoxOnlyCQs";
            this.checkBoxOnlyCQs.Size = new System.Drawing.Size(70, 17);
            this.checkBoxOnlyCQs.TabIndex = 2;
            this.checkBoxOnlyCQs.Text = "Only CQs";
            this.checkBoxOnlyCQs.ThreeState = true;
            this.checkBoxOnlyCQs.UseVisualStyleBackColor = true;
            this.checkBoxOnlyCQs.CheckedChanged += new System.EventHandler(this.checkBoxOnlyCQs_CheckedChanged);
            this.checkBoxOnlyCQs.CheckStateChanged += new System.EventHandler(this.checkBoxOnlyCQs_CheckedChanged);
            // 
            // checkBoxCQboth
            // 
            this.checkBoxCQboth.AutoSize = true;
            this.checkBoxCQboth.Checked = true;
            this.checkBoxCQboth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCQboth.Location = new System.Drawing.Point(8, 11);
            this.checkBoxCQboth.Name = "checkBoxCQboth";
            this.checkBoxCQboth.Size = new System.Drawing.Size(71, 17);
            this.checkBoxCQboth.TabIndex = 1;
            this.checkBoxCQboth.Text = "Both CQs";
            this.checkBoxCQboth.UseVisualStyleBackColor = true;
            this.checkBoxCQboth.CheckedChanged += new System.EventHandler(this.checkBoxCQboth_CheckedChanged);
            // 
            // checkBoxShowMenu
            // 
            this.checkBoxShowMenu.AutoSize = true;
            this.checkBoxShowMenu.Checked = true;
            this.checkBoxShowMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowMenu.Location = new System.Drawing.Point(163, 11);
            this.checkBoxShowMenu.Name = "checkBoxShowMenu";
            this.checkBoxShowMenu.Size = new System.Drawing.Size(83, 17);
            this.checkBoxShowMenu.TabIndex = 3;
            this.checkBoxShowMenu.Text = "Show Menu";
            this.checkBoxShowMenu.UseVisualStyleBackColor = true;
            this.checkBoxShowMenu.CheckedChanged += new System.EventHandler(this.checkBoxShowMenu_CheckedChanged);
            // 
            // labelClock
            // 
            this.labelClock.AutoSize = true;
            this.labelClock.Location = new System.Drawing.Point(168, 216);
            this.labelClock.Name = "labelClock";
            this.labelClock.Size = new System.Drawing.Size(19, 13);
            this.labelClock.TabIndex = 11;
            this.labelClock.Text = "14";
            // 
            // listToMe
            // 
            this.listToMe.BackColor = System.Drawing.SystemColors.Window;
            this.listToMe.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listToMe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listToMe.Font = new System.Drawing.Font("Lucida Console", 9F);
            this.listToMe.Location = new System.Drawing.Point(0, 0);
            this.listToMe.Name = "listToMe";
            this.listToMe.Size = new System.Drawing.Size(260, 92);
            this.listToMe.TabIndex = 2;
            // 
            // timerFt8Clock
            // 
            this.timerFt8Clock.Interval = 50;
            this.timerFt8Clock.Tick += new System.EventHandler(this.timerFt8Clock_Tick);
            // 
            // timerSpectrum
            // 
            this.timerSpectrum.Interval = 300;
            this.timerSpectrum.Tick += new System.EventHandler(this.timerSpectrum_Tick);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label2);
            this.panel5.Controls.Add(this.checkBoxRespondAny);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(3, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(260, 24);
            this.panel5.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Messages to me";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxRespondAny
            // 
            this.checkBoxRespondAny.Location = new System.Drawing.Point(98, 4);
            this.checkBoxRespondAny.Name = "checkBoxRespondAny";
            this.checkBoxRespondAny.Size = new System.Drawing.Size(150, 17);
            this.checkBoxRespondAny.TabIndex = 1;
            this.checkBoxRespondAny.Text = "Auto respond to non-dupe";
            this.checkBoxRespondAny.UseVisualStyleBackColor = true;
            this.checkBoxRespondAny.CheckedChanged += new System.EventHandler(this.checkBoxRespondAny_CheckedChanged);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(355, 24);
            this.label4.TabIndex = 0;
            this.label4.Text = "QSO(s) in &Progress. L or R click";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBoxInProgress
            // 
            this.checkBoxInProgress.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxInProgress.Location = new System.Drawing.Point(4, 5);
            this.checkBoxInProgress.Name = "checkBoxInProgress";
            this.checkBoxInProgress.Size = new System.Drawing.Size(16, 14);
            this.checkBoxInProgress.TabIndex = 0;
            this.checkBoxInProgress.UseVisualStyleBackColor = true;
            this.checkBoxInProgress.Visible = false;
            // 
            // labelInProgress
            // 
            this.labelInProgress.BackColor = System.Drawing.SystemColors.Window;
            this.labelInProgress.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInProgress.Location = new System.Drawing.Point(20, 5);
            this.labelInProgress.Margin = new System.Windows.Forms.Padding(0);
            this.labelInProgress.Name = "labelInProgress";
            this.labelInProgress.Size = new System.Drawing.Size(156, 14);
            this.labelInProgress.TabIndex = 1;
            this.labelInProgress.Text = "1 KA1AQP/AB 1234 1234";
            this.labelInProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelInProgress.Visible = false;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(0, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(355, 23);
            this.label5.TabIndex = 2;
            this.label5.Text = "Conversation chronology";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label8);
            this.panel6.Controls.Add(this.checkBoxAutoXmit);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(355, 52);
            this.panel6.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(118, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "&Calculated next to send";
            // 
            // checkBoxAutoXmit
            // 
            this.checkBoxAutoXmit.AutoSize = true;
            this.checkBoxAutoXmit.Location = new System.Drawing.Point(8, 6);
            this.checkBoxAutoXmit.Name = "checkBoxAutoXmit";
            this.checkBoxAutoXmit.Size = new System.Drawing.Size(150, 17);
            this.checkBoxAutoXmit.TabIndex = 0;
            this.checkBoxAutoXmit.Text = "Automatically transmit next";
            this.checkBoxAutoXmit.UseVisualStyleBackColor = true;
            this.checkBoxAutoXmit.CheckedChanged += new System.EventHandler(this.checkBoxAutoXmit_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labelClockAnimation);
            this.panel3.Controls.Add(this.buttonEqTx);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.numericUpDownRxFrequency);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.listBoxAlternativesPanel);
            this.panel3.Controls.Add(this.checkBoxManualEntry);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.textBoxMessageEdit);
            this.panel3.Controls.Add(this.labelClock);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(622, 24);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(225, 456);
            this.panel3.TabIndex = 2;
            // 
            // labelClockAnimation
            // 
            this.labelClockAnimation.Location = new System.Drawing.Point(193, 207);
            this.labelClockAnimation.Name = "labelClockAnimation";
            this.labelClockAnimation.Size = new System.Drawing.Size(30, 30);
            this.labelClockAnimation.TabIndex = 10;
            this.labelClockAnimation.Text = "an";
            this.labelClockAnimation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonEqTx
            // 
            this.buttonEqTx.Location = new System.Drawing.Point(122, 211);
            this.buttonEqTx.Name = "buttonEqTx";
            this.buttonEqTx.Size = new System.Drawing.Size(43, 23);
            this.buttonEqTx.TabIndex = 8;
            this.buttonEqTx.Text = "=Tx";
            this.buttonEqTx.UseVisualStyleBackColor = true;
            this.buttonEqTx.Click += new System.EventHandler(this.buttonEqTx_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 216);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "&Rx";
            // 
            // numericUpDownRxFrequency
            // 
            this.numericUpDownRxFrequency.Increment = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownRxFrequency.Location = new System.Drawing.Point(30, 212);
            this.numericUpDownRxFrequency.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownRxFrequency.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownRxFrequency.Name = "numericUpDownRxFrequency";
            this.numericUpDownRxFrequency.Size = new System.Drawing.Size(66, 20);
            this.numericUpDownRxFrequency.TabIndex = 6;
            this.numericUpDownRxFrequency.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownRxFrequency.ValueChanged += new System.EventHandler(this.numericUpDownRxFrequency_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(99, 216);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Hz";
            // 
            // listBoxAlternativesPanel
            // 
            this.listBoxAlternativesPanel.Controls.Add(this.listBoxAlternatives);
            this.listBoxAlternativesPanel.Location = new System.Drawing.Point(0, 76);
            this.listBoxAlternativesPanel.Margin = new System.Windows.Forms.Padding(0);
            this.listBoxAlternativesPanel.Name = "listBoxAlternativesPanel";
            this.listBoxAlternativesPanel.Size = new System.Drawing.Size(225, 126);
            this.listBoxAlternativesPanel.TabIndex = 11;
            // 
            // listBoxAlternatives
            // 
            this.listBoxAlternatives.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxAlternatives.CheckOnClick = true;
            this.listBoxAlternatives.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBoxAlternatives.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxAlternatives.FormattingEnabled = true;
            this.listBoxAlternatives.Location = new System.Drawing.Point(45, 0);
            this.listBoxAlternatives.Name = "listBoxAlternatives";
            this.listBoxAlternatives.Size = new System.Drawing.Size(180, 126);
            this.listBoxAlternatives.TabIndex = 4;
            this.listBoxAlternatives.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listBoxAlternatives_ItemCheck);
            this.listBoxAlternatives.SelectedIndexChanged += new System.EventHandler(this.listBoxAlternatives_SelectedIndexChanged);
            // 
            // checkBoxManualEntry
            // 
            this.checkBoxManualEntry.AutoSize = true;
            this.checkBoxManualEntry.Location = new System.Drawing.Point(8, 25);
            this.checkBoxManualEntry.Name = "checkBoxManualEntry";
            this.checkBoxManualEntry.Size = new System.Drawing.Size(15, 14);
            this.checkBoxManualEntry.TabIndex = 1;
            this.checkBoxManualEntry.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(216, 23);
            this.label7.TabIndex = 3;
            this.label7.Text = "Alternative messages";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxMessageEdit
            // 
            this.textBoxMessageEdit.Font = new System.Drawing.Font("Lucida Console", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMessageEdit.Location = new System.Drawing.Point(30, 22);
            this.textBoxMessageEdit.Name = "textBoxMessageEdit";
            this.textBoxMessageEdit.Size = new System.Drawing.Size(161, 22);
            this.textBoxMessageEdit.TabIndex = 2;
            this.textBoxMessageEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMessageEdit_KeyPress);
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(225, 23);
            this.label6.TabIndex = 0;
            this.label6.Text = "&Manual edit. Enter to send";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonTxToQip);
            this.groupBox3.Controls.Add(this.labelTxValue);
            this.groupBox3.Controls.Add(this.trackBarTxGain);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.buttonAbort);
            this.groupBox3.Controls.Add(this.buttonTune);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.numericUpDownStreams);
            this.groupBox3.Controls.Add(this.labelPtt);
            this.groupBox3.Controls.Add(this.buttonEqRx);
            this.groupBox3.Controls.Add(this.comboBoxCQ);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.numericUpDownFrequency);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox3.Location = new System.Drawing.Point(5, 240);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox3.Size = new System.Drawing.Size(215, 211);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "&Tx";
            // 
            // buttonTxToQip
            // 
            this.buttonTxToQip.AutoSize = true;
            this.buttonTxToQip.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonTxToQip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.buttonTxToQip.Location = new System.Drawing.Point(146, 22);
            this.buttonTxToQip.Name = "buttonTxToQip";
            this.buttonTxToQip.Size = new System.Drawing.Size(66, 23);
            this.buttonTxToQip.TabIndex = 3;
            this.buttonTxToQip.Text = "apply QnP";
            this.buttonTxToQip.UseVisualStyleBackColor = true;
            this.buttonTxToQip.Click += new System.EventHandler(this.buttonTxToQip_Click);
            // 
            // labelTxValue
            // 
            this.labelTxValue.AutoSize = true;
            this.labelTxValue.Location = new System.Drawing.Point(187, 190);
            this.labelTxValue.Name = "labelTxValue";
            this.labelTxValue.Size = new System.Drawing.Size(19, 13);
            this.labelTxValue.TabIndex = 13;
            this.labelTxValue.Text = "99";
            // 
            // trackBarTxGain
            // 
            this.trackBarTxGain.AutoSize = false;
            this.trackBarTxGain.Location = new System.Drawing.Point(185, 52);
            this.trackBarTxGain.Maximum = 100;
            this.trackBarTxGain.Minimum = 1;
            this.trackBarTxGain.Name = "trackBarTxGain";
            this.trackBarTxGain.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarTxGain.Size = new System.Drawing.Size(25, 135);
            this.trackBarTxGain.TabIndex = 12;
            this.trackBarTxGain.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarTxGain.Value = 1;
            this.trackBarTxGain.Scroll += new System.EventHandler(this.trackBarTxGain_Scroll);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(1, 139);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(25, 13);
            this.label13.TabIndex = 8;
            this.label13.Text = "CQ:";
            // 
            // buttonTune
            // 
            this.buttonTune.Location = new System.Drawing.Point(131, 66);
            this.buttonTune.Name = "buttonTune";
            this.buttonTune.Size = new System.Drawing.Size(43, 23);
            this.buttonTune.TabIndex = 5;
            this.buttonTune.Text = "Tune";
            this.buttonTune.UseVisualStyleBackColor = true;
            this.buttonTune.Click += new System.EventHandler(this.buttonTune_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 110);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 6;
            this.label12.Text = "&Streams:";
            // 
            // numericUpDownStreams
            // 
            this.numericUpDownStreams.Location = new System.Drawing.Point(63, 106);
            this.numericUpDownStreams.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownStreams.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownStreams.Name = "numericUpDownStreams";
            this.numericUpDownStreams.Size = new System.Drawing.Size(32, 20);
            this.numericUpDownStreams.TabIndex = 7;
            this.numericUpDownStreams.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelPtt
            // 
            this.labelPtt.AutoSize = true;
            this.labelPtt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPtt.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelPtt.Location = new System.Drawing.Point(88, 185);
            this.labelPtt.Name = "labelPtt";
            this.labelPtt.Size = new System.Drawing.Size(76, 13);
            this.labelPtt.TabIndex = 9;
            this.labelPtt.Text = "PTT on COM1";
            // 
            // buttonEqRx
            // 
            this.buttonEqRx.Location = new System.Drawing.Point(94, 22);
            this.buttonEqRx.Name = "buttonEqRx";
            this.buttonEqRx.Size = new System.Drawing.Size(43, 23);
            this.buttonEqRx.TabIndex = 2;
            this.buttonEqRx.Text = "=Rx";
            this.buttonEqRx.UseVisualStyleBackColor = true;
            this.buttonEqRx.Click += new System.EventHandler(this.buttonEqRx_Click);
            // 
            // comboBoxCQ
            // 
            this.comboBoxCQ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCQ.Items.AddRange(new object[] {
            "Off",
            "On idle",
            "Use stream"});
            this.comboBoxCQ.Location = new System.Drawing.Point(30, 135);
            this.comboBoxCQ.Name = "comboBoxCQ";
            this.comboBoxCQ.Size = new System.Drawing.Size(116, 21);
            this.comboBoxCQ.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonOdd);
            this.groupBox1.Controls.Add(this.radioButtonEven);
            this.groupBox1.Location = new System.Drawing.Point(7, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(112, 43);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cycle";
            // 
            // radioButtonOdd
            // 
            this.radioButtonOdd.AutoSize = true;
            this.radioButtonOdd.Location = new System.Drawing.Point(60, 17);
            this.radioButtonOdd.Name = "radioButtonOdd";
            this.radioButtonOdd.Size = new System.Drawing.Size(43, 17);
            this.radioButtonOdd.TabIndex = 1;
            this.radioButtonOdd.Text = "odd";
            this.radioButtonOdd.UseVisualStyleBackColor = true;
            this.radioButtonOdd.CheckedChanged += new System.EventHandler(this.radioOddEven_CheckedChanged);
            // 
            // radioButtonEven
            // 
            this.radioButtonEven.AutoSize = true;
            this.radioButtonEven.Checked = true;
            this.radioButtonEven.Location = new System.Drawing.Point(7, 17);
            this.radioButtonEven.Name = "radioButtonEven";
            this.radioButtonEven.Size = new System.Drawing.Size(49, 17);
            this.radioButtonEven.TabIndex = 0;
            this.radioButtonEven.TabStop = true;
            this.radioButtonEven.Text = "even";
            this.radioButtonEven.UseVisualStyleBackColor = true;
            this.radioButtonEven.CheckedChanged += new System.EventHandler(this.radioOddEven_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(73, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Hz";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.qsoToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(847, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.fontToolStripMenuItem,
            this.abortTxToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.setupToolStripMenuItem.Text = "&Setup...";
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.fontToolStripMenuItem.Text = "Font...";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // abortTxToolStripMenuItem
            // 
            this.abortTxToolStripMenuItem.Name = "abortTxToolStripMenuItem";
            this.abortTxToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.abortTxToolStripMenuItem.Text = "&Abort Tx";
            this.abortTxToolStripMenuItem.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // qsoToolStripMenuItem
            // 
            this.qsoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAllInactiveToolStripMenuItem});
            this.qsoToolStripMenuItem.Name = "qsoToolStripMenuItem";
            this.qsoToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.qsoToolStripMenuItem.Text = "Q&SO\'s";
            // 
            // removeAllInactiveToolStripMenuItem
            // 
            this.removeAllInactiveToolStripMenuItem.Name = "removeAllInactiveToolStripMenuItem";
            this.removeAllInactiveToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.removeAllInactiveToolStripMenuItem.Text = "In progress: remove all &Inactive";
            this.removeAllInactiveToolStripMenuItem.Click += new System.EventHandler(this.removeAllInactiveToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.viewReadMeToolStripMenuItem,
            this.viewLogToolStripMenuItem,
            this.logFileLengthToolStripMenuItem,
            this.resetLogFileToEmpyToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.DropDownOpened += new System.EventHandler(this.helpToolStripMenuItem_DropDownOpened);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // viewReadMeToolStripMenuItem
            // 
            this.viewReadMeToolStripMenuItem.Name = "viewReadMeToolStripMenuItem";
            this.viewReadMeToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.viewReadMeToolStripMenuItem.Text = "View ReadMe";
            this.viewReadMeToolStripMenuItem.Click += new System.EventHandler(this.viewReadMeToolStripMenuItem_Click);
            // 
            // viewLogToolStripMenuItem
            // 
            this.viewLogToolStripMenuItem.Name = "viewLogToolStripMenuItem";
            this.viewLogToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.viewLogToolStripMenuItem.Text = "&View Log";
            this.viewLogToolStripMenuItem.Click += new System.EventHandler(this.viewLogToolStripMenuItem_Click);
            // 
            // logFileLengthToolStripMenuItem
            // 
            this.logFileLengthToolStripMenuItem.Name = "logFileLengthToolStripMenuItem";
            this.logFileLengthToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.logFileLengthToolStripMenuItem.Text = "&Log file length:";
            // 
            // resetLogFileToEmpyToolStripMenuItem
            // 
            this.resetLogFileToEmpyToolStripMenuItem.Name = "resetLogFileToEmpyToolStripMenuItem";
            this.resetLogFileToEmpyToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.resetLogFileToEmpyToolStripMenuItem.Text = "&Reset log file to empty";
            this.resetLogFileToEmpyToolStripMenuItem.Click += new System.EventHandler(this.resetLogFileToEmpyToolStripMenuItem_Click);
            // 
            // splitContainerCqLeft
            // 
            this.splitContainerCqLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerCqLeft.Location = new System.Drawing.Point(0, 24);
            this.splitContainerCqLeft.Name = "splitContainerCqLeft";
            // 
            // splitContainerCqLeft.Panel1
            // 
            this.splitContainerCqLeft.Panel1.Controls.Add(this.splitContainerAnswerUpCqsDown);
            this.splitContainerCqLeft.Panel1.Controls.Add(this.panel5);
            this.splitContainerCqLeft.Panel1.Controls.Add(this.panel1);
            this.splitContainerCqLeft.Panel1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // splitContainerCqLeft.Panel2
            // 
            this.splitContainerCqLeft.Panel2.Controls.Add(this.splitContainerCenter);
            this.splitContainerCqLeft.Size = new System.Drawing.Size(622, 456);
            this.splitContainerCqLeft.SplitterDistance = 263;
            this.splitContainerCqLeft.TabIndex = 2;
            // 
            // splitContainerAnswerUpCqsDown
            // 
            this.splitContainerAnswerUpCqsDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAnswerUpCqsDown.Location = new System.Drawing.Point(3, 24);
            this.splitContainerAnswerUpCqsDown.Name = "splitContainerAnswerUpCqsDown";
            this.splitContainerAnswerUpCqsDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerAnswerUpCqsDown.Panel1
            // 
            this.splitContainerAnswerUpCqsDown.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainerAnswerUpCqsDown.Panel1.Controls.Add(this.listToMe);
            // 
            // splitContainerAnswerUpCqsDown.Panel2
            // 
            this.splitContainerAnswerUpCqsDown.Panel2.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainerAnswerUpCqsDown.Panel2.Controls.Add(this.splitContainerCQ);
            this.splitContainerAnswerUpCqsDown.Panel2MinSize = 100;
            this.splitContainerAnswerUpCqsDown.Size = new System.Drawing.Size(260, 393);
            this.splitContainerAnswerUpCqsDown.SplitterDistance = 92;
            this.splitContainerAnswerUpCqsDown.TabIndex = 2;
            // 
            // splitContainerCQ
            // 
            this.splitContainerCQ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerCQ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerCQ.Location = new System.Drawing.Point(0, 0);
            this.splitContainerCQ.Name = "splitContainerCQ";
            this.splitContainerCQ.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerCQ.Panel1
            // 
            this.splitContainerCQ.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainerCQ.Panel1.Controls.Add(this.panelEvenCQs);
            this.splitContainerCQ.Panel1.Controls.Add(this.label3);
            // 
            // splitContainerCQ.Panel2
            // 
            this.splitContainerCQ.Panel2.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainerCQ.Panel2.Controls.Add(this.panelOddCQs);
            this.splitContainerCQ.Panel2.Controls.Add(this.label11);
            this.splitContainerCQ.Size = new System.Drawing.Size(260, 297);
            this.splitContainerCQ.SplitterDistance = 158;
            this.splitContainerCQ.TabIndex = 3;
            // 
            // panelEvenCQs
            // 
            this.panelEvenCQs.Controls.Add(this.checkBoxCqTable);
            this.panelEvenCQs.Controls.Add(this.labelCqTable);
            this.panelEvenCQs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEvenCQs.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEvenCQs.Location = new System.Drawing.Point(0, 17);
            this.panelEvenCQs.Name = "panelEvenCQs";
            this.panelEvenCQs.Size = new System.Drawing.Size(258, 139);
            this.panelEvenCQs.TabIndex = 1;
            // 
            // checkBoxCqTable
            // 
            this.checkBoxCqTable.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxCqTable.Location = new System.Drawing.Point(3, 4);
            this.checkBoxCqTable.Name = "checkBoxCqTable";
            this.checkBoxCqTable.Size = new System.Drawing.Size(16, 14);
            this.checkBoxCqTable.TabIndex = 1;
            this.checkBoxCqTable.UseVisualStyleBackColor = true;
            this.checkBoxCqTable.Visible = false;
            // 
            // labelCqTable
            // 
            this.labelCqTable.BackColor = System.Drawing.SystemColors.Window;
            this.labelCqTable.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCqTable.Location = new System.Drawing.Point(19, 4);
            this.labelCqTable.Margin = new System.Windows.Forms.Padding(0);
            this.labelCqTable.Name = "labelCqTable";
            this.labelCqTable.Size = new System.Drawing.Size(203, 14);
            this.labelCqTable.TabIndex = 0;
            this.labelCqTable.Text = "M +07 CQ PA3/WA3FFF EM10";
            this.labelCqTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelCqTable.Visible = false;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(258, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Even C&Qs heard. Click to answer";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelOddCQs
            // 
            this.panelOddCQs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOddCQs.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelOddCQs.Location = new System.Drawing.Point(0, 17);
            this.panelOddCQs.Name = "panelOddCQs";
            this.panelOddCQs.Size = new System.Drawing.Size(258, 116);
            this.panelOddCQs.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Top;
            this.label11.Location = new System.Drawing.Point(0, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(258, 17);
            this.label11.TabIndex = 0;
            this.label11.Text = "Odd C&Qs heard. Click to answer";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainerCenter
            // 
            this.splitContainerCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerCenter.Location = new System.Drawing.Point(0, 0);
            this.splitContainerCenter.Name = "splitContainerCenter";
            this.splitContainerCenter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerCenter.Panel1
            // 
            this.splitContainerCenter.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainerCenter.Panel1.Controls.Add(this.panelInProgress);
            this.splitContainerCenter.Panel1.Controls.Add(this.panelQipLabel);
            this.splitContainerCenter.Panel1.Font = new System.Drawing.Font("Lucida Console", 9F);
            // 
            // splitContainerCenter.Panel2
            // 
            this.splitContainerCenter.Panel2.Controls.Add(this.listBoxConversation);
            this.splitContainerCenter.Panel2.Controls.Add(this.label5);
            this.splitContainerCenter.Panel2.Controls.Add(this.checkedlbNextToSend);
            this.splitContainerCenter.Panel2.Controls.Add(this.panel6);
            this.splitContainerCenter.Size = new System.Drawing.Size(355, 456);
            this.splitContainerCenter.SplitterDistance = 160;
            this.splitContainerCenter.TabIndex = 6;
            // 
            // panelInProgress
            // 
            this.panelInProgress.Controls.Add(this.labelInProgress);
            this.panelInProgress.Controls.Add(this.checkBoxInProgress);
            this.panelInProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInProgress.Location = new System.Drawing.Point(0, 24);
            this.panelInProgress.Name = "panelInProgress";
            this.panelInProgress.Size = new System.Drawing.Size(355, 136);
            this.panelInProgress.TabIndex = 1;
            // 
            // panelQipLabel
            // 
            this.panelQipLabel.Controls.Add(this.label4);
            this.panelQipLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelQipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.panelQipLabel.Location = new System.Drawing.Point(0, 0);
            this.panelQipLabel.Name = "panelQipLabel";
            this.panelQipLabel.Size = new System.Drawing.Size(355, 24);
            this.panelQipLabel.TabIndex = 4;
            // 
            // timerCleanup
            // 
            this.timerCleanup.Interval = 10000;
            this.timerCleanup.Tick += new System.EventHandler(this.timerCleanup_Tick);
            // 
            // listBoxConversation
            // 
            this.listBoxConversation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxConversation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxConversation.Font = new System.Drawing.Font("Lucida Console", 9F);
            this.listBoxConversation.FormattingEnabled = true;
            this.listBoxConversation.ItemHeight = 12;
            this.listBoxConversation.Location = new System.Drawing.Point(0, 165);
            this.listBoxConversation.Name = "listBoxConversation";
            this.listBoxConversation.Size = new System.Drawing.Size(355, 127);
            this.listBoxConversation.TabIndex = 3;
            // 
            // checkedlbNextToSend
            // 
            this.checkedlbNextToSend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedlbNextToSend.CheckOnClick = true;
            this.checkedlbNextToSend.ColumnWidth = 150;
            this.checkedlbNextToSend.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkedlbNextToSend.FormattingEnabled = true;
            this.checkedlbNextToSend.Location = new System.Drawing.Point(0, 52);
            this.checkedlbNextToSend.MultiColumn = true;
            this.checkedlbNextToSend.Name = "checkedlbNextToSend";
            this.checkedlbNextToSend.Size = new System.Drawing.Size(355, 90);
            this.checkedlbNextToSend.Sorted = true;
            this.checkedlbNextToSend.TabIndex = 1;
            this.checkedlbNextToSend.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedlbNextToSend_ItemCheck);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonAbort;
            this.ClientSize = new System.Drawing.Size(847, 480);
            this.Controls.Add(this.splitContainerCqLeft);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "DigiRite";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrequency)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRxFrequency)).EndInit();
            this.listBoxAlternativesPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTxGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStreams)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainerCqLeft.Panel1.ResumeLayout(false);
            this.splitContainerCqLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCqLeft)).EndInit();
            this.splitContainerCqLeft.ResumeLayout(false);
            this.splitContainerAnswerUpCqsDown.Panel1.ResumeLayout(false);
            this.splitContainerAnswerUpCqsDown.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAnswerUpCqsDown)).EndInit();
            this.splitContainerAnswerUpCqsDown.ResumeLayout(false);
            this.splitContainerCQ.Panel1.ResumeLayout(false);
            this.splitContainerCQ.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCQ)).EndInit();
            this.splitContainerCQ.ResumeLayout(false);
            this.panelEvenCQs.ResumeLayout(false);
            this.splitContainerCenter.Panel1.ResumeLayout(false);
            this.splitContainerCenter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCenter)).EndInit();
            this.splitContainerCenter.ResumeLayout(false);
            this.panelInProgress.ResumeLayout(false);
            this.panelQipLabel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.NumericUpDown numericUpDownFrequency;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel listToMe;
        private System.Windows.Forms.Label labelClock;
        private System.Windows.Forms.Timer timerFt8Clock;
        private System.Windows.Forms.Timer timerSpectrum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxAutoXmit;
        private System.Windows.Forms.CheckBox checkBoxRespondAny;
        private System.Windows.Forms.Panel panel3;
        private ConversationListBox listBoxConversation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonOdd;
        private System.Windows.Forms.RadioButton radioButtonEven;
        private System.Windows.Forms.TextBox textBoxMessageEdit;
        private ToSendListBox checkedlbNextToSend;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxManualEntry;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckedListBox listBoxAlternatives;
        private System.Windows.Forms.Panel listBoxAlternativesPanel;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ComboBox comboBoxCQ;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxShowMenu;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerCqLeft;
        private System.Windows.Forms.SplitContainer splitContainerAnswerUpCqsDown;
        private System.Windows.Forms.CheckBox checkBoxCQboth;
        private System.Windows.Forms.SplitContainer splitContainerCQ;
        private System.Windows.Forms.CheckBox checkBoxCqTable;
        private System.Windows.Forms.Label labelCqTable;
        private System.Windows.Forms.ToolStripMenuItem viewLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logFileLengthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetLogFileToEmpyToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxInProgress;
        private System.Windows.Forms.Label labelInProgress;
        private System.Windows.Forms.ToolStripMenuItem abortTxToolStripMenuItem;
        private System.Windows.Forms.Button buttonEqTx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownRxFrequency;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonEqRx;
        private System.Windows.Forms.Label labelPtt;
        private System.Windows.Forms.Panel panelEvenCQs;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelOddCQs;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelClockAnimation;
        private System.Windows.Forms.TrackBar trackBarTxGain;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDownStreams;
        private System.Windows.Forms.ToolStripMenuItem viewReadMeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qsoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllInactiveToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerCenter;
        private System.Windows.Forms.Panel panelInProgress;
        private System.Windows.Forms.Button buttonTune;
        private System.Windows.Forms.Timer timerCleanup;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBoxOnlyCQs;
        private System.Windows.Forms.Label labelTxValue;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.Panel panelQipLabel;
        private System.Windows.Forms.Button buttonTxToQip;
    }
}

