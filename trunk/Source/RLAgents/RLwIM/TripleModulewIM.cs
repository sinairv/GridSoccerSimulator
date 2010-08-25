using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;
using System.IO;

namespace RLwIM
{
    class TripleModulewIM : QTableBase
    {
        private int m_myUnum = -1;
        private int m_oppToMonitor = -1;
        private int m_teammateToMonitor = -1;
        private int m_teammatesCount;
        private int m_opponentsCount;

        private State m_prevOriginalState;
        private State m_curOriginalState;


        private double[, , , , , , , ,] m_QTable;
        private double[, , , , , , ,] m_InternalModel;


        /// <summary>
        /// Initializes a new instance of the <see cref="TripleModule"/> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The cols.</param>
        /// <param name="teammatesCount">The teammates count.</param>
        /// <param name="myUnum">My unum.</param>
        /// <param name="teammateToMonitor">0-based index of the teammate to monitor.</param>
        /// <param name="opponentToMonitor">0-based index of the opponent to monitor.</param>
        public TripleModulewIM(int rows, int cols, int teammatesCount, int opponentsCount, int myUnum, int teammateToMonitor, int opponentToMonitor)
        {
            m_myUnum = myUnum;
            m_oppToMonitor = opponentToMonitor;
            m_teammateToMonitor = teammateToMonitor;
            m_teammatesCount = teammatesCount;
            m_opponentsCount = opponentsCount;

            m_QTable = new double[
                rows, cols,              // my position 
                rows, cols,              // my teammate's position
                rows, cols,              // one of opponents's position
                5,                       // ball owner index (0: me)(1: the-teammate)(2:We)(3: the opponent)(4: they)
                SoccerAction.GetActionCount(Params.MoveKings, teammatesCount), // number of actions_self
                SoccerAction.GetActionCount(Params.MoveKings, opponentsCount) // number of actions_opp
            ];

            m_InternalModel = new double[
                rows, cols,              // my position 
                rows, cols,              // my teammate's position
                rows, cols,              // one of opponents's position
                5,                       // ball owner index (0: me)(1: the-teammate)(2:We)(3: the opponent)(4: they)
                SoccerAction.GetActionCount(Params.MoveKings, opponentsCount)       // number of actions_opp
            ];
        }

        protected override int MyUnum
        {
            get
            {
                return m_myUnum;
            }
        }

        protected override int TeammatesCount
        {
            get
            {
                return m_teammatesCount;
            }
        }

        protected override State DicomposeState(State s)
        {
            m_prevOriginalState = m_curOriginalState;
            m_curOriginalState = s;
            return s.GetDecomposedState(new int[] { m_teammateToMonitor }, new int[] { m_oppToMonitor });
        }

        // ball owner index (0: me)(1: the-teammate)(2:We)(3: the opponent)(4: they)
        private int GetBallOwnerIndex(State s)
        {
            if (s.AmIBallOwner)
                return 0;
            else if (s.AreWeBallOwner)
            {
                if (s.OurPlayersList[0].IsBallOwner)
                    return 1;
                else
                    return 2;
            }
            else
            {
                if (s.OppPlayersList[0].IsBallOwner)
                    return 3;
                else
                    return 4;
            }
        }

        protected double GetIMValue(State s, int ai_other)
        {
            return m_InternalModel[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OurPlayersList[0].Position.Row - 1, s.OurPlayersList[0].Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                GetBallOwnerIndex(s), ai_other];
        }

        protected void UpdateIMValue(State s, int ai_other, double newValue)
        {
            m_InternalModel[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OurPlayersList[0].Position.Row - 1, s.OurPlayersList[0].Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                GetBallOwnerIndex(s), ai_other] = newValue;
        }

        protected double GetQValue(State s, int ai_self, int ai_other)
        {
            return m_QTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OurPlayersList[0].Position.Row - 1, s.OurPlayersList[0].Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                GetBallOwnerIndex(s), ai_self, ai_other];
        }

        protected void UpdateQValue(State s, int ai_self, int ai_other, double newValue)
        {
            m_QTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OurPlayersList[0].Position.Row - 1, s.OurPlayersList[0].Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                GetBallOwnerIndex(s), ai_self, ai_other] = newValue;
        }

        protected override double GetQValue(State s, int ai_self)
        {
            int a_other_count = SoccerAction.GetActionCount(Params.MoveKings, m_opponentsCount);
            double sum = 0.0;
            for(int i = 0; i < a_other_count; ++i)
            {
                sum += GetQValue(s, ai_self, i) * GetIMValue(s, i);
            }
            return sum;
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            throw new NotImplementedException();
        }

        public override void UpdateQ_QLearning(int prevActIndex)
        {
            double reward = EnvironmentModeler.GetReward(m_prevState, m_curState,
                SoccerAction.GetActionTypeFromIndex(prevActIndex, Params.MoveKings));

            int ai_star_other = EnvironmentModeler.GuessOpponentAction(m_prevOriginalState, m_curOriginalState, m_oppToMonitor);

            UpdateIM_Learning(m_prevState, ai_star_other);

            int ai_prime_other = GetMaxIMIndex(m_prevState);

            double oldQ = GetQValue(m_prevState, prevActIndex, ai_star_other);
            double bestQ = GetBestQ(m_curState, ai_prime_other);
            double newQ = oldQ + Params.Alpha*(reward + Params.Gamma * bestQ - oldQ);
            UpdateQValue(m_prevState, prevActIndex, ai_star_other, newQ);
        }

        private void UpdateIM_Learning(State s, int ai_star_other)
        {
            int a_other_count = SoccerAction.GetActionCount(Params.MoveKings, m_opponentsCount);
            
            double curIMValue, newIMValue;
            for (int i = 0; i < a_other_count; ++i)
            {
                curIMValue = GetIMValue(s, i);
                newIMValue = (1 - Params.Theta)*curIMValue;
                if (i == ai_star_other)
                    newIMValue += Params.Theta;
                UpdateIMValue(s, i, newIMValue);
            }
            
        }

        private double GetBestQ(State s, int ai_prime_other)
        {
            int a_self_count = SoccerAction.GetActionCount(Params.MoveKings, m_teammatesCount);
            double maxQ = Double.MinValue;
            double curQ;
            for (int i = 0; i < a_self_count; ++i)
            {
                curQ = GetQValue(s, i, ai_prime_other);
                if (curQ > maxQ)
                    maxQ = curQ;
            }
            return maxQ;
        }

        protected int GetMaxIMIndex(State s)
        {
            int a_other_count = SoccerAction.GetActionCount(Params.MoveKings, m_opponentsCount);
            int maxIndex = 0;
            double maxValue = Double.MinValue;
            double curValue;
            for (int i = 0; i < a_other_count; ++i)
            {
                curValue = GetIMValue(s, i);
                if (curValue > maxValue)
                {
                    maxValue = curValue;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        public override void Save(TextWriter tw)
        {
            m_QTable.SaveArrayContents(tw);
        }

        public override void Load(TextReader tr)
        {
            m_QTable.LoadDoubleArrayContents(tr);
        }

        public override Array QTableArray
        {
            get { return m_QTable; }
        }
    }
}
