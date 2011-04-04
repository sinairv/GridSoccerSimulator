namespace NNetTest01
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.btnDo = new System.Windows.Forms.Button();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnTestMine = new System.Windows.Forms.Button();
            this.btnTestRpropBatch = new System.Windows.Forms.Button();
            this.btnIncRProp = new System.Windows.Forms.Button();
            this.btnIncBPROP = new System.Windows.Forms.Button();
            this.btnIncAForge = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDo
            // 
            this.btnDo.Location = new System.Drawing.Point(12, 12);
            this.btnDo.Name = "btnDo";
            this.btnDo.Size = new System.Drawing.Size(114, 26);
            this.btnDo.TabIndex = 0;
            this.btnDo.Text = "Batch AForge";
            this.btnDo.UseVisualStyleBackColor = true;
            this.btnDo.Click += new System.EventHandler(this.btnTestAForge_Click);
            // 
            // rtbOutput
            // 
            this.rtbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.rtbOutput.Location = new System.Drawing.Point(12, 74);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.Size = new System.Drawing.Size(211, 223);
            this.rtbOutput.TabIndex = 1;
            this.rtbOutput.Text = "";
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(229, 74);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "Legend1";
            series1.Name = "Error";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(618, 223);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            // 
            // btnTestMine
            // 
            this.btnTestMine.Location = new System.Drawing.Point(132, 12);
            this.btnTestMine.Name = "btnTestMine";
            this.btnTestMine.Size = new System.Drawing.Size(114, 26);
            this.btnTestMine.TabIndex = 3;
            this.btnTestMine.Text = "Batch BPROP";
            this.btnTestMine.UseVisualStyleBackColor = true;
            this.btnTestMine.Click += new System.EventHandler(this.btnBatchBProp_Click);
            // 
            // btnTestRpropBatch
            // 
            this.btnTestRpropBatch.Location = new System.Drawing.Point(252, 12);
            this.btnTestRpropBatch.Name = "btnTestRpropBatch";
            this.btnTestRpropBatch.Size = new System.Drawing.Size(114, 26);
            this.btnTestRpropBatch.TabIndex = 4;
            this.btnTestRpropBatch.Text = "Batch RPROP";
            this.btnTestRpropBatch.UseVisualStyleBackColor = true;
            this.btnTestRpropBatch.Click += new System.EventHandler(this.btnBatchRprop_Click);
            // 
            // btnIncRProp
            // 
            this.btnIncRProp.Location = new System.Drawing.Point(252, 44);
            this.btnIncRProp.Name = "btnIncRProp";
            this.btnIncRProp.Size = new System.Drawing.Size(114, 26);
            this.btnIncRProp.TabIndex = 7;
            this.btnIncRProp.Text = "Inc RPROP";
            this.btnIncRProp.UseVisualStyleBackColor = true;
            this.btnIncRProp.Click += new System.EventHandler(this.btnIncRProp_Click);
            // 
            // btnIncBPROP
            // 
            this.btnIncBPROP.Location = new System.Drawing.Point(132, 44);
            this.btnIncBPROP.Name = "btnIncBPROP";
            this.btnIncBPROP.Size = new System.Drawing.Size(114, 26);
            this.btnIncBPROP.TabIndex = 6;
            this.btnIncBPROP.Text = "Inc BPROP";
            this.btnIncBPROP.UseVisualStyleBackColor = true;
            this.btnIncBPROP.Click += new System.EventHandler(this.btnIncBPROP_Click);
            // 
            // btnIncAForge
            // 
            this.btnIncAForge.Location = new System.Drawing.Point(12, 44);
            this.btnIncAForge.Name = "btnIncAForge";
            this.btnIncAForge.Size = new System.Drawing.Size(114, 26);
            this.btnIncAForge.TabIndex = 5;
            this.btnIncAForge.Text = "Inc AForge";
            this.btnIncAForge.UseVisualStyleBackColor = true;
            this.btnIncAForge.Click += new System.EventHandler(this.btnIncAForge_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(446, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "2, 1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 309);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnIncRProp);
            this.Controls.Add(this.btnIncBPROP);
            this.Controls.Add(this.btnIncAForge);
            this.Controls.Add(this.btnTestRpropBatch);
            this.Controls.Add(this.btnTestMine);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.btnDo);
            this.Name = "Form1";
            this.Text = "Neural Network Test Application";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDo;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button btnTestMine;
        private System.Windows.Forms.Button btnTestRpropBatch;
        private System.Windows.Forms.Button btnIncRProp;
        private System.Windows.Forms.Button btnIncBPROP;
        private System.Windows.Forms.Button btnIncAForge;
        private System.Windows.Forms.TextBox textBox1;


    }
}

