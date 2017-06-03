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

public enum ActionTypes
{
    Hold,
    MoveEast,
    MoveSouth,
    MoveWest,
    MoveNorth,
    MoveNorthEast,
    MoveSouthEast,
    MoveSouthWest,
    MoveNorthWest,
    Pass;
    
    public static ActionTypes fromInt(int i)
    {
        switch(i)
        {
            case 0:
                return ActionTypes.Hold;
            case 1:
                return ActionTypes.MoveEast;
            case 2:
                return ActionTypes.MoveSouth;
            case 3:
                return ActionTypes.MoveWest;
            case 4:
                return ActionTypes.MoveNorth;
            case 5:
                return ActionTypes.MoveNorthEast;
            case 6:
                return ActionTypes.MoveSouthEast;
            case 7:
                return ActionTypes.MoveSouthWest;
            case 8:
                return ActionTypes.MoveNorthWest;
            case 9:
                return ActionTypes.Pass;
        }
        
        return ActionTypes.Hold;
    }
    
    public int toInt()
    {
        switch(this)
        {
            case Hold:
                return 0;
            case MoveEast:
                return 1;
            case MoveSouth:
                return 2;
            case MoveWest:
                return 3;
            case MoveNorth:
                return 4;
            case MoveNorthEast:
                return 5;
            case MoveSouthEast:
                return 6;
            case MoveSouthWest:
                return 7;
            case MoveNorthWest:
                return 8;
            case Pass:
                return 9;
        }
        
        return -1;
    }
    
    @Override
    public String toString()
    {
        switch(this)
        {
            case Hold:
                return "Hold";
            case MoveEast:
                return "MoveEast";
            case MoveSouth:
                return "MoveSouth";
            case MoveWest:
                return "MoveWest";
            case MoveNorth:
                return "MoveNorth";
            case MoveNorthEast:
                return "MoveNorthEast";
            case MoveSouthEast:
                return "MoveSouthEast";
            case MoveSouthWest:
                return "MoveSouthWest";
            case MoveNorthWest:
                return "MoveNorthWest";
            case Pass:
                return "Pass";
        }
        
        return "";
    }
}
