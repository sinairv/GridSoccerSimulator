using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;

namespace PlottingTools
{
    public class Surface3DRenderer
    {
        double screenDistance, sf, cf, st, ct, R, A, B, C, D; //transformations coeficients

        PointF startPoint = new PointF(-20, -20);
        PointF endPoint = new PointF(20, 20);
        RendererFunction function = defaultFunction;
        ColorSchema colorSchema = ColorSchema.Autumn;

        private int m_screenWidth, m_screenHeight;

        private void Init()
        {
            DrawBaseSurface = true;
            IsWired = false;
            PenWidth = 1.0f;
            Density = 0.5;
            PenColor = Color.Black;
            BaseSurfacePenColor = Color.LightGray;
            BaseSurfacePenWidth = 1.0f;
            DrawWires = true;
        }


        #region Properties

        public bool DrawBaseSurface { get; set; }

        public Color BaseSurfacePenColor { get; set; }

        public float BaseSurfacePenWidth { get; set; }

        public bool DrawWires { get; set; }

        /// <summary>
        /// Surface spanning net density
        /// </summary>
        public double Density { get; set; }

        public bool IsWired { get; set; }

        public float PenWidth { get; set; }

        /// <summary>
        /// Quadrilateral pen color
        /// </summary>
        public Color PenColor { get; set; }

        public PointF StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }

        public PointF EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        public RendererFunction Function
        {
            get { return function; }
            set { function = value; }
        }

        public ColorSchema ColorSchema
        {
            get { return colorSchema; }
            set { colorSchema = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Surface3DRenderer"/> class. Calculates transformations coeficients.
        /// </summary>
        /// <param name="obsX">Observator's X position</param>
        /// <param name="obsY">Observator's Y position</param>
        /// <param name="obsZ">Observator's Z position</param>
        /// <param name="xs0">X coordinate of screen</param>
        /// <param name="ys0">Y coordinate of screen</param>
        /// <param name="screenWidth">Drawing area width in pixels.</param>
        /// <param name="screenHeight">Drawing area height in pixels.</param>
        /// <param name="screenDistance">The screen distance.</param>
        /// <param name="screenWidthPhys">Width of the screen in meters.</param>
        /// <param name="screenHeightPhys">Height of the screen in meters.</param>
        public Surface3DRenderer(double obsX, double obsY, double obsZ, int xs0, int ys0, int screenWidth, int screenHeight, double screenDistance, double screenWidthPhys, double screenHeightPhys)
        {
            Init();
            ReCalculateTransformationsCoeficients(obsX, obsY, obsZ, xs0, ys0, screenWidth, screenHeight, screenDistance, screenWidthPhys, screenHeightPhys);
        }


        public void ReCalculateTransformationsCoeficients(double obsX, double obsY, double obsZ, int xs0, int ys0, int screenWidth, int screenHeight, double screenDistance, double screenWidthPhys, double screenHeightPhys)
        {
            double r1, a;

            m_screenWidth = screenWidth;
            m_screenHeight = screenHeight;

            if (screenWidthPhys <= 0)//when screen dimensions are not specified
                screenWidthPhys = screenWidth * 0.0257 / 72.0;        //0.0257 m = 1 inch. Screen has 72 px/inch
            if (screenHeightPhys <= 0)
                screenHeightPhys = screenHeight * 0.0257 / 72.0;

            r1 = obsX * obsX + obsY * obsY;
            a = Math.Sqrt(r1);//distance in XY plane
            R = Math.Sqrt(r1 + obsZ * obsZ);//distance from observator to center
            if (a != 0) //rotation matrix coeficients calculation
            {
                sf = obsY / a;//sin( fi)
                cf = obsX / a;//cos( fi)
            }
            else
            {
                sf = 0;
                cf = 1;
            }
            st = a / R;//sin( teta)
            ct = obsZ / R;//cos( teta)

            //linear tranfrormation coeficients
            A = screenWidth / screenWidthPhys;
            B = xs0 + A * screenWidthPhys / 2.0;
            C = -(double)screenHeight / screenHeightPhys;
            D = ys0 - C * screenHeightPhys / 2.0;

            this.screenDistance = screenDistance;
        }

        /// <summary>
        /// Performs projection. Calculates screen coordinates for 3D point.
        /// </summary>
        /// <param name="x">Point's x coordinate.</param>
        /// <param name="y">Point's y coordinate.</param>
        /// <param name="z">Point's z coordinate.</param>
        /// <returns>Point in 2D space of the screen.</returns>
        public PointF Project(double x, double y, double z)
        {
            double xn, yn, zn;//point coordinates in computer's frame of reference

            //transformations
            xn = -sf * x + cf * y;
            yn = -cf * ct * x - sf * ct * y + st * z;
            zn = -cf * st * x - sf * st * y - ct * z + R;

            if (zn == 0) zn = 0.01;

            //Tales' theorem
            return new PointF((float)(A * xn * screenDistance / zn + B), (float)(C * yn * screenDistance / zn + D));
        }

        /// <summary>
        /// Gets the mesh used only for saving purposes.
        /// </summary>
        /// <returns></returns>
        private double[,] GetMesh()
        {
            double[,] mesh = null;
            if (m_defaultMesh != null)
            {
                mesh = m_defaultMesh;
            }
            else
            {
                mesh = new double[(int)((endPoint.X - startPoint.X) / this.Density + 1), (int)((endPoint.Y - startPoint.Y) / this.Density + 1)];

                double xi = startPoint.X, yi, minZ = double.PositiveInfinity, maxZ = double.NegativeInfinity;

                for (int x = 0; x < mesh.GetLength(0); x++)
                {
                    yi = startPoint.Y;
                    for (int y = 0; y < mesh.GetLength(1); y++)
                    {
                        double zz = 0.0;
                        if (function != null)
                        {
                            zz = function(xi, yi);
                            mesh[x, y] = zz;
                        }

                        yi += Density;
                    }

                    xi += Density;
                }

            }

            return mesh;
        }

        public void RenderSurface(Graphics graphics)
        {
            var density = this.Density;

            SolidBrush[] brushes = new SolidBrush[colorSchema.Length];
            for (int i = 0; i < brushes.Length; i++)
                brushes[i] = new SolidBrush(colorSchema[i]);
                        
            double z1, z2;
            PointF[] polygon = new PointF[4];

            double xi = startPoint.X, yi, minZ = double.PositiveInfinity, maxZ = double.NegativeInfinity;

            double[,] mesh = null;
            if(m_defaultMesh != null)
            {
                mesh = m_defaultMesh;
            }
            else
            {
                mesh = new double[(int)((endPoint.X - startPoint.X) / density + 1), (int)((endPoint.Y - startPoint.Y) / density + 1)];
            }

            PointF[,] meshF = new PointF[mesh.GetLength(0), mesh.GetLength(1)];
            for (int x = 0; x < mesh.GetLength(0); x++)
            {
                yi = startPoint.Y;
                for (int y = 0; y < mesh.GetLength(1); y++)
                {
                    double zz = 0.0;
                    if (function != null)
                    {
                        zz = function(xi, yi);
                        mesh[x, y] = zz;
                    }
                    else
                    {
                        zz = mesh[x, y];
                    }

                    meshF[x, y] = Project(xi, yi, zz);
                    yi += density;

                    if (minZ > zz) minZ = zz;
                    if (maxZ < zz) maxZ = zz;
                }
                xi += density;
            }


            double cc = (maxZ - minZ) / (brushes.Length - 1.0);

            if (DrawBaseSurface)
            {
                DrawBase(graphics, mesh);
            }

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
             
            using (Pen pen = new Pen(this.PenColor, this.PenWidth))
            {
                for (int x = 0; x < mesh.GetLength(0) - 1; x++)
                {
                    for (int y = 0; y < mesh.GetLength(1) - 1; y++)
                    {
                        z1 = mesh[x, y];
                        z2 = mesh[x, y + 1];

                        polygon[0] = meshF[x, y];
                        polygon[1] = meshF[x, y + 1];
                        polygon[2] = meshF[x + 1, y + 1];
                        polygon[3] = meshF[x + 1, y];

                        if (!this.IsWired)
                        {
                            int brushIndex = 0;
                            if (cc > 0.0)
                            {
                                brushIndex = (int)(((z1 + z2) / 2.0 - minZ) / cc);
                            }
                            graphics.FillPolygon(brushes[brushIndex], polygon);
                        }

                        if (this.DrawWires)
                        {
                            graphics.DrawPolygon(pen, polygon);
                        }
                    }
                }
            }

            for (int i = 0; i < brushes.Length; i++)
            {
                brushes[i].Dispose();
            }
        }

        private void DrawBase(Graphics graphics, double[,] baseMesh)
        {
            var density = this.Density;

            SolidBrush[] brushes = new SolidBrush[colorSchema.Length];
            for (int i = 0; i < brushes.Length; i++)
                brushes[i] = new SolidBrush(colorSchema[i]);

            double z1, z2;
            PointF[] polygon = new PointF[4];

            double xi = startPoint.X, yi, minZ = double.PositiveInfinity, maxZ = double.NegativeInfinity;

            double[,] mesh = new double[baseMesh.GetLength(0), baseMesh.GetLength(1)];

            PointF[,] meshF = new PointF[mesh.GetLength(0), mesh.GetLength(1)];
            for (int x = 0; x < mesh.GetLength(0); x++)
            {
                yi = startPoint.Y;
                for (int y = 0; y < mesh.GetLength(1); y++)
                {
                    double zz = mesh[x, y]; // i.e., 0.0

                    meshF[x, y] = Project(xi, yi, zz);
                    yi += density;

                    if (minZ > zz) minZ = zz;
                    if (maxZ < zz) maxZ = zz;
                }
                xi += density;
            }


            double cc = (maxZ - minZ) / (brushes.Length - 1.0);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(this.BaseSurfacePenColor, this.BaseSurfacePenWidth))
            {
                for (int x = 0; x < mesh.GetLength(0) - 1; x++)
                {
                    for (int y = 0; y < mesh.GetLength(1) - 1; y++)
                    {
                        z1 = mesh[x, y];
                        z2 = mesh[x, y + 1];

                        polygon[0] = meshF[x, y];
                        polygon[1] = meshF[x, y + 1];
                        polygon[2] = meshF[x + 1, y + 1];
                        polygon[3] = meshF[x + 1, y];

                        graphics.DrawPolygon(pen, polygon);
                    }
                }
            }

            for (int i = 0; i < brushes.Length; i++)
            {
                brushes[i].Dispose();
            }
        }


        public static RendererFunction GetFunctionHandle(string formula)
        {
            CompiledFunction fn = FunctionCompiler.Compile(2, formula);
            return new RendererFunction(delegate(double x, double y)
            {
                return fn(x, y);
            });
        }

        public void SetFunction(string formula)
        {
            function = GetFunctionHandle(formula);
            m_defaultMesh = null;
        }

        private double[,] m_defaultMesh = null;
        public void SetMesh(double[,] mesh)
        {
            function = null;
            m_defaultMesh = mesh;
        }

        private static double defaultFunction(double a, double b)
        {
            double an = a, bn = b, anPlus1;
            short iter = 0;
            do
            {
                anPlus1 = (an + bn) / 2.0;
                bn = Math.Sqrt(an * bn);
                an = anPlus1;
                if (iter++ > 1000) return an;
            } while (Math.Abs(an - bn)<0.1);
            return an;
        }

        internal void SaveMeshAsMatlabScript(string fileName)
        {
            var funcName = Path.GetFileNameWithoutExtension(fileName);
            double[,] mesh = GetMesh();

            double startX = 0.0;
            double stepX = 1.0;
            double endX = mesh.GetLength(0) - 1;

            double startY = 0.0;
            double stepY = 1.0;
            double endY = mesh.GetLength(1) - 1;

            if (m_defaultMesh == null)
            {
                stepX = stepY = this.Density;
                startX = startPoint.X;
                startY = startPoint.Y;

                endX = endPoint.X;
                endY = endPoint.Y;
            }


            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine("function {0}", funcName);
                sw.WriteLine();
                sw.WriteLine("[X, Y] = meshgrid({0}:{1}:{2}, {3}:{4}:{5});",
                    startX, stepX, endX, startY, stepY, endY);

                sw.WriteLine();

                sw.WriteLine("Z = zeros({0}, {1});", mesh.GetLength(0), mesh.GetLength(1));
                sw.WriteLine();

                for (int xi = 0, upxi = mesh.GetLength(0); xi < upxi; xi++)
                {
                    for (int yi = 0, upyi = mesh.GetLength(1); yi < upyi; yi++)
                    {
                        sw.WriteLine("Z({0}, {1}) = {2};", xi + 1, yi + 1, mesh[xi, yi]);
                    }
                }
                sw.WriteLine();

                sw.WriteLine("surf(X', Y', Z);");
                sw.WriteLine("%mesh(X', Y', Z);");
                sw.WriteLine();
            }

        }

        internal void SaveMeshAsImage(string fileName)
        {
            Bitmap b = new Bitmap(m_screenWidth, m_screenHeight);
            Graphics g = Graphics.FromImage(b);

            this.RenderSurface(g);

            b.Save(fileName);
        }
    }

    public delegate double RendererFunction(double x, double y);

    public struct Point3D
    {
        public double x, y, z;

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}