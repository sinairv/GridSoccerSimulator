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
            Params.RLMethod = Params.RLMethods.Q_Lambda_Watkins;
            Params.IsStateUniformNeutral = false;

            Params.RewardEveryMovment = -0.01;
            Params.RewardSuccessfulPass = -0.01;
            Params.RewardHold = -0.01;
            Params.RewardIllegalMovment = -0.01;

            Params.RewardTeamCatchBall = 0.01;
            Params.RewardTeamLooseBall = -0.02;

            Params.RewardSelfCatchBall = 0.02;
            Params.RewardSelfLooseBall = -0.02;

            Params.RewardTeamScoreGoal = 5;
            Params.RewardSelfScoreGoal = 5;
            Params.RewardTeamRecvGoal = -5;

            Params.RewardTeamOwnGoal = -5;
            Params.RewardSelfOwnGoal = -5;
            Params.RewardOpponentOwnGoal = 1;
        }

        protected override QTableBase InstantiateQTable()
        {
            return new RL1PQTable(EnvRows, EnvCols, MyUnum);
        }

        protected override int TeammatesCount
        {
            get { return 1; }
        }

        public override string PerformanceLoggerMethodName
        {
            get { return "RL1P"; }
        }
    }
}
