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
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;
using System.IO;

namespace GridSoccer.RLAgentsCommon.Modules
{
    public class TripleModule : QTableBase
    {
        private int m_myUnum = -1;
        private int m_oppToMonitor = -1;
        private int m_teammateToMonitor = -1;
        private int m_teammatesCount;

        private double[, , , , , , ,] m_QTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TripleModule"/> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The cols.</param>
        /// <param name="teammatesCount">The teammates count.</param>
        /// <param name="myUnum">My unum.</param>
        /// <param name="teammateToMonitor">0-based index of the teammate to monitor.</param>
        /// <param name="opponentToMonitor">0-based index of the opponent to monitor.</param>
        public TripleModule(int rows, int cols, int teammatesCount, int myUnum, int teammateToMonitor, int opponentToMonitor)
        {
            m_myUnum = myUnum;
            m_oppToMonitor = opponentToMonitor;
            m_teammateToMonitor = teammateToMonitor;
            m_teammatesCount = teammatesCount;

            m_QTable = new double[
                rows, cols,              // my position 
                rows, cols,              // my teammate's position
                rows, cols,              // one of opponents's position
                5,                       // ball owner index (0: me)(1: the-teammate)(2:We)(3: the opponent)(4: they)
                SoccerAction.GetActionCount(Params.MoveKings, teammatesCount)       // number of actions
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

        // ball owner index (0: me)(1: the-teammate)(2:We)(3: the opponent)(4: they)
        protected int GetBallOwnerIndex(State s)
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

        protected override double GetQValue(State s, int ai)
        {
            return m_QTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OurPlayersList[0].Position.Row - 1, s.OurPlayersList[0].Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                GetBallOwnerIndex(s), ai];
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            m_QTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OurPlayersList[0].Position.Row - 1, s.OurPlayersList[0].Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                GetBallOwnerIndex(s), ai] = newValue;
        }

        protected override State DicomposeState(State s)
        {
            return s.GetDecomposedState(new int[] { m_teammateToMonitor }, new int[] { m_oppToMonitor });
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
