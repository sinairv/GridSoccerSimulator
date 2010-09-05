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

namespace GridSoccer.Common
{
    public class SoccerAction
    {
        public ActionTypes ActionType;
        public int DestinationUnum;

        public SoccerAction(ActionTypes act) : this(act, -1)
        {
        }

        public SoccerAction(ActionTypes act, int dstNum)
        {
            this.ActionType = act;
            this.DestinationUnum = dstNum;
        }

        public void Set(ActionTypes act, int dstNum)
        {
            this.ActionType = act;
            this.DestinationUnum = dstNum;
        }

        public void Set(SoccerAction act)
        {
            Set(act.ActionType, act.DestinationUnum);
        }

        public static SoccerAction GetActionFromIndex(int n, bool moveKings, int selfUnum)
        {
            if ((!moveKings && n <= 4) || (moveKings && n <= 8))
                return new SoccerAction((ActionTypes)n);

            int dst = moveKings ?  (n - 9 + 1) : (n - 5 + 1);
            if (dst >= selfUnum)
                dst++;
            return new SoccerAction(ActionTypes.Pass, dst);
        }

        public static ActionTypes GetActionTypeFromIndex(int n, bool moveKings)
        {
            if ((!moveKings && n <= 4) || (moveKings && n <= 8))
                return (ActionTypes)n;
            else
                return ActionTypes.Pass;
        }


        /// <summary>
        /// Gets the number of possible actions that an agent can perform.
        /// </summary>
        /// <param name="moveKings">if set to <c>true</c> can move kings.</param>
        /// <param name="teamMatesCount">The number of teammates including the player itself.</param>
        /// <returns></returns>
        public static int GetActionCount(bool moveKings, int teamMatesCount)
        {
            return (moveKings ? 9: 5) + teamMatesCount - 1;
        }

        public static int GetIndexFromAction(SoccerAction act, bool moveKings, int selfUnum)
        {
            int n = (int)act.ActionType;
            if ((!moveKings && n <= 4) || (moveKings && n <= 8))
                return n;

            int dst = act.DestinationUnum;
            if (dst == selfUnum)
            {
                // pass to self -> hold
                n = 0;
            }
            else
            {
                n += dst - 1;

                if (dst >= selfUnum)
                    n--;
            }
            return moveKings ? n : n - 4;
        }

        public override string ToString()
        {
            if (this.ActionType == ActionTypes.Pass)
                return String.Format("(pass {0})", DestinationUnum);
            else
                return String.Format("({0})", ActionType);
        }
    }
}
