using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PlottingTools
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.propertyGrid1.SelectedObject = this.meshPlotter1.PlotterView;

            //InitWithExplicitMesh();
            InitWithFunction();
        }

        private void InitWithFunction()
        {
            meshPlotter1.SetFunction("sin(x1)*cos(x2)/(sqrt(sqrt(x1*x1+x2*x2))+1)*10");
        }

        private void InitWithExplicitMesh()
        {
            double[,] mesh = new double[30, 30];

            for (int x = 0; x < mesh.GetLength(0); x++)
            {
                for (int y = 0; y < mesh.GetLength(1); y++)
                {
                    mesh[x, y] = Math.Sin(x);
                }
            }

            meshPlotter1.SetMesh(mesh);
        }
    }
}
