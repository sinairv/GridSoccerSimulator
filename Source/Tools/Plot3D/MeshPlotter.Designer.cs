namespace PlottingTools
{
    partial class MeshPlotter
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
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            this.contextMenuStripMain.Size = new System.Drawing.Size(125, 26);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save as ...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // MeshPlotter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStripMain;
            this.Name = "MeshPlotter";
            this.Size = new System.Drawing.Size(324, 282);
            this.MouseLeave += new System.EventHandler(this.MeshPlotter_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MeshPlotter_MouseMove);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MeshPlotter_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MeshPlotter_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MeshPlotter_MouseUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MeshPlotter_KeyDown);
            this.contextMenuStripMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripMain;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
    }
}
