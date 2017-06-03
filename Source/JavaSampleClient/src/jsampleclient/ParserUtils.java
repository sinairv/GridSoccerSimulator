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

public class ParserUtils
{
    public static boolean IsTeamNameValid(String teamName)
    {
        if (teamName == null || teamName.length() <= 0)
            return false;

        return true;
    }

    public static String GetTokenAt(String[] tokens, int i)
    {
        if (tokens.length > i)
            return tokens[i];
        else
            return "";
    }
}
