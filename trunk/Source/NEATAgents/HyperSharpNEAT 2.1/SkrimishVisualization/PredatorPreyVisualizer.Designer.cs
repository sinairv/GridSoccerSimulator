namespace SkirmishVisualization
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGenomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleMultiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadCCEAGenomesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.start100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.start100ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.play1000ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.play2000ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.play10000ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCCEANeatGenomesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "(*.xml)|*.xml";
            this.openFileDialog1.InitialDirectory = "..";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.evolutionToolStripMenuItem,
            this.start100ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(292, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadGenomeToolStripMenuItem,
            this.toggleMultiToolStripMenuItem,
            this.toolStripMenuItem1,
            this.loadCCEAGenomesToolStripMenuItem,
            this.loadCCEANeatGenomesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadGenomeToolStripMenuItem
            // 
            this.loadGenomeToolStripMenuItem.Name = "loadGenomeToolStripMenuItem";
            this.loadGenomeToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.loadGenomeToolStripMenuItem.Text = "Load Genome";
            this.loadGenomeToolStripMenuItem.Click += new System.EventHandler(this.loadGenomeToolStripMenuItem_Click);
            // 
            // toggleMultiToolStripMenuItem
            // 
            this.toggleMultiToolStripMenuItem.Checked = true;
            this.toggleMultiToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleMultiToolStripMenuItem.Name = "toggleMultiToolStripMenuItem";
            this.toggleMultiToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.toggleMultiToolStripMenuItem.Text = "Toggle Multi";
            this.toggleMultiToolStripMenuItem.Click += new System.EventHandler(this.toggleMultiToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(211, 6);
            // 
            // loadCCEAGenomesToolStripMenuItem
            // 
            this.loadCCEAGenomesToolStripMenuItem.Name = "loadCCEAGenomesToolStripMenuItem";
            this.loadCCEAGenomesToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.loadCCEAGenomesToolStripMenuItem.Text = "Load CCEA Genomes";
            this.loadCCEAGenomesToolStripMenuItem.Click += new System.EventHandler(this.loadCCEAGenomesToolStripMenuItem_Click);
            // 
            // evolutionToolStripMenuItem
            // 
            this.evolutionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem});
            this.evolutionToolStripMenuItem.Enabled = false;
            this.evolutionToolStripMenuItem.Name = "evolutionToolStripMenuItem";
            this.evolutionToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.evolutionToolStripMenuItem.Text = "Evolution";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startEvolutionToolStripMenuItem_Click);
            // 
            // start100ToolStripMenuItem
            // 
            this.start100ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.start100ToolStripMenuItem1,
            this.play1000ToolStripMenuItem,
            this.play2000ToolStripMenuItem,
            this.play10000ToolStripMenuItem});
            this.start100ToolStripMenuItem.Name = "start100ToolStripMenuItem";
            this.start100ToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.start100ToolStripMenuItem.Text = "Demo";
            // 
            // start100ToolStripMenuItem1
            // 
            this.start100ToolStripMenuItem1.Name = "start100ToolStripMenuItem1";
            this.start100ToolStripMenuItem1.Size = new System.Drawing.Size(129, 22);
            this.start100ToolStripMenuItem1.Text = "Play 100";
            this.start100ToolStripMenuItem1.Click += new System.EventHandler(this.start100ToolStripMenuItem1_Click);
            // 
            // play1000ToolStripMenuItem
            // 
            this.play1000ToolStripMenuItem.Name = "play1000ToolStripMenuItem";
            this.play1000ToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.play1000ToolStripMenuItem.Text = "Play 1000";
            this.play1000ToolStripMenuItem.Click += new System.EventHandler(this.play1000ToolStripMenuItem_Click);
            // 
            // play2000ToolStripMenuItem
            // 
            this.play2000ToolStripMenuItem.Name = "play2000ToolStripMenuItem";
            this.play2000ToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.play2000ToolStripMenuItem.Text = "Play 2000";
            this.play2000ToolStripMenuItem.Click += new System.EventHandler(this.play2000ToolStripMenuItem_Click);
            // 
            // play10000ToolStripMenuItem
            // 
            this.play10000ToolStripMenuItem.Name = "play10000ToolStripMenuItem";
            this.play10000ToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.play10000ToolStripMenuItem.Text = "Play 10000";
            this.play10000ToolStripMenuItem.Click += new System.EventHandler(this.play10000ToolStripMenuItem_Click);
            // 
            // loadCCEANeatGenomesToolStripMenuItem
            // 
            this.loadCCEANeatGenomesToolStripMenuItem.Name = "loadCCEANeatGenomesToolStripMenuItem";
            this.loadCCEANeatGenomesToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.loadCCEANeatGenomesToolStripMenuItem.Text = "Load CCEA Neat Genomes";
            this.loadCCEANeatGenomesToolStripMenuItem.Click += new System.EventHandler(this.loadCCEANeatGenomesToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Predator Prey Visualizer";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadGenomeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleMultiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem start100ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem start100ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem play1000ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem play2000ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem play10000ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadCCEAGenomesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCCEANeatGenomesToolStripMenuItem;
    }
}

