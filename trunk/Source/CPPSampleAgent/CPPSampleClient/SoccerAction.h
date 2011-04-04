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

#ifndef SOCCERACTION_H
#define SOCCERACTION_H

enum ActionTypes
{
    ActHold,
    ActMoveEast,
    ActMoveSouth,
    ActMoveWest,
    ActMoveNorth,
    ActMoveNorthEast,
    ActMoveSouthEast,
    ActMoveSouthWest,
    ActMoveNorthWest,
    ActPass,
	ActIllegal
};

class SoccerAction
{
public:
	ActionTypes ActionType;
    int DestinationUnum;

    SoccerAction(ActionTypes act, int dstNum)
    {
        ActionType = act;
        DestinationUnum = dstNum;
    }

    SoccerAction(ActionTypes act)
    {
        ActionType = act;
        DestinationUnum = -1;
    }

    void Set(ActionTypes act, int dstNum)
    {
        ActionType = act;
        DestinationUnum = dstNum;
    }

	bool IsIllegal()
	{
		return ActionType == ActionTypes::ActIllegal;
	}

    void Set(SoccerAction act)
    {
        Set(act.ActionType, act.DestinationUnum);
    }

    static SoccerAction GetActionFromIndex(int n, bool moveKings, int selfUnum)
    {
        if ((!moveKings && n <= 4) || (moveKings && n <= 8))
            return SoccerAction((ActionTypes)n);

        int dst = moveKings ?  (n - 9 + 1) : (n - 5 + 1);
        if (dst >= selfUnum)
            dst++;
        return SoccerAction(ActionTypes::ActPass, dst);
    }

    static ActionTypes GetActionTypeFromIndex(int n, bool moveKings)
    {
        if ((!moveKings && n <= 4) || (moveKings && n <= 8))
            return (ActionTypes)n;
        else
            return ActionTypes::ActPass;
    }

    /// <summary>
    /// Gets the number of possible actions that an agent can perform.
    /// </summary>
    /// <param name="moveKings">if set to <c>true</c> can move kings.</param>
    /// <param name="teamMatesCount">The number of teammates including the player itself.</param>
    /// <returns></returns>
    static int GetActionCount(bool moveKings, int teamMatesCount)
    {
        return (moveKings ? 9: 5) + teamMatesCount - 1;
    }

    static int GetIndexFromAction(SoccerAction act, bool moveKings, int selfUnum)
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

	static SoccerAction GetIllegalAction()
	{
		return SoccerAction(ActionTypes::ActIllegal, -1);
	}
};

#endif
