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
using GridSoccer.Common;
using System.IO;

namespace GridSoccer.RLAgentsCommon.Modules
{
    public class SelfOnlyModule : QTableBase
    {
        private int m_myUnum = -1;
        private int m_teammatesCount;

        private double[,,,] m_QTable;

        public SelfOnlyModule(int rows, int cols, int teammatesCount, int myUnum)
        {
            m_myUnum = myUnum;
            m_teammatesCount = teammatesCount;

            m_QTable = new double[
                rows, cols,              // my position 
                3,                       // ball owner status (0: Me)(1: We)(2: Opp) own the ball
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

        private int GetBallOwnerIndex(State s)
        {
            if(s.AmIBallOwner)
                return 0;
            else if(s.AreWeBallOwner)
                return 1;
            else
                return 2;
        }

        protected override double GetQValue(State s, int ai)
        {
            return m_QTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                GetBallOwnerIndex(s) , ai];
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            m_QTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                GetBallOwnerIndex(s), ai] = newValue;
        }

        protected override State DicomposeState(State s)
        {
            return s.GetDecomposedState(new int[0], new int[0]);
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
