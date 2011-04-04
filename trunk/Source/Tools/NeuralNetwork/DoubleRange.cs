using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork
{
    public class DoubleRange
    {
        public DoubleRange(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }

        public double Min { get; set; }

        public double Max { get; set; }

        public double Length { get { return Max - Min; } }
    }
}
