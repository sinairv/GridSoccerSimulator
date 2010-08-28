using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;

namespace GridSoccer.ClientBasic
{
    internal class ServerMessageParser
    {
        public static IMessageInfo ParseInputMessage(string str)
        {
            string[] toks = str.Split(new char[] { ' ', '(', ')', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (toks.Length > 0)
            {
                string msg = ParserUtils.GetTokenAt(toks, 0);
                if (msg == "see")
                {
                    return new SeeMessage() { SeeMsgTokens = toks };
                }
                else if (msg == "init")
                {
                    Sides side = Sides.Left;
                    string strSide = ParserUtils.GetTokenAt(toks, 1);
                    if(strSide == "l")
                    {
                        side = Sides.Left;
                    }
                    else if(strSide == "r")
                    {
                        side = Sides.Right;
                    }
                    else // it could be init error
                    {
                        return new InitErrorMessage();
                    }
                    return new InitOKMessage() { Side = side };
                }
                else if (msg == "cycle")
                {
                    int cl = Int32.Parse(ParserUtils.GetTokenAt(toks, 1));
                    return new CycleMessage() { CycleLength = cl };
                }
                else if (msg == "turbo")
                {
                    bool isTurboMode = (ParserUtils.GetTokenAt(toks, 1) == "on");
                    return new TurboMessage() { TurboOn = isTurboMode };
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
                    return new SettingsMessage() { SettingsMsgTokens = toks };
                }
            }

            return IMessageInfo.GetIllegalMessage();
        }
    }
}
