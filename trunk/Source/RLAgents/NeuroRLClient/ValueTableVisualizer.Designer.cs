namespace GridSoccer.DPClient
{
    partial class ValueTableVisualizer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValueTableVisualizer));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.meshPlotter1 = new PlottingTools.MeshPlotter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtOppRow = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.txtOppCol = new System.Windows.Forms.ToolStripTextBox();
            this.tbtnBOwner = new System.Windows.Forms.ToolStripButton();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.meshPlotter1);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(753, 507);
            this.splitContainer1.SplitterDistance = 478;
            this.splitContainer1.TabIndex = 0;
            // 
            // meshPlotter1
            // 
            this.meshPlotter1.BackColor = System.Drawing.Color.White;
            this.meshPlotter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meshPlotter1.IsWired = false;
            this.meshPlotter1.Location = new System.Drawing.Point(0, 25);
            this.meshPlotter1.Name = "meshPlotter1";
            this.meshPlotter1.Size = new System.Drawing.Size(478, 482);
            this.meshPlotter1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.txtOppRow,
            this.toolStripLabel2,
            this.txtOppCol,
            this.tbtnBOwner});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(478, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel1.Text = "OppRow:";
            // 
            // txtOppRow
            // 
            this.txtOppRow.Name = "txtOppRow";
            this.txtOppRow.Size = new System.Drawing.Size(50, 25);
            this.txtOppRow.Text = "1";
            this.txtOppRow.Leave += new System.EventHandler(this.txtOppRow_Leave);
            this.txtOppRow.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOppRow_KeyDown);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(51, 22);
            this.toolStripLabel2.Text = "OppCol:";
            // 
            // txtOppCol
            // 
            this.txtOppCol.Name = "txtOppCol";
            this.txtOppCol.Size = new System.Drawing.Size(50, 25);
            this.txtOppCol.Text = "1";
            this.txtOppCol.Leave += new System.EventHandler(this.txtOppRow_Leave);
            this.txtOppCol.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOppRow_KeyDown);
            // 
            // tbtnBOwner
            // 
            this.tbtnBOwner.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnBOwner.Image = ((System.Drawing.Image)(resources.GetObject("tbtnBOwner.Image")));
            this.tbtnBOwner.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnBOwner.Name = "tbtnBOwner";
            this.tbtnBOwner.Size = new System.Drawing.Size(23, 22);
            this.tbtnBOwner.Text = "Am I Ball Owner?";
            this.tbtnBOwner.Click += new System.EventHandler(this.tbtnBOwner_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(271, 507);
            this.propertyGrid1.TabIndex = 0;
            // 
            // ValueTableVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 507);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ValueTableVisualizer";
            this.Text = "Value-table visualizer - You need to close this window before starting the game";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtOppRow;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox txtOppCol;
        private System.Windows.Forms.ToolStripButton tbtnBOwner;
        private PlottingTools.MeshPlotter meshPlotter1;
    }
}