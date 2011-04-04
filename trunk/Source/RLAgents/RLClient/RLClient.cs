using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.ClientBasic;
using GridSoccer.Common;
using GridSoccer.RLAgentsCommon;

namespace RLClient
{
    class RLClient : RLClientBase
    {
        public RLClient(string teamName, int unum)
            : base("127.0.0.1", 5050, teamName, unum)
        {
            if (unum == 1)
                SetHomePosition(2, 3);
            else if (unum == 2)
                SetHomePosition(EnvRows - 1, 3);

            if (unum == 2)
                m_performanceLogger.Enabled = false;
        }

        protected override QTableBase InstantiateQTable()
        {
            return new QTable(this.EnvRows, this.EnvCols, TeammatesCount, MyUnum);
        }

        public override string PerformanceLoggerMethodName
        {
            get { return "ReplacingTrace"; }
        }

        protected override void SetGlobalParams()
        {
            Params.MoveKings = false;
            Params.RLMethod = Params.RLMethods.Q_Lambda_Naive;
            Params.TraceType = Params.EligibilityTraceTypes.Replacing;
            Params.IsStateUniformNeutral = false;
        }

        protected override int TeammatesCount
        {
            get { return 2; }
        }
    }
}
