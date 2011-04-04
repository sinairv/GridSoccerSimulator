namespace PlottingTools
{
    partial class ChartWrapper
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.mainChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStripChart = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLegendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTitle = new System.Windows.Forms.Label();
            this.legendPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.contextMenuStripLegendPanel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.mainChart)).BeginInit();
            this.contextMenuStripChart.SuspendLayout();
            this.contextMenuStripLegendPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainChart
            // 
            chartArea1.Name = "ChartArea1";
            this.mainChart.ChartAreas.Add(chartArea1);
            this.mainChart.ContextMenuStrip = this.contextMenuStripChart;
            this.mainChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.mainChart.Legends.Add(legend1);
            this.mainChart.Location = new System.Drawing.Point(0, 23);
            this.mainChart.Name = "mainChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.mainChart.Series.Add(series1);
            this.mainChart.Size = new System.Drawing.Size(427, 356);
            this.mainChart.TabIndex = 0;
            // 
            // contextMenuStripChart
            // 
            this.contextMenuStripChart.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showLegendToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveToolStripMenuItem});
            this.contextMenuStripChart.Name = "contextMenuStrip1";
            this.contextMenuStripChart.Size = new System.Drawing.Size(186, 76);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveToolStripMenuItem.Text = "Save Chart as";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // showLegendToolStripMenuItem
            // 
            this.showLegendToolStripMenuItem.Name = "showLegendToolStripMenuItem";
            this.showLegendToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.showLegendToolStripMenuItem.Text = "Toggle Show Legend";
            this.showLegendToolStripMenuItem.Click += new System.EventHandler(this.showLegendToolStripMenuItem_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.White;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(427, 23);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Chart Title";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // legendPanel
            // 
            this.legendPanel.AutoScroll = true;
            this.legendPanel.AutoSize = true;
            this.legendPanel.BackColor = System.Drawing.Color.White;
            this.legendPanel.ContextMenuStrip = this.contextMenuStripLegendPanel;
            this.legendPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.legendPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.legendPanel.Location = new System.Drawing.Point(0, 0);
            this.legendPanel.Name = "legendPanel";
            this.legendPanel.Size = new System.Drawing.Size(0, 379);
            this.legendPanel.TabIndex = 2;
            this.legendPanel.WrapContents = false;
            // 
            // contextMenuStripLegendPanel
            // 
            this.contextMenuStripLegendPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem});
            this.contextMenuStripLegendPanel.Name = "contextMenuStripLegendPanel";
            this.contextMenuStripLegendPanel.Size = new System.Drawing.Size(138, 48);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.selectNoneToolStripMenuItem.Text = "Select None";
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(182, 6);
            // 
            // ChartWrapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainChart);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.legendPanel);
            this.Name = "ChartWrapper";
            this.Size = new System.Drawing.Size(427, 379);
            ((System.ComponentModel.ISupportInitialize)(this.mainChart)).EndInit();
            this.contextMenuStripChart.ResumeLayout(false);
            this.contextMenuStripLegendPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart mainChart;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripChart;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel legendPanel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLegendPanel;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLegendToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}
