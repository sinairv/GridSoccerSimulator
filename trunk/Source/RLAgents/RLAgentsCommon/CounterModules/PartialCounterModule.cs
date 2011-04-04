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

namespace GridSoccer.RLAgentsCommon.CounterModules
{
    public class PartialCounterModule : CounterModuleBase
    {
        private int m_myUnum = -1;
        private int m_teammatesCount;
        private int m_oppsCount;
        private int m_dist;

        private int m_rows;
        private int m_cols;
        private int m_actionsCount;
        private int m_ballOwnerStatesCount;

        private int m_playerStates;

        private int m_dmUpdatesCount = 0;


        private int[,,,,] m_counterTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TripleModule"/> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The cols.</param>
        /// <param name="teammatesCount">The teammates count.</param>
        /// <param name="myUnum">My unum.</param>
        /// <param name="teammateToMonitor">0-based index of the teammate to monitor.</param>
        /// <param name="opponentToMonitor">0-based index of the opponent to monitor.</param>
        public PartialCounterModule(int rows, int cols, int dist, int teammatesCount, int oppsCount, int myUnum)
        {
            m_rows = rows;
            m_cols = cols;
            m_actionsCount = SoccerAction.GetActionCount(Params.MoveKings, teammatesCount);
            m_ballOwnerStatesCount = 5;  // ball owner index (0: me)(1: the-teammate)(2:Opp1)(3: Opp2)(4: Unknown)

            m_myUnum = myUnum;
            m_teammatesCount = teammatesCount;
            m_oppsCount = oppsCount;
            m_dist = dist;

            m_playerStates = (2 * m_dist + 1) * (2 * m_dist + 1) + 1;


            m_counterTable = new int[
                m_playerStates,              // my teammate's position
                m_playerStates,              // one of opponents's position
                m_playerStates,              // the other opponents's position
                m_ballOwnerStatesCount,      // ball owner index (0: me)(1: the-teammate)(2:Opp1)(3: Opp2)(4: Unknown)
                m_actionsCount               // number of actions
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

        // ball owner index (0: me)(1: the-teammate)(2: Opp1)(3: Opp2)(4: Unknown)
        private int GetBallOwnerIndex(State s)
        {
            int index = 0;
            Position ppos = null;
            if (s.AmIBallOwner)
                return 0;
            else if (s.OurPlayersList[0].IsBallOwner)
            {
                index = 1;
                ppos = s.OurPlayersList[0].Position;
            }
            else if (s.OppPlayersList[0].IsBallOwner)
            {
                index = 2;
                ppos = s.OppPlayersList[0].Position;
            }
            else if (s.OppPlayersList[1].IsBallOwner)
            {
                index = 3;
                ppos = s.OppPlayersList[1].Position;
            }
            else
                index = 4;

            if (index != 4)
            {
                if (GetMaxDist(ppos, s.Me.Position) > m_dist)
                    return 4;
                else
                    return index;
            }

            return 4;
        }

        private int GetMaxDist(Position p1, Position p2)
        {
            return Math.Max(Math.Abs(p2.Row - p1.Row), Math.Abs(p2.Col - p1.Col));
        }

        private int GetPlayerLocationIndex(Position myPos, Position pPos)
        {
            int r = pPos.Row - myPos.Row + m_dist;
            int c = pPos.Col - myPos.Col + m_dist;
            if(r < 0 || c < 0 || r >= 2 * m_dist + 1 || c >= 2* m_dist + 1)
                return 0; // i.e. unavailablee

            return r * (2 * m_dist + 1) + c + 1;
        }

        private void GetPositionFromIndex(int i, out int r, out int c)
        {
            r = (i - 1) / (2 * m_dist + 1);
            c = (i - 1) % (2 * m_dist + 1);
        }

        public override int GetCountValue(State s, int ai)
        {
            Position myPos = s.Me.Position;
            return m_counterTable[
                GetPlayerLocationIndex(myPos, s.OurPlayersList[0].Position),
                GetPlayerLocationIndex(myPos, s.OppPlayersList[0].Position),
                GetPlayerLocationIndex(myPos, s.OppPlayersList[1].Position),
                GetBallOwnerIndex(s), ai];
        }

        protected override void IncrementCountValueBase(State s, int ai)
        {
            Position myPos = s.Me.Position;
            m_counterTable[
                GetPlayerLocationIndex(myPos, s.OurPlayersList[0].Position),
                GetPlayerLocationIndex(myPos, s.OppPlayersList[0].Position),
                GetPlayerLocationIndex(myPos, s.OppPlayersList[1].Position),
                GetBallOwnerIndex(s), ai]++;
        }

        protected override void SaveBase(TextWriter tw)
        {
            m_counterTable.SaveArrayContents(tw);
        }

        protected override void LoadBase(TextReader tr)
        {
            m_counterTable.LoadInt32ArrayContents(tr);
        }

        #region Data-Mining Stuff

        public override int NumberOfDMUpdates
        {
            get { return m_dmUpdatesCount; }
        }

        public override void PerformKCyclicNeighborQUpdate(QTableBase qtable)
        {
            m_dmUpdatesCount = 0;

            for (int tmi = 0; tmi < m_playerStates; ++tmi)
                for(int op1i = 0; op1i < m_playerStates; ++op1i)
                    for (int op2i = 0; op2i < m_playerStates; ++op2i)
                        for (int boi = 0; boi < m_ballOwnerStatesCount; ++boi)
                        {
                            double support = GetSupport(tmi, op1i, op2i, boi);

                            // if the current state has a low support, there's a need to update the qValues
                            if (support < Params.DM.MinSupport)
                            {
                                if (Params.DM.Method == Params.DM.MethodTypes.Averaging)
                                {
                                    PerformAveraging(qtable, tmi, op1i, op2i, boi);
                                }
                                else if (Params.DM.Method == Params.DM.MethodTypes.TopQ)
                                {
                                    PerformTopQ(qtable, tmi, op1i, op2i, boi);
                                }
                                else if (Params.DM.Method == Params.DM.MethodTypes.Voting)
                                {
                                    PerformVoting(qtable, tmi, op1i, op2i, boi);
                                }
                            }

                        }

        }

        private void PerformVoting(QTableBase qtable, int tmi, int op1i, int op2i, int boi)
        {
            double[] qValues = new double[m_actionsCount];
            int[] counts = new int[m_actionsCount];

            for (int i = 0; i < qValues.Length; ++i)
                qValues[i] = Double.MinValue;

            // iterate through states in k-cyclic neighbors
            foreach (var neighbor in GetKCyclicNeighborsForPartialModule(Params.DM.K))
            {
                // if they have enough support
                if (GetSupportNeighbor(neighbor, tmi, op1i, op2i, boi) >= Params.DM.MinSupport)
                {
                    int greedyActIndex = -1;
                    double greedyActValue = Double.MinValue;

                    // Find greedy action index and value
                    for (int ai = 0; ai < m_actionsCount; ++ai)
                    {
                        if (GetConfidenceNeighbor(neighbor, tmi, op1i, op2i, boi, ai) >= Params.DM.MinConfidence)
                        {
                            double qValue = GetQValueNeighbor(qtable, neighbor, tmi, op1i, op2i, boi, ai);
                            if (qValue > greedyActValue)
                            {
                                greedyActValue = qValue;
                                greedyActIndex = ai;
                            }
                        }
                    }

                    if (greedyActIndex >= 0)
                    {
                        counts[greedyActIndex]++;
                        if (greedyActValue > qValues[greedyActIndex])
                            qValues[greedyActIndex] = greedyActValue;
                    }
                }
            }

            int maxCountIndex = -1;
            int maxValue = 0;
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] > maxValue)
                {
                    maxCountIndex = i;
                    maxValue = counts[i];
                }
            }

            if (maxCountIndex >= 0 && maxValue > 0)
            {
                qtable.QTableArray.SetValue(qValues[maxCountIndex], tmi, op1i, op2i, boi, maxCountIndex);
                m_dmUpdatesCount++;
            }

        }

        private void PerformTopQ(QTableBase qtable, int tmi, int op1i, int op2i, int boi)
        {
            double[] qValues = new double[m_actionsCount];
            int[] counts = new int[m_actionsCount];

            for (int i = 0; i < qValues.Length; ++i)
                qValues[i] = Double.MinValue;

            // iterate through states in k-cyclic neighbors
            foreach (var neighbor in GetKCyclicNeighborsForPartialModule(Params.DM.K))
            {
                // if they have enough support
                if (GetSupportNeighbor(neighbor, tmi, op1i, op2i, boi) >= Params.DM.MinSupport)
                {
                    // accumulate Q for high-confidence actions
                    for (int ai = 0; ai < m_actionsCount; ++ai)
                    {
                        if (GetConfidenceNeighbor(neighbor, tmi, op1i, op2i, boi, ai) >= Params.DM.MinConfidence)
                        {
                            double qValue = GetQValueNeighbor(qtable, neighbor, tmi, op1i, op2i, boi, ai);
                            if (qValue > qValues[ai])
                                qValues[ai] = qValue;
                            counts[ai]++;
                        }
                    }
                }
            }

            // compute the mean of q-Values, and update in the q-Table
            for (int ai = 0; ai < m_actionsCount; ++ai)
            {
                if (counts[ai] > 0 && qValues[ai] != Double.MinValue)
                {
                    qtable.QTableArray.SetValue(qValues[ai], tmi, op1i, op2i, boi, ai);
                    m_dmUpdatesCount++;
                }
            }

        }

        private void PerformAveraging(QTableBase qtable, int tmi, int op1i, int op2i, int boi)
        {
            double[] qValues = new double[m_actionsCount];
            int[] counts = new int[m_actionsCount];

            // iterate through states in k-cyclic neighbors
            foreach (var neighbor in GetKCyclicNeighborsForPartialModule(Params.DM.K))
            {
                // if they have enough support
                if (GetSupportNeighbor(neighbor, tmi, op1i, op2i, boi) >= Params.DM.MinSupport)
                {
                    // accumulate Q for high-confidence actions
                    for (int ai = 0; ai < m_actionsCount; ++ai)
                    {
                        if (GetConfidenceNeighbor(neighbor, tmi, op1i, op2i, boi, ai) >= Params.DM.MinConfidence)
                        {
                            qValues[ai] += GetQValueNeighbor(qtable, neighbor, tmi, op1i, op2i, boi, ai);
                            counts[ai]++;
                        }
                    }
                }
            }

            // compute the mean of q-Values, and update in the q-Table
            for (int ai = 0; ai < m_actionsCount; ++ai)
            {
                if (counts[ai] > 0)
                {
                    qValues[ai] /= counts[ai];
                    qtable.QTableArray.SetValue(qValues[ai], tmi, op1i, op2i, boi, ai);
                    m_dmUpdatesCount++;
                }
            }
        }

        private int GetMovedLocationIndex(int[] neighbor, int i)
        {
            int r, c;
            GetPositionFromIndex(i, out r, out c);
            r += neighbor[0];
            c += neighbor[1];

            if (r < 0 || c < 0 || r >= 2 * m_dist + 1 || c >= 2 * m_dist + 1)
                return 0;

            return r * (2 * m_dist + 1) + c + 1;
        }

        private double GetSupportNeighbor(int[] neighbor, int tmi, int op1i, int op2i, int boi)
        {
            return GetSupport(
                GetMovedLocationIndex(neighbor, tmi),
                GetMovedLocationIndex(neighbor, op1i),
                GetMovedLocationIndex(neighbor, op2i),
                boi);
        }

        private double GetConfidenceNeighbor(int[] neighbor, int tmi, int op1i, int op2i, int boi, int ai)
        {
            return GetConfidence(
                GetMovedLocationIndex(neighbor, tmi),
                GetMovedLocationIndex(neighbor, op1i),
                GetMovedLocationIndex(neighbor, op2i),
                boi, ai);
        }

        private double GetQValue(QTableBase qtable, int tmi, int op1i, int op2i, int boi, int ai)
        {
            return (double) qtable.QTableArray.GetValue(tmi, op1i, op2i, boi, ai);
        }

        private double GetQValueNeighbor(QTableBase qtable, int[] neighbor, int tmi, int op1i, int op2i, int boi, int ai)
        {
            return (double)qtable.QTableArray.GetValue(
                GetMovedLocationIndex(neighbor, tmi),
                GetMovedLocationIndex(neighbor, op1i),
                GetMovedLocationIndex(neighbor, op2i),
                boi, ai);
        }


        private double GetConfidence(int tmi, int op1i, int op2i, int boi, int ai)
        {
            double support = GetSupport(tmi, op1i, op2i, boi);
            return ((double)m_counterTable[tmi, op1i, op2i, boi, ai]) / support;
        }

        private double GetSupport(int tmi, int op1i, int op2i, int boi)
        {
            double sum = 0.0;
            for (int i = 0; i < m_actionsCount; ++i)
            {
                sum += m_counterTable[tmi, op1i, op2i, boi, i];
            }
            return sum;
        }

        private List<int[]> GetKCyclicNeighborsForPartialModule(int k)
        {
            int r = 0; int c = 0;

            List<int[]> result = new List<int[]>();
            for (int ki = 1; ki <= k; ++ki)
            {
                int newr;
                int newc;

                // upper side
                newr = r - ki;
                for (newc = c - ki; newc <= c + ki; newc++)
                {
                    result.Add(new int[] { newr, newc });
                }

                // lower side
                newr = r + ki;
                for (newc = c - ki; newc <= c + ki; newc++)
                {
                    result.Add(new int[] { newr, newc });
                }

                // left side
                newc = c - ki;
                for (newr = r - ki + 1; newr <= r + ki - 1; newr++)
                {
                    result.Add(new int[] { newr, newc });
                }

                // right side
                newc = c + ki;
                for (newr = r - ki + 1; newr <= r + ki - 1; newr++)
                {
                    result.Add(new int[] { newr, newc });
                }
            }
            return result;
        }

        //public void OldPerformKCyclicNeighborQUpdate(QTableBase qtable)
        //{
        //    // opponent row
        //    for (int op2r = 0; op2r < m_rows; ++op2r)
        //    {
        //        // opponent col
        //        for (int op2c = 0; op2c < m_cols; ++op2c)
        //        {
        //            // opponent row
        //            for (int op1r = 0; op1r < m_rows; ++op1r)
        //            {
        //                // opponent col
        //                for (int op1c = 0; op1c < m_cols; ++op1c)
        //                {
        //                    // tmr for team-mate row
        //                    for (int tmr = 0; tmr < m_rows; ++tmr)
        //                    {
        //                        // tmc for team-mate col
        //                        for (int tmc = 0; tmc < m_cols; ++tmc)
        //                        {
        //                            // boi for ball-owner-index
        //                            for (int boi = 0; boi < m_ballOwnerStatesCount; ++boi)
        //                            {
        //                                for (int r = 0; r < m_rows; ++r)
        //                                {
        //                                    for (int c = 0; c < m_cols; ++c)
        //                                    {
        //                                        double support = GetSupport(r, c, tmr, tmc, op1r, op1c, op2r, op2c, boi);

        //                                        // if the current state has a low support, there's a need to update the qValues
        //                                        if (support < Params.DM.MinSupport)
        //                                        {
        //                                            double[] qValues = new double[m_actionsCount];
        //                                            int[] counts = new int[m_actionsCount];

        //                                            // iterate through states in k-cyclic neighbors
        //                                            foreach (var neighbor in GetKCyclicNeighborsForPartialModule(r, c, Params.DM.K))
        //                                            {
        //                                                // if they have enough support
        //                                                if (GetSupport(neighbor[0], neighbor[1], tmr, tmc, op1r, op1c, op2r, op2c, boi) >= Params.DM.MinSupport)
        //                                                {
        //                                                    // accumulate Q for high-confidence actions
        //                                                    for (int ai = 0; ai < m_actionsCount; ++ai)
        //                                                    {
        //                                                        if (GetActionConfidence(r, c, tmr, tmc, op1r, op1c, op2r, op2c, boi, ai) >= Params.DM.MinConfidence)
        //                                                        {
        //                                                            qValues[ai] += GetQValueFromIndices(qtable, r, c, tmr, tmc, op1r, op1c, op2r, op2c, boi, ai);
        //                                                            counts[ai]++;
        //                                                        }
        //                                                    }

        //                                                    // compute the mean of q-Values, and update in the q-Table
        //                                                    for (int ai = 0; ai < m_actionsCount; ++ai)
        //                                                    {
        //                                                        if (counts[ai] > 0)
        //                                                        {
        //                                                            qValues[ai] /= counts[ai];
        //                                                            SetQValueFromIndices(qtable, qValues[ai], r, c, tmr, tmc, op1r, op1c, op2r, op2c, boi, ai);
        //                                                        }
        //                                                    }
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private double OldGetActionConfidence(int r, int c, int tmr, int tmc, int op1r, int op1c, int op2r, int op2c, int boi, int ai)
        //{
        //    double support = GetSupport(r, c, tmr, tmc, op1r, op1c, op2r, op2c, boi);
        //    return ((double)GetCountValueFromIndices(r, c, tmr, tmc, op1r, op1c, op2r, op2c, boi, ai)) / support;
        //}

        //private double OldGetSupport(int r, int c, int tmr, int tmc, int op1r, int op1c, int op2r, int op2c, int boi)
        //{
        //    double sum = 0.0;

        //    for (int i = 0; i < m_actionsCount; ++i)
        //    {
        //        sum += GetCountValueFromIndices(r, c, tmr, tmc, op1r, op1c, op2r, op2c, boi, i);
        //    }

        //    return sum;
        //}

        private int GetCountValueFromIndices(int r, int c, int tmr, int tmc, int op1r, int op1c, int op2r, int op2c, int boi, int ai)
        {
            Position myPos = new Position(r, c);
            Position tmPos = new Position(tmr, tmc);
            Position op1Pos = new Position(op1r, op1c);
            Position op2Pos = new Position(op2r, op2c);

            return m_counterTable[
                GetPlayerLocationIndex(myPos, tmPos),
                GetPlayerLocationIndex(myPos, op1Pos),
                GetPlayerLocationIndex(myPos, op2Pos),
                boi, ai];
        }

        private double GetQValueFromIndices(QTableBase qTable, int r, int c, int tmr, int tmc, int op1r, int op1c, int op2r, int op2c, int boi, int ai)
        {
            Position myPos = new Position(r, c);
            Position tmPos = new Position(tmr, tmc);
            Position op1Pos = new Position(op1r, op1c);
            Position op2Pos = new Position(op2r, op2c);

            return (double)qTable.QTableArray.GetValue(
                GetPlayerLocationIndex(myPos, tmPos),
                GetPlayerLocationIndex(myPos, op1Pos),
                GetPlayerLocationIndex(myPos, op2Pos),
                boi, ai);
        }

        private void SetQValueFromIndices(QTableBase qTable, double value, int r, int c, int tmr, int tmc, int op1r, int op1c, int op2r, int op2c, int boi, int ai)
        {
            Position myPos = new Position(r, c);
            Position tmPos = new Position(tmr, tmc);
            Position op1Pos = new Position(op1r, op1c);
            Position op2Pos = new Position(op2r, op2c);

            qTable.QTableArray.SetValue(value,
                GetPlayerLocationIndex(myPos, tmPos),
                GetPlayerLocationIndex(myPos, op1Pos),
                GetPlayerLocationIndex(myPos, op2Pos),
                boi, ai);
        }

        #endregion

    }
}
