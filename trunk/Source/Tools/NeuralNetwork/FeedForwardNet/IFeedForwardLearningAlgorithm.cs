using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.FeedForwardNet
{
    public interface IFeedForwardLearningAlgorithm
    {
        void RunOneIteration(bool updateNeuronWeights, double[] inputs, double[] targetOutputs, out double errorBefore, out double errorAfter);

        void RunIncremental(double[] inputs, double[] outputs, out double errorBefore, out double errorAfter);
        void RunBatch(double[][] inputs, double[][] outputs, out double errorBefore, out double errorAfter);
    }
}
