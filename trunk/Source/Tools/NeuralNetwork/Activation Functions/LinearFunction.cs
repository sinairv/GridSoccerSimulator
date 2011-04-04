using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork
{
    public class LinearFunction : IActivationFunction
    {
        #region IActivationFunction Members

        public double Function(double x)
        {
            return x;
        }

        public double Derivative(double x)
        {
            return 1;
        }

        public double DerivativeUsingValue(double y)
        {
            return 1;
        }

        #endregion
    }
}
