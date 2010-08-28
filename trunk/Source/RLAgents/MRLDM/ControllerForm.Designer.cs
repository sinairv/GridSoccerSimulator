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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(6, 51);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 22);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPerformDM
            // 
            this.btnPerformDM.Location = new System.Drawing.Point(20, 120);
            this.btnPerformDM.Name = "btnPerformDM";
            this.btnPerformDM.Size = new System.Drawing.Size(126, 23);
            this.btnPerformDM.TabIndex = 5;
            this.btnPerformDM.Text = "Perform DM Once";
            this.btnPerformDM.UseVisualStyleBackColor = true;
            this.btnPerformDM.Click += new System.EventHandler(this.btnPerformDM_Click);
            // 
            // txtK
            // 
            this.txtK.Location = new System.Drawing.Point(69, 45);
            this.txtK.Name = "txtK";
            this.txtK.Size = new System.Drawing.Size(77, 20);
            this.txtK.TabIndex = 6;
            this.txtK.Text = "1";
            this.txtK.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtK_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "K";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "MinSupp";
            // 
            // txtMinSupp
            // 
            this.txtMinSupp.Location = new System.Drawing.Point(69, 68);
            this.txtMinSupp.Name = "txtMinSupp";
            this.txtMinSupp.Size = new System.Drawing.Size(77, 20);
            this.txtMinSupp.TabIndex = 8;
            this.txtMinSupp.Text = "2000";
            this.txtMinSupp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMinSupp_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "MinConf";
            // 
            // txtMinConf
            // 
            this.txtMinConf.Location = new System.Drawing.Point(69, 94);
            this.txtMinConf.Name = "txtMinConf";
            this.txtMinConf.Size = new System.Drawing.Size(77, 20);
            this.txtMinConf.TabIndex = 10;
            this.txtMinConf.Text = "0.0";
            this.txtMinConf.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMinConf_KeyDown);
            // 
            // btnShowStats
            // 
            this.btnShowStats.Location = new System.Drawing.Point(6, 101);
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
            this.cbIsOnline.Location = new System.Drawing.Point(91, 26);
            this.cbIsOnline.Name = "cbIsOnline";
            this.cbIsOnline.Size = new System.Drawing.Size(97, 17);
            this.cbIsOnline.TabIndex = 13;
            this.cbIsOnline.Text = "Is Online Mode";
            this.cbIsOnline.UseVisualStyleBackColor = true;
            this.cbIsOnline.CheckedChanged += new System.EventHandler(this.cbIsOnline_CheckedChanged);
            // 
            // btnBatch
            // 
            this.btnBatch.Location = new System.Drawing.Point(6, 23);
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
            this.comboMethods.Location = new System.Drawing.Point(20, 19);
            this.comboMethods.Name = "comboMethods";
            this.comboMethods.Size = new System.Drawing.Size(126, 21);
            this.comboMethods.TabIndex = 16;
            this.comboMethods.SelectedIndexChanged += new System.EventHandler(this.comboMethods_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.btnShowStats);
            this.groupBox1.Controls.Add(this.cbIsOnline);
            this.groupBox1.Location = new System.Drawing.Point(3, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 176);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboMethods);
            this.groupBox2.Controls.Add(this.btnPerformDM);
            this.groupBox2.Controls.Add(this.txtK);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtMinSupp);
            this.groupBox2.Controls.Add(this.txtMinConf);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(226, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 176);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnBatch);
            this.groupBox3.Location = new System.Drawing.Point(432, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(151, 176);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            // 
            // ControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 189);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ControllerForm";
            this.Text = "ControllerForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPerformDM;
        private System.Windows.Forms.TextBox txtK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMinSupp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMinConf;
        private System.Windows.Forms.Button btnShowStats;
        private System.Windows.Forms.CheckBox cbIsOnline;
        private System.Windows.Forms.Button btnBatch;
        private System.Windows.Forms.ComboBox comboMethods;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}