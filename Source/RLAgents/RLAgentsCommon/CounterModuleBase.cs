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
    public abstract class CounterModuleBase
    {
        protected double m_minSupport = Double.MaxValue;
        protected double m_maxSupport = Double.MinValue;
        protected double m_meanSupport = 0.0;
        protected long m_stateVisits = 0;

        public double MinSupport
        {
            get
            {
                return m_minSupport;
            }
        }

        public double MaxSupport
        {
            get
            {
                return m_maxSupport;
            }
        }

        public double MeanSupport
        {
            get
            {
                return m_meanSupport;
            }
        }

        public abstract int GetCountValue(State s, int ai);
        protected abstract void IncrementCountValueBase(State s, int ai);

        public virtual void IncrementCountValue(State s, int ai)
        {
            IncrementCountValueBase(s, ai);
            UpdateStatistics(s);
        }

        protected abstract int MyUnum { get; }
        protected abstract int TeammatesCount { get; }

        protected abstract void SaveBase(TextWriter tw);
        protected abstract void LoadBase(TextReader tr);

        public virtual void Save(TextWriter tw)
        {
            SaveBase(tw);
            tw.WriteLine();
            double[] ar = new double[] { m_maxSupport, m_meanSupport, m_minSupport, m_stateVisits };
            ar.SaveArrayContents(tw);
            tw.WriteLine();
        }

        public virtual void Load(TextReader tr)
        {
            LoadBase(tr);
            double[] ar = new double[4];
            ar.LoadDoubleArrayContents(tr);
            m_maxSupport = ar[0]; 
            m_meanSupport = ar[1];
            m_minSupport = ar[2];
            m_stateVisits = (int) ar[3];
        }

        public abstract void PerformKCyclicNeighborQUpdate(QTableBase qtable);

        public abstract int NumberOfDMUpdates { get; }

        public virtual int GetStateCount(State s)
        {
            int numActions = SoccerAction.GetActionCount(Params.MoveKings, TeammatesCount);
            int sum = 0;
            for (int i = 0; i < numActions; ++i)
            {
                sum += GetCountValue(s, i);
            }

            return sum;
        }

        public virtual double GetActionConfidenceInState(State s, int ai)
        {
            int stateCount = GetStateCount(s);
            int actionCount = GetCountValue(s, ai);
            return ((double)actionCount) / stateCount;
        }

        public virtual void UpdateStatistics(State s)
        {
            m_stateVisits++;
            double support = GetStateCount(s);
            if (support < m_minSupport)
                m_minSupport = support;
            if (support > m_maxSupport)
                m_maxSupport = support;
            m_meanSupport = (m_meanSupport * (m_stateVisits - 1) + support) / m_stateVisits; 
        }
    }
}
