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
