using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.AccessControl;

namespace PlottingTools
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();
        }

        public static void ShowChartForm(string[] names, double[][] values, string chartTitle, string xTitle, string yTitle, string formTitle)
        {
            var frm = new ChartForm();

            for (int i = 0; i < names.Length; i++)
            {
                frm.chartMain.AddLinePlot(names[i], values[i]);
            }

            frm.chartMain.Title = chartTitle;
            frm.chartMain.AxisXTitle = xTitle;
            frm.chartMain.AxisYTitle = yTitle;
            frm.Text = formTitle;

            frm.Show();
        }
    }
}
