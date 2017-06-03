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
using GridSoccer.Common;

namespace GridSoccer.Simulator.Net
{
    public class MessageParser
    {
        /// <summary>
        /// Parses the input message and returns an isntance of the 
        /// <c>IMessageInfo</c> containing the parsed data.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns></returns>
        public static IMessageInfo ParseInputMessage(string str)
        {
            string[] toks = str.Split(new char[] { ' ', '(', ')', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (toks.Length > 0)
            {
                string msg = ParserUtils.GetTokenAt(toks, 0);
                if (msg == "init")
                {
                    bool isOK = false;
                    string teamname = ParserUtils.GetTokenAt(toks, 1);
                    isOK = ParserUtils.IsTeamNameValid(teamname);
                    int unum;
                    isOK &= Int32.TryParse(ParserUtils.GetTokenAt(toks, 2), out unum);

                    if (!isOK)
                        return IMessageInfo.GetIllegalMessage();
                    else
                        return new InitMessage() { TeamName = teamname, UNum = unum };

                }
                else if (msg == "home")
                {
                    bool isOK = false;
                    int r, c;
                    isOK = Int32.TryParse(ParserUtils.GetTokenAt(toks, 1), out r);
                    isOK &= Int32.TryParse(ParserUtils.GetTokenAt(toks, 2), out c);

                    if (!isOK)
                        return IMessageInfo.GetIllegalMessage();
                    else
                        return new HomeMessage() { R = r, C = c };
                }
                else if (msg == "episode-timeout")
                {
                    string stat = ParserUtils.GetTokenAt(toks, 1);
                    EpisodeTimeoutMessage eptMsg = new EpisodeTimeoutMessage();
                    switch (stat)
                    {
                        case "opp-pass":
                            eptMsg.IsOur = false;
                            eptMsg.IsPass = true;
                            break;
                        case "opp-fail":
                            eptMsg.IsOur = false;
                            eptMsg.IsPass = false;
                            break;
                        case "our-pass":
                            eptMsg.IsOur = true;
                            eptMsg.IsPass = true;
                            break;
                        case "our-fail":
                        default:
                            eptMsg.IsOur = true;
                            eptMsg.IsPass = false;
                            break;
                    }

                    return eptMsg;
                }
                else if (msg == "hold")
                {
                    return new HoldMessage();
                }
                else if (msg == "pass")
                {
                    string unumstr = ParserUtils.GetTokenAt(toks, 1);
                    int dstUnum;
                    if (Int32.TryParse(unumstr, out dstUnum))
                        return new PassMessage() { DstUnum = dstUnum };
                    else
                        return IMessageInfo.GetIllegalMessage();

                }
                else if (msg == "move")
                {
                    ActionTypes at = ActionTypes.Hold;
                    string dir = ParserUtils.GetTokenAt(toks, 1);

                    switch (dir)
                    {
                        case "east":
                            at = ActionTypes.MoveEast;
                            break;
                        case "north":
                            at = ActionTypes.MoveNorth;
                            break;
                        case "west":
                            at = ActionTypes.MoveWest;
                            break;
                        case "south":
                            at = ActionTypes.MoveSouth;
                            break;
                        case "north-east":
                            at = ActionTypes.MoveNorthEast;
                            break;
                        case "south-east":
                            at = ActionTypes.MoveSouthEast;
                            break;
                        case "south-west":
                            at = ActionTypes.MoveSouthWest;
                            break;
                        case "north-west":
                            at = ActionTypes.MoveNorthWest;
                            break;
                        default:
                            at = ActionTypes.Hold;
                            break;
                    }

                    if (at == ActionTypes.Hold)
                        return IMessageInfo.GetIllegalMessage();
                    else
                        return new MoveMessage() { ActionType = at };
                }
            }

            return IMessageInfo.GetIllegalMessage();
        }
    }
}
