using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;

namespace SharpNeatLib.Experiments
{
    class Skirmish3DPopulationEvaluator : MultiThreadedPopulationEvaluator
    {
        public Skirmish3DPopulationEvaluator(INetworkEvaluator eval)
            : base(eval, null)
        {

        }
    }
}
