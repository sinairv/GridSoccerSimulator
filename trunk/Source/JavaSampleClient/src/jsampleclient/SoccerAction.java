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

package jsampleclient;

public class SoccerAction
{
    public ActionTypes ActionType;
    public int DestinationUnum;

    public SoccerAction(ActionTypes act) 
    {
        this(act, -1);
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

    public static SoccerAction GetActionFromIndex(int n, boolean moveKings, int selfUnum)
    {
        if ((!moveKings && n <= 4) || (moveKings && n <= 8))
            return new SoccerAction(ActionTypes.fromInt(n));

        int dst = moveKings ?  (n - 9 + 1) : (n - 5 + 1);
        if (dst >= selfUnum)
            dst++;
        return new SoccerAction(ActionTypes.Pass, dst);
    }

    public static ActionTypes GetActionTypeFromIndex(int n, boolean moveKings)
    {
        if ((!moveKings && n <= 4) || (moveKings && n <= 8))
            return ActionTypes.fromInt(n);
        else
            return ActionTypes.Pass;
    }

    public static int GetActionCount(boolean moveKings, int teamMatesCount)
    {
        return (moveKings ? 9: 5) + teamMatesCount - 1;
    }

    public static int GetIndexFromAction(SoccerAction act, boolean moveKings, int selfUnum)
    {
        int n = act.ActionType.toInt();
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

    @Override
    public String toString()
    {
        if (this.ActionType == ActionTypes.Pass)
            return String.format("(pass %d)", DestinationUnum);
        else
            return String.format("(%s)", ActionType.toString());
    }
}
