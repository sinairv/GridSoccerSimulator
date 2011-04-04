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
    // NOTE: this class is not working
    public class PartialSelfAndOneOpponentModule : QTableBase
    {
        private int m_myUnum = -1;
        private int m_teammatesCount;
        private int m_oppsCount;
        private int m_dist;

        private double[,,,,] m_QTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TripleModule"/> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The cols.</param>
        /// <param name="teammatesCount">The teammates count.</param>
        /// <param name="myUnum">My unum.</param>
        /// <param name="teammateToMonitor">0-based index of the teammate to monitor.</param>
        /// <param name="opponentToMonitor">0-based index of the opponent to monitor.</param>
        public PartialSelfAndOneOpponentModule(int rows, int cols, int dist, int teammatesCount, int oppsCount, int myUnum)
        {
            m_myUnum = myUnum;
            m_teammatesCount = teammatesCount;
            m_oppsCount = oppsCount;
            m_dist = dist;

            int playerStates = (2 * m_dist + 1) * (2 * m_dist + 1) + 1;


            m_QTable = new double[
                playerStates,              // my teammate's position
                playerStates,              // one of opponents's position
                playerStates,              // the other opponents's position
                5,                         // ball owner index (0: me)(1: the-teammate)(2:Opp1)(3: Opp2)(4: Unknown)
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

        protected override double GetQValue(State s, int ai)
        {
            Position myPos = s.Me.Position;
            return m_QTable[
                GetPlayerLocationIndex(myPos, s.OurPlayersList[0].Position),
                GetPlayerLocationIndex(myPos, s.OppPlayersList[0].Position),
                GetPlayerLocationIndex(myPos, s.OppPlayersList[1].Position),
                GetBallOwnerIndex(s), ai];
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            Position myPos = s.Me.Position;
            m_QTable[
                GetPlayerLocationIndex(myPos, s.OurPlayersList[0].Position),
                GetPlayerLocationIndex(myPos, s.OppPlayersList[0].Position),
                GetPlayerLocationIndex(myPos, s.OppPlayersList[1].Position),
                GetBallOwnerIndex(s), ai] = newValue;
        }

        //protected override State DicomposeState(State s)
        //{
        //    return s;
        //}

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
 