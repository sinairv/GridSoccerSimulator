using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork
{
    /// <summary>
    /// Generates Normal Random Numbers using the Polar Box Muller method
    /// </summary>
    public class NormalRandomGenerator
    {
        private Random rnd = new Random();

        private int returnCount = 0;

        private double prevR1;
        private double prevR2;

        /// <summary>
        /// Gets the mean aka mu.
        /// </summary>
        /// <value>The mean.</value>
        public double Mean { get; private set; }

        /// <summary>
        /// Gets the standard deviation
        /// </summary>
        /// <value>The standard deviation</value>
        public double Sigma2 { get; private set; }

        private double sigma;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalRandomGenerator"/> class.
        /// </summary>
        /// <param name="mean">The mean or mu.</param>
        /// <param name="sigma2">The sigma2 or standard deviation.</param>
        public NormalRandomGenerator(double mean, double sigma2)
        {
            this.Mean = mean;
            this.Sigma2 = sigma2;
            this.sigma = Math.Sqrt(Sigma2);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalRandomGenerator"/> class, 
        /// with standard normal distibution values, i.e. mu = 0, and sigma2 = 1.
        /// </summary>
        public NormalRandomGenerator()
            : this(0.0, 1.0)
        {
        }

        /// <summary>
        /// Generates the next normal distribution number
        /// </summary>
        /// <returns></returns>
        public double Next()
        {
            if (returnCount % 2 == 0)
            {
                NextStandardNormalDistribution(out prevR1, out prevR2);
                prevR1 = this.sigma * prevR1 + this.Mean;
                returnCount = (returnCount + 1) % 2;
                return prevR1;
            }
            else
            {
                prevR2 = this.sigma * prevR2 + this.Mean;
                returnCount = (returnCount + 1) % 2;
                return prevR2;
            }
        }

        private void NextStandardNormalDistribution(out double r1, out double r2)
        {
            double r = 1.0;
            double u1, u2;
            do
            {
                u1 = -1 + 2 * rnd.NextDouble();
                u2 = -1 + 2 * rnd.NextDouble();
                r = u1 * u1 + u2 * u2;
            } while (r >= 1.0);

            r1 = u1 * Math.Sqrt(-2 * Math.Log(r) / r);
            r2 = u2 * Math.Sqrt(-2 * Math.Log(r) / r);
        }

        public static void Tester()
        {
            NormalRandomGenerator nrGen = new NormalRandomGenerator(5, 2);

            int maxnums = 10000;

            double sum = 0.0;
            double sum2 = 0.0;
            double r = 0.0;

            for (int i = 0; i < maxnums; ++i)
            {
                r = nrGen.Next();
                sum += r;
                sum2 += r * r;
            }

            Console.WriteLine("Mean is: {0}", sum / maxnums);
            Console.WriteLine("Sigma2 is : {0}", (maxnums * sum2 - (sum * sum)) / (maxnums * maxnums));

            Console.ReadLine();
        }

    }
}
