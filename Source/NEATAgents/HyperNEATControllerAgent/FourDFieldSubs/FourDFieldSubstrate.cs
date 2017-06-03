using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.CPPNs;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeatGenome;

namespace GridSoccer.HyperNEATControllerAgent.FourDFieldSubs
{
    public class FourDFieldSubstrate : TwoLayerSandwichSubstrate
    {
        float m_homex = 0.0f;
        float m_homey = 0.0f;
        public FourDFieldSubstrate(float homex, float homey, int rows, int cols, bool useBias, 
            IActivationFunction activationFunction, int cppnOutIndex)
            : base(rows, cols, useBias, activationFunction, cppnOutIndex)
        {
            m_homex = homex;
            m_homey = homey;
        }

        public override SharpNeatLib.NeatGenome.NeatGenome GenerateGenome(INetwork network)
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
            uint inputsEnd = (uint)(inputsStart + (m_rows * m_cols));
            uint outputsStart = inputsEnd;
            uint outputsEnd = (uint)(outputsStart + (m_rows * m_cols));

            float[] coordinates = new float[8];

            coordinates[2] = coordinates[6] = m_homex;
            coordinates[3] = coordinates[7] = m_homey;

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
                for (int ni = 0, ncount = m_rows * m_cols; ni < ncount; ni++)
                {
                    int row = (ni / m_cols);
                    int col = (ni % m_cols);

                    coordinates[4] = (-1.0f) + (m_colDelta * (col + 0.5f));
                    coordinates[5] = (-1.0f) + (m_rowDelta * (row + 0.5f));

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
                    coordinates[4] = (-1.0f) + (m_colDelta * (outcol + 0.5f));
                    coordinates[5] = (-1.0f) + (m_rowDelta * (outrow + 0.5f));

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


    }
}
