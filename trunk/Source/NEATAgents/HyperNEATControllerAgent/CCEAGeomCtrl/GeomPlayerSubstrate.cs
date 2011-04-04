using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.CPPNs;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;

namespace GridSoccer.HyperNEATControllerAgent.CCEAGeomCtrl
{
    public class GeomPlayerSubstrate : ISubstrate
    {
        private float m_start = 0.0f;
        private float m_delta = 0.0f;
        private bool m_useBias = false;
        private int m_cppnOutIndex = 0;

        protected IActivationFunction m_activationFunction;
        protected NeuronGeneList m_neurons;

        int m_inputsCount = 4;
        int m_outputsCount = 9;

        public GeomPlayerSubstrate(bool useBias, 
            IActivationFunction activationFunction, int cppnOutIndex)
        {
            m_delta = 2.0f / 5.0f;
            m_start = -1.0f + (m_delta * 0.5f);

            m_activationFunction = activationFunction;

            m_neurons = new NeuronGeneList(m_inputsCount + m_outputsCount + (m_useBias ? 1 : 0));

            uint curNeurId = 0;
            if (m_useBias)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Bias, ActivationFunctionFactory.GetActivationFunction("NullFn")));
            }

            for (int i = 0; i < m_inputsCount; i++)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Input, ActivationFunctionFactory.GetActivationFunction("NullFn")));
            }

            for (int i = 0; i < m_outputsCount; i++)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Output, m_activationFunction));
            }

        }
        public INetwork GenerateNetwork(INetwork CPPN)
        {
            return GenerateGenome(CPPN).Decode(null);
        }

        private void GetInputNodeCoordinate(int nodeIdx, out float x, out float y)
        {
            switch (nodeIdx)
            {
                case 0:
                    x = 0.0f;
                    y = m_delta * 0.5f;
                    break;
                case 1:
                    x = m_delta * 0.5f;
                    y = 0.0f;
                    break;
                case 2:
                    x = 0.0f;
                    y = -m_delta * 0.5f;
                    break;
                case 3:
                    x = -m_delta * 0.5f;
                    y = 0.0f;
                    break;
                default:
                    throw new Exception("Invalid input node index");
            }
        }

        private void GetOutputNodeCoordinate(int nodeIdx, out float x, out float y)
        {
            switch (nodeIdx)
            {
                case 0:
                    x = 0.0f;
                    y = 0.0f;
                    return;
                case 1:
                    x = 0.0f;
                    y = m_start + 3 * m_delta;
                    return;
                case 2:
                    x = m_start + 3 * m_delta;
                    y = 0.0f;
                    return;
                case 3:
                    x = 0.0f;
                    y = m_start + 1 * m_delta;
                    return;
                case 4:
                    x = m_start + 1 * m_delta;
                    y = 0.0f;
                    return;
                case 5:
                    x = 0.0f;
                    y = m_start + 4 * m_delta;
                    return;
                case 6:
                    x = m_start + 4 * m_delta;
                    y = 0.0f;
                    return;
                case 7:
                    x = 0.0f;
                    y = m_start + 0 * m_delta;
                    return;
                case 8:
                    x = m_start + 0 * m_delta;
                    y = 0.0f;
                    return;
                default:
                    throw new Exception("Invalid output node index");
            }
        }

        public NeatGenome GenerateGenome(INetwork network)
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
            uint inputsEnd = (uint)(inputsStart + m_inputsCount);
            uint outputsStart = inputsEnd;
            uint outputsEnd = (uint)(outputsStart + m_outputsCount);

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
                for (int ni = 0; ni < m_outputsCount; ni++)
                {
                    float tempX, tempY;
                    GetOutputNodeCoordinate(ni, out tempX, out tempY);
                    coordinates[2] = tempX;
                    coordinates[3] = tempY;

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

            for (int inpi = 0; inpi < m_inputsCount; inpi++)
            {
                float inpx, inpy;
                GetInputNodeCoordinate(inpi, out inpx, out inpy);
                coordinates[0] = inpx;
                coordinates[1] = inpy;

                for (int outi = 0; outi < m_outputsCount; outi++)
                {
                    float outx, outy;
                    GetOutputNodeCoordinate(outi, out outx, out outy);

                    coordinates[2] = outx;
                    coordinates[3] = outy;

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

            return new SharpNeatLib.NeatGenome.NeatGenome(0, m_neurons, connections, m_inputsCount, m_outputsCount);
        }
    }
}
