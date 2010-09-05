// Copyright (c) 2009 - 2010 
//  - Sina Iravanian <sina@sinairv.com>
//  - Sahar Araghi   <sahar_araghi@aut.ac.ir>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

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
        /// <summary>
        /// Gets the right-to-left coordination as if the point has been
        /// mirrored with respect to the center of the field.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        public static Position GetRTL(this Position pos)
        {
            int r = Settings.Default.NumRows - pos.Row + 1;
            int c = Settings.Default.NumCols - pos.Col + 1;
            return new Position(r, c);
        }
    }
}
