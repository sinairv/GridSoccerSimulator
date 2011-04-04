using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.FeedForwardNet
{
    public class FFBackpropLearning : IFeedForwardLearningAlgorithm
    {
        private FeedForwardNetwork m_ffn;
        public FFBackpropLearning(FeedForwardNetwork ffn)
            : this(ffn, 0.1, 0.0)
        {
        }

        public FFBackpropLearning(FeedForwardNetwork ffn, double learningRate, double momentum)
        {
            m_ffn = ffn;
            this.LearningRate = learningRate;
            this.Momentum = momentum;
        }

        public double LearningRate { get; set; }

        public double Momentum { get; set; }


        public void RunOneIteration(bool updateNeuronWeights, double[] inputs, double[] targetOutputs, out double errorBefore, out double errorAfter)
        {
            double[] curOutput = m_ffn.FeedForward(inputs);
            // the ordedr of arguments is important
            double[] absErrs = MathUtils.VectorSubtract(curOutput, targetOutputs);

            m_ffn.Backpropagate(absErrs);

            if (updateNeuronWeights)
            {
                for (int li = 0, lcount = m_ffn.NumLayers; li < lcount; li++)
                {
                    var layer = m_ffn.Layer(li);
                    for (int ni = 0, ncount = layer.Length; ni < ncount; ni++)
                    {
                        var neuron = layer[ni];

                        checked
                        {
                            for (int wi = 0, wcount = neuron.Weights.Length; wi < wcount; wi++)
                            {
                                neuron.PendingWeightIncrements[wi] =
                                    (-this.LearningRate * neuron.WeightGradients[wi])
                                    + (this.Momentum * neuron.PreviousWeightIncrements[wi]);
                            }

                            neuron.PendingBiasIncrement =
                                (-this.LearningRate * neuron.BiasGradient)
                                + (this.Momentum * neuron.PreviousBiasIncrement);
                        }

                        neuron.UpdateWeights();
                    }
                }
            }

            errorBefore = CalculateError(curOutput, targetOutputs);
            errorAfter = CalculateError(m_ffn.Compute(inputs), targetOutputs);
        }

        #region IFeedForwardLearningAlgorithm Members

        public void RunIncremental(double[] inputs, double[] targetOutputs, out double errorBefore, out double errorAfter)
        {
            RunOneIteration(true, inputs, targetOutputs, out errorBefore, out errorAfter);
        }

        public void RunBatch(double[][] inputs, double[][] outputs, out double errorBefore, out double errorAfter)
        {
            bool update = false;
            errorBefore = errorAfter = 0.0;
            double curErrBefore, curErrAfter;
            for (int si = 0, scount = inputs.Length; si < scount; si++)
            {
                if (si == scount - 1) update = true;

                RunOneIteration(update, inputs[si], outputs[si], out curErrBefore, out curErrAfter);
                errorBefore += curErrBefore;
                errorAfter += curErrAfter;
            }
        }

        #endregion

        private double CalculateError(double[] result, double[] target)
        {
            double sumErrs = 0.0;
            for (int i = 0, icount = result.Length; i < icount; i++)
            {
                double absErr = result[i] - target[i];
                sumErrs += absErr * absErr;
            }

            return 0.5 * sumErrs;
        }
    }
}
