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

import java.util.ArrayList;
import java.util.Random;

public class RandomClient extends ClientBase
{
    public RandomClient(String serverAddr, int serverPort, String teamname, int unum) throws Exception
    {
        super(serverAddr, serverPort, teamname, unum);

        if (unum == 1)
            SetHomePosition(2, 2);
        else if (unum == 2)
            SetHomePosition(4, 2);
        else if (unum == 3)
            SetHomePosition(3, 3);
    }

    private static Random rnd = new Random();

    private static int nextBoundedRandomInt(Random rnd, int incLowerBound, int exUpperBound)
    {
        return (int)(rnd.nextDouble() * (exUpperBound - incLowerBound) + incLowerBound);
    }

    private static int[] ListToArray(ArrayList<Integer> list)
    {
        int[] ar = new int[list.size()];
        for(int i = 0; i < ar.length; i++)
        {
            ar[i] = list.get(i);
        }
        return ar;
    }

    @Override
    protected SoccerAction Think()
    {
        int dst = -1;
        ActionTypes at = ActionTypes.Hold;

        at = ActionTypes.fromInt(nextBoundedRandomInt(rnd, 0, 10));
        if (at == ActionTypes.Pass)
        {
            int[] teammates = ListToArray(this.GetAvailableTeammatesUnums());
            if (teammates.length <= 0)
                at = ActionTypes.Hold;
            else
                dst = teammates[nextBoundedRandomInt(rnd, 0, teammates.length)];
        }

        return new SoccerAction(at, dst);
    }
}
