namespace GridSoccer.Simulator
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
            this.propController = new System.Windows.Forms.PropertyGrid();
            this.panelControlsContainer = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tbtnStartResume = new System.Windows.Forms.ToolStripButton();
            this.tbtnPause = new System.Windows.Forms.ToolStripButton();
            this.tbtnStep = new System.Windows.Forms.ToolStripButton();
            this.tbtnStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnTurbo = new System.Windows.Forms.ToolStripButton();
            this.tbtnNormalSpeed = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnBindMonitor = new System.Windows.Forms.ToolStripButton();
            this.tbtnUnbindMonitor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnSettings = new System.Windows.Forms.ToolStripButton();
            this.tbtnAbout = new System.Windows.Forms.ToolStripButton();
            this.soccerMonitor = new GridSoccer.Simulator.Monitor.SoccerMonitor();
            this.panelControlsContainer.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // propController
            // 
            this.propController.Dock = System.Windows.Forms.DockStyle.Right;
            this.propController.Location = new System.Drawing.Point(602, 0);
            this.propController.Name = "propController";
            this.propController.Size = new System.Drawing.Size(238, 495);
            this.propController.TabIndex = 4;
            // 
            // panelControlsContainer
            // 
            this.panelControlsContainer.Controls.Add(this.soccerMonitor);
            this.panelControlsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlsContainer.Location = new System.Drawing.Point(0, 55);
            this.panelControlsContainer.Name = "panelControlsContainer";
            this.panelControlsContainer.Size = new System.Drawing.Size(599, 440);
            this.panelControlsContainer.TabIndex = 5;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(599, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 495);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // toolStripMain
            // 
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnStartResume,
            this.tbtnPause,
            this.tbtnStep,
            this.tbtnStop,
            this.toolStripSeparator2,
            this.tbtnTurbo,
            this.tbtnNormalSpeed,
            this.toolStripSeparator1,
            this.tbtnBindMonitor,
            this.tbtnUnbindMonitor,
            this.toolStripSeparator3,
            this.tbtnSettings,
            this.tbtnAbout,
            });
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(599, 55);
            this.toolStripMain.TabIndex = 7;
            this.toolStripMain.Text = "Main Functions";
            // 
            // tbtnStartResume
            // 
            this.tbtnStartResume.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnStartResume.Image = ((System.Drawing.Image)(resources.GetObject("tbtnStartResume.Image")));
            this.tbtnStartResume.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnStartResume.Name = "tbtnStartResume";
            this.tbtnStartResume.Size = new System.Drawing.Size(52, 52);
            this.tbtnStartResume.Text = "Start or Resume Game";
            this.tbtnStartResume.ToolTipText = "Start or Resume Game";
            this.tbtnStartResume.Click += new System.EventHandler(this.tbtnStartResume_Click);
            // 
            // tbtnPause
            // 
            this.tbtnPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnPause.Image = ((System.Drawing.Image)(resources.GetObject("tbtnPause.Image")));
            this.tbtnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnPause.Name = "tbtnPause";
            this.tbtnPause.Size = new System.Drawing.Size(52, 52);
            this.tbtnPause.Text = "Pause";
            this.tbtnPause.Click += new System.EventHandler(this.tbtnPause_Click);
            // 
            // tbtnStep
            // 
            this.tbtnStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnStep.Image = ((System.Drawing.Image)(resources.GetObject("tbtnStep.Image")));
            this.tbtnStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnStep.Name = "tbtnStep";
            this.tbtnStep.Size = new System.Drawing.Size(52, 52);
            this.tbtnStep.Text = "Step";
            this.tbtnStep.Click += new System.EventHandler(this.tbtnStep_Click);
            // 
            // tbtnStop
            // 
            this.tbtnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnStop.Image = ((System.Drawing.Image)(resources.GetObject("tbtnStop.Image")));
            this.tbtnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnStop.Name = "tbtnStop";
            this.tbtnStop.Size = new System.Drawing.Size(52, 52);
            this.tbtnStop.Text = "Stop";
            this.tbtnStop.Click += new System.EventHandler(this.tbtnStop_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 55);
            // 
            // tbtnTurbo
            // 
            this.tbtnTurbo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnTurbo.Image = ((System.Drawing.Image)(resources.GetObject("tbtnTurbo.Image")));
            this.tbtnTurbo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnTurbo.Name = "tbtnTurbo";
            this.tbtnTurbo.Size = new System.Drawing.Size(52, 52);
            this.tbtnTurbo.Text = "Enable Turbo";
            this.tbtnTurbo.ToolTipText = "Enable Turbo (no delay between cycles)";
            this.tbtnTurbo.Click += new System.EventHandler(this.tbtnTurbo_Click);
            // 
            // tbtnNormalSpeed
            // 
            this.tbtnNormalSpeed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnNormalSpeed.Image = ((System.Drawing.Image)(resources.GetObject("tbtnNormalSpeed.Image")));
            this.tbtnNormalSpeed.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnNormalSpeed.Name = "tbtnNormalSpeed";
            this.tbtnNormalSpeed.Size = new System.Drawing.Size(52, 52);
            this.tbtnNormalSpeed.Text = "Disable Turbo";
            this.tbtnNormalSpeed.Click += new System.EventHandler(this.tbtnNormalSpeed_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 55);
            // 
            // tbtnBindMonitor
            // 
            this.tbtnBindMonitor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnBindMonitor.Image = ((System.Drawing.Image)(resources.GetObject("tbtnBindMonitor.Image")));
            this.tbtnBindMonitor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnBindMonitor.Name = "tbtnBindMonitor";
            this.tbtnBindMonitor.Size = new System.Drawing.Size(52, 52);
            this.tbtnBindMonitor.Text = "Monitor is Turned Off (may improve simulation performance, especially in turbo mo" +
                "de)";
            this.tbtnBindMonitor.Click += new System.EventHandler(this.tbtnBindMonitor_Click);
            // 
            // tbtnUnbindMonitor
            // 
            this.tbtnUnbindMonitor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnUnbindMonitor.Image = ((System.Drawing.Image)(resources.GetObject("tbtnUnbindMonitor.Image")));
            this.tbtnUnbindMonitor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnUnbindMonitor.Name = "tbtnUnbindMonitor";
            this.tbtnUnbindMonitor.Size = new System.Drawing.Size(52, 52);
            this.tbtnUnbindMonitor.Text = "Monitor is Turned On (may decrease simulation performance, especially in turbo mo" +
                "de)";
            this.tbtnUnbindMonitor.Click += new System.EventHandler(this.tbtnUnbindMonitor_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 55);
            // 
            // tbtnSettings
            // 
            this.tbtnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnSettings.Image = ((System.Drawing.Image)(resources.GetObject("tbtnSettings.Image")));
            this.tbtnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnSettings.Name = "tbtnSettings";
            this.tbtnSettings.Size = new System.Drawing.Size(52, 52);
            this.tbtnSettings.Text = "Settings";
            this.tbtnSettings.Click += new System.EventHandler(this.tbtnSettings_Click);
            // 
            // tbtnAbout
            // 
            this.tbtnAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnAbout.Image = ((System.Drawing.Image)(resources.GetObject("tbtnAbout.Image")));
            this.tbtnAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnAbout.Name = "tbtnAbout";
            this.tbtnAbout.Size = new System.Drawing.Size(52, 52);
            this.tbtnAbout.Text = "About Grid-Soccer Simulator";
            this.tbtnAbout.Click += new System.EventHandler(this.tbtnAbout_Click);
            // 
            // soccerMonitor
            // 
            this.soccerMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.soccerMonitor.IntervalUpdateUI = 500;
            this.soccerMonitor.Location = new System.Drawing.Point(3, 3);
            this.soccerMonitor.Name = "soccerMonitor";
            this.soccerMonitor.Size = new System.Drawing.Size(590, 434);
            this.soccerMonitor.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 495);
            this.Controls.Add(this.panelControlsContainer);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.propController);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "Grid-Soccer Simulator";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.panelControlsContainer.ResumeLayout(false);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GridSoccer.Simulator.Monitor.SoccerMonitor soccerMonitor;
        private System.Windows.Forms.PropertyGrid propController;
        private System.Windows.Forms.Panel panelControlsContainer;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton tbtnStartResume;
        private System.Windows.Forms.ToolStripButton tbtnStep;
        private System.Windows.Forms.ToolStripButton tbtnStop;
        private System.Windows.Forms.ToolStripButton tbtnTurbo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbtnBindMonitor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tbtnPause;
        private System.Windows.Forms.ToolStripButton tbtnNormalSpeed;
        private System.Windows.Forms.ToolStripButton tbtnUnbindMonitor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tbtnSettings;
        private System.Windows.Forms.ToolStripButton tbtnAbout;

    }
}

