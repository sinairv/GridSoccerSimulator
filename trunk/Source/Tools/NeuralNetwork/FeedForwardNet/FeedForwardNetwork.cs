using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.FeedForwardNet
{
    public class FeedForwardNetwork
    {
        private FeedForwardNeuron[][] m_net = null;

        public FeedForwardNetwork(IActivationFunction func, WeightInitMethods initMethod, double weightInitParam,
            int numInputs, params int[] numNeuronsPerLayer)
        {
            this.NumInputs = numInputs;
            this.NumLayers = numNeuronsPerLayer.Length;
            this.NumOutputs = numNeuronsPerLayer[NumLayers - 1];

            m_net = new FeedForwardNeuron[numNeuronsPerLayer.Length][];
            for (int i = 0; i < m_net.Length; i++)
            {
                int ncount = numNeuronsPerLayer[i];
                int prevLayerNCount = (i == 0) ? numInputs : numNeuronsPerLayer[i - 1];

                m_net[i] = new FeedForwardNeuron[ncount];
                for (int ni = 0; ni < ncount; ni++)
                {
                    m_net[i][ni] = new FeedForwardNeuron(prevLayerNCount, func, initMethod, weightInitParam);
                }
            }
        }

        public int NumInputs { get; private set; }
        public int NumLayers { get; private set; }
        public int NumOutputs { get; private set; }

        public double[] GetNeuronWeights(int layer, int neuIndex)
        {
            return m_net[layer][neuIndex].Weights;
        }

        public double GetNeuronBias(int layer, int neuIndex)
        {
            return m_net[layer][neuIndex].Bias;
        }

        public FeedForwardNeuron[] Layer(int i)
        {
            return m_net[i];
        }

        public void SetLayerActivationFunction(int layerIndex, IActivationFunction theFunc)
        {
            FeedForwardNeuron[] theLayer = m_net[layerIndex];
            for (int i = 0; i < theLayer.Length; i++)
            {
                theLayer[i].ActivationFunction = theFunc;
            }
        }

        public void SetLastLayerActivationFunction(IActivationFunction theFunc)
        {
            int layerIndex = this.NumLayers - 1;
            SetLayerActivationFunction(layerIndex, theFunc);
        }

        public double[] Compute(double[] input)
        {
            double[] curInput = input;
            double[] curOutput = null;

            for (int layer = 0, lcount = m_net.Length; layer < lcount; layer++)
            {
                curOutput = new double[m_net[layer].Length];

                for (int neu = 0, neuCount = m_net[layer].Length; neu < neuCount; neu++)
                {
                    curOutput[neu] = m_net[layer][neu].Compute(curInput);
                }

                curInput = curOutput;
            }

            return curOutput;
        }

        public double[] FeedForward(double[] input)
        {
            double[] curInput = input;
            double[] curOutput = null;

            for (int layer = 0, lcount = m_net.Length; layer < lcount; layer++)
            {
                curOutput = new double[m_net[layer].Length];

                for (int neu = 0, neuCount = m_net[layer].Length; neu < neuCount; neu++)
                {
                    curOutput[neu] = m_net[layer][neu].Feedforward(curInput);    
                }

                curInput = curOutput;
            }

            return curOutput;
        }

        public void Backpropagate(double[] bpError)
        {
            if (bpError.Length != this.NumOutputs)
                throw new ArgumentException("output length does not match the number of outputs of the network");

            double[] curBpErrorIn = bpError;

            for(int layer = m_net.Length - 1; layer >= 0; layer --)
            {
                int numLayerInputs = m_net[layer][0].NumInputs;
                double[] curLayerBpErrOut = new double[numLayerInputs];

                for(int ni = 0, ncount = m_net[layer].Length; ni < ncount; ni++)
                {
                    double[] curNeuBpErrOut = m_net[layer][ni].Backpropagate(curBpErrorIn[ni]);
                    curLayerBpErrOut = MathUtils.VectorAdd(curLayerBpErrOut, curNeuBpErrOut);
                }

                curBpErrorIn = curLayerBpErrOut;
            }

        }


        #region Static Stuff
        public static Random UniformRandomGenerator = new Random();
        public static NormalRandomGenerator NormalRandomGenerator = new NormalRandomGenerator();
        #endregion
    }
}
