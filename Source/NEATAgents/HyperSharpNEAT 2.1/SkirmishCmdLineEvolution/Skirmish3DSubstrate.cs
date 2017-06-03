//#define OUTPUT
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.CPPNs;
using System.Threading;
using System;

namespace SharpNeatLib.Experiments 
{
    public class Skirmish3DSubstrate : Substrate
    {
        private float m_zValue;

        /* This substrate configuration is a bit different than the ones normally used, as the different layers have
         * different numbers of nodes and I wanted them all to line up like this:
         * -0-0-0- Outputs
         * -00000- Hidden
         * -00000- Inputs
         * This way there is a clear correlation between the x values of the sensors and effectors.  To achieve this 
         * efficiently, I had to do some hacky stuff, which will have to be altered for different substrates.  The 
         * Substrate class has a much simpler and more generic way of querying the connections.
         */ 
        public Skirmish3DSubstrate(uint inputs, uint outputs, uint hidden, IActivationFunction function, float z)
            : base(inputs, outputs, hidden, function)
        {
            m_zValue = z;
        }

        public override NeatGenome.NeatGenome GenerateGenome(INetwork cppn)
        {
            ConnectionGeneList connections = new ConnectionGeneList((int)((inputCount * hiddenCount) + (hiddenCount * outputCount)));
            float[] coordinates = new float[6];
            float output;
            uint connectionCounter = 0;
            int iterations = 2 * (cppn.TotalNeuronCount - (cppn.InputNeuronCount + cppn.OutputNeuronCount)) + 1;
            // TODO:
            iterations = Math.Min(iterations, 4);

            coordinates[0] = -1 + inputDelta / 2.0f;
            coordinates[1] = -1;
            coordinates[2] = m_zValue;
            coordinates[3] = -1 + hiddenDelta / 2.0f;
            coordinates[4] = 0;
            coordinates[5] = m_zValue;

            for (uint source = 0; source < inputCount; source++, coordinates[0] += inputDelta)
            {
                coordinates[3] = -1 + hiddenDelta / 2.0f;
                for (uint target = 0; target < hiddenCount; target++, coordinates[3] += hiddenDelta)
                {

                    //Since there are an equal number of input and hidden nodes, we check these everytime
                    cppn.ClearSignals();
                    cppn.SetInputSignals(coordinates);
                    cppn.MultipleSteps(iterations);
                    output = cppn.GetOutputSignal(0);

                    if (Math.Abs(output) > threshold)
                    {
                        float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                        connections.Add(new ConnectionGene(connectionCounter++, source, target + inputCount + outputCount, weight));
                    }

                    //Since every other hidden node has a corresponding output node, we check every other time
                    if (target % 2 == 0)
                    {
                        cppn.ClearSignals();
                        coordinates[1] = 0;
                        coordinates[4] = 1;
                        cppn.SetInputSignals(coordinates);
                        cppn.MultipleSteps(iterations);
                        output = cppn.GetOutputSignal(0);

                        if (Math.Abs(output) > threshold)
                        {
                            float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                            connections.Add(new ConnectionGene(connectionCounter++, source + inputCount + outputCount, (target / 2) + inputCount, weight));
                        }
                        coordinates[1] = -1;
                        coordinates[4] = 0;

                    }
                }
            }

            return new SharpNeatLib.NeatGenome.NeatGenome(0, neurons, connections, (int)inputCount, (int)outputCount);
        }

        #region Commented out
        //public INetwork generateMultiNetwork(INetwork network, uint numberOfAgents)
        //{
        //    return generateMultiGenomeModulus(network, numberOfAgents).Decode(activationFunction);
        //}

//        public NeatGenome.NeatGenome generateMultiGenomeModulus(INetwork network, uint numberOfAgents)
//        {
//#if OUTPUT
//            System.IO.StreamWriter sw = new System.IO.StreamWriter("testfile.txt");
//#endif
//            float[] coordinates = new float[4];
//            float output;
//            uint connectionCounter = 0;

//            uint inputsPerAgent = inputCount / numberOfAgents;
//            uint hiddenPerAgent = hiddenCount / numberOfAgents;
//            uint outputsPerAgent = outputCount / numberOfAgents;

//            ConnectionGeneList connections = new ConnectionGeneList((int)((inputCount*hiddenCount)+(hiddenCount*outputCount)));

//            int iterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;

//            coordinates[0] = -1 + inputDelta / 2.0f;    //x1
//            coordinates[1] = -1;                        //y1 
//            coordinates[2] = -1 + hiddenDelta / 2.0f;   //x2
//            coordinates[3] = 0;                         //y2

//            for (uint agent = 0; agent < numberOfAgents; agent++)
//            {
//                coordinates[0] = -1 + (agent * inputsPerAgent * inputDelta) + inputDelta / 2.0f;
//                for (uint source = 0; source < inputsPerAgent; source++, coordinates[0] += inputDelta)
//                {
//                    coordinates[2] = -1 + (agent * hiddenPerAgent * hiddenDelta) + hiddenDelta / 2.0f;
//                    for (uint target = 0; target < hiddenPerAgent; target++, coordinates[2] += hiddenDelta)
//                    {

//                        //Since there are an equal number of input and hidden nodes, we check these everytime
//                        network.ClearSignals();
//                        network.SetInputSignals(coordinates);
//                        ((FloatFastConcurrentNetwork)network).MultipleStepsWithMod(iterations, (int)numberOfAgents);
//                        output = network.GetOutputSignal(0);
//#if OUTPUT
//                            foreach (double d in inputs)
//                                sw.Write(d + " ");
//                            sw.Write(output);
//                            sw.WriteLine();
//#endif
//                        if (Math.Abs(output) > threshold)
//                        {
//                            float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
//                            connections.Add(new ConnectionGene(connectionCounter++, (agent*inputsPerAgent) + source, (agent*hiddenPerAgent) + target + inputCount + outputCount, weight));
//                        }

//                        //Since every other hidden node has a corresponding output node, we check every other time
//                        if (target % 2 == 0)
//                        {
//                            network.ClearSignals();
//                            coordinates[1] = 0;
//                            coordinates[3] = 1;
//                            network.SetInputSignals(coordinates);
//                            ((FloatFastConcurrentNetwork)network).MultipleStepsWithMod(iterations, (int)numberOfAgents);
//                            output = network.GetOutputSignal(0);
//#if OUTPUT
//                            foreach (double d in inputs)
//                                sw.Write(d + " ");
//                            sw.Write(output);
//                            sw.WriteLine();
//#endif
//                            if (Math.Abs(output) > threshold)
//                            {
//                                float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
//                                connections.Add(new ConnectionGene(connectionCounter++, (agent*hiddenPerAgent) + source + inputCount + outputCount, ((outputsPerAgent * agent) + ((target) / 2)) + inputCount, weight));
//                            }
//                            coordinates[1] = -1;
//                            coordinates[3] = 0;

//                        }
//                    }
//                }
//            }
//#if OUTPUT
//            sw.Flush();
//#endif
//            //Console.WriteLine(count);
//            //Console.ReadLine();
//            return new SharpNeatLib.NeatGenome.NeatGenome(0, neurons, connections, (int)inputCount, (int)outputCount);
        //        }

        #endregion
    }
}
