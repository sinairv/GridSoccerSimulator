using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.FeedForwardNet
{
    public class FFRpropLearning : IFeedForwardLearningAlgorithm
    {
        private FeedForwardNetwork m_ffn;
        public FFRpropLearning(FeedForwardNetwork ffn)
        {
            m_ffn = ffn;
            this.DeltaZero = 0.1;
            this.DeltaMin = 1e-6;
            this.DeltaMax = 50; // 1;
            this.EthaMinus = 0.5;
            this.EthaPlus = 1.2;
        }

        private void SetDeltaZero(double deltaZero)
        {
            for (int li = 0, lcount = m_ffn.NumLayers; li < lcount; li++)
            {
                var layer = m_ffn.Layer(li);
                for (int ni = 0, ncount = layer.Length; ni < ncount; ni++)
                {
                    var neuron = layer[ni];
                    for(int wi = 0, wcount = neuron.NumInputs; wi < wcount; wi++)
                    {
                        neuron.DeltaWeights[wi] = deltaZero;
                    }

                    neuron.DeltaBias = deltaZero;
                }
            }
        }

        private double m_deltaZero = 0.1;
        public double DeltaZero
        { 
            get { return m_deltaZero; } 
            set { m_deltaZero = value; SetDeltaZero(value); } 
        }

        public double DeltaMin { get; set; }
        public double DeltaMax { get; set; }
        public double EthaMinus { get; set; }
        public double EthaPlus { get; set; }

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
                        var dicKeepUpdates = new Dictionary<int, double>();

                        checked
                        {
                            for (int wi = 0, wcount = neuron.Weights.Length; wi < wcount; wi++)
                            {
                                double gradProduct = neuron.PreviousWeightGradients[wi] * neuron.WeightGradients[wi];
                                if (gradProduct > 0)
                                {
                                    neuron.DeltaWeights[wi] = Math.Min(neuron.DeltaWeights[wi] * this.EthaPlus, this.DeltaMax);
                                    neuron.PendingWeightIncrements[wi] = -Math.Sign(neuron.WeightGradients[wi]) * neuron.DeltaWeights[wi];
                                }
                                else if (gradProduct < 0)
                                {
                                    neuron.DeltaWeights[wi] = Math.Max(neuron.DeltaWeights[wi] * this.EthaMinus, this.DeltaMin);
                                    neuron.PendingWeightIncrements[wi] = -neuron.PreviousWeightIncrements[wi];

                                    // TODO
                                    dicKeepUpdates.Add(wi, neuron.PreviousWeightIncrements[wi]);

                                    neuron.WeightGradients[wi] = 0.0;
                                }
                                else
                                {
                                    neuron.PendingWeightIncrements[wi] = -Math.Sign(neuron.WeightGradients[wi]) * neuron.DeltaWeights[wi];
                                }
                            }

                            double biasGradProduct = neuron.PreviousBiasGradient * neuron.BiasGradient;
                            if (biasGradProduct > 0)
                            {
                                neuron.DeltaBias = Math.Min(neuron.DeltaBias * this.EthaPlus, this.DeltaMax);
                                neuron.PendingBiasIncrement = -Math.Sign(neuron.BiasGradient) * neuron.DeltaBias;
                            }
                            else if (biasGradProduct < 0)
                            {
                                neuron.DeltaBias = Math.Max(neuron.DeltaBias * this.EthaMinus, this.DeltaMin);
                                neuron.PendingBiasIncrement = -neuron.PreviousBiasIncrement;
                                // TODO
                                dicKeepUpdates.Add(-1, neuron.PreviousBiasIncrement);

                                neuron.BiasGradient = 0.0;
                            }
                            else
                            {
                                neuron.PendingBiasIncrement = -Math.Sign(neuron.BiasGradient) * neuron.DeltaBias;
                            }
                        }

                        neuron.UpdateWeights();

                        foreach (var pair in dicKeepUpdates)
                        {
                            if (pair.Key < 0)
                            {
                                neuron.PreviousBiasIncrement = pair.Value;
                            }
                            else
                            {
                                neuron.PreviousWeightIncrements[pair.Key] = pair.Value;
                            }
                        }
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
