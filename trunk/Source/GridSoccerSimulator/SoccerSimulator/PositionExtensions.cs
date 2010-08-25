using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;
using GridSoccer.Simulator.Properties;

namespace GridSoccer.Simulator
{
    public static class PositionExtensions
    {
        public static Position GetRTL(this Position pos)
        {
            int r = Settings.Default.NumRows - pos.Row + 1;
            int c = Settings.Default.NumCols - pos.Col + 1;
            return new Position(r, c);
        }
    }
}
