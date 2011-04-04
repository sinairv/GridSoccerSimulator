using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;

namespace SharpNeatLib.CPPNs
{
    public class Substrate
    {
        public uint inputCount;
        public uint outputCount;
        public uint hiddenCount;

        public float inputDelta;
        public float hiddenDelta;
        public float outputDelta;

        public double threshold;
        public double weightRange;
        public IActivationFunction activationFunction;
        public NeuronGeneList neurons;
        
        public Substrate()
        {
        }
        public Substrate(uint input, uint output, uint hidden, IActivationFunction function)
        {
            weightRange = HyperNEATParameters.weightRange;
            threshold = HyperNEATParameters.threshold;

            inputCount = input;
            outputCount = output;
            hiddenCount = hidden;
            activationFunction = function;

            inputDelta = 2.0f / (inputCount);
            if (hiddenCount != 0)
                hiddenDelta = 2.0f / (hiddenCount);
            else
                hiddenDelta = 0;
            outputDelta = 2.0f / (outputCount);

            //SharpNEAT requires that the neuronlist be input|bias|output|hidden
            neurons=new NeuronGeneList((int)(inputCount + outputCount+ hiddenCount));
            //setup the inputs
            for (uint a = 0; a < inputCount; a++)
            {
                neurons.Add(new NeuronGene(a, NeuronType.Input, activationFunction));
            }

            //setup the outputs
            for (uint a = 0; a < outputCount; a++)
            {
                neurons.Add(new NeuronGene(a + inputCount, NeuronType.Output, activationFunction));
            }
            for (uint a = 0; a < hiddenCount; a++)
            {
                neurons.Add(new NeuronGene(a + inputCount+outputCount, NeuronType.Hidden, activationFunction));
            }
        }

        public INetwork generateNetwork(INetwork CPPN)
        {
            return generateGenome(CPPN).Decode(null);
        }

        public virtual NeatGenome.NeatGenome generateGenome(INetwork network)
        {
            float[] coordinates = new float[4];
            float output;
            uint connectionCounter = 0;
            int iterations = 2 * (network.TotalNeuronCount - (network.InputNeuronCount + network.OutputNeuronCount)) + 1;
            ConnectionGeneList connections=new ConnectionGeneList();
            if (hiddenCount > 0)
            {
                coordinates[0] = -1 + inputDelta / 2.0f;
                coordinates[1] = -1;
                coordinates[2] = -1 + hiddenDelta / 2.0f;
                coordinates[3] = 0;
                for (uint input = 0; input < inputCount; input++, coordinates[0] += inputDelta)
                {
                    coordinates[2] = -1 + hiddenDelta / 2.0f;
                    for (uint hidden = 0; hidden < hiddenCount; hidden++, coordinates[2] += hiddenDelta)
                    {
                        network.ClearSignals();
                        network.SetInputSignals(coordinates);
                        network.MultipleSteps(iterations);
                        output = network.GetOutputSignal(0);

                        if (Math.Abs(output) > threshold)
                        {
                            float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                            connections.Add(new ConnectionGene(connectionCounter++, input, hidden + inputCount + outputCount, weight));
                        }
                    }
                }
                coordinates[0] = -1 + hiddenDelta / 2.0f;
                coordinates[1] = 0;
                coordinates[2] = -1 + outputDelta / 2.0f;
                coordinates[3] = 1;
                for (uint hidden = 0; hidden < hiddenCount; hidden++, coordinates[0] += hiddenDelta)
                {
                    coordinates[2] = -1 + outputDelta / 2.0f;
                    for (uint outputs = 0; outputs < outputCount; outputs++, coordinates[2] += outputDelta)
                    {
                        network.ClearSignals();
                        network.SetInputSignals(coordinates);
                        network.MultipleSteps(iterations);
                        output = network.GetOutputSignal(0);

                        if (Math.Abs(output) > threshold)
                        {
                            float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                            connections.Add(new ConnectionGene(connectionCounter++, hidden + inputCount + outputCount, outputs + inputCount, weight));
                        }
                    }
                }
            }
            else
            {
                coordinates[0] = -1 + inputDelta / 2.0f;
                coordinates[1] = -1;
                coordinates[2] = -1 + outputDelta / 2.0f;
                coordinates[3] = 1;
                for (uint input = 0; input < inputCount; input++, coordinates[0] += inputDelta)
                {
                    coordinates[2] = -1 + outputDelta / 2.0f;
                    for (uint outputs = 0; outputs < outputCount; outputs++, coordinates[2] += outputDelta)
                    {
                        network.ClearSignals();
                        network.SetInputSignals(coordinates);
                        network.MultipleSteps(iterations);
                        output = network.GetOutputSignal(0);

                        if (Math.Abs(output) > threshold)
                        {
                            float weight = (float)(((Math.Abs(output) - (threshold)) / (1 - threshold)) * weightRange * Math.Sign(output));
                            connections.Add(new ConnectionGene(connectionCounter++, input, outputs + inputCount, weight));
                        }
                    }
                }
            }
            return new SharpNeatLib.NeatGenome.NeatGenome(0, neurons, connections, (int)inputCount, (int)outputCount);
        }

    }
}
