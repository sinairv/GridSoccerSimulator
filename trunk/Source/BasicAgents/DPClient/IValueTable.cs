using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.DPClient
{
    public interface IValueTable
    {
        int[] StateDimensions { get; }

        long NumStates { get; }

        int[] GetDimentionalState(int stateLinInd);

        double GetValueForState(params int[] inds);
    }
}
