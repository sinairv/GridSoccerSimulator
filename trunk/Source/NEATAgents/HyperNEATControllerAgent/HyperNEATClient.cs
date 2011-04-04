using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.NEATAgentsBase;

namespace GridSoccer.HyperNEATControllerAgent
{
    public class HyperNEATClient : RLClientBase
    {
        //protected int m_teammatesCount = 1;
        public HyperNEATClient(string teamname, int unum, int teammates)
            : base("127.0.0.1", 5050, teamname, unum)
        {
            //m_teammatesCount = teammates;

            if (unum == 1)
                SetHomePosition(2, 2);
            else if (unum == 2)
                SetHomePosition(EnvRows - 1, 2);
            else if (unum == 3)
                SetHomePosition(EnvRows / 2 + 1, 4);
        }

        //public HyperNEATClient(string teamname, int unum)
        //    : this(teamname, unum, 1)
        //{
        //}

        protected override void SetGlobalParams()
        {
            Params.MoveKings = false;

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

            Params.RLMethod = Params.RLMethods.Evolutionary;
            Params.Epsillon = 0.0; // 0.01; // this small value may prevent unwanted loops in the game

            // these values are not used by the clients
            Params.Alpha = 0.0;
            Params.Gamma = 1.0;

            Params.IsStateUniformNeutral = false;

            // Now setting EA Params
            NeatExpParams.AddBiasToSubstrate = true;
            NeatExpParams.SubstrateLayers = 3;
            NeatExpParams.EpisodesPerGenome = 4;
            NeatExpParams.PopulationSize = 150;

            Console.WriteLine("AddBiasToSubstrate: {0}", NeatExpParams.AddBiasToSubstrate);
            Console.WriteLine("SubstrateLayers: {0}", NeatExpParams.SubstrateLayers);
            Console.WriteLine("PopulationSize: {0}", NeatExpParams.PopulationSize);


            NeatExpParams.CreditAssignment = CreditAssignments.Global;
            NeatExpParams.DoSimplifyingPhase = false;
            NeatExpParams.SaveEachGenerationChampionCPPN = true;
            NeatExpParams.SaveFitnessGrowth = true;
            NeatExpParams.GoalScoreEffect = 1000.0;
            NeatExpParams.GoalRecvEffect = 100.0;
        }

        protected override QTableBase InstantiateQTable()
        {
            switch (Program.ExpType)
            {
                case ExperimentTypes.CCEAFieldSubs:
                    return new HyperNEATQTable(this, TeammatesCount, MyUnum);
                case ExperimentTypes.FourDFieldSubs:
                    return new FourDFieldSubs.FourDFieldSubsQTable(this, TeammatesCount, MyUnum);
                case ExperimentTypes.CCEAGeomCtrl:
                    return new CCEAGeomCtrl.CCEAGeomCtrlQTable(this, TeammatesCount, MyUnum);
                case ExperimentTypes.CCEA3LayersFieldSubs:
                    return new CCEA3LayersFieldSubs.FieldSubs3LQTable(this, TeammatesCount, MyUnum);
                default:
                    break;
            }

            throw new Exception("Proper Q Table not specified");
        }

        protected override int TeammatesCount
        {
            get { return Program.Teammates; }
        }

        public override string PerformanceLoggerMethodName
        {
            get 
            { 
                return String.Format("{3}-{2}Credit-{0}-{1}",
                    NeatExpParams.AddBiasToSubstrate ? "WithBias" : "NoBias",
                    NeatExpParams.DoSimplifyingPhase ? "SimpComp" : "OnlyComp",
                    NeatExpParams.CreditAssignment.ToString(),
                    Program.ExpType);
            }
        }


    }
}
