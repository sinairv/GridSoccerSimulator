using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.NEATAgentsBase
{
    public class NeatExpParams
    {
        public static string EALogDir = "EALogs";
        public static bool SaveFitnessGrowth = true;
        public static bool SaveEachGenerationChampionCPPN = true;

        public static bool AddBiasToSubstrate = true;
        public static int SubstrateLayers = 3;

        //public static int CyclesPerGenome = 200;
        public static int EpisodesPerGenome = 4;

        public static bool DoSimplifyingPhase = false;
        public static int PopulationSize = 100;

        public static double GoalScoreEffect = 1000.0;
        public static double GoalRecvEffect = 100.0;

        public static CreditAssignments CreditAssignment = CreditAssignments.Global;

    }

    public enum CreditAssignments
    {
        Global,
        Local,
        Mid
    }
}
