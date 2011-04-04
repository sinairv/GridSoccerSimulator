using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.CPPNs
{
    public interface ISubstrate
    {
        INetwork GenerateNetwork(INetwork CPPN);
        NeatGenome.NeatGenome GenerateGenome(INetwork network);
    }
}
