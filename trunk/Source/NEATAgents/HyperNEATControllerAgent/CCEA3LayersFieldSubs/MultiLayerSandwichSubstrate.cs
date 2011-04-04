using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;
using SharpNeatLib.CPPNs;

namespace GridSoccer.HyperNEATControllerAgent.CCEA3LayersFieldSubs
{
    public class MultiLayerSandwichSubstrate : ISubstrate
    {
        protected int m_rows, m_cols;
        protected bool m_useBias;

        protected IActivationFunction m_activationFunction;
        protected NeuronGeneList m_neurons;

        protected float m_rowDelta = 0.0f;
        protected float m_colDelta = 0.0f;

        protected int m_cppnOutIndex = 0;
        protected int m_numLayers = 2;

        public MultiLayerSandwichSubstrate(int rows, int cols, int numLayers, bool useBias, IActivationFunction activationFunction)
        {
            m_rows = rows;
            m_cols = cols;
            m_numLayers = numLayers;
            m_useBias = useBias;

            m_rowDelta = 2.0f / m_rows;
            m_colDelta = 2.0f / m_cols;

            m_activationFunction = activationFunction;

            // add only one bias 
            m_neurons = new NeuronGeneList(m_numLayers * (m_rows * m_cols) + (m_useBias ? 1 : 0) );

            uint curNeurId = 0;

            // inserting the bias if needed
            if (m_useBias)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Bias, ActivationFunctionFactory.GetActivationFunction("NullFn")));
            }

            // inserting input layer neurons
            for (int i = 0, len = m_rows * m_cols; i < len; i++)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Input, ActivationFunctionFactory.GetActivationFunction("NullFn")));
            }

            // inserting output layer neurons
            for (int i = 0, len = m_rows * m_cols; i < len; i++)
            {
                m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Output, m_activationFunction));
            }

            // inserting hidden layer neurons
            for (int li = 0; li < m_numLayers - 2; li++)
            {
                for (int i = 0, len = m_rows * m_cols; i < len; i++)
                {
                    m_neurons.Add(new NeuronGene(curNeurId++, NeuronType.Hidden, m_activationFunction));
                }
            }
        }

        public SharpNeatLib.NeuralNetwork.INetwork GenerateNetwork(INetwork CPPN)
        {
            return GenerateGenome(CPPN).Decode(null);
        }

        public virtual NeatGenome GenerateGenome(SharpNeatLib.NeuralNetwork.INetwork cppnNetwork)
        {
            ConnectionGeneList connections = new ConnectionGeneList();

            int maxIterations = 2 * (cppnNetwork.TotalNeuronCount - (cppnNetwork.InputNeuronCount + cppnNetwork.OutputNeuronCount)) + 1;

            // TODO:
            maxIterations = Math.Min(maxIterations, 4);

            // TODO:
            double epsilon = 0.0;
            double threshold = HyperNEATParameters.threshold;
            double weightRange = HyperNEATParameters.weightRange;


            uint biasid = 0u;
            uint inputsStart = biasid + (m_useBias ? 1u : 0u);
            uint inputsEnd = (uint)(inputsStart + (m_rows * m_cols));
            uint outputsStart = inputsEnd;
            uint outputsEnd = (uint)(outputsStart + (m_rows * m_cols));
            uint firstHiddenStart = outputsEnd;

            float[] coordinates = new float[4];
            float output;
            uint connectionCounter = 0u;

            if (m_useBias)
            {
                // we use the bias neuron on the center, and use the
                // 2nd output of the CPPN to compute its weight
                coordinates[0] = 0.0f;
                coordinates[1] = 0.0f;

                // add the bias link to all neurons of the next layer
                for (int ni = 0, ncount = m_rows * m_cols; ni < ncount; ni++)
                {
                    int row = (ni / m_cols);
                    int col = (ni % m_cols);

                    coordinates[2] = (-1.0f) + (m_colDelta * (col + 0.5f));
                    coordinates[3] = (-1.0f) + (m_rowDelta * (row + 0.5f));

                    cppnNetwork.ClearSignals();
                    cppnNetwork.SetInputSignals(coordinates);
                    cppnNetwork.RelaxNetwork(maxIterations, epsilon);

                    for (int li = 0; li < m_numLayers - 1; li++)
                    {
                        output = cppnNetwork.GetOutputSignal(li * 2 + 1);
                        uint lstart = (uint)(firstHiddenStart  + li*(m_cols * m_rows));
                        if (li == m_numLayers - 2) 
                            lstart = outputsStart;

                        if (Math.Abs(output) > threshold)
                        {
                            float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                            connections.Add(new ConnectionGene(connectionCounter++, 
                                biasid, (uint)(lstart + ni), weight));
                        }
                    }
                }
            }

            // now add the connections
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

                    cppnNetwork.ClearSignals();
                    cppnNetwork.SetInputSignals(coordinates);
                    cppnNetwork.RelaxNetwork(maxIterations, epsilon);

                    for (int li = 0; li < m_numLayers - 1; li++)
                    {
                        output = cppnNetwork.GetOutputSignal(m_useBias ? li * 2 : li);

                        uint dststart = (uint)(firstHiddenStart + li * (m_cols * m_rows));
                        if (li == m_numLayers - 2)
                            dststart = outputsStart;

                        uint srcstart = (uint)(firstHiddenStart + (li - 1) * (m_cols * m_rows));
                        if (li == 0)
                            srcstart = inputsStart;

                        if (Math.Abs(output) > threshold)
                        {
                            float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                            connections.Add(new ConnectionGene(connectionCounter++,
                                (uint)(srcstart + inpi), (uint)(dststart + outi), weight));
                        }
                    }
                }
            }


            return new SharpNeatLib.NeatGenome.NeatGenome(0, m_neurons, connections, m_rows * m_cols, m_rows * m_cols);
        }

        //protected void ConnectLayerToNext(int layerIndex, ConnectionGeneList connections, INetwork cppnNetwork)
        //{
        //    int maxIterations = 2 * (cppnNetwork.TotalNeuronCount - (cppnNetwork.InputNeuronCount + cppnNetwork.OutputNeuronCount)) + 1;

        //    // TODO:
        //    maxIterations = Math.Min(maxIterations, 4);

        //    // TODO:
        //    double epsilon = 0.01;
        //    double threshold = HyperNEATParameters.threshold;
        //    double weightRange = HyperNEATParameters.weightRange;

        //    uint biasid = (uint)(layerIndex * (m_cols * m_rows + (m_useBias ? 1 : 0)));
        //    uint inputsStart = biasid + ( m_useBias ? 1u : 0u );
        //    uint inputsEnd = (uint)(inputsStart + (m_rows * m_cols));
        //    // the bias of the hidden layer should be considered, 
        //    // but there's no bias added for the output layer
        //    uint outputsStart = inputsEnd + ((m_useBias && layerIndex < m_numLayers - 2 ) ? 1u : 0u); 
        //    uint outputsEnd = (uint)(outputsStart + (m_rows * m_cols));

        //    float[] coordinates = new float[4];
        //    float output;
        //    uint connectionCounter = (uint)connections.Count;
        //    if (m_useBias)
        //    {
        //        // we use the bias neuron on the center, and use the
        //        // 2nd output of the CPPN to compute its weight
        //        coordinates[0] = 0.0f;
        //        coordinates[1] = 0.0f;

        //        // add the bias link to all output neurons
        //        for (int ni = 0, ncount = m_rows * m_cols; ni < ncount; ni++)
        //        {
        //            int row = (ni / m_cols);
        //            int col = (ni % m_cols);

        //            coordinates[2] = (-1.0f) + (m_colDelta * (col + 0.5f));
        //            coordinates[3] = (-1.0f) + (m_rowDelta * (row + 0.5f));

        //            cppnNetwork.ClearSignals();
        //            cppnNetwork.SetInputSignals(coordinates);
        //            cppnNetwork.RelaxNetwork(maxIterations, epsilon);
        //            output = cppnNetwork.GetOutputSignal(m_cppnOutIndex + 1);

        //            if (Math.Abs(output) > threshold)
        //            {
        //                float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
        //                connections.Add(new ConnectionGene(connectionCounter++, biasid, (uint)(outputsStart + ni), weight));
        //            }
        //        }
        //    }

        //    // now add possible connections between all input output neuron pairs

        //    for (int inpi = 0, inpcount = m_rows * m_cols; inpi < inpcount; inpi++)
        //    {
        //        for (int outi = 0, outcount = m_rows * m_cols; outi < outcount; outi++)
        //        {
        //            int inrow = (inpi / m_cols); int incol = (inpi % m_cols);
        //            int outrow = (outi / m_cols); int outcol = (outi % m_cols);

        //            coordinates[0] = (-1.0f) + (m_colDelta * (incol + 0.5f));
        //            coordinates[1] = (-1.0f) + (m_rowDelta * (inrow + 0.5f));
        //            coordinates[2] = (-1.0f) + (m_colDelta * (outcol + 0.5f));
        //            coordinates[3] = (-1.0f) + (m_rowDelta * (outrow + 0.5f));

        //            cppnNetwork.ClearSignals();
        //            cppnNetwork.SetInputSignals(coordinates);
        //            cppnNetwork.RelaxNetwork(maxIterations, epsilon);
        //            output = cppnNetwork.GetOutputSignal(m_cppnOutIndex);

        //            if (Math.Abs(output) > threshold)
        //            {
        //                float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
        //                connections.Add(new ConnectionGene(connectionCounter++,
        //                    (uint)(inputsStart + inpi), (uint)(outputsStart + outi), weight));
        //            }
        //        }
        //    }

        //}

        //public virtual NeatGenome GenerateGenome(SharpNeatLib.NeuralNetwork.INetwork cppnNetwork)
        //{
        //    int maxIterations = 2 * (cppnNetwork.TotalNeuronCount - (cppnNetwork.InputNeuronCount + cppnNetwork.OutputNeuronCount)) + 1;

        //    // TODO:
        //    maxIterations = Math.Min(maxIterations, 4);

        //    // TODO:
        //    double epsilon     = 0.01;
        //    double threshold   = HyperNEATParameters.threshold;
        //    double weightRange = HyperNEATParameters.weightRange;

        //    // store constant ids for later references

        //    uint biasid = 0u;
        //    uint inputsStart = m_useBias ? 1u : 0u;
        //    uint inputsEnd = (uint)(inputsStart + (m_rows * m_cols));
        //    uint outputsStart = inputsEnd;
        //    uint outputsEnd = (uint)(outputsStart + (m_rows * m_cols));

        //    float[] coordinates = new float[4];
        //    float output;
        //    uint connectionCounter = 0;
        //    ConnectionGeneList connections = new ConnectionGeneList();

        //    if (m_useBias)
        //    {
        //        // we use the bias neuron on the center, and use the
        //        // 2nd output of the CPPN to compute its weight
        //        coordinates[0] = 0.0f;
        //        coordinates[1] = 0.0f;

        //        // add the bias link to all output neurons
        //        for (int ni = 0, ncount = m_rows * m_cols; ni < ncount; ni++)
        //        {
        //            int row = (ni / m_cols);
        //            int col = (ni % m_cols);

        //            coordinates[2] = (-1.0f) + (m_colDelta * (col + 0.5f));
        //            coordinates[3] = (-1.0f) + (m_rowDelta * (row + 0.5f));

        //            cppnNetwork.ClearSignals();
        //            cppnNetwork.SetInputSignals(coordinates);
        //            cppnNetwork.RelaxNetwork(maxIterations, epsilon);
        //            output = cppnNetwork.GetOutputSignal(m_cppnOutIndex + 1);

        //            if (Math.Abs(output) > threshold)
        //            {
        //                float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
        //                connections.Add(new ConnectionGene(connectionCounter++, biasid, (uint)(outputsStart + ni), weight));
        //            }
        //        }
        //    }

        //    // now add possible connections between all input output neuron pairs

        //    for (int inpi = 0, inpcount = m_rows * m_cols; inpi < inpcount; inpi++)
        //    {
        //        for (int outi = 0, outcount = m_rows * m_cols; outi < outcount; outi++)
        //        {
        //            int inrow = (inpi / m_cols); int incol = (inpi % m_cols);
        //            int outrow = (outi / m_cols); int outcol = (outi % m_cols);

        //            coordinates[0] = (-1.0f) + (m_colDelta * (incol + 0.5f));
        //            coordinates[1] = (-1.0f) + (m_rowDelta * (inrow + 0.5f));
        //            coordinates[2] = (-1.0f) + (m_colDelta * (outcol + 0.5f));
        //            coordinates[3] = (-1.0f) + (m_rowDelta * (outrow + 0.5f));

        //            cppnNetwork.ClearSignals();
        //            cppnNetwork.SetInputSignals(coordinates);
        //            cppnNetwork.RelaxNetwork(maxIterations, epsilon);
        //            output = cppnNetwork.GetOutputSignal(m_cppnOutIndex);

        //            if (Math.Abs(output) > threshold)
        //            {
        //                float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
        //                connections.Add(new ConnectionGene(connectionCounter++,
        //                    (uint)(inputsStart + inpi), (uint)(outputsStart + outi), weight));
        //            }
        //        }
        //    }

        //    return new SharpNeatLib.NeatGenome.NeatGenome(0, m_neurons, connections, m_rows * m_cols, m_rows * m_cols);
        //}

        //public int PosToId(int row, int col, bool isInput)
        //{
        //    int id = row * m_cols + col;
        //    if (m_useBias)
        //        id++;
        //    if (!isInput)
        //        id += m_rows * m_cols;

        //    return id;
        //}

        //public void IdToPos(int id, bool isInput, out int row, out int col)
        //{
        //    int zeroBasedId = id;
        //    if (m_useBias) zeroBasedId--;
        //    if (!isInput)
        //        zeroBasedId -= m_rows * m_cols;

        //    row = (zeroBasedId / m_cols);
        //    col = (zeroBasedId % m_cols);
        //}
    }
}
