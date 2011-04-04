using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using GridSoccer.LogPlotter;

namespace LogPlotter
{
    public partial class FormMain : Form
    {
        public class PathListItem
        {
            public PathListItem(string path)
            {
                this.PathString = path;
            }

            public string PathString { get; private set; }

            public override string ToString()
            {
                return Path.GetFileName(PathString);
            }
        }

        private readonly string[] m_colorNames = new string[] { "b", "r", "g", "k", "c", "m" };

        private int m_wndSize = 200;
        private int m_plotEvery = 1;
        private int m_dataLength = 0;

        private string m_logsDir = ".";

        public FormMain()
        {
            InitializeComponent();



            string curDir = Path.GetFullPath(Environment.CurrentDirectory);



            string logsDir = ".";

            string[] cmdLines = Environment.GetCommandLineArgs();

            if (cmdLines.Length > 1 && Directory.Exists(cmdLines[1]))
            {
                logsDir = Path.GetFullPath(cmdLines[1]);
                this.Text = cmdLines[1] + " - " + this.Text;
            }
            else if (Directory.Exists(Path.Combine(curDir, "Logs")))
            {
                logsDir = Path.Combine(curDir, "Logs");
            }
            else if (Directory.Exists(Path.Combine(curDir, "../Scripts/Logs")))
            {
                logsDir = Path.Combine(curDir, "../Scripts/Logs");
            }

            logsDir = Path.GetFullPath(logsDir);

            m_logsDir = logsDir;

            RefreshAvailableLogs();

        }

        private void RefreshAvailableLogs()
        {
            lstAvailableLogs.Items.Clear();

            foreach (string fileName in Directory.GetFiles(m_logsDir, "*.log"))
            {
                lstAvailableLogs.Items.Add(new PathListItem(fileName));
            }
        }

        private void btnSelectFiles_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstAvailableLogs.SelectedIndices.Count; i++)
            {
                lstSelectedLogs.Items.Add(lstAvailableLogs.Items[lstAvailableLogs.SelectedIndices[i]]);
            }
        }

        private void btnRemoveSelectedFiles_Click(object sender, EventArgs e)
        {
            for (int i = lstSelectedLogs.SelectedIndices.Count - 1; i >= 0; i--)
            {
                lstSelectedLogs.Items.RemoveAt(lstSelectedLogs.SelectedIndices[i]);
            }
        }

        private void tbtnSaveAverage_Click(object sender, EventArgs e)
        {
            var paths = lstSelectedLogs.Items.Cast<PathListItem>().ToArray();

            if (paths.Length <= 1)
                return;

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "*.log|*.log";
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                SaveAverageOfFiles(dlg.FileName, paths);

                MessageBox.Show("Finished");

            }
        }

        private void SaveAverageOfFiles(string dstFileName, PathListItem[] paths)
        {
            StreamReader[] lstReaders = new StreamReader[paths.Length];
            for (int fi = 0; fi < paths.Length; fi++)
            {
                lstReaders[fi] = File.OpenText(paths[fi].PathString);
            }

            var sw = File.CreateText(dstFileName);
            sw.AutoFlush = true;

            //bool[] isFileFinished = new bool[paths.Length];

            for (; ; )
            {
                var allBlocks = new List<double[]>[paths.Length];
                for (int fi = 0; fi < paths.Length; fi++)
                {
                    var blockData = ReadBlockData(lstReaders[fi]);
                    allBlocks[fi] = blockData;
                }

                if (!WriteAverageOfBlocks(allBlocks, sw))
                    break;

            }


            sw.Close();
            for (int fi = 0; fi < paths.Length; fi++)
            {
                lstReaders[fi].Close();
            }
        }

        private bool WriteAverageOfBlocks(List<double[]>[] allBlocks, StreamWriter sw)
        {
            bool areLengthsEqual = true;
            int minLength = Int32.MaxValue;
            int commonLength = allBlocks[0].Count;

            // foreach file
            for (int i = 0; i < allBlocks.Length; i++)
            {
                var curFileBlocks = allBlocks[i];
                if (curFileBlocks.Count < minLength)
                    minLength = curFileBlocks.Count;

                if (curFileBlocks.Count != commonLength)
                    areLengthsEqual = false;
            }

            //List<double[]> avgBlock = new List<double[]>(minLength);

            // for each line of data
            for (int i = 0; i < minLength; i++)
            {
                // number of cols in ith row of the first file
                int cols = allBlocks[0][i].Length;
                double[] avgValues = new double[cols];

                // foreach file
                for (int bi = 0; bi < allBlocks.Length; bi++)
                {
                    for(int ci = 0; ci < cols; ci++)
                    {
                        avgValues[ci] += allBlocks[bi][i][ci] / allBlocks.Length;
                    }
                }

                for(int ci = 0; ci < cols; ci++)
                {
                    sw.Write("{0} ", avgValues[ci]);
                }
                sw.WriteLine();
            }

            return areLengthsEqual && minLength > 0;
        }

        private const int blockLength = 1000;
        private List<double[]> ReadBlockData(StreamReader sr)
        {
            List<double[]> curBlock = new List<double[]>(blockLength);

            int lineNo = -1;
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (String.IsNullOrEmpty(line) || line.StartsWith("%"))
                    continue;

                lineNo++;

                // read line
                var cols = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                List<double> colValues = new List<double>(cols.Length);
                for (int i = 0; i < cols.Length; i++)
                {
                    double d;
                    if (Double.TryParse(cols[i], out d))
                        colValues.Add(d);
                    else
                        colValues.Add(0.0);
                }

                curBlock.Add(colValues.ToArray());

                if (lineNo >= blockLength)
                    break;
            }

            return curBlock;
        }

        private void averageRewScoreDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paths = lstSelectedLogs.Items.Cast<PathListItem>().ToArray();

            if (paths.Length <= 0)
                return;

            double[][] avgRewards = new double[paths.Length][];
            double[][] scoreDiffs = new double[paths.Length][];
            string[] names = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                var pathItem = paths[i];
                string path = pathItem.PathString;
                double[] curAvgRews, curScoreDiffs;
                ReadColumnsAvgRewAndScoreDiff(path, out curAvgRews, out curScoreDiffs);
                names[i] = Path.GetFileNameWithoutExtension(path);
                avgRewards[i] = curAvgRews;
                scoreDiffs[i] = curScoreDiffs;
            }

            string xTitle = "Cycles";
            if (m_plotEvery > 1)
                xTitle += " * " + m_plotEvery.ToString();

            ChartForm.ShowChartForm(names, avgRewards, "Average Rewards", xTitle, "Average Rewards", "Average Rewards");
            ChartForm.ShowChartForm(names, scoreDiffs, "Score Difference", xTitle, "Score Difference", "Score Difference");
        }

        private void meanOfSelectedLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paths = lstSelectedLogs.Items.Cast<PathListItem>().ToArray();

            if (paths.Length <= 0)
                return;

            double[][] bestFits = new double[paths.Length][];
            double[][] meanFits = new double[paths.Length][];
            double[][] meanComplexities = new double[paths.Length][];
            int meanArrayLen = Int32.MaxValue;

            for (int i = 0; i < paths.Length; i++)
            {
                var pathItem = paths[i];
                string path = pathItem.PathString;
                double[] curBestFit, curMeanFit, curMeanComplex;

                ReadColumnsForEA(path, out curBestFit, out curMeanFit, out curMeanComplex);

                bestFits[i] = curBestFit;
                meanFits[i] = curMeanFit;
                meanComplexities[i] = curMeanComplex;

                if (curBestFit.Length < meanArrayLen)
                    meanArrayLen = curBestFit.Length;
            }

            double[] meanBestFits = new double[meanArrayLen];
            double[] meanMeanFits = new double[meanArrayLen];
            double[] meanMeanComp = new double[meanArrayLen];

            for (int p = 0; p < paths.Length; p++)
            {
                for (int i = 0; i < meanBestFits.Length; i++)
                {
                    meanBestFits[i] += bestFits[p][i] / paths.Length;
                    meanMeanFits[i] += meanFits[p][i] / paths.Length;
                    meanMeanComp[i] += meanComplexities[p][i] / paths.Length;
                }
            }

            string xTitle = "Generations";
            if (m_plotEvery > 1)
                xTitle += " * " + m_plotEvery.ToString();

            ChartForm.ShowChartForm(new [] {"Mean Best Fitness"}, new [] { meanBestFits }, 
                "Mean Best Fitness", xTitle, "Mean Best Fitness", "Mean Best Fitness");

            ChartForm.ShowChartForm(new[] { "Mean Mean Fitness" }, new[] { meanMeanFits },
                "Mean Mean Fitness", xTitle, "Mean Mean Fitness", "Mean Mean Fitness");

            ChartForm.ShowChartForm(new[] { "Mean Avg Complexities" }, new[] { meanMeanComp },
                "Mean Avg Complexities", xTitle, "Mean Avg Complexities", "Mean Avg Complexities");
        }

        private void bestAndMeanInOneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paths = lstSelectedLogs.Items.Cast<PathListItem>().ToArray();

            if (paths.Length <= 0)
                return;

            double[][] bestFits = new double[paths.Length][];
            double[][] meanFits = new double[paths.Length][];

            string[] names = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                var pathItem = paths[i];
                string path = pathItem.PathString;
                double[] curBestFit, curMeanFit, curMeanComplex;

                ReadColumnsForEA(path, out curBestFit, out curMeanFit, out curMeanComplex);

                names[i] = Path.GetFileNameWithoutExtension(path);
                bestFits[i] = curBestFit;
                meanFits[i] = curMeanFit;
            }

            string xTitle = "Generations";
            if (m_plotEvery > 1)
                xTitle += " * " + m_plotEvery.ToString();

            for (int i = 0; i < paths.Length; i++)
            {
                string title = Path.GetFileNameWithoutExtension(paths[i].PathString);
                ChartForm.ShowChartForm(
                    new string[] {"Best Fitness", "Mean Fitness"}, 
                    new [] {bestFits[i], meanFits[i]},
                    title, xTitle, "Fitness", title);
            }


        }

        private void eAPlotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paths = lstSelectedLogs.Items.Cast<PathListItem>().ToArray();

            if (paths.Length <= 0)
                return;

            double[][] bestFits = new double[paths.Length][];
            double[][] meanFits = new double[paths.Length][];
            double[][] meanNetComplex = new double[paths.Length][];

            string[] names = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                var pathItem = paths[i];
                string path = pathItem.PathString;
                double[] curBestFit, curMeanFit, curMeanComplex;

                ReadColumnsForEA(path, out curBestFit, out curMeanFit, out curMeanComplex);
                
                names[i] = Path.GetFileNameWithoutExtension(path);
                bestFits[i] = curBestFit;
                meanFits[i] = curMeanFit;
                meanNetComplex[i] = curMeanComplex;
            }

            string xTitle = "Generations";
            if (m_plotEvery > 1)
                xTitle += " * " + m_plotEvery.ToString();

            ChartForm.ShowChartForm(names, bestFits, "Best Fitness", xTitle, "Best Fitness", "Best Fitness");
            ChartForm.ShowChartForm(names, meanFits, "Mean Fitness", xTitle, "Mean Fitness", "Mean Fitness");
            ChartForm.ShowChartForm(names, meanNetComplex, "Mean Net Complexity", xTitle, "Mean Net Complexity", "Mean Net Complexity");
        }

        private void scoreDelaysAndCumulativeRewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paths = lstSelectedLogs.Items.Cast<PathListItem>().ToArray();

            if (paths.Length <= 0)
                return;

            double[][] recvDelays = new double[paths.Length][];
            double[][] scoreDelays = new double[paths.Length][];
            double[][] cumRews = new double[paths.Length][];
            string[] names = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                var pathItem = paths[i];
                string path = pathItem.PathString;
                double[] curRecvDelay, curScoreDelay, curCumRew;
                ReadColumnsScoreDelayAndCumRew(path, out curRecvDelay, out curScoreDelay, out curCumRew);
                names[i] = Path.GetFileNameWithoutExtension(path);
                recvDelays[i] = curRecvDelay;
                scoreDelays[i] = curScoreDelay;
                cumRews[i] = curCumRew;
            }

            string xTitle = "Episodes";
            if (m_plotEvery > 1)
                xTitle += " * " + m_plotEvery.ToString();

            ChartForm.ShowChartForm(names, recvDelays, "Goal Recv Delay", xTitle, "Time Between Receiving Two Goals", "Goal Recv Delay");
            ChartForm.ShowChartForm(names, scoreDelays, "Goal Score Delay", xTitle, "Time Between Scoring Two Goals", "Goal Score Delay");
            ChartForm.ShowChartForm(names, cumRews, "Cumulative Rewards", xTitle, "Cumulative Rewards", "Cumulative Rewards");
        }

        private void ReadColumnsScoreDelayAndCumRew(string path, out double[] recvDelay, 
            out double[] scoreDelay, out double[] cumRew)
        {
            List<double> lstRcvDelay = new List<double>(1000);
            List<double> lstScoreDelay = new List<double>(1000);
            List<double> lstCumRew = new List<double>(1000);

            string prevStrScoreDiff = "", curStrScoreDiff = "";
            int prevScoreDiff = 0, curScoreDiff = 0;
            int prevScoreLineNo = 0, prevRecvLineNo = 0;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                int lineNo = -1;
                int episodeNo = -1;
                double sumRew = 0.0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("%"))
                        continue;

                    lineNo++;

                    string[] cols = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    curStrScoreDiff = (cols.Length > 4) ? cols[4] : prevStrScoreDiff;

                    if (curStrScoreDiff != prevStrScoreDiff)
                    {
                        if (!Int32.TryParse(curStrScoreDiff, out curScoreDiff))
                            curScoreDiff = prevScoreDiff;

                        if (episodeNo >= 0)
                        {
                            int delay = 0;
                            if (curScoreDiff > prevScoreDiff)
                            {
                                delay = lineNo - prevScoreLineNo;
                                prevScoreLineNo = lineNo;
                            }
                            else if (curScoreDiff < prevScoreDiff)
                            {
                                delay = lineNo - prevRecvLineNo;
                                prevRecvLineNo = lineNo;
                            }

                            if (curScoreDiff != prevScoreDiff && episodeNo % m_plotEvery == 0)
                            {
                                lstCumRew.Add(sumRew);
                                
                                if(curScoreDiff < prevScoreDiff)
                                    lstRcvDelay.Add(delay);

                                if (curScoreDiff > prevScoreDiff)
                                    lstScoreDelay.Add(delay);
                            }

                            sumRew = 0.0;
                        }
                        else
                        {
                            prevScoreLineNo = lineNo;
                            prevRecvLineNo = lineNo;
                        }

                        episodeNo++;
                    }

                    double curRew;
                    if (cols.Length > 1 && Double.TryParse(cols[1], out curRew))
                        sumRew += curRew;

                    prevStrScoreDiff = curStrScoreDiff;
                    prevScoreDiff = curScoreDiff; 
                }
            }

            recvDelay = SimpleMovingAverages(lstRcvDelay.ToArray(), m_wndSize);
            scoreDelay = SimpleMovingAverages(lstScoreDelay.ToArray(), m_wndSize);
            cumRew = SimpleMovingAverages(lstCumRew.ToArray(), m_wndSize);
        }

        private double[] SimpleMovingAverages(double[] input, int winSize)
        {
            if (winSize <= 1)
                return input;

            if (input.Length < winSize * 1.5)
                return input;

            double[] sma = new double[input.Length - winSize + 1];

            double firstSum = 0.0;
            for (int i = 0; i < winSize; i++)
                firstSum += input[i];

            sma[0] = firstSum / winSize;

            for (int i = 1; i < sma.Length; i++)
            {
                firstSum = firstSum - input[i - 1] + input[i + winSize - 1];
                sma[i] = firstSum / winSize;
            }

            return sma;
        }

        private void ReadColumnsForEA(string path, out double[] curBestFit, 
            out double[] curMeanFit, out double[] curMeanComplex)
        {
            List<double> lstValues1 = new List<double>(1000);
            List<double> lstValues2 = new List<double>(1000);
            List<double> lstValues3 = new List<double>(1000);

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                int lineNo = -1;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("%"))
                        continue;

                    lineNo++;

                    if (lineNo % m_plotEvery != 0)
                        continue;

                    string[] cols = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    double curValue;
                    if (cols.Length > 1 && Double.TryParse(cols[1], out curValue))
                        lstValues1.Add(curValue);

                    if (cols.Length > 2 && Double.TryParse(cols[2], out curValue))
                        lstValues2.Add(curValue);

                    if (cols.Length > 3 && Double.TryParse(cols[3], out curValue))
                        lstValues3.Add(curValue);

                    if (m_dataLength > 0 && lineNo >= m_dataLength)
                        break;
                }
            }

            curBestFit = lstValues1.ToArray();
            curMeanFit = lstValues2.ToArray();
            curMeanComplex = lstValues3.ToArray();
        }




        private void ReadColumnsAvgRewAndScoreDiff(string path, out double[] col0, out double[] col4)
        {
            List<double> lstValues0 = new List<double>(1000);
            List<double> lstValues4 = new List<double>(1000);

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                int lineNo = -1;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("%"))
                        continue;

                    lineNo++;

                    if (lineNo % m_plotEvery != 0)
                        continue;

                    string[] cols = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    double curValue;
                    if (cols.Length > 0 && Double.TryParse(cols[0], out curValue))
                        lstValues0.Add(curValue);

                    if (cols.Length > 4 && Double.TryParse(cols[4], out curValue))
                        lstValues4.Add(curValue);

                    if (m_dataLength > 0 && lineNo >= m_dataLength)
                        break;
                }
            }

            col0 = lstValues0.ToArray();
            col4 = lstValues4.ToArray();
        }


        private void tbtnGeneratePltFile_Click(object sender, EventArgs e)
        {
            if (lstSelectedLogs.Items.Count <= 0)
                return;

            string pltFileName;

            using (var dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = m_logsDir;
                dlg.Filter = "Plt file|*.plt";
                if (DialogResult.OK != dlg.ShowDialog())
                    return;

                pltFileName = dlg.FileName;
            }

            using (var sw = File.CreateText(pltFileName))
            {
                sw.WriteLine("% How to use");
                sw.WriteLine("% 1st line: file name");
                sw.WriteLine("% 2nd line: plot line settings");
                sw.WriteLine("% 3rd line: plot title");
                sw.WriteLine("% 4th line: data length (if 0, all the data is plotted)");
                sw.WriteLine();

                int i = -1;
                foreach (var objItem in lstSelectedLogs.Items)
                {
                    i++;
                    PathListItem lstItem = objItem as PathListItem;

                    if (lstItem.PathString.StartsWith(m_logsDir))
                        sw.WriteLine(Path.GetFileName(lstItem.PathString));
                    else
                        sw.WriteLine(lstItem.PathString);


                    sw.WriteLine(m_colorNames[i % m_colorNames.Length]);

                    
                    string plotName = Path.GetFileNameWithoutExtension(lstItem.PathString).Replace('_', '-');
                    if (plotName.Length > "-0-00000000-000000".Length)
                    {
                        string ending = plotName.Substring(plotName.Length - "-0-00000000-000000".Length);
                        var match = Regex.Match(ending, @"-\d-\d{8,8}-\d{6,6}");
                        if (match.Success && match.Value == ending)
                        {
                            plotName = plotName.Substring(0, plotName.Length - "-0-00000000-000000".Length);
                        }
                    }

                    sw.WriteLine(plotName);

                    sw.WriteLine(m_dataLength.ToString());
                    sw.WriteLine();
                }

            }
        }


        private void tbtnSetLogsDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = m_logsDir;
                if (DialogResult.OK != dlg.ShowDialog())
                    return;

                m_logsDir = dlg.SelectedPath;
                RefreshAvailableLogs();
            }
        }

        private void tbtnReloadLogsDir_Click(object sender, EventArgs e)
        {
            RefreshAvailableLogs();
        }

        private void ttxtMvAvgWnd_TextChanged(object sender, EventArgs e)
        {
            int wndSize = 200;
            if (!Int32.TryParse(ttxtMvAvgWnd.Text, out wndSize))
                wndSize = 200;

            if (wndSize <= 0)
                wndSize = 200;

            m_wndSize = wndSize;
        }

        private void ttxtMvAvgWnd_Leave(object sender, EventArgs e)
        {
            int wndSize = 200;
            if (!Int32.TryParse(ttxtMvAvgWnd.Text, out wndSize))
                wndSize = 200;

            if (wndSize <= 0)
                wndSize = 200;

            ttxtMvAvgWnd.Text = wndSize.ToString();
        }

        private void ttxtDataLength_TextChanged(object sender, EventArgs e)
        {
            int dataLength = 0;
            if (!Int32.TryParse(ttxtDataLength.Text, out dataLength))
                dataLength = 0;

            if (dataLength < 0)
                dataLength = 0;

            m_dataLength = dataLength;
        }

        private void ttxtDataLength_Leave(object sender, EventArgs e)
        {
            int dataLength = 0;
            if (!Int32.TryParse(ttxtDataLength.Text, out dataLength))
                dataLength = 0;

            if (dataLength < 0)
                dataLength = 0;

            ttxtDataLength.Text = dataLength.ToString();
        }

        private void ttxtPlotEvery_Leave(object sender, EventArgs e)
        {
            int plotEvery = 1;
            if (!Int32.TryParse(ttxtPlotEvery.Text, out plotEvery))
                plotEvery = 1;

            if (plotEvery <= 0)
                plotEvery = 1;

            ttxtPlotEvery.Text = plotEvery.ToString();
        }

        private void ttxtPlotEvery_TextChanged(object sender, EventArgs e)
        {
            int plotEvery = 1;
            if (!Int32.TryParse(ttxtPlotEvery.Text, out plotEvery))
                plotEvery = 1;

            if (plotEvery <= 0)
                plotEvery = 1;

            m_plotEvery = plotEvery;
        }


    }
}
