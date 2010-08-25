using System;

namespace GridSoccer.Common
{
    public enum MessageTypes
    {
        Illegal, Init, Move, Pass, Hold, Home,
        Settings, InitOK, InitError, Error, See, Start, Stop, Cycle, Jet
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
