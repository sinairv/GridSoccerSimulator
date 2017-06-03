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

namespace GridSoccer.Common
{
    public enum MessageTypes
    {
        Illegal, Init, Move, Pass, Hold, Home, EpisodeTimeout,
        Settings, InitOK, InitError, Error, See, Start, Stop, Cycle, Turbo
    }

    public abstract class IMessageInfo
    {
        public MessageTypes MessageType { get; set; }

        public static IMessageInfo GetIllegalMessage()
        {
            return new IllegalMessage();
        }
    }

    public class IllegalMessage : IMessageInfo
    {
        public IllegalMessage()
        {
            MessageType = MessageTypes.Illegal;
        }
    }
}
