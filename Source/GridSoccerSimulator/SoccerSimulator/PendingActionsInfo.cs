// Copyright (c) 2009 - 2011 
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

using GridSoccer.Common;

namespace GridSoccer.Simulator
{
    /// <summary>
    /// Encapsulates information about pending actions
    /// </summary>
    internal class PendingActionsInfo
    {
        public bool Updated;
        public Position NewPos;
        public Position DesiredPos;
        public SoccerAction Action;

        public PendingActionsInfo()
        {
            Updated = false;
            NewPos = new Position();
            DesiredPos = new Position();
            Action = new SoccerAction(ActionTypes.Hold, -1);
        }
    }
}
