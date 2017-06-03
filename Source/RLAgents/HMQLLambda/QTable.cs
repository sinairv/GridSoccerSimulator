using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.RLAgentsCommon.Modules;
using GridSoccer.Common;
using System.IO;

namespace GridSoccer.HMQLLambda
{
    public class QTable : QTableBase
    {
        private List<QTableBase> m_modules = new List<QTableBase>();
        private List<long> m_moduleSelectionCounts = new List<long>();

        private int m_teammatesCount;
        private int m_myUnum;

        public QTable (int rows, int cols, int teammatesCount, int myUnum)
        {
            m_myUnum = myUnum;
            m_teammatesCount = teammatesCount;

            if (Program.s_numModules == 3)
            {
                AddModule(new SelfAndTeammateModule(rows, cols, teammatesCount, myUnum, 0));
                AddModule(new SelfAndOneOpponentModule(rows, cols, teammatesCount, myUnum, 0));
                AddModule(new SelfAndOneOpponentModule(rows, cols, teammatesCount, myUnum, 1));
            }
            else if (Program.s_numModules == 5)
            {
                AddModule(new SelfOnlyModule(rows, cols, teammatesCount, myUnum));
                AddModule(new PartialModule(rows, cols, 2, teammatesCount, 2, myUnum));
                AddModule(new SelfAndTeammateModule(rows, cols, teammatesCount, myUnum, 0));
                AddModule(new SelfAndOneOpponentModule(rows, cols, teammatesCount, myUnum, 0));
                AddModule(new SelfAndOneOpponentModule(rows, cols, teammatesCount, myUnum, 1));
            }
            else // modify here for default settings
            {
                // TODO: instantiate the modules and add them to the m_modules list
                AddModule(new SelfOnlyModule(rows, cols, teammatesCount, myUnum));
                AddModule(new PartialModule(rows, cols, 2, teammatesCount, 2, myUnum));
                AddModule(new SelfAndTeammateModule(rows, cols, teammatesCount, myUnum, 0));
                AddModule(new SelfAndOneOpponentModule(rows, cols, teammatesCount, myUnum, 0));
                AddModule(new SelfAndOneOpponentModule(rows, cols, teammatesCount, myUnum, 1));
                AddModule(new TripleModule(rows, cols, teammatesCount, myUnum, 0, 0));
                AddModule(new TripleModule(rows, cols, teammatesCount, myUnum, 0, 1));
            }
        }

        private void AddModule(QTableBase module)
        {
            m_modules.Add(module);
            m_moduleSelectionCounts.Add(0L);
        }

        protected override double GetQValue(State s, int ai)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The mediator module of the modular Q-Learning structure.
        /// It gets the greedy actions of all the modules, and returns the
        /// action index corresponding to the maximum q-value.
        /// Returns the index of the action.
        /// </summary>
        public override int GetCurStateMaxQ(out double maxQValue)
        {
            int ai = 0;
            double maxModMaxQ = Double.MinValue;

            double curModMaxQ = 0.0;
            int curModAi;

            int greedyModuleIndex = -1;

            for (int i = 0; i < m_modules.Count; ++i)
            {
                var module = m_modules[i];
                curModAi = module.GetCurStateMaxQ(out curModMaxQ);
                if (curModMaxQ > maxModMaxQ)
                {
                    maxModMaxQ = curModMaxQ;
                    ai = curModAi;
                    greedyModuleIndex = i;
                }
            }

            m_moduleSelectionCounts[greedyModuleIndex]++;

            maxQValue = maxModMaxQ;
            return ai;
        }

        public override void SetCurrentState(State s)
        {
            foreach (var module in m_modules)
            {
                module.SetCurrentState(s);
            }
        }

        public override void UpdateQ_QLearning(int prevActIndex)
        {
            foreach (var module in m_modules)
            {
                module.UpdateQ_QLearning(prevActIndex);
            }
        }

        public override void UpdateQ_SARSA(int prevActIndex, int curActIndex)
        {
            foreach (var module in m_modules)
            {
                module.UpdateQ_SARSA(prevActIndex, curActIndex);
            }
        }

        public override void UpdateQ_QL_Watkins(int prevActIndex, int curActIndex, bool isNaive)
        {
            foreach (var module in m_modules)
            {
                module.UpdateQ_QL_Watkins(prevActIndex, curActIndex, isNaive);
            }
        }

        public override void UpdateQ_SARSA_Lambda(int prevActIndex, int curActIndex)
        {
            foreach (var module in m_modules)
            {
                module.UpdateQ_SARSA_Lambda(prevActIndex, curActIndex);
            }
        }

        public long[] GetModuleSelectionCounts()
        {
            return m_moduleSelectionCounts.ToArray();
        }

        protected override int MyUnum
        {
            get { return m_myUnum; }
        }

        protected override int TeammatesCount
        {
            get { return m_teammatesCount; }
        }

        public override void Save(TextWriter tw)
        {
            foreach (var module in m_modules)
            {
                module.Save(tw);
                // put an empty file after each module, since we expect 
                // that every array should end in a new-line
                tw.WriteLine();
            }
        }

        public override void Load(TextReader tr)
        {
            foreach (var module in m_modules)
                module.Load(tr);
        }

        public override Array QTableArray
        {
            get { return null; }
        }

    }
}
