using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.CPPNs;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;

namespace SharpNeatLib.CPPNs
{
    public class TwoLayerSandwichSubstrate : ISubstrate
    {
        protected int m_rows, m_cols;
        protected bool m_useBias;

        protected IActivationFunction m_activationFunction;
        protected NeuronGeneList m_neurons;

        protected float m_rowDelta = 0.0f;
        protected float m_colDelta = 0.0f;

        protected int m_cppnOutIndex = 0;

        public TwoLayerSandwichSubstrate(int rows, int cols, bool useBias, IActivationFunction activationFunction)
            : this(rows, cols, useBias, activationFunction, 0)
        {
        }

        public TwoLayerSandwichSubstrate(int rows, int cols, bool useBias, 
            IActivationFunction activationFunction, int cppnOutIndex)
        {
            m_rows = rows;
            m_cols = cols;
            m_useBias = useBias;
            m_cppnOutIndex = cppnOutIndex;

            m_rowDelta = 2.0f / m_rows;
            m_colDelta = 2.0f / m_cols;

            m_activationFunction = activationFunction;

            m_neurons = new NeuronGeneList(2 * (m_rows * m_cols) + (m_useBias ? 1 : 0) );

            uint curNeurId = 0;
            if (m_useBias)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Bias, ActivationFunctionFactory.GetActivationFunction("NullFn")));
            }

            for (int i = 0, len = m_rows * m_cols; i < len; i++)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Input, ActivationFunctionFactory.GetActivationFunction("NullFn")));
            }

            for (int i = 0, len = m_rows * m_cols; i < len; i++)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Output, m_activationFunction));
            }
        }


        public NeuralNetwork.INetwork GenerateNetwork(NeuralNetwork.INetwork CPPN)
        {
            return GenerateGenome(CPPN).Decode(null);
        }

        public virtual NeatGenome.NeatGenome GenerateGenome(NeuralNetwork.INetwork network)
        {
            int maxIterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;

            // TODO:
            maxIterations = Math.Min(maxIterations, 4);

            double epsilon = 0.0;
            double threshold = HyperNEATParameters.threshold;
            double weightRange = HyperNEATParameters.weightRange;

            // store constant ids for later references

            uint biasid = 0u;
            uint inputsStart = m_useBias ? 1u : 0u;
            uint inputsEnd   = (uint)(inputsStart + (m_rows * m_cols));
            uint outputsStart = inputsEnd;
            uint outputsEnd = (uint)(outputsStart + (m_rows * m_cols));
           
            float[] coordinates = new float[4];
            float output;
            uint connectionCounter = 0;
            ConnectionGeneList connections = new ConnectionGeneList();

            if (m_useBias)
            {
                // we use the bias neuron on the center, and use the
                // 2nd output of the CPPN to compute its weight
                coordinates[0] = 0.0f;
                coordinates[1] = 0.0f;

                // add the bias link to all output neurons
                for (int ni = 0, ncount = m_rows * m_cols; ni < ncount; ni++ )
                {
                    int row = (ni / m_cols);
                    int col = (ni % m_cols);

                    coordinates[2] = (-1.0f) + (m_colDelta * (col + 0.5f));
                    coordinates[3] = (-1.0f) + (m_rowDelta * (row + 0.5f));

                    network.ClearSignals();
                    network.SetInputSignals(coordinates);
                    network.RelaxNetwork(maxIterations, epsilon);
                    output = network.GetOutputSignal(m_cppnOutIndex + 1);

                    if (Math.Abs(output) > threshold)
                    {
                        float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                        connections.Add(new ConnectionGene(connectionCounter++, biasid, (uint)(outputsStart + ni), weight));
                    }
                }
            }

            // now add possible connections between all input output neuron pairs

            for (int inpi = 0, inpcount = m_rows * m_cols; inpi < inpcount; inpi++)
            {
                for (int outi = 0, outcount = m_rows * m_cols; outi < outcount; outi++)
                {
                    int inrow = (inpi / m_cols); int incol = (inpi % m_cols);
                    int outrow = (outi / m_cols); int outcol = (outi % m_cols);

                    coordinates[0] = (-1.0f) + (m_colDelta * (incol + 0.5f));
                    coordinates[1] = (-1.0f) + (m_rowDelta * (inrow + 0.5f));
                    coordinates[2] = (-1.0f) + (m_colDelta * (outcol + 0.5f));
                    coordinates[3] = (-1.0f) + (m_rowDelta * (outrow + 0.5f));

                    network.ClearSignals();
                    network.SetInputSignals(coordinates);
                    network.RelaxNetwork(maxIterations, epsilon);
                    output = network.GetOutputSignal(m_cppnOutIndex);

                    if (Math.Abs(output) > threshold)
                    {
                        float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                        connections.Add(new ConnectionGene(connectionCounter++, 
                            (uint)(inputsStart + inpi), (uint)(outputsStart + outi), weight));
                    }
                }
            }

            return new SharpNeatLib.NeatGenome.NeatGenome(0, m_neurons, connections, m_rows * m_cols, m_rows * m_cols);
        }

        public int PosToId(int row, int col, bool isInput)
        {
            int id = row * m_cols + col;
            if (m_useBias)
                id++;
            if (!isInput)
                id += m_rows * m_cols;

            return id;
        }

        public void IdToPos(int id, bool isInput, out int row, out int col)
        {
            int zeroBasedId = id;
            if (m_useBias) zeroBasedId--;
            if (!isInput)
                zeroBasedId -= m_rows * m_cols;

            row = (zeroBasedId / m_cols);
            col = (zeroBasedId % m_cols);
        }


    }

}
