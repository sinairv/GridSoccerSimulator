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

namespace GridSoccer.RLAgentsCommon
{
    public class EligibilityTrace
    {
        #region StateActionPair internal struct
        public struct StateActionPair
        {
            private State m_state;
            private string m_stateString;
            private int m_actionIndex;

            public State State
            { get { return m_state; } }

            public int ActionIndex
            { get { return m_actionIndex; } }

            public StateActionPair(State state, int actionIndex)
            {
                this.m_state = state;
                this.m_actionIndex = actionIndex;
                this.m_stateString = m_state.GetStateString();
            }

            public StateActionPair(string stateString, int actionIndex)
            {
                m_stateString = stateString;
                m_actionIndex = actionIndex;
                m_state = null;
            }

            public override bool Equals(object obj)
            {
                if (obj is StateActionPair)
                {
                    StateActionPair sec = (StateActionPair)obj;
                    return this.m_actionIndex == sec.m_actionIndex && this.m_stateString == sec.m_stateString;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return m_stateString.GetHashCode() + m_actionIndex.GetHashCode();
            }

            public override string ToString()
            {
                return String.Format("({0} -> {1})", m_stateString, m_actionIndex);
            }

            public static bool operator ==(StateActionPair p1, StateActionPair p2)
            {
                return p1.Equals(p2);
            }

            public static bool operator !=(StateActionPair p1, StateActionPair p2)
            {
                return !p1.Equals(p2);
            }
        }
        #endregion

        private Dictionary<StateActionPair, double> m_dicTrace = new Dictionary<StateActionPair, double>();

        public void ClearTrace()
        {
            m_dicTrace.Clear();
        }

        public double GetValue(State state, int actionIndex)
        {
            return m_dicTrace[new StateActionPair(state.GetStateString(), actionIndex)];
        }

        public bool ContainsStateActionPair(State state, int actionIndex)
        {
            return m_dicTrace.ContainsKey(new StateActionPair(state.GetStateString(), actionIndex));
        }

        public void UpdateValue(State state, int actionIndex, double newValue)
        {
            m_dicTrace[new StateActionPair(state.GetStateString(), actionIndex)] = newValue;
        }

        public void IncrementValue(State state, int actionIndex)
        {
            m_dicTrace[new StateActionPair(state.GetStateString(), actionIndex)]++;
        }

        public void MultiplyValue(State state, int actionIndex, double coef)
        {
            m_dicTrace[new StateActionPair(state.GetStateString(), actionIndex)] *= coef;
        }


        /// <summary>
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="actIndexToIgnore">The act index to ignore, specify -1 to remove all actions.</param>
        /// <param name="actionsCount">The actions count.</param>
        public void RemoveTracesForState(State state, int actIndexToIgnore, int actionsCount)
        {
            string stateString = state.GetStateString();
            for (int i = 0; i < actionsCount; ++i)
            {
                if (i == actIndexToIgnore)
                    continue;

                m_dicTrace.Remove(new StateActionPair(stateString, i));
            }
        }

        public void AddStateActionPair(State state, int actionIndex, double value)
        {
            m_dicTrace.Add(new StateActionPair(state, actionIndex), value);
        }

        public IEnumerable<KeyValuePair<StateActionPair, double>> GetTraceItems()
        {
            foreach (var item in m_dicTrace)
            {
                yield return item;
            }
        }
    }
}
