using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;

namespace RLwIM
{
    public class RLwIMClient : RLClientBase
    {
        public RLwIMClient(string teamName, int unum)
            : base("127.0.0.1", 5050, teamName, unum)
        {
            if (unum == 1)
                SetHomePosition(2, 3);
            else if (unum == 2)
                SetHomePosition(EnvRows - 1, 3);

            if (unum == 2)
                m_performanceLogger.Enabled = false;
        }

        protected override void SetGlobalParams()
        {
            Params.RLMethod = Params.RLMethods.Q_Zero;
            Params.IsStateUniformNeutral = false;
        }

        protected override QTableBase InstantiateQTable()
        {
            return new RLwIMQTable(this.EnvRows, this.EnvCols, TeammatesCount, 2, MyUnum);
        }

        protected override int TeammatesCount
        {
            get { return 2; }
        }

        protected override string PerformanceLoggerMethodName
        {
            get { return ""; }
        }

    }
}
