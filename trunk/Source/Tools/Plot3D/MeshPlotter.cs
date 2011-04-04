using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace PlottingTools
{
    public partial class MeshPlotter : UserControl
    {
        private Surface3DRenderer sr;
        private PlotterView m_plotterView;

        public MeshPlotter()
        {
            InitializeComponent();
            this.BackColor = Color.White;

            m_plotterView = new PlotterView(this);

            sr = new Surface3DRenderer(
                m_plotterView.ObservatorX, m_plotterView.ObservatorY,
                m_plotterView.ObservatorZ, m_plotterView.ScreenX, m_plotterView.ScreenY,
                ClientRectangle.Width, ClientRectangle.Height, 0.5, 0, 0);

            sr.ColorSchema = new ColorSchema(10);

            //sr.SetFunction("sin(x1)*cos(x2)/(sqrt(sqrt(x1*x1+x2*x2))+1)*10");
            this.OnResize(EventArgs.Empty);
            ResizeRedraw = true;
            DoubleBuffered = true;
        }

        public void SetMesh(double[,] mesh)
        {
            sr.SetMesh(mesh);
        }

        public void SetFunction(string function)
        {
            sr.SetFunction(function);
        }

        public Surface3DRenderer SurfaceRenderer
        {
            get { return sr; }
        }

        public PlotterView PlotterView
        {
            get
            {
                return m_plotterView;
            }
        }

        public ColorSchema ColorSchema
        {
            get
            {
                return sr.ColorSchema;
            }

            set
            {
                sr.ColorSchema = value;
                Invalidate();
            }
        }

        public bool IsWired
        {
            get
            {
                return sr.IsWired;
            }

            set
            {
                sr.IsWired = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(BackColor);
            sr.RenderSurface(e.Graphics);
        }

        public void ResizeView()
        {
            this.OnResize(EventArgs.Empty);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (sr != null)
            {
                sr.ReCalculateTransformationsCoeficients(
                    m_plotterView.ObservatorX, m_plotterView.ObservatorY,
                    m_plotterView.ObservatorZ, m_plotterView.ScreenX, m_plotterView.ScreenY,
                    ClientRectangle.Width, ClientRectangle.Height, 0.5, 0, 0);
            }
        }

        bool isDragging = false;

        private void MeshPlotter_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_keyDown == Keys.Menu || m_keyDown == Keys.ShiftKey || m_keyDown == Keys.ControlKey)
            {
                isDragging = true;
            }
            else
            {
                isDragging = false;
            }
        }

        private void MeshPlotter_MouseLeave(object sender, EventArgs e)
        {
            isDragging = false;
        }

        private int m_prevX = Int32.MinValue;
        private int m_prevY = Int32.MinValue;
        private void MeshPlotter_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltax = 0, deltay = 0;
                if (m_prevX != Int32.MinValue)
                {
                    deltax = e.X - m_prevX;
                    deltay = e.Y - m_prevY;
                }

                m_prevX = e.X;
                m_prevY = e.Y;

                if (m_keyDown == Keys.ControlKey)
                {
                    if(deltay != 0)
                        m_plotterView.ObservatorZ += deltay;
                }
                else if (m_keyDown == Keys.ShiftKey)
                {
                    if(deltax != 0)
                        m_plotterView.ObservatorX -= deltax;

                    if (deltay != 0)
                        m_plotterView.ObservatorY -= deltay;
                }
                else if (m_keyDown == Keys.Menu)
                {
                    if (deltax != 0)
                        m_plotterView.ScreenX += deltax;

                    if (deltay != 0)
                        m_plotterView.ScreenY += deltay;
                }
            }
        }

        private void MeshPlotter_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private Keys m_keyDown = Keys.Insert;

        private void MeshPlotter_KeyDown(object sender, KeyEventArgs e)
        {
            m_keyDown = e.KeyCode;
        }

        private void MeshPlotter_KeyUp(object sender, KeyEventArgs e)
        {
            m_keyDown = Keys.Insert;
            isDragging = false;
            m_prevX = Int32.MinValue;
            m_prevY = Int32.MinValue;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Matlab M File|*.m|JPEG files|*.jpg";
                if (DialogResult.OK != dlg.ShowDialog(this))
                    return;

                if (Path.GetExtension(dlg.FileName) == ".m")
                {
                    this.SurfaceRenderer.SaveMeshAsMatlabScript(dlg.FileName);
                }
                else
                {
                    this.SurfaceRenderer.SaveMeshAsImage(dlg.FileName);
                }
            }
            
        }
    }
}
