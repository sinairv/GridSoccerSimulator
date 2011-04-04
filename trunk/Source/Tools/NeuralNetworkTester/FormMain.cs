using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NeuralNetwork;
using NeuralNetwork.Learning;
using NeuralNetwork.FeedForwardNet;

namespace NNetTest01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const int m_maxIters = 2000;
        private List<PointData> m_lstTrainData = new List<PointData>();


        private void FillORData()
        {
            m_lstTrainData.Clear();
            m_lstTrainData.Add(new PointData(0.0, 0.0, 0.0));
            m_lstTrainData.Add(new PointData(0.0, 1.0, 1.0));
            m_lstTrainData.Add(new PointData(1.0, 0.0, 1.0));
            m_lstTrainData.Add(new PointData(1.0, 1.0, 1.0));
        }

        private void FillANDData()
        {
            m_lstTrainData.Clear();
            m_lstTrainData.Add(new PointData(0.0, 0.0, 0.0));
            m_lstTrainData.Add(new PointData(0.0, 1.0, 0.0));
            m_lstTrainData.Add(new PointData(1.0, 0.0, 0.0));
            m_lstTrainData.Add(new PointData(1.0, 1.0, 1.0));
        }

        private void FillXORData()
        {
            m_lstTrainData.Clear();
            m_lstTrainData.Add(new PointData(0.0, 0.0, 0.0));
            m_lstTrainData.Add(new PointData(0.0, 1.0, 1.0));
            m_lstTrainData.Add(new PointData(1.0, 0.0, 1.0));
            m_lstTrainData.Add(new PointData(1.0, 1.0, 0.0));
        }

        private void PointDataToArray(List<PointData> pdata, out double[][] inputs, out double[][] outputs)
        {
            inputs = new double[pdata.Count][];
            outputs = new double[pdata.Count][];

            for (int r = 0, count = pdata.Count; r < count; r++)
            {
                var curPoint = pdata[r];

                inputs[r] = new double[2] { curPoint.X, curPoint.Y};
                outputs[r] = new double[1] { curPoint.Value };
            }
        }

        private void btnTestAForge_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.Series mainSeries;
            double[][] inputs;
            double[][] outputs;
            InitExp(out mainSeries, out inputs, out outputs);

            var ann = new ActivationNetwork(
                new SigmoidFunction(),  // threshold function
                2,  // 2 inputs
                GetTopoplogy());

            //ann.SetLastLayerActivationFunction(new LinearFunction());

            var learn = new BackPropagationLearning(ann);

            learn.LearningRate = 0.1;
            //learn.Momentum = 0.4;

            double err = Double.MaxValue;

            for (int iter = 0; err > 0.01 && iter < m_maxIters; iter++)
            {
                err = learn.RunEpoch(inputs, outputs);

                mainSeries.Points.AddY(err);

                if (iter % 400 == 0)
                {
                    Application.DoEvents();
                }
            }
        }

        private int[] GetTopoplogy()
        {
            string[] strs = textBox1.Text.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> lstTop = new List<int>();

            foreach (var str in strs)
            {
                int n;
                if (Int32.TryParse(str, out n))
                    lstTop.Add(n);
            }

            return lstTop.ToArray();

        }

        private void btnBatchBProp_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.Series mainSeries;
            double[][] inputs;
            double[][] outputs;
            InitExp(out mainSeries, out inputs, out outputs);

            var ffn = new FeedForwardNetwork(
                new SigmoidFunction(), WeightInitMethods.NormalRandom, 1,
                2, // 2 inputs
                GetTopoplogy()); // 2 layers

            //ffn.SetLastLayerActivationFunction(new LinearFunction());

            var learn = new FFBackpropLearning(ffn, 0.1, 0.3);

            double err = Double.MaxValue;
            double errBefore;
            for (int iter = 0; err > 0.01 && iter < m_maxIters; iter++)
            {
                learn.RunBatch(inputs, outputs, out errBefore, out err);

                if (iter == 0)
                    mainSeries.Points.AddY(errBefore);

                mainSeries.Points.AddY(err);

                if (iter % 400 == 0)
                {
                    Application.DoEvents();
                }
            }
        }

        private void btnBatchRprop_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.Series mainSeries;
            double[][] inputs;
            double[][] outputs;
            InitExp(out mainSeries, out inputs, out outputs);

            var ffn = new FeedForwardNetwork(
                new BipolarSigmoidFunction(), WeightInitMethods.NormalRandom, 1,
                2, // 2 inputs
                GetTopoplogy()); // 2 layers

            //ffn.SetLastLayerActivationFunction(new LinearFunction());

            var learn = new FFRpropLearning(ffn);

            double err = Double.MaxValue;
            double errBefore;
            for (int iter = 0; err > 0.01 && iter < m_maxIters; iter++)
            {
                learn.RunBatch(inputs, outputs, out errBefore, out err);

                if (iter == 0)
                    mainSeries.Points.AddY(errBefore);

                mainSeries.Points.AddY(err);

                if (iter % 400 == 0)
                {
                    Application.DoEvents();
                }
            }
        }

        private void InitExp(out System.Windows.Forms.DataVisualization.Charting.Series mainSeries, out double[][] inputs, out double[][] outputs)
        {
            rtbOutput.Text = "";

            FillXORData();

            mainSeries = chart1.Series[0];
            mainSeries.Points.Clear();


            PointDataToArray(m_lstTrainData, out inputs, out outputs);
        }

        private void btnIncAForge_Click(object sender, EventArgs e)
        {

        }

        private void btnIncBPROP_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.Series mainSeries;
            double[][] inputs;
            double[][] outputs;
            InitExp(out mainSeries, out inputs, out outputs);

            var ffn = new FeedForwardNetwork(
                new SigmoidFunction(), WeightInitMethods.NormalRandom, 1,
                2, // 2 inputs
                GetTopoplogy()); // 2 layers

            //ffn.SetLastLayerActivationFunction(new LinearFunction());

            var learn = new FFBackpropLearning(ffn, 0.1, 0.0);

            double err = Double.MaxValue;
            double errBefore;
            for (int iter = 0; err > 0.01 && iter < m_maxIters; iter++)
            {
                learn.RunIncremental(inputs[iter %4], outputs[iter %4], out errBefore, out err);

                if (iter == 0)
                    mainSeries.Points.AddY(errBefore);

                mainSeries.Points.AddY(err);

                if (iter % 400 == 0)
                {
                    Application.DoEvents();
                }
            }
        }

        private void btnIncRProp_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.Series mainSeries;
            double[][] inputs;
            double[][] outputs;
            InitExp(out mainSeries, out inputs, out outputs);

            var ffn = new FeedForwardNetwork(
                new SigmoidFunction(), WeightInitMethods.NormalRandom, 1,
                2, // 2 inputs
                GetTopoplogy()); // 2 layers

            //ffn.SetLastLayerActivationFunction(new LinearFunction());

            var learn = new FFRpropLearning(ffn);

            double err = Double.MaxValue;
            double errBefore;
            for (int iter = 0; err > 0.01 && iter < m_maxIters; iter++)
            {
                learn.RunIncremental(inputs[iter % 4], outputs[iter %4], out errBefore, out err);

                if (iter == 0)
                    mainSeries.Points.AddY(errBefore);

                mainSeries.Points.AddY(err);

                if (iter % 400 == 0)
                {
                    Application.DoEvents();
                }
            }
        }
    }
}
