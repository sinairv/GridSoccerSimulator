// Copyright (c) 2009 - 2011 
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
using GridSoccer.Common;
using System.IO;

namespace GridSoccer.RLAgentsCommon
{
    public abstract class QTableBase
    {
        protected EligibilityTrace m_eTrace = null;
        protected State m_curState;
        protected State m_prevState;
        protected bool m_updateOnlyTDTarget = false;

        public QTableBase()
            : this(false)
        {
        }

        /// <summary>
        /// 
        /// Initializes a new instance of the <see cref="QTableBase"/> class.
        /// </summary>
        /// <param name="updateOnlyTDs">Default value is <c>false</c>. If set 
        /// to <c>true</c> the values passed to
        /// <c>UpdateQValue</c> method contains only the temporal difference target part;
        /// which is (r + \gamma \max Q).
        /// If set to <c>false</c> the new Q value is calculated from the RL learning 
        /// rule which applies learning rate and adds the temporal difference to the old
        /// Q value. The value hence passed to the <c>UpdateQValue</c> method would be:
        /// oldQ + \alpha * (r + \gamma \max Q - oldQ)
        /// 
        /// The table based Q tables are interested in the latter case, while neural Q tables
        /// use the first case. The default case is the latter (i.e., <c>false</c>).
        /// </param>
        public QTableBase(bool updateOnlyTDTarget)
        {
            m_updateOnlyTDTarget = updateOnlyTDTarget;
            if (Params.RLMethod != Params.RLMethods.Q_Zero && Params.RLMethod != Params.RLMethods.SARSA_Zero)
                m_eTrace = new EligibilityTrace();
        }


        protected abstract double GetQValue(State s, int ai);

        protected abstract void UpdateQValue(State s, int ai, double newValue);

        protected abstract int MyUnum { get; }

        protected abstract int TeammatesCount { get; }

        public abstract void Save(TextWriter tw);

        public abstract void Load(TextReader tr);
        
        public abstract Array QTableArray { get; }

        public virtual void SetCurrentState(State s)
        {
            m_prevState = m_curState;
            m_curState = DicomposeState(s);
        }

        protected virtual State DicomposeState(State s)
        {
            return s;
        }

        public int GetCurrentGreedyActionIndex()
        {
            double dummy;
            return GetCurStateMaxQ(out dummy);
        }

        //public int GetGreedyActionIndex(State s)
        //{
        //    double dummy;
        //    return GetMaxQ(s, out dummy);
        //}

        public virtual int GetCurStateMaxQ(out double maxQValue)
        {
            return GetMaxQ(m_curState, out maxQValue);
        }

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

        public double GetQValue(State s, SoccerAction a)
        {
            int aIndex = SoccerAction.GetIndexFromAction(a, Params.MoveKings, MyUnum);
            return GetQValue(s, aIndex);
        }

        protected void UpdateQValue(State s, SoccerAction a, double newValue)
        {
            int aIndex = SoccerAction.GetIndexFromAction(a, Params.MoveKings, MyUnum);
            UpdateQValue(s, aIndex, newValue);
        }

        protected void UpdateQ_SARSA(double reward, State prevState, State curState, int prevActIndex, int curActIndex)
        {
            double oldQ = GetQValue(prevState, prevActIndex);
            double qOfNewState = GetQValue(curState, curActIndex);
            
            double newQ;
            if (m_updateOnlyTDTarget)
            {
                newQ = reward + Params.Gamma * qOfNewState;
            }
            else
            {
                newQ = oldQ + Params.Alpha * (reward + Params.Gamma * qOfNewState - oldQ);
            }

            UpdateQValue(prevState, prevActIndex, newQ);
        }

        protected void UpdateQ_QLearning(double reward, State prevState, State curState, int prevActIndex)
        {
            double maxQ = 0.0;
            GetMaxQ(curState, out maxQ);
            double oldQ = GetQValue(prevState, prevActIndex);
            double newQ;

            if (m_updateOnlyTDTarget)
            {
                newQ = reward + Params.Gamma * maxQ;
            }
            else
            {
                newQ = oldQ + Params.Alpha * (reward + Params.Gamma * maxQ - oldQ);
            }

            UpdateQValue(prevState, prevActIndex, newQ);
        }

        protected void UpdateQ_QL_Watkins(double reward, State prevState, State curState, int prevActIndex, int curActIndex, bool isNaive)
        {
            double oldQ = GetQValue(prevState, prevActIndex);

            double maxQ = 0.0;
            GetMaxQ(curState, out maxQ);

            double delta;
            if(m_updateOnlyTDTarget)
            {
                delta = reward + Params.Gamma * maxQ;
            }
            else
            {
                delta = reward + Params.Gamma * maxQ - oldQ;
            }

            bool isGreedy =
                GetCurrentGreedyActionIndex() == curActIndex;

            if (!m_eTrace.ContainsStateActionPair(prevState, prevActIndex))
                m_eTrace.AddStateActionPair(prevState, prevActIndex, 0.0);
            //else
            //    Console.WriteLine("Trace exists! " + prevState.GetStateString());

            if (Params.TraceType == Params.EligibilityTraceTypes.Accumulating)
            {
                m_eTrace.IncrementValue(prevState, prevActIndex);
            }
            else if (Params.TraceType == Params.EligibilityTraceTypes.Replacing)
            {
                m_eTrace.RemoveTracesForState(prevState, prevActIndex, SoccerAction.GetActionCount(Params.MoveKings, TeammatesCount));
                m_eTrace.UpdateValue(prevState, prevActIndex, 1.0);
            }

            var listTraceItems = m_eTrace.GetTraceItems().ToList();
            foreach (var pair in listTraceItems)
            {
                double q = GetQValue(pair.Key.State, pair.Key.ActionIndex);
                double e = pair.Value;

                double newQ;
                if (m_updateOnlyTDTarget)
                {
                    newQ = delta * e;
                }
                else
                {
                    newQ = q + Params.Alpha * delta * e;
                }


                UpdateQValue(pair.Key.State, pair.Key.ActionIndex, newQ);

                if (isGreedy)
                    m_eTrace.MultiplyValue(pair.Key.State, pair.Key.ActionIndex, Params.Gamma * Params.Lambda);
            }

            if (!isNaive && !isGreedy)
                m_eTrace.ClearTrace();

            // see if episode changed
            if (curState.OurScore != prevState.OurScore || curState.OppScore != prevState.OppScore)
                m_eTrace.ClearTrace();
        }

        protected void UpdateQ_SARSA_Lambda(double reward, State prevState, State curState, int prevActIndex, int curActIndex)
        {
            double oldQ = GetQValue(prevState, prevActIndex);
            double qOfNewState = GetQValue(curState, curActIndex);

            double delta;
            if (m_updateOnlyTDTarget)
            {
                delta = reward + Params.Gamma * qOfNewState;
            }
            else
            {
                delta = reward + Params.Gamma * qOfNewState - oldQ;
            }

            if (!m_eTrace.ContainsStateActionPair(prevState, prevActIndex))
                m_eTrace.AddStateActionPair(prevState, prevActIndex, 0.0);
            else
                Console.WriteLine("Trace exists! " + prevState.GetStateString());

            if (Params.TraceType == Params.EligibilityTraceTypes.Accumulating)
            {
                m_eTrace.IncrementValue(prevState, prevActIndex);
            }
            else if (Params.TraceType == Params.EligibilityTraceTypes.Replacing)
            {
                m_eTrace.RemoveTracesForState(prevState, prevActIndex, SoccerAction.GetActionCount(Params.MoveKings, TeammatesCount));
                m_eTrace.UpdateValue(prevState, prevActIndex, 1.0);
            }

            var listTraceItems = m_eTrace.GetTraceItems().ToList();
            foreach (var pair in listTraceItems)
            {
                double q = GetQValue(pair.Key.State, pair.Key.ActionIndex);
                double e = pair.Value;
                double newQ;

                if (m_updateOnlyTDTarget)
                {
                    newQ = delta * e;
                }
                else
                {
                    newQ = q + Params.Alpha * delta * e;
                }

                UpdateQValue(pair.Key.State, pair.Key.ActionIndex, newQ);
                m_eTrace.MultiplyValue(pair.Key.State, pair.Key.ActionIndex, Params.Gamma * Params.Lambda);
            }

            // see if episode changed
            if (curState.OurScore != prevState.OurScore || curState.OppScore != prevState.OppScore)
                m_eTrace.ClearTrace();
        }


        public virtual void UpdateQ_SARSA(int prevActIndex, int curActIndex)
        {
            double reward = EnvironmentModeler.GetReward(m_prevState, m_curState, 
                SoccerAction.GetActionTypeFromIndex( prevActIndex,Params.MoveKings));
            UpdateQ_SARSA(reward, m_prevState, m_curState, prevActIndex, curActIndex);
        }

        public virtual void UpdateQ_QLearning(int prevActIndex)
        {
            double reward = EnvironmentModeler.GetReward(m_prevState, m_curState,
                SoccerAction.GetActionTypeFromIndex(prevActIndex, Params.MoveKings));
            UpdateQ_QLearning(reward, m_prevState, m_curState, prevActIndex);
        }

        public virtual void UpdateQ_QL_Watkins(int prevActIndex, int curActIndex, bool isNaive)
        {
            double reward = EnvironmentModeler.GetReward(m_prevState, m_curState,
                SoccerAction.GetActionTypeFromIndex(prevActIndex, Params.MoveKings));
            UpdateQ_QL_Watkins(reward, m_prevState, m_curState, prevActIndex, curActIndex, isNaive);
        }

        public virtual void UpdateQ_SARSA_Lambda(int prevActIndex, int curActIndex)
        {
            double reward = EnvironmentModeler.GetReward(m_prevState, m_curState,
                SoccerAction.GetActionTypeFromIndex(prevActIndex, Params.MoveKings));
            UpdateQ_SARSA_Lambda(reward, m_prevState, m_curState, prevActIndex, curActIndex);
        }

        public virtual void UpdateQ_Evolutionary(int prevActIndex, double reward)
        {
            UpdateQValue(m_prevState, prevActIndex, reward);
        }
    }
}
