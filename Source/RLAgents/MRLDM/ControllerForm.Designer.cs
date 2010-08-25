namespace MRLDM
{
    partial class ControllerForm
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
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.btnPerformDM = new System.Windows.Forms.Button();
            this.txtK = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMinSupp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMinConf = new System.Windows.Forms.TextBox();
            this.btnShowStats = new System.Windows.Forms.Button();
            this.cbIsOnline = new System.Windows.Forms.CheckBox();
            this.btnBatch = new System.Windows.Forms.Button();
            this.comboMethods = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 44);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 15);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape2,
            this.lineShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(574, 145);
            this.shapeContainer1.TabIndex = 4;
            this.shapeContainer1.TabStop = false;
            // 
            // lineShape2
            // 
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.X1 = 376;
            this.lineShape2.X2 = 376;
            this.lineShape2.Y1 = 6;
            this.lineShape2.Y2 = 131;
            // 
            // lineShape1
            // 
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 232;
            this.lineShape1.X2 = 232;
            this.lineShape1.Y1 = 5;
            this.lineShape1.Y2 = 130;
            // 
            // btnPerformDM
            // 
            this.btnPerformDM.Location = new System.Drawing.Point(244, 108);
            this.btnPerformDM.Name = "btnPerformDM";
            this.btnPerformDM.Size = new System.Drawing.Size(126, 23);
            this.btnPerformDM.TabIndex = 5;
            this.btnPerformDM.Text = "Perform DM Once";
            this.btnPerformDM.UseVisualStyleBackColor = true;
            this.btnPerformDM.Click += new System.EventHandler(this.btnPerformDM_Click);
            // 
            // txtK
            // 
            this.txtK.Location = new System.Drawing.Point(293, 31);
            this.txtK.Name = "txtK";
            this.txtK.Size = new System.Drawing.Size(77, 20);
            this.txtK.TabIndex = 6;
            this.txtK.Text = "1";
            this.txtK.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtK_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(241, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "K";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "MinSupp";
            // 
            // txtMinSupp
            // 
            this.txtMinSupp.Location = new System.Drawing.Point(293, 54);
            this.txtMinSupp.Name = "txtMinSupp";
            this.txtMinSupp.Size = new System.Drawing.Size(77, 20);
            this.txtMinSupp.TabIndex = 8;
            this.txtMinSupp.Text = "2000";
            this.txtMinSupp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMinSupp_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(241, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "MinConf";
            // 
            // txtMinConf
            // 
            this.txtMinConf.Location = new System.Drawing.Point(293, 80);
            this.txtMinConf.Name = "txtMinConf";
            this.txtMinConf.Size = new System.Drawing.Size(77, 20);
            this.txtMinConf.TabIndex = 10;
            this.txtMinConf.Text = "0.0";
            this.txtMinConf.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMinConf_KeyDown);
            // 
            // btnShowStats
            // 
            this.btnShowStats.Location = new System.Drawing.Point(12, 99);
            this.btnShowStats.Name = "btnShowStats";
            this.btnShowStats.Size = new System.Drawing.Size(75, 23);
            this.btnShowStats.TabIndex = 12;
            this.btnShowStats.Text = "Show Stats";
            this.btnShowStats.UseVisualStyleBackColor = true;
            this.btnShowStats.Click += new System.EventHandler(this.btnShowStats_Click);
            // 
            // cbIsOnline
            // 
            this.cbIsOnline.AutoSize = true;
            this.cbIsOnline.Location = new System.Drawing.Point(97, 19);
            this.cbIsOnline.Name = "cbIsOnline";
            this.cbIsOnline.Size = new System.Drawing.Size(97, 17);
            this.cbIsOnline.TabIndex = 13;
            this.cbIsOnline.Text = "Is Online Mode";
            this.cbIsOnline.UseVisualStyleBackColor = true;
            this.cbIsOnline.CheckedChanged += new System.EventHandler(this.cbIsOnline_CheckedChanged);
            // 
            // btnBatch
            // 
            this.btnBatch.Location = new System.Drawing.Point(402, 108);
            this.btnBatch.Name = "btnBatch";
            this.btnBatch.Size = new System.Drawing.Size(126, 23);
            this.btnBatch.TabIndex = 15;
            this.btnBatch.Text = "Perform DM Batch";
            this.btnBatch.UseVisualStyleBackColor = true;
            this.btnBatch.Click += new System.EventHandler(this.btnBatch_Click);
            // 
            // comboMethods
            // 
            this.comboMethods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMethods.FormattingEnabled = true;
            this.comboMethods.Location = new System.Drawing.Point(244, 5);
            this.comboMethods.Name = "comboMethods";
            this.comboMethods.Size = new System.Drawing.Size(126, 21);
            this.comboMethods.TabIndex = 16;
            this.comboMethods.SelectedIndexChanged += new System.EventHandler(this.comboMethods_SelectedIndexChanged);
            // 
            // ControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 145);
            this.Controls.Add(this.comboMethods);
            this.Controls.Add(this.btnBatch);
            this.Controls.Add(this.cbIsOnline);
            this.Controls.Add(this.btnShowStats);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMinConf);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMinSupp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtK);
            this.Controls.Add(this.btnPerformDM);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.shapeContainer1);
            this.Name = "ControllerForm";
            this.Text = "ControllerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private System.Windows.Forms.Button btnPerformDM;
        private System.Windows.Forms.TextBox txtK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMinSupp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMinConf;
        private System.Windows.Forms.Button btnShowStats;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        private System.Windows.Forms.CheckBox cbIsOnline;
        private System.Windows.Forms.Button btnBatch;
        private System.Windows.Forms.ComboBox comboMethods;
    }
}