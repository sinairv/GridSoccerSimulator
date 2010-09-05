// Copyright (c) 2009 - 2010 
//  - Sina Iravanian <sina@sinairv.com>
//  - Sahar Araghi   <sahar_araghi@aut.ac.ir>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.ClientBasic;
using GridSoccer.Common;
using System.IO;
using System.Threading;

namespace GridSoccer.RLAgentsCommon
{
    public abstract class RLClientBase : ClientBase
    {
        protected Random m_rnd = new Random();
        protected PerformanceLogger m_performanceLogger;
        protected QTableBase m_qTable;

        private Mutex m_mutexQTable = new Mutex();

        protected State m_curState = null;
        protected State m_prevState = null;
        protected int m_prevActionIndex = 0;

        public RLClientBase(string serverAddr, int serverPort, string teamname, int unum) 
            : base(serverAddr, serverPort, teamname, unum)
        {
            // the order of these statements are toooo important
            SetGlobalParams();
            m_qTable = InstantiateQTable();
            m_performanceLogger = new PerformanceLogger(String.Format("Logs/{0}-{4}-{3}-{2}-{1}",
                teamname, unum,
                Params.IsStateUniformNeutral ? "UniformNeutral" : "UniformBased",
                Params.RLMethod.ToString(),
                PerformanceLoggerMethodName
                ));

            LogSimulatorSettings();
        }

        private void LogSimulatorSettings()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("%-- Simulator Settings -----------------------------------------------------------------");
            sb.AppendLine(String.Format("% Rows: {0}", this.EnvRows));
            sb.AppendLine(String.Format("% Cols: {0}", this.EnvCols));
            sb.AppendLine(String.Format("% GoalWidth: {0}", this.EnvGoalWidth));
            sb.AppendLine(String.Format("% PassDistance: {0}", this.EnvPassDistance));
            sb.AppendLine(String.Format("% VisibilityDistance: {0}", this.EnvVisibilityDistance));
            sb.AppendLine("%-- Client Internal Settings -----------------------------------------------------------");
            sb.Append(Params.GetParamsString());
            sb.Append    ("%---------------------------------------------------------------------------------------");
            
            m_performanceLogger.WriteLine(sb.ToString());
        }

        protected abstract void SetGlobalParams();

        protected abstract QTableBase InstantiateQTable();

        public void SaveQTable(TextWriter tw)
        {
            m_mutexQTable.WaitOne();
            m_qTable.Save(tw);
            m_mutexQTable.ReleaseMutex();
        }

        public void LoadQTable(TextReader tr)
        {
            m_mutexQTable.WaitOne();
            m_qTable.Load(tr);
            m_mutexQTable.ReleaseMutex();
        }

        /// <summary>
        /// Gets the number of teammates including the player.
        /// </summary>
        /// <value>The teammates count.</value>
        protected abstract int TeammatesCount { get; }

        protected abstract string PerformanceLoggerMethodName { get; }

        /// <summary>
        /// updates the current state varialbe (i.e., m_curState) with the 
        /// data available at the current time cycle
        /// </summary>
        protected virtual State GetCurrentState()
        {
            State s = new State(GetPlayerInfoFromIndex(m_myIndex));
            foreach (int n in GetAvailableTeammatesIndeces())
                s.AddOurPlayer(GetPlayerInfoFromIndex(n));

            foreach (int n in GetAvailableOpponentsIndeces())
                s.AddOppPlayer(GetPlayerInfoFromIndex(n));

            s.OurScore = this.OurScore;
            s.OppScore = this.OppScore;
            return s;
        }

        protected PlayerInfo GetPlayerInfoFromIndex(int i)
        {
            Position pos = new Position(PlayerPositions[i]);

            return new PlayerInfo()
            {
                Unum = base.GetPlayerUnumFromIndex(i), 
                Position = pos, 
                IsBallOwner = (pos == BallPosition)
            };
        }

        /// <summary>
        /// keep previous state and updste current state and then set previous action
        /// </summary>
        /// <returns></returns>
        protected override SoccerAction Think()
        {
            m_mutexQTable.WaitOne();
            m_prevState = m_curState;
            m_curState = GetCurrentState();
            m_qTable.SetCurrentState(m_curState);

            int actIndex;
            try
            {
                actIndex = RLThink();
            }
            catch (Exception ex)
            {
                Console.WriteLine("-------------");
                Console.WriteLine(ex.ToString());
                throw;
                //act = new SoccerAction(ActionTypes.Hold);
            }

            m_prevActionIndex = actIndex;
            m_mutexQTable.ReleaseMutex();
            return SoccerAction.GetActionFromIndex(actIndex, Params.MoveKings, this.MyUnum);
        }

        protected virtual int RLThink()
        {
            int greedyActIndex = m_qTable.GetCurrentGreedyActionIndex();

            int actIndex = ChooseActionEpsilonGreedy(Params.Epsillon, SoccerAction.GetActionCount(Params.MoveKings, TeammatesCount), greedyActIndex);
            //SoccerAction act = SoccerAction.GetActionFromIndex(actIndex, Params.MoveKings, MyUnum);

            if (Cycle > 0) // because in cycle 0 there is no prev state
            {
                switch (Params.RLMethod)
                {
                    case Params.RLMethods.Q_Zero:
                        m_qTable.UpdateQ_QLearning(m_prevActionIndex);
                        break;
                    case Params.RLMethods.SARSA_Zero:
                        m_qTable.UpdateQ_SARSA(m_prevActionIndex, actIndex);
                        break;
                    case Params.RLMethods.SARSA_Lambda:
                        m_qTable.UpdateQ_SARSA_Lambda(m_prevActionIndex, actIndex);
                        break;
                    case Params.RLMethods.Q_Lambda_Watkins:
                        m_qTable.UpdateQ_QL_Watkins(m_prevActionIndex, actIndex, false);
                        break;
                    case Params.RLMethods.Q_Lambda_Naive:
                        m_qTable.UpdateQ_QL_Watkins(m_prevActionIndex, actIndex, true);
                        break;
                    default:
                        break;
                }

                if (m_performanceLogger.Enabled)
                {
                    double reward = EnvironmentModeler.GetReward(m_prevState, m_curState,
                        SoccerAction.GetActionTypeFromIndex(m_prevActionIndex, Params.MoveKings));
                    m_performanceLogger.Log(Cycle, reward, OurScore, OppScore);
                }

            }

            return actIndex;
        }

        protected virtual int ChooseActionEpsilonGreedy(double eps, int actionsCount, int greedyActIndex)
        {
            return m_rnd.NextDouble() <= eps ? m_rnd.Next(0, actionsCount) : greedyActIndex;
        }
    }
}
