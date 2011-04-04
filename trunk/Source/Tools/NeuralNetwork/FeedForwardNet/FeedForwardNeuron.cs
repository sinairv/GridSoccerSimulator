using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.FeedForwardNet
{
    public class FeedForwardNeuron
    {
        public FeedForwardNeuron(int numInputs)
            : this(numInputs, new SigmoidFunction(1.0) , WeightInitMethods.Constant, 0.0)
        {
        }

        public FeedForwardNeuron(int numInputs, IActivationFunction func)
            : this(numInputs, func, WeightInitMethods.Constant, 0.0)
        {

        }

        public FeedForwardNeuron(int numInputs, IActivationFunction func, 
            WeightInitMethods initMethod, double weightInitParam)
        {
            if (numInputs <= 0)
                throw new ArgumentException("Number of inputs cannot be less than 1");
            if (func == null)
                throw new ArgumentException("Activation function must not be null!");

            this.NumInputs = numInputs;
            Weights = new double[NumInputs];
            PendingWeightIncrements = new double[NumInputs];
            PreviousWeightIncrements = new double[NumInputs];
            WeightGradients = new double[this.NumInputs];
            PreviousWeightGradients = new double[this.NumInputs];

            PendingBiasIncrement = 0.0;
            PreviousBiasIncrement = 0.0;

            ActivationFunction = func;

            DeltaWeights = new double[this.NumInputs];

            InitWeights(initMethod, weightInitParam);
        }

        public IActivationFunction ActivationFunction { get; set; }
        public int NumInputs { get; private set; }

        public double[] Weights { get; private set; }
        public double Bias { get; set; }

        private void InitWeights(WeightInitMethods initMethod, double w0)
        {
            switch (initMethod)
            {
                case WeightInitMethods.Constant:
                    {
                        for (int i = 0; i < this.NumInputs; i++)
                        {
                            this.Weights[i] = w0;
                        }
                        this.Bias = w0;
                    }
                    break;
                case WeightInitMethods.UniformRandom:
                    {
                        for (int i = 0; i < this.NumInputs; i++)
                        {
                            this.Weights[i] = (2 * FeedForwardNetwork.UniformRandomGenerator.NextDouble() * w0) - w0;
                        }

                        this.Bias = (2 * FeedForwardNetwork.UniformRandomGenerator.NextDouble() * w0) - w0;
                    }
                    break;
                case WeightInitMethods.NormalRandom:
                    {
                        for (int i = 0; i < this.NumInputs; i++)
                        {
                            this.Weights[i] = FeedForwardNetwork.NormalRandomGenerator.Next() * w0;
                        }

                        this.Bias = FeedForwardNetwork.NormalRandomGenerator.Next() * w0;
                    }
                    break;
                default:
                    break;
            }
        }

        public double Compute(double[] inputs)
        {
            if (inputs.Length != NumInputs)
                throw new ArgumentException("Length of inputs and weights do not match");

            checked
            {
                return ActivationFunction.Function(
                    ComputeExcitation(inputs));
            }
        }

        private double ComputeExcitation(double[] inputs)
        {
            if (inputs.Length != NumInputs)
                throw new ArgumentException("Length of inputs and weights do not match");

            checked
            {
                double sum = 0.0;
                for (int i = 0; i < NumInputs; i++)
                    sum += inputs[i] * Weights[i];

                sum += Bias;

                return sum;
            }
        }

        #region Rprop Stuff

        /// <summary>
        /// The adaptive learning rate which is used by RPROP only.
        /// </summary>
        public double[] DeltaWeights { get; set; }
        public double DeltaBias { get; set; }

        #endregion

        #region Updating

        public double[] PendingWeightIncrements { get; private set; }
        public double PendingBiasIncrement { get; set; }

        public double[] PreviousWeightIncrements { get; private set; }
        public double PreviousBiasIncrement { get; set; }

        public void UpdateWeights()
        {
            checked
            {
                for (int i = 0, ni = this.PendingWeightIncrements.Length; i < ni; i++)
                {
                    this.PreviousWeightIncrements[i] = this.PendingWeightIncrements[i];
                    this.Weights[i] += this.PendingWeightIncrements[i];
                    this.PendingWeightIncrements[i] = 0.0;

                    this.PreviousWeightGradients[i] = this.WeightGradients[i];
                    this.WeightGradients[i] = 0;
                }

                this.PreviousBiasIncrement = this.PendingBiasIncrement;
                this.Bias += this.PendingBiasIncrement;
                this.PendingBiasIncrement = 0.0;

                this.PreviousBiasGradient = this.BiasGradient;
                this.BiasGradient = 0.0;
            }
        }

        #endregion

        #region Learning

        public double[] WeightGradients { get; private set; }
        public double[] PreviousWeightGradients { get; private set; }
        public double BiasGradient { get; set; }
        public double PreviousBiasGradient { get; private set; }

        private double[] m_lastInput = null;
        private double m_derivative;
        private double m_output;

        public double Feedforward(double[] inputs)
        {
            if(inputs.Length != NumInputs)
                throw new ArgumentException("Length of inputs and weights do not match");

            m_lastInput = (double[]) inputs.Clone();

            checked
            {
                m_output = ActivationFunction.Function(
                    ComputeExcitation(inputs));

                m_derivative = ActivationFunction.DerivativeUsingValue(m_output);

                return m_output;
            }
        }

        public double[] Backpropagate(double bpValue)
        {
            double[] bpResult = new double[this.NumInputs];

            checked
            {
                double bpDerv = bpValue * m_derivative;

                for (int i = 0, icount = bpResult.Length; i < icount; i++)
                {
                    bpResult[i] = bpDerv * this.Weights[i];
                    this.WeightGradients[i] += bpDerv * m_lastInput[i];
                }

                this.BiasGradient += bpDerv;// *1;
            }

            return bpResult;
        }

        #endregion
    }
}
