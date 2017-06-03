using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soccer.RLCommon;
using Soccer.Common;

namespace HetroModuleRL
{
    public abstract class IRLModule
    {
        public abstract double GetQValue(State s, int ai);

        public abstract void UpdateQValue(State s, int ai, double newValue);

        protected abstract int MyUnum { get; set; }

        protected abstract int TeammatesCount { get; set; }

        public int GetMaxQ(State s, out double maxQValue)
        {
            double curValue = 0.0;
            maxQValue = Double.MinValue;
            int maxIndex = 0;

            int count = SoccerAction.GetActionCount(Params.MoveKings, TeammatesCount);
            for (int i = 0; i < count; ++i)
            {
                curValue = GetQValue(s, i);
                if (curValue > maxQValue)
                {
                    maxQValue = curValue;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public double GetQValue(State s, SoccerAction a, int selfUnum)
        {
            int aIndex = SoccerAction.GetIndexFromAction(a, Params.MoveKings, selfUnum);
            return this.GetQValue(s, aIndex);
        }

        public void UpdateQValue(State s, SoccerAction a, int selfUnum, double newValue)
        {
            int aIndex = SoccerAction.GetIndexFromAction(a, Params.MoveKings, selfUnum);
            this.UpdateQValue(s, aIndex, newValue);
        }

        public void UpdateQ_SARSA(double reward, State prevState, State curState, SoccerAction prevAct, SoccerAction curAct)
        {
            double oldQ0 = GetQValue(prevState, prevAct, MyUnum);
            double qOfNewState0 = GetQValue(curState, curAct, MyUnum);
            double newQ0 = oldQ0 + Params.Alpha * (reward + Params.Gamma * qOfNewState0 - oldQ0);
            this.UpdateQValue(prevState, prevAct, MyUnum, newQ0);
        }

        public void UpdateQ_QLearning(double reward, State prevState, State curState, SoccerAction prevAct)
        {
            double maxQ0 = 0.0;
            GetMaxQ(curState, out maxQ0);
            double oldQ0 = GetQValue(prevState, prevAct, MyUnum);
            double newQ0 = oldQ0 + Params.Alpha * (reward + Params.Gamma * maxQ0 - oldQ0);
            UpdateQValue(prevState, prevAct, MyUnum, newQ0);
        }

        public void UpdateQ_SARSA(State prevState, State curState, SoccerAction prevAct, SoccerAction curAct)
        {
            double reward = EnvironmentModeler.GetReward(prevState, curState, prevAct.ActionType);
            this.UpdateQ_SARSA(reward, prevState, curState, prevAct, curAct);
        }

        public void UpdateQ_QLearning(State prevState, State curState, SoccerAction prevAct)
        {
            double reward = EnvironmentModeler.GetReward(prevState, curState, prevAct.ActionType);
            this.UpdateQ_QLearning(reward, prevState, curState, prevAct);
        }
    }

}
