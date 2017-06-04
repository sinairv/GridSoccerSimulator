using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.RLAgentsCommon.Modules;
using GridSoccer.Common;

namespace GridSoccer.HMQLLambda
{
    /// <summary>
    /// Hetrogenous Modular Reinforcement Learning Lambda Client
    /// </summary>
    public class HMRLLambdaClient : RLClientBase
    {
        private QTable m_modularqTable;

        public HMRLLambdaClient(string teamName, int unum)
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
            m_modularqTable = new QTable(EnvRows, EnvCols, TeammatesCount, MyUnum);
            return m_modularqTable;
        }

        /// <summary>
        /// Gets the number of teammates including the player.
        /// </summary>
        /// <value>The teammates count.</value>
        protected override int TeammatesCount
        {
            get { return 2; }
        }

        public override string PerformanceLoggerMethodName
        {
            get 
            {
                if (Program.s_numModules == 3)
                    return "3M";
                else if (Program.s_numModules == 5)
                    return "3M+Self+Rel";
                else
                    return "3M+Self+Rel"; 
            }
        }

        private void PrintModuleSelectionPercents()
        {
            long[] counts = m_modularqTable.GetModuleSelectionCounts();
            int cycle = this.Cycle;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < counts.Length; ++i)
            {
                sb.AppendFormat("{0:F03}   ", (double)counts[i] / cycle);
            }
            Console.WriteLine(sb.ToString());
        }

        protected override int RLThink()
        {
            //Console.WriteLine(m_curState.ToString());

            if (Cycle % 5000 == 0)
            {
                PrintModuleSelectionPercents();
            }

            return base.RLThink();
        }

    }
}
