using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;

namespace SharpNeatLib.Experiments
{
    class SkirmishPopulationEvaluator : MultiThreadedPopulationEvaluator
    {
        public SkirmishPopulationEvaluator(INetworkEvaluator eval)
            : base(eval, null)
        {

        }
    }
}
