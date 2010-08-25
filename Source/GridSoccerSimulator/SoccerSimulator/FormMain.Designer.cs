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
            this.btnStart = new System.Windows.Forms.Button();
            this.propController = new System.Windows.Forms.PropertyGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUnbindMonitor = new System.Windows.Forms.Button();
            this.btnBindMonitor = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btnGoJet = new System.Windows.Forms.Button();
            this.soccerMonitor1 = new GridSoccer.Simulator.Monitor.SoccerMonitor();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(3, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // propController
            // 
            this.propController.Dock = System.Windows.Forms.DockStyle.Right;
            this.propController.Location = new System.Drawing.Point(602, 0);
            this.propController.Name = "propController";
            this.propController.Size = new System.Drawing.Size(238, 495);
            this.propController.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnGoJet);
            this.panel1.Controls.Add(this.btnUnbindMonitor);
            this.panel1.Controls.Add(this.btnBindMonitor);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Controls.Add(this.btnStep);
            this.panel1.Controls.Add(this.btnPause);
            this.panel1.Controls.Add(this.soccerMonitor1);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(599, 495);
            this.panel1.TabIndex = 5;
            // 
            // btnUnbindMonitor
            // 
            this.btnUnbindMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnbindMonitor.Location = new System.Drawing.Point(518, 3);
            this.btnUnbindMonitor.Name = "btnUnbindMonitor";
            this.btnUnbindMonitor.Size = new System.Drawing.Size(75, 23);
            this.btnUnbindMonitor.TabIndex = 8;
            this.btnUnbindMonitor.Text = "Unbind";
            this.btnUnbindMonitor.UseVisualStyleBackColor = true;
            this.btnUnbindMonitor.Click += new System.EventHandler(this.btnUnbindMonitor_Click);
            // 
            // btnBindMonitor
            // 
            this.btnBindMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBindMonitor.Enabled = false;
            this.btnBindMonitor.Location = new System.Drawing.Point(437, 3);
            this.btnBindMonitor.Name = "btnBindMonitor";
            this.btnBindMonitor.Size = new System.Drawing.Size(75, 23);
            this.btnBindMonitor.TabIndex = 7;
            this.btnBindMonitor.Text = "Bind";
            this.btnBindMonitor.UseVisualStyleBackColor = true;
            this.btnBindMonitor.Click += new System.EventHandler(this.btnBindMonitor_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(246, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStep
            // 
            this.btnStep.Enabled = false;
            this.btnStep.Location = new System.Drawing.Point(165, 3);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 5;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(84, 3);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 4;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
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
            // btnGoJet
            // 
            this.btnGoJet.BackColor = System.Drawing.Color.Red;
            this.btnGoJet.Location = new System.Drawing.Point(327, 3);
            this.btnGoJet.Name = "btnGoJet";
            this.btnGoJet.Size = new System.Drawing.Size(75, 23);
            this.btnGoJet.TabIndex = 9;
            this.btnGoJet.Text = "Go Jet";
            this.btnGoJet.UseVisualStyleBackColor = false;
            this.btnGoJet.Click += new System.EventHandler(this.btnGoJet_Click);
            // 
            // soccerMonitor1
            // 
            this.soccerMonitor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.soccerMonitor1.Location = new System.Drawing.Point(3, 32);
            this.soccerMonitor1.Name = "soccerMonitor1";
            this.soccerMonitor1.Size = new System.Drawing.Size(590, 460);
            this.soccerMonitor1.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 495);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.propController);
            this.Name = "FormMain";
            this.Text = "Grid-Soccer Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GridSoccer.Simulator.Monitor.SoccerMonitor soccerMonitor1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PropertyGrid propController;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnUnbindMonitor;
        private System.Windows.Forms.Button btnBindMonitor;
        private System.Windows.Forms.Button btnGoJet;

    }
}

