using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.DPClient
{
    public class State
    {
        public State(int[] state)
        {
            this.MyRow = state[0] + 1;
            this.MyCol = state[1] + 1;
            this.OppRow = state[2] + 1;
            this.OppCol = state[3] + 1;
            this.AmIBallOwner = state[4] == 0 ? true : false;
        }

        public State(State s)
        {
            this.MyRow = s.MyRow;
            this.MyCol = s.MyCol;
            this.OppRow = s.OppRow;
            this.OppCol = s.OppCol;
            this.AmIBallOwner = s.AmIBallOwner;
        }

        public int[] GetStateInds()
        {
            return new int[] { MyRow - 1, MyCol - 1, OppRow - 1, OppCol - 1, AmIBallOwner ? 0 : 1 };
        }

        public int GetLinearIndex(IValueTable table)
        {
            return (int) Utils.GetLinearIndex(GetStateInds(), table.StateDimensions, table.NumStates);
        }

        public int MyRow { get; set; }
        public int MyCol { get; set; }
        public int OppRow { get; set; }
        public int OppCol { get; set; }
        public bool AmIBallOwner { get; set; }

    }
}
