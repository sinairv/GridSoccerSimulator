using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.CPPNs;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;

namespace GridSoccer.HyperNEATControllerAgent
{
    public class HyperNEATPlayerControllerSubstrate : Substrate
    {
        public HyperNEATPlayerControllerSubstrate(uint inputs, uint outputs, uint hidden)
            : base(inputs, outputs, hidden, HyperNEATParameters.substrateActivationFunction)
        {
    
        }

        public override SharpNeatLib.NeatGenome.NeatGenome GenerateGenome(INetwork network)
        {

            return base.GenerateGenome(network);
        }

    }
}
