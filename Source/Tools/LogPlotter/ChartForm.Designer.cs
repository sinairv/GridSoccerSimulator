namespace GridSoccer.LogPlotter
{
    partial class ChartForm
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
            this.chartMain = new PlottingTools.ChartWrapper();
            this.SuspendLayout();
            // 
            // chartMain
            // 
            this.chartMain.AxisXTitle = "";
            this.chartMain.AxisYTitle = "";
            this.chartMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartMain.Location = new System.Drawing.Point(0, 0);
            this.chartMain.Name = "chartMain";
            this.chartMain.Size = new System.Drawing.Size(428, 309);
            this.chartMain.TabIndex = 0;
            this.chartMain.Title = "Chart Title";
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 309);
            this.Controls.Add(this.chartMain);
            this.Name = "ChartForm";
            this.Text = "Figure";
            this.ResumeLayout(false);

        }

        #endregion

        private PlottingTools.ChartWrapper chartMain;
    }
}