using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;

namespace MRLDM
{
    public class MRLDMClient : RLClientBase
    {
        private MRLDMQTable m_mrldmTable;

        public MRLDMClient(string teamName, int unum)
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
            Params.RLMethod = Params.RLMethods.Q_Lambda_Watkins;
            Params.IsStateUniformNeutral = true;
            Params.TraceType = Params.EligibilityTraceTypes.Replacing;
        }

        protected override QTableBase InstantiateQTable()
        {
            m_mrldmTable = new MRLDMQTable(EnvRows, EnvCols, TeammatesCount, MyUnum);
            return m_mrldmTable;
        }

        protected override int TeammatesCount
        {
            get { return 2; }
        }

        string m_performanceLoggerMethodName = "";
        protected override string PerformanceLoggerMethodName
        {
            get { return m_performanceLoggerMethodName; }
        }

        public void SetLoggerMethodName(string str)
        {
            m_performanceLoggerMethodName = str;
        }

        protected override int RLThink()
        {
            int result = base.RLThink();
            m_mrldmTable.UpdateCounterModules(m_curState, result);
            return result;
        }

        public int PerformDM()
        {
            return m_mrldmTable.PerformDM();
        }

        public double[,] GetQTableStats()
        {
            return m_mrldmTable.GetStats();
        }

        public override void OnGameStopped()
        {
        }

    }
}
