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

import java.util.StringTokenizer;

public class ServerMessageParser
{
    public static String[] getAllTokens(StringTokenizer tokenizer)
    {
        String[] toks = new String[tokenizer.countTokens()];

        int i = 0;
        while(tokenizer.hasMoreTokens())
        {
            toks[i] = tokenizer.nextToken();
            i++;
        }

        return toks;
    }

    public static IMessageInfo ParseInputMessage(String str)
    {
        StringTokenizer tokenizer = new StringTokenizer(str, " ()\t\r\n", false);
        String[] toks = getAllTokens(tokenizer);

        if (toks.length > 0)
        {
            String msg = ParserUtils.GetTokenAt(toks, 0);
            if (msg.equals("see"))
            {
                SeeMessage seemsg = new SeeMessage();
                seemsg.SeeMsgTokens = toks;
                return seemsg;
            }
            else if (msg.equals("init"))
            {
                Sides side = Sides.Left;
                String strSide = ParserUtils.GetTokenAt(toks, 1);
                if(strSide.equals("l"))
                {
                    side = Sides.Left;
                }
                else if(strSide.equals("r"))
                {
                    side = Sides.Right;
                }
                else // it could be init error
                {
                    return new InitErrorMessage();
                }
                
                InitOKMessage initmsg = new InitOKMessage();
                initmsg.Side = side;
                return initmsg;
            }
            else if (msg.equals("cycle"))
            {
                int cl = Integer.parseInt(ParserUtils.GetTokenAt(toks, 1));
                CycleMessage cyclemsg = new CycleMessage();
                cyclemsg.CycleLength = cl;
                return cyclemsg;
            }
            else if (msg.equals("turbo"))
            {
                boolean isTurboMode = (ParserUtils.GetTokenAt(toks, 1).equals("on"));
                TurboMessage tmsg =new TurboMessage();
                tmsg.TurboOn = isTurboMode;
                return tmsg;
            }
            else if (msg.equals("error"))
            {
                return new ErrorMessage();
            }
            else if (msg.equals("start"))
            {
                return new StartMessage();
            }
            else if (msg.equals("stop"))
            {
                return new StopMessage();
            }
            else if (msg.equals("settings"))
            {
                SettingsMessage setmsg = new SettingsMessage();
                setmsg.SettingsMsgTokens = toks;
                return setmsg;
            }
        }

        return IMessageInfo.GetIllegalMessage();
    }
}
