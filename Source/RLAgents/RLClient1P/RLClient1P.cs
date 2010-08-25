using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.ClientBasic;
using GridSoccer.RLAgentsCommon;

namespace RLClient1P
{
    class RLClient1P : RLClientBase
    {
        public RLClient1P(string teamname, int unum )
            : base("127.0.0.1", 5050, teamname, unum)
        {
            if (unum == 1)
                SetHomePosition(2, 3);
        }

        protected override void SetGlobalParams()
        {
            Params.MoveKings = false;
            Params.RLMethod = Params.RLMethods.Q_Zero;
            Params.IsStateUniformNeutral = false;
        }

        protected override QTableBase InstantiateQTable()
        {
            return new RL1PQTable(EnvRows, EnvCols, MyUnum);
        }

        protected override int TeammatesCount
        {
            get { return 1; }
        }

        protected override string PerformanceLoggerMethodName
        {
            get { return "RL1P"; }
        }
    }
}
