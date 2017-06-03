using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace PlottingTools
{
    public partial class ChartWrapper : UserControl
    {
        private List<string> m_seriesNames = new List<string>();
        private List<string> m_markerSeriesNames = new List<string>();

        public ChartWrapper()
        {
            InitializeComponent();
            mainChart.Series.Clear();
            mainChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            mainChart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            mainChart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            mainChart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            mainChart.Legends.Clear();

            AddMarkers = true;
            MarkerCounts = 15;
            MarkerSize = 8;
        }

        public bool AddMarkers { get; set; }
        public int MarkerCounts { get; set; } // set to -1 to disable each
        public int MarkerFreq { get; set; }
        public int MarkerSize { get; set; }

        public string AxisYTitle
        {
            get
            {
                return mainChart.ChartAreas[0].AxisY.Title;
            }

            set
            {
                mainChart.ChartAreas[0].AxisY.Title = value;
            }
        }

        public string AxisXTitle
        {
            get
            {
                return mainChart.ChartAreas[0].AxisX.Title;
            }

            set
            {
                mainChart.ChartAreas[0].AxisX.Title = value;
            }
        }

        public string Title
        {
            get
            {
                return lblTitle.Text;
            }

            set
            {
                lblTitle.Text = value;
            }
        }

        public Chart TheChart
        {
            get
            {
                return mainChart;
            }
        }

        public void InitPlot()
        {
            mainChart.Series.Clear();
            mainChart.Legends.Clear();
            legendPanel.Controls.Clear();
            m_seriesNames.Clear();
            m_markerSeriesNames.Clear();
            m_colorCounter = -1;
            m_markerCounter = -1;
        }

        public void SaveChart()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "png|*.png|jpg|*.jpg|tiff|*.tiff";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            string ext = Path.GetExtension(dlg.FileName);
            ChartImageFormat format = ChartImageFormat.Jpeg;

            switch (ext.ToLower())
            {
                case ".jpg":
                    format = ChartImageFormat.Jpeg;
                    break;
                case ".png":
                    format = ChartImageFormat.Png;
                    break;
                case ".tiff":
                    format = ChartImageFormat.Tiff;
                    break;
            }
            mainChart.SaveImage(dlg.FileName, format);
        }

        private static Color[] PredefinedColors = new Color[] 
            {
                Color.Blue, Color.Red, Color.Green, Color.Black, Color.Brown, Color.Cyan, Color.Magenta,
                Color.LightGreen, Color.Orange, Color.Peru, Color.DarkRed, Color.LightBlue, Color.SlateGray
            };

        private static MarkerStyle[] MarkerStylesArray = new MarkerStyle[] {
            MarkerStyle.Circle, MarkerStyle.Cross, MarkerStyle.Diamond, MarkerStyle.Square, 
            MarkerStyle.Star10, MarkerStyle.Star4, MarkerStyle.Star5, MarkerStyle.Star6,
            MarkerStyle.Triangle
        };

        private int m_colorCounter = -1;
        private Color GetNextColor()
        {
            m_colorCounter++;
            return PredefinedColors[m_colorCounter % PredefinedColors.Length];
        }

        private int m_markerCounter = -1;
        private MarkerStyle GetNextMarkerStyle()
        {
            m_markerCounter++;
            return MarkerStylesArray[m_markerCounter % MarkerStylesArray.Length];
        }

        public void AddBarPlot<T>(string name, string[] labels, T[] values)
        {
            if (m_seriesNames.Count == 1)
            {
                AddSideLegend(m_seriesNames[0]);
            }

            string serName = name;
            var curSeries = mainChart.Series.Add(serName);
            curSeries.Tag = serName;
            m_seriesNames.Add(serName);
            curSeries["DrawingStyle"] = "Cylinder";
            curSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            mainChart.ChartAreas[0].AxisX.Interval = 1.0;
            curSeries.IsValueShownAsLabel = false;
            curSeries.Color = GetNextColor();

            for(int i = 0; i < labels.Length; i++)
                curSeries.Points.AddXY(labels[i], values[i]);

            if (m_seriesNames.Count > 1)
            {
                AddSideLegend(serName);
            }
        }

        public void AddLinePlot(string name, double[] values)
        {
            if (m_seriesNames.Count == 1)
            {
                AddSideLegend(m_seriesNames[0]);
            }

            string serName = name;
            Series curSeries = mainChart.Series.Add(serName);
            curSeries.LegendText = serName;
            m_seriesNames.Add(serName);
            curSeries.ChartType = SeriesChartType.FastLine;
            curSeries.Color = GetNextColor();

            var seriesPoints = curSeries.Points;
            for (int i = 0; i < values.Length; i++)
                seriesPoints.AddY(values[i]);


            if(this.AddMarkers && (this.MarkerCounts > 0 || this.MarkerFreq > 0))
            {
                curSeries.IsVisibleInLegend = false;

                string ptSeriesName = "_pt_" + serName;
                Series ptSeries = mainChart.Series.Add(ptSeriesName);
                ptSeries.LegendText = serName;
                m_markerSeriesNames.Add(ptSeriesName);
                ptSeries.ChartType = SeriesChartType.FastPoint;
                ptSeries.Color = curSeries.Color;
                ptSeries.MarkerSize = this.MarkerSize;
                ptSeries.MarkerStyle = GetNextMarkerStyle();

                int markerFreq = 0;
                if (this.MarkerCounts > 0)
                {
                    markerFreq = values.Length / this.MarkerCounts;
                }
                else if (this.MarkerFreq > 0)
                {
                    markerFreq = this.MarkerFreq;
                }

                if (markerFreq > 0)
                {
                    var ptSeriesPoints = ptSeries.Points;
                    for (int i = 0; i < values.Length; i+= markerFreq)
                        ptSeriesPoints.AddXY(i, values[i]);
                }
            }

            if (m_seriesNames.Count > 1)
            {
                AddSideLegend(serName);
            }
        }

        /// <summary>
        /// i.e., the list of check boxes
        /// </summary>
        private void AddSideLegend(string serName)
        {
            var series = mainChart.Series[serName];
            Series markers = null;

            if (this.AddMarkers)
            {
                markers = mainChart.Series["_pt_" + serName];
            }

            CheckBox ckBox = new CheckBox();
            ckBox.AutoSize = true;
            ckBox.Tag = new[] { series, markers };
            ckBox.Text = series.Name;
            ckBox.ForeColor = series.Color;
            ckBox.Checked = true;
            ckBox.Margin = new Padding(0);

            ckBox.CheckedChanged += new EventHandler(ckBox_CheckedChanged);

            legendPanel.AutoSize = false;
            legendPanel.Width = Math.Max(legendPanel.Width, ckBox.Width + 30);
            legendPanel.Controls.Add(ckBox);
        }


        ///// <summary>
        ///// i.e., the list of check boxes
        ///// </summary>
        //private void AddSideLegend(Series series, Series markers)
        //{
        //    CheckBox ckBox = new CheckBox();
        //    ckBox.AutoSize = true;
        //    ckBox.Tag = new [] {series, markers};
        //    ckBox.Text = series.Name;
        //    ckBox.ForeColor = series.Color;
        //    ckBox.Checked = true;
        //    ckBox.Margin = new Padding(0);

        //    ckBox.CheckedChanged += new EventHandler(ckBox_CheckedChanged);

        //    legendPanel.AutoSize = false;
        //    legendPanel.Width = Math.Max(legendPanel.Width, ckBox.Width + 30);
        //    legendPanel.Controls.Add(ckBox);
        //}

        void ckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckBox = sender as CheckBox;
            Series[] sers = (Series[])ckBox.Tag;

            if (ckBox.Checked)
            {
                foreach (var ser in sers)
                {
                    if (ser == null) continue;
                    mainChart.Series.Add(ser);
                }
            }
            else
            {
                foreach (var ser in sers)
                {
                    if (ser == null) continue;
                    mainChart.Series.Remove(ser);
                }
            }
        }

        public bool ContainsSeries(string name)
        {
            return m_seriesNames.Contains(name);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveChart();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in legendPanel.Controls)
            {
                if (ctrl is CheckBox)
                {
                    (ctrl as CheckBox).Checked = true;
                }
            }
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in legendPanel.Controls)
            {
                if (ctrl is CheckBox)
                {
                    (ctrl as CheckBox).Checked = false;
                }
            }
        }

        private bool m_isLegendVisible = false;

        private void showLegendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_isLegendVisible)
            {
                mainChart.Legends.Clear();
                mainChart.ChartAreas[0].Position.Auto = true;
            }
            else
            {
                if (AddMarkers)
                {
                    foreach (string name in m_markerSeriesNames)
                    {
                        mainChart.Legends.Add(name);
                    }
                }
                else
                {
                    foreach (string name in m_seriesNames)
                    {
                        mainChart.Legends.Add(name);
                    }
                }

                mainChart.ChartAreas[0].Position.Width = 80.0f;

            }
            m_isLegendVisible = !m_isLegendVisible;
        }


    }
}
