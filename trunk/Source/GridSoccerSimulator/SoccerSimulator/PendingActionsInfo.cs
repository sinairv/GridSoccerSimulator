using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;

namespace GridSoccer.Simulator
{
    public class PendingActionsInfo
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
