using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;

namespace GridSoccer.ClientBasic
{
    public class InitOKMessage : IMessageInfo
    {
        public InitOKMessage()
        {
            MessageType = MessageTypes.InitOK;
        }

        public Sides Side;
    }

    public class InitErrorMessage : IMessageInfo
    {
        public InitErrorMessage()
        {
            MessageType = MessageTypes.InitError;
        }
    }

    public class ErrorMessage : IMessageInfo
    {
        public ErrorMessage()
        {
            MessageType = MessageTypes.Error;
        }
    }

    public class StartMessage : IMessageInfo
    {
        public StartMessage()
        {
            MessageType = MessageTypes.Start;
        }
    }

    public class StopMessage : IMessageInfo
    {
        public StopMessage()
        {
            MessageType = MessageTypes.Stop;
        }
    }

    public class CycleMessage : IMessageInfo
    {
        public CycleMessage()
        {
            MessageType = MessageTypes.Cycle;
        }

        public int CycleLength;
    }

    public class SeeMessage : IMessageInfo
    {
        public SeeMessage()
        {
            MessageType = MessageTypes.See;
        }

        public string[] SeeMsgTokens;
    }

    public class SettingsMessage : IMessageInfo
    {
        public SettingsMessage ()
        {
            MessageType = MessageTypes.Settings;
        }

        public string[] SettingsMsgTokens;
    }

    public class TurboMessage : IMessageInfo
    {
        public TurboMessage()
        {
            MessageType = MessageTypes.Turbo;
        }

        public bool TurboOn;
    }
}
