using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.NEATClient1P
{
    public class MathUtils
    {
        #region Sigmoid
        public static double Sigmoid(double x, double alpha)
        {
            return 1.0 / (1 + Math.Exp(-alpha * x));
        }

        public static double SigmoidInverse(double s, double alpha)
        {
            return (1.0 / alpha) * Math.Log(s / (1 - s));
        }

        public static double SigmoidDerivative(double x, double alpha)
        {
            double y = Sigmoid(x, alpha);
            return alpha * y * (1 - y);
        }

        public static double SigmoidDerivativeUsingValue(double value, double alpha)
        {
            return alpha * value * (1 - value);
        }
        #endregion

        #region Bipolar Sigmoid
        public static double BipolarSigmoid(double x, double alpha)
        {
            //return 2 * Sigmoid(x, alpha) - 1;
            return (2.0 / (1 + Math.Exp(-alpha * x))) - 1;
        }

        public static double BipolarSigmoidInverse(double s, double alpha)
        {
            return (1 / alpha) * Math.Log((1 + s) / (1 - s));
        }

        public static double BipolarSigmoidDerivative(double x, double alpha)
        {
            double y = BipolarSigmoid(x, alpha);
            return alpha * 0.5 * (1 - y*y);
        }

        public static double BipolarSigmoidDerivativeUsingValue(double value, double alpha)
        {
            return alpha * 0.5 * (1 - value * value);
        }

        #endregion

        /// <summary>
        /// returns v1 - v2
        /// </summary>
        public static double[] VectorSubtract(double[] v1, double[] v2)
        {
            double[] result = new double[Math.Min(v1.Length, v2.Length)];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = v1[i] - v2[i];
            }

            return result;
        }

        public static double[] VectorAdd(double[] v1, double[] v2)
        {
            double[] result = new double[Math.Min(v1.Length, v2.Length)];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = v1[i] + v2[i];
            }

            return result;
        }

        public static double VectorElementsSum(double[] v)
        {
            double sum = 0.0;
            for (int i = 0, icount = v.Length; i < icount; i++)
            {
                sum += v[i];
            }

            return sum;
        }

    }
}
