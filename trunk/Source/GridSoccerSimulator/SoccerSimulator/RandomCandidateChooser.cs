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

namespace GridSoccer.Simulator
{
    public class RandomCandidateChooser <T>
    {
        private Dictionary<T, double> m_dicCandidates = new Dictionary<T, double>();
        private double sum = 0.0;
        private Random rnd = null;

        public RandomCandidateChooser(Random rnd)
        {
            this.rnd = rnd;
        }

        public RandomCandidateChooser() : this(new Random())
        {
        }

        public void Clear()
        {
            m_dicCandidates.Clear();
            sum = 0.0;
        }

        public void AddCandidate(T key, double w)
        {
            m_dicCandidates.Add(key, w);
            sum += w;
        }

        public T ChooseRandomly()
        {
            double r = rnd.NextDouble();

            foreach (var pair in m_dicCandidates)
            {
                r -= (pair.Value / sum);
                if (r < 0)
                    return pair.Key;
            }

            return default(T);
        }
    }
}
