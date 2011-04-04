using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NNetTest01
{
    public class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Value { get; set; }

        public PointData()
        {
            X = 0.0; Y = 0.0; Value = 0.0;
        }

        public PointData(double x, double y, double value)
        {
            this.X = x; this.Y = y; this.Value = value;
        }
    }
}
