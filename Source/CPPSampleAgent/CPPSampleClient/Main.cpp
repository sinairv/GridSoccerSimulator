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

#include "ClientException.h"
#include "ClientBase.h"
#include <iostream>
#include <cstdlib>
#include <time.h>

using namespace std;

class RandomClient : public ClientBase
{

public:
	RandomClient(string serverAddr, int serverPort, string teamname, int unum)
        : ClientBase(serverAddr, serverPort, teamname, unum)
    {
		srand((unsigned)time(NULL));
		m_moveKings = false;


        if (unum == 1)
            SetHomePosition(2, 2);
        else if (unum == 2)
            SetHomePosition(4, 2);
        else if (unum == 3)
            SetHomePosition(3, 3);
    }

private:
	bool m_moveKings;

	// C++ rand generates random numbers between 0 and RAND_MAX. This is quite a big range
	// Normally one would want the generated random number within a range to be really
	// useful. So the arguments have default values which can be overridden by the caller
	// returns random numbers in U~[low, high)
	int NextRandomNum(int low = 0, int high = 100) const 
	{
		int range = (high - low) + 1;
		// this modulo operation does not generate a truly uniformly distributed random number
		// in the span (since in most cases lower numbers are slightly more likely), 
		// but it is generally a good approximation for short spans. Use it if essential
		int res = ( std::rand() % high + low );
		//int res = low + static_cast<int>( ( range * std::rand() / ( RAND_MAX + 1.0) ) );
		return res;
	}


    SoccerAction Think()
    {
        vector<int> teammates = GetAvailableTeammatesUnums();
        int numActions = SoccerAction::GetActionCount(m_moveKings, teammates.size() + 1);
        int ai =  NextRandomNum(0, numActions);
        return SoccerAction::GetActionFromIndex(ai, m_moveKings, m_myUnum);
    }
};




int main()
{
	try
	{
		RandomClient cl("127.0.0.1", 5050, "C++Rnd", 1);
		cl.Start();
	}
	catch (ClientException ex)
	{
		cerr << "Exception: " << ex.GetExceptionMessage() << endl;
		cin.get();
	}

	return 0;
}
