using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.NEATAgentsBase
{
    public class NeatPlayerPerformanceStats
    {
        public double Reward;
        public int GoalsScoredByMe;
        public int GoalsScoredByTeam;
        public int GoalsReceived;
        public int PerformanceTime;

        public NeatPlayerPerformanceStats()
        {
            Reset();
        }

        public void Reset()
        {
            Reward = 0.0;
            GoalsReceived = 0;
            GoalsScoredByMe = 0;
            GoalsScoredByTeam = 0;
            PerformanceTime = 0;
        }
    }
}
