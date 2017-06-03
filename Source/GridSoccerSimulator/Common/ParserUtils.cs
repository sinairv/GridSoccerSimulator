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

using System;

namespace GridSoccer.Common
{
    public class ParserUtils
    {
        public static bool IsTeamNameValid(string teamName)
        {
            if (String.IsNullOrEmpty(teamName))
                return false;

            return true;
        }

        public static string GetTokenAt(string[] tokens, int i)
        {
            if (tokens.Length > i)
                return tokens[i];
            else
                return "";
        }
    }
}
