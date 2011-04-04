namespace LogPlotter
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbtnGeneratePltFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnSetLogsDir = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnReloadLogsDir = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnSaveAverage = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.ttxtMvAvgWnd = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.ttxtDataLength = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.ttxtPlotEvery = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.lstAvailableLogs = new System.Windows.Forms.ListBox();
            this.lstSelectedLogs = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectFiles = new System.Windows.Forms.Button();
            this.btnRemoveSelectedFiles = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.averageRewScoreDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scoreDelaysAndCumulativeRewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.eAPlotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meanOfSelectedLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.bestAndMeanInOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnGeneratePltFile,
            this.toolStripSeparator2,
            this.tbtnSetLogsDir,
            this.toolStripSeparator3,
            this.tbtnReloadLogsDir,
            this.toolStripSeparator1,
            this.tbtnSaveAverage});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbtnGeneratePltFile
            // 
            this.tbtnGeneratePltFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnGeneratePltFile.Image = ((System.Drawing.Image)(resources.GetObject("tbtnGeneratePltFile.Image")));
            this.tbtnGeneratePltFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnGeneratePltFile.Name = "tbtnGeneratePltFile";
            this.tbtnGeneratePltFile.Size = new System.Drawing.Size(96, 22);
            this.tbtnGeneratePltFile.Text = "Generate Plt File";
            this.tbtnGeneratePltFile.Click += new System.EventHandler(this.tbtnGeneratePltFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tbtnSetLogsDir
            // 
            this.tbtnSetLogsDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnSetLogsDir.Image = ((System.Drawing.Image)(resources.GetObject("tbtnSetLogsDir.Image")));
            this.tbtnSetLogsDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnSetLogsDir.Name = "tbtnSetLogsDir";
            this.tbtnSetLogsDir.Size = new System.Drawing.Size(73, 22);
            this.tbtnSetLogsDir.Text = "Set Logs Dir";
            this.tbtnSetLogsDir.Click += new System.EventHandler(this.tbtnSetLogsDir_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tbtnReloadLogsDir
            // 
            this.tbtnReloadLogsDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnReloadLogsDir.Image = ((System.Drawing.Image)(resources.GetObject("tbtnReloadLogsDir.Image")));
            this.tbtnReloadLogsDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnReloadLogsDir.Name = "tbtnReloadLogsDir";
            this.tbtnReloadLogsDir.Size = new System.Drawing.Size(93, 22);
            this.tbtnReloadLogsDir.Text = "Reload Logs Dir";
            this.tbtnReloadLogsDir.Click += new System.EventHandler(this.tbtnReloadLogsDir_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbtnSaveAverage
            // 
            this.tbtnSaveAverage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnSaveAverage.Image = ((System.Drawing.Image)(resources.GetObject("tbtnSaveAverage.Image")));
            this.tbtnSaveAverage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnSaveAverage.Name = "tbtnSaveAverage";
            this.tbtnSaveAverage.Size = new System.Drawing.Size(121, 22);
            this.tbtnSaveAverage.Text = "Save Average of Files";
            this.tbtnSaveAverage.Click += new System.EventHandler(this.tbtnSaveAverage_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(95, 22);
            this.toolStripLabel1.Text = "MvAvg Window:";
            // 
            // ttxtMvAvgWnd
            // 
            this.ttxtMvAvgWnd.Name = "ttxtMvAvgWnd";
            this.ttxtMvAvgWnd.Size = new System.Drawing.Size(50, 25);
            this.ttxtMvAvgWnd.Text = "200";
            this.ttxtMvAvgWnd.Leave += new System.EventHandler(this.ttxtMvAvgWnd_Leave);
            this.ttxtMvAvgWnd.TextChanged += new System.EventHandler(this.ttxtMvAvgWnd_TextChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(74, 22);
            this.toolStripLabel2.Text = "Date Length:";
            // 
            // ttxtDataLength
            // 
            this.ttxtDataLength.Name = "ttxtDataLength";
            this.ttxtDataLength.Size = new System.Drawing.Size(80, 25);
            this.ttxtDataLength.Text = "0";
            this.ttxtDataLength.Leave += new System.EventHandler(this.ttxtDataLength_Leave);
            this.ttxtDataLength.TextChanged += new System.EventHandler(this.ttxtDataLength_TextChanged);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(62, 22);
            this.toolStripLabel3.Text = "Plot Every:";
            // 
            // ttxtPlotEvery
            // 
            this.ttxtPlotEvery.Name = "ttxtPlotEvery";
            this.ttxtPlotEvery.Size = new System.Drawing.Size(80, 25);
            this.ttxtPlotEvery.Text = "1";
            this.ttxtPlotEvery.Leave += new System.EventHandler(this.ttxtPlotEvery_Leave);
            this.ttxtPlotEvery.TextChanged += new System.EventHandler(this.ttxtPlotEvery_TextChanged);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(30, 22);
            this.toolStripLabel4.Text = "data";
            // 
            // lstAvailableLogs
            // 
            this.lstAvailableLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAvailableLogs.Font = new System.Drawing.Font("Courier New", 9F);
            this.lstAvailableLogs.FormattingEnabled = true;
            this.lstAvailableLogs.HorizontalScrollbar = true;
            this.lstAvailableLogs.ItemHeight = 15;
            this.lstAvailableLogs.Location = new System.Drawing.Point(3, 28);
            this.lstAvailableLogs.Name = "lstAvailableLogs";
            this.lstAvailableLogs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAvailableLogs.Size = new System.Drawing.Size(314, 320);
            this.lstAvailableLogs.TabIndex = 1;
            // 
            // lstSelectedLogs
            // 
            this.lstSelectedLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSelectedLogs.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstSelectedLogs.FormattingEnabled = true;
            this.lstSelectedLogs.HorizontalScrollbar = true;
            this.lstSelectedLogs.ItemHeight = 15;
            this.lstSelectedLogs.Location = new System.Drawing.Point(483, 28);
            this.lstSelectedLogs.Name = "lstSelectedLogs";
            this.lstSelectedLogs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedLogs.Size = new System.Drawing.Size(314, 320);
            this.lstSelectedLogs.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Available Log files:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(483, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Selected Log files:";
            // 
            // btnSelectFiles
            // 
            this.btnSelectFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectFiles.Location = new System.Drawing.Point(3, 3);
            this.btnSelectFiles.Name = "btnSelectFiles";
            this.btnSelectFiles.Size = new System.Drawing.Size(149, 23);
            this.btnSelectFiles.TabIndex = 5;
            this.btnSelectFiles.Text = ">>";
            this.btnSelectFiles.UseVisualStyleBackColor = true;
            this.btnSelectFiles.Click += new System.EventHandler(this.btnSelectFiles_Click);
            // 
            // btnRemoveSelectedFiles
            // 
            this.btnRemoveSelectedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveSelectedFiles.Location = new System.Drawing.Point(2, 32);
            this.btnRemoveSelectedFiles.Name = "btnRemoveSelectedFiles";
            this.btnRemoveSelectedFiles.Size = new System.Drawing.Size(149, 23);
            this.btnRemoveSelectedFiles.TabIndex = 6;
            this.btnRemoveSelectedFiles.Text = "<<";
            this.btnRemoveSelectedFiles.UseVisualStyleBackColor = true;
            this.btnRemoveSelectedFiles.Click += new System.EventHandler(this.btnRemoveSelectedFiles_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lstSelectedLogs, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lstAvailableLogs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 351);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSelectFiles);
            this.panel1.Controls.Add(this.btnRemoveSelectedFiles);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(323, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(154, 320);
            this.panel1.TabIndex = 5;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.toolStripSeparator4,
            this.toolStripLabel1,
            this.ttxtMvAvgWnd,
            this.toolStripSeparator5,
            this.toolStripLabel2,
            this.ttxtDataLength,
            this.toolStripSeparator6,
            this.toolStripLabel3,
            this.ttxtPlotEvery,
            this.toolStripLabel4});
            this.toolStrip2.Location = new System.Drawing.Point(0, 25);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(800, 25);
            this.toolStrip2.TabIndex = 8;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.averageRewScoreDiffToolStripMenuItem,
            this.scoreDelaysAndCumulativeRewToolStripMenuItem,
            this.toolStripMenuItem1,
            this.eAPlotsToolStripMenuItem,
            this.bestAndMeanInOneToolStripMenuItem,
            this.meanOfSelectedLogsToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(91, 22);
            this.toolStripSplitButton1.Text = "Plot Selected";
            // 
            // averageRewScoreDiffToolStripMenuItem
            // 
            this.averageRewScoreDiffToolStripMenuItem.Name = "averageRewScoreDiffToolStripMenuItem";
            this.averageRewScoreDiffToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.averageRewScoreDiffToolStripMenuItem.Text = "Average Rew and Score Diff";
            this.averageRewScoreDiffToolStripMenuItem.Click += new System.EventHandler(this.averageRewScoreDiffToolStripMenuItem_Click);
            // 
            // scoreDelaysAndCumulativeRewToolStripMenuItem
            // 
            this.scoreDelaysAndCumulativeRewToolStripMenuItem.Name = "scoreDelaysAndCumulativeRewToolStripMenuItem";
            this.scoreDelaysAndCumulativeRewToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.scoreDelaysAndCumulativeRewToolStripMenuItem.Text = "Score Delays and Cumulative Rew";
            this.scoreDelaysAndCumulativeRewToolStripMenuItem.Click += new System.EventHandler(this.scoreDelaysAndCumulativeRewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(249, 6);
            // 
            // eAPlotsToolStripMenuItem
            // 
            this.eAPlotsToolStripMenuItem.Name = "eAPlotsToolStripMenuItem";
            this.eAPlotsToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.eAPlotsToolStripMenuItem.Text = "EA Plots";
            this.eAPlotsToolStripMenuItem.Click += new System.EventHandler(this.eAPlotsToolStripMenuItem_Click);
            // 
            // meanOfSelectedLogsToolStripMenuItem
            // 
            this.meanOfSelectedLogsToolStripMenuItem.Name = "meanOfSelectedLogsToolStripMenuItem";
            this.meanOfSelectedLogsToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.meanOfSelectedLogsToolStripMenuItem.Text = "Mean of Selected Logs";
            this.meanOfSelectedLogsToolStripMenuItem.Click += new System.EventHandler(this.meanOfSelectedLogsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // bestAndMeanInOneToolStripMenuItem
            // 
            this.bestAndMeanInOneToolStripMenuItem.Name = "bestAndMeanInOneToolStripMenuItem";
            this.bestAndMeanInOneToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.bestAndMeanInOneToolStripMenuItem.Text = "Best and Mean in One";
            this.bestAndMeanInOneToolStripMenuItem.Click += new System.EventHandler(this.bestAndMeanInOneToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 376);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormMain";
            this.Text = "Grid Soccer Log Plotter";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tbtnGeneratePltFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tbtnSetLogsDir;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tbtnReloadLogsDir;
        private System.Windows.Forms.ListBox lstAvailableLogs;
        private System.Windows.Forms.ListBox lstSelectedLogs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectFiles;
        private System.Windows.Forms.Button btnRemoveSelectedFiles;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox ttxtMvAvgWnd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox ttxtDataLength;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox ttxtPlotEvery;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem averageRewScoreDiffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scoreDelaysAndCumulativeRewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem eAPlotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meanOfSelectedLogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbtnSaveAverage;
        private System.Windows.Forms.ToolStripMenuItem bestAndMeanInOneToolStripMenuItem;
    }
}

