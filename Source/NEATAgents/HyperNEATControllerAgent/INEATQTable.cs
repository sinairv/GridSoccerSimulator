using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;

namespace GridSoccer.HyperNEATControllerAgent
{
    public interface INEATQTable
    {
        SharpNeatLib.NeatGenome.NeatGenome BestGenomeOveral { get; }
        RLClientBase NeatClient { get; }

        event EventHandler EAUpdate;
    }
}
