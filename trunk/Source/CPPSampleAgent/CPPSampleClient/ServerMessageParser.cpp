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

#include "ServerMessageParser.h"

#include <vector>
#include <sstream>
#include "ParserUtils.h"
using namespace std;

IMessageInfo* ServerMessageParser::ParseInputMessage(const string & str)
{
	vector<string> toks = ParserUtils::TokenizeString(str);

	if (toks.size() > 0)
    {
		string msg = toks[0];

        if (msg == "see")
        {
            return new SeeMessage(toks) ;
        }
        else if (msg == "init")
        {
            Sides side = Sides::Left;
					
            string strSide = toks[1];
            if(strSide == "l")
            {
                side = Sides::Left;
            }
            else if(strSide == "r")
            {
                side = Sides::Right;
            }
            else // it could be init error
            {
                return new InitErrorMessage();
            }
            return new InitOKMessage(side);
        }
        else if (msg == "cycle")
        {
			int cl = ParserUtils::Str2Int(toks[1]);
            return new CycleMessage(cl);
        }
        else if (msg == "turbo")
        {
            bool isTurboMode = (toks[1] == "on");
            return new TurboMessage(isTurboMode);
        }
        else if (msg == "error")
        {
            return new ErrorMessage();
        }
        else if (msg == "start")
        {
            return new StartMessage();
        }
        else if (msg == "stop")
        {
            return new StopMessage();
        }
        else if (msg == "settings")
        {
            return new SettingsMessage(toks);
        }
    }

    return IMessageInfo::GetIllegalMessage();
}
