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
