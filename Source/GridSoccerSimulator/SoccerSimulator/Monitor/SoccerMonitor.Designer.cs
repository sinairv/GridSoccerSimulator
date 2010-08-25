namespace GridSoccer.Simulator.Monitor
{
    partial class SoccerMonitor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelControl = new System.Windows.Forms.Panel();
            this.lblCycle = new System.Windows.Forms.Label();
            this.lblRightTeamName = new System.Windows.Forms.Label();
            this.lblLeftTeamName = new System.Windows.Forms.Label();
            this.soccerField = new GridSoccer.Simulator.Monitor.SoccerField();
            this.panelControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl
            // 
            this.panelControl.Controls.Add(this.lblCycle);
            this.panelControl.Controls.Add(this.lblRightTeamName);
            this.panelControl.Controls.Add(this.lblLeftTeamName);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl.Location = new System.Drawing.Point(0, 0);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(683, 40);
            this.panelControl.TabIndex = 1;
            // 
            // lblCycle
            // 
            this.lblCycle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCycle.BackColor = System.Drawing.Color.White;
            this.lblCycle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCycle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCycle.ForeColor = System.Drawing.Color.Black;
            this.lblCycle.Location = new System.Drawing.Point(240, 0);
            this.lblCycle.Name = "lblCycle";
            this.lblCycle.Size = new System.Drawing.Size(203, 40);
            this.lblCycle.TabIndex = 2;
            this.lblCycle.Text = "0";
            this.lblCycle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRightTeamName
            // 
            this.lblRightTeamName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblRightTeamName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRightTeamName.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblRightTeamName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRightTeamName.ForeColor = System.Drawing.Color.Blue;
            this.lblRightTeamName.Location = new System.Drawing.Point(443, 0);
            this.lblRightTeamName.Name = "lblRightTeamName";
            this.lblRightTeamName.Size = new System.Drawing.Size(240, 40);
            this.lblRightTeamName.TabIndex = 1;
            this.lblRightTeamName.Text = "[Right]";
            this.lblRightTeamName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLeftTeamName
            // 
            this.lblLeftTeamName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblLeftTeamName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLeftTeamName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblLeftTeamName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeftTeamName.ForeColor = System.Drawing.Color.Red;
            this.lblLeftTeamName.Location = new System.Drawing.Point(0, 0);
            this.lblLeftTeamName.Name = "lblLeftTeamName";
            this.lblLeftTeamName.Size = new System.Drawing.Size(240, 40);
            this.lblLeftTeamName.TabIndex = 0;
            this.lblLeftTeamName.Text = "[Left]";
            this.lblLeftTeamName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // soccerField
            // 
            this.soccerField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.soccerField.Location = new System.Drawing.Point(0, 40);
            this.soccerField.MinimumSize = new System.Drawing.Size(30, 30);
            this.soccerField.Name = "soccerField";
            this.soccerField.Size = new System.Drawing.Size(683, 463);
            this.soccerField.TabIndex = 0;
            // 
            // SoccerMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.soccerField);
            this.Controls.Add(this.panelControl);
            this.Name = "SoccerMonitor";
            this.Size = new System.Drawing.Size(683, 503);
            this.Resize += new System.EventHandler(this.SoccerMonitor_Resize);
            this.panelControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SoccerField soccerField;
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.Label lblCycle;
        private System.Windows.Forms.Label lblRightTeamName;
        private System.Windows.Forms.Label lblLeftTeamName;
    }
}
