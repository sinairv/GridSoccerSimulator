using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;

namespace GridSoccer.Simulator.Net
{
    public class InitMessage : IMessageInfo
    {
        public InitMessage()
        {
            MessageType = MessageTypes.Init;
        }

        public string TeamName;
        public int UNum;
    }

    public class HomeMessage : IMessageInfo
    {
        public HomeMessage()
        {
            MessageType = MessageTypes.Home;
        }

        public int R;
        public int C;
    }

    public class HoldMessage : IMessageInfo
    {
        public HoldMessage()
        {
            MessageType = MessageTypes.Hold;
        }
    }

    public class PassMessage : IMessageInfo
    {
        public PassMessage()
        {
            MessageType = MessageTypes.Pass;
        }

        public int DstUnum;
    }


    public class MoveMessage : IMessageInfo
    {
        public MoveMessage()
        {
            MessageType = MessageTypes.Move;
        }

        public ActionTypes ActionType;
    }
}
