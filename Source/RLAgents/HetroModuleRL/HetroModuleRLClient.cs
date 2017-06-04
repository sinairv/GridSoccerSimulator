using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.ClientBasic;
using GridSoccer.Common;
using GridSoccer.RLAgentsCommon;

namespace GridSoccer.HetroModuleRL
{
    /// <summary>
    /// Hetro Module Reinforcement Learning
    /// </summary>
    public class HetroModuleRLClient : RLClientBase
    {
        // another reference to the Q-Table
        private HetroModularQTable m_hetroModuleQTable;

        public HetroModuleRLClient(string teamName, int unum)
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
            m_hetroModuleQTable = new HetroModularQTable(EnvRows, EnvCols, TeammatesCount, MyUnum);
            return m_hetroModuleQTable;
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
            get { return "4MWOTriple"; }
        }

        private void PrintModuleSelectionPercents()
        {
            long[] counts = m_hetroModuleQTable.GetModuleSelectionCounts();
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

            if (Cycle  % 5000 == 0)
            {
                PrintModuleSelectionPercents();
            }

            return base.RLThink();
        }
    }
}
