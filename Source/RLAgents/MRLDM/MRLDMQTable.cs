using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.RLAgentsCommon.Modules;
using System.IO;
using GridSoccer.RLAgentsCommon.CounterModules;

namespace MRLDM
{
    public class MRLDMQTable : QTableBase
    {
        private List<CounterModuleBase> m_counterModules = new List<CounterModuleBase>();
        private List<QTableBase> m_modules = new List<QTableBase>();
        private List<long> m_moduleSelectionCounts = new List<long>();

        private int m_teammatesCount;
        private int m_myUnum;

        public MRLDMQTable(int rows, int cols, int teammatesCount, int myUnum)
        {
            m_myUnum = myUnum;
            m_teammatesCount = teammatesCount;

            // TODO: instantiate the modules and add them to the m_modules list, 
            // the order is important
            AddModule(new SelfOnlyModule(rows, cols, teammatesCount, myUnum));
            AddCounterModule(new SelfOnlyCounterModule(rows, cols, teammatesCount, myUnum));

            AddModule(new PartialModule(rows, cols, 2, teammatesCount, 2, myUnum));
            AddCounterModule(new PartialCounterModule(rows, cols, 2, teammatesCount, 2, myUnum));

            AddModule(new SelfAndTeammateModule(rows, cols, teammatesCount, myUnum, 0));
            AddCounterModule(new SelfAndTeammateCounterModule(rows, cols, teammatesCount, myUnum, 0));

            AddModule(new SelfAndOneOpponentModule(rows, cols, teammatesCount, myUnum, 0));
            AddCounterModule(new SelfAndOneOpponentCounterModule(rows, cols, teammatesCount, myUnum, 0));

            AddModule(new SelfAndOneOpponentModule(rows, cols, teammatesCount, myUnum, 1));
            AddCounterModule(new SelfAndOneOpponentCounterModule(rows, cols, teammatesCount, myUnum, 1));

            //AddModule(new TripleModule(rows, cols, teammatesCount, myUnum, 0, 0));
            //AddCounterModule(new TripleCounterModule(rows, cols, teammatesCount, myUnum, 0, 0));
            
            //AddModule(new TripleModule(rows, cols, teammatesCount, myUnum, 0, 1));        
            //AddCounterModule(new TripleCounterModule(rows, cols, teammatesCount, myUnum, 0, 1));
        }

        private void AddModule(QTableBase module)
        {
            m_modules.Add(module);
            m_moduleSelectionCounts.Add(0L);
        }

        private void AddCounterModule(CounterModuleBase counterModule)
        {
            m_counterModules.Add(counterModule);
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

            foreach (var cmodule in m_counterModules)
            {
                cmodule.Save(tw);
                tw.WriteLine();
            }

        }

        public override void Load(TextReader tr)
        {
            foreach (var module in m_modules)
                module.Load(tr);

            foreach (var cmodule in m_counterModules)
                cmodule.Load(tr);
        }

        public override Array QTableArray
        {
            get { return null; }
        }

        public void UpdateCounterModules(State s, int ai)
        {
            foreach (var cm in m_counterModules)
            {
                cm.IncrementCountValue(s, ai);
            }
        }

        public int PerformDM()
        {
            int numChanges = 0;
            for(int i = 0; i < m_counterModules.Count; ++i)
            {
                m_counterModules[i].PerformKCyclicNeighborQUpdate(m_modules[i]);
                numChanges += m_counterModules[i].NumberOfDMUpdates;
            }

            return numChanges;
                
        }

        public double[,] GetStats()
        {
            double[,] stats = new double[m_counterModules.Count, 3];

            CounterModuleBase cm;
            for (int i = 0; i < m_counterModules.Count; ++i)
            {
                cm = m_counterModules[i];
                stats[i, 0] = cm.MaxSupport == Double.MinValue ? 0.0 : cm.MaxSupport;
                stats[i, 1] = cm.MeanSupport;
                stats[i, 2] = cm.MinSupport == Double.MaxValue ? 0.0 : cm.MinSupport;
            }

            return stats;
        }
    }
}
