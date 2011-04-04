using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;

namespace GridSoccer.NeuroRLClient
{
    public class NeuralClient : RLClientBase
    {
        private NeuralQTableBase m_neuroQTable;

        public QTableBase NeuralQTable
        {
            get { return m_neuroQTable; }
        }

        public NeuralClient(string teamname, int unum)
            : base("127.0.0.1", 5050, teamname, unum)
        {
            if (unum == 1)
                SetHomePosition(2, 3);
        }

        protected override void SetGlobalParams()
        {
            Params.MoveKings = false;

            Params.RewardTeamCatchBall = Params.RewardTeamScoreGoal / 2;
            Params.RewardTeamLooseBall = Params.RewardIllegalMovment;

            Params.RewardSelfCatchBall = Params.RewardSelfScoreGoal / 2;
            Params.RewardSelfLooseBall = Params.RewardIllegalMovment;

            Params.RLMethod = Params.RLMethods.Q_Zero;
            Params.Alpha = 0.1;
            Params.Gamma = 0.9;

            Params.IsStateUniformNeutral = false;
        }

        protected override QTableBase InstantiateQTable()
        {
            //m_neuroQTable = new RL1PQTable(this.EnvRows, this.EnvCols, 1);
            m_neuroQTable = new OnlineNFQTable(this, 1, 1);
            return m_neuroQTable;
        }

        protected override int TeammatesCount
        {
            get { return 1; }
        }

        public override string PerformanceLoggerMethodName
        {
            get { return m_neuroQTable.MethodName + Params.RLMethod.ToString(); }
        }
    }
}
