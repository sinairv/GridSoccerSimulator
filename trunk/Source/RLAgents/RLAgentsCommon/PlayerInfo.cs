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

namespace GridSoccer.RLAgentsCommon
{
    public class PlayerInfo
    {
        public int Unum;
        //public bool IsOur;
        public bool IsBallOwner;
        public Position Position;

        public override string ToString()
        {
            return String.Format("({0} Pos:{1}{2})", Unum, Position, IsBallOwner ? " BO" : "");
        }

        public PlayerInfo Clone()
        {
            return new PlayerInfo()
            {
                Unum = this.Unum,
                IsBallOwner = this.IsBallOwner,
                Position = this.Position.Clone()
            };
        }

    }
}
