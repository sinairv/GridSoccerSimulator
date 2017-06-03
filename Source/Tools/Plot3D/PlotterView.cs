using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PlottingTools
{
    public class PlotterView
    {
        MeshPlotter m_plotter;
        public PlotterView(MeshPlotter plotter)
        {
            m_plotter = plotter;
        }

        private double m_obsX = 70;

        public double ObservatorX
        {
            get { return m_obsX; }
            set { m_obsX = value; m_plotter.ResizeView(); }
        }

        private double m_obsY = 35;

        public double ObservatorY
        {
            get { return m_obsY; }
            set { m_obsY = value; m_plotter.ResizeView(); }
        }

        private double m_obsZ = 40;

        public double ObservatorZ
        {
            get { return m_obsZ; }
            set { m_obsZ = value; m_plotter.ResizeView(); }
        }

        private int m_screenX = 0;

        public int ScreenX
        {
            get { return m_screenX; }
            set { m_screenX = value; m_plotter.ResizeView(); }
        }

        private int m_screenY = 0;

        public int ScreenY
        {
            get { return m_screenY; }
            set { m_screenY = value; m_plotter.ResizeView(); }
        }


        private double m_hueCS = 10;
        public double ColorSchema
        {
            get
            {
                return m_hueCS;
            }

            set
            {
                m_hueCS = value;
                m_plotter.ColorSchema = new ColorSchema(m_hueCS);
            }
        }

        public Color BaseColor
        {
            get
            {
                return m_plotter.ColorSchema.BaseColor;
            }

            set
            {
                m_plotter.ColorSchema = new ColorSchema(value);
            }
            
        }

        public bool IsWired
        {
            get
            {
                return m_plotter.IsWired;
            }

            set
            {
                m_plotter.IsWired = value;
            }
        }

        public Color PenColor
        {
            get
            {
                return m_plotter.SurfaceRenderer.PenColor;
            }

            set
            {
                m_plotter.SurfaceRenderer.PenColor = value;
                m_plotter.Invalidate();
            }
        }

        public float PenWidth
        {
            get
            {
                return m_plotter.SurfaceRenderer.PenWidth;
            }

            set
            {
                m_plotter.SurfaceRenderer.PenWidth = value;
                m_plotter.Invalidate();
            }
        }

        public Color BackColor
        {
            get
            {
                return m_plotter.BackColor;
            }

            set
            {
                m_plotter.BackColor = value;
                m_plotter.Invalidate();
            }
        }

        public bool DrawBaseSurface
        {
            get
            {
                return m_plotter.SurfaceRenderer.DrawBaseSurface;
            }

            set
            {
                m_plotter.SurfaceRenderer.DrawBaseSurface = value;
                m_plotter.Invalidate();
            }

        }

        public Color BaseSurfaceColor
        {
            get
            {
                return m_plotter.SurfaceRenderer.BaseSurfacePenColor;
            }

            set
            {
                m_plotter.SurfaceRenderer.BaseSurfacePenColor = value;
                m_plotter.Invalidate();
            }
        }

        public float BaseSurfacePenWidth
        {
            get
            {
                return m_plotter.SurfaceRenderer.BaseSurfacePenWidth;
            }

            set
            {
                m_plotter.SurfaceRenderer.BaseSurfacePenWidth = value;
                m_plotter.Invalidate();
            }
        }

        public bool DrawWires
        {
            get
            {
                return m_plotter.SurfaceRenderer.DrawWires;
            }

            set
            {
                m_plotter.SurfaceRenderer.DrawWires = value;
                m_plotter.Invalidate();
            }

        }

        public double Density
        {
            get
            {
                return m_plotter.SurfaceRenderer.Density;
            }

            set
            {
                if (value <= 0.0)
                    value = 0.5;

                m_plotter.SurfaceRenderer.Density = value;
                m_plotter.Invalidate();
            }

        }

    }
}
