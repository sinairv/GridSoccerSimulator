// Copyright (c) 2009 - 2010 
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
