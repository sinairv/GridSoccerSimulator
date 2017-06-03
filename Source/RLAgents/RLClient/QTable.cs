using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;
using GridSoccer.RLAgentsCommon;
using System.IO;

namespace RLClient
{
    public class QTable : QTableBase
    {
        private double[, , , , , , , , ,] m_qTable;
        private int m_teammatesCount;
        private int m_myUnum;

        public QTable(int rows, int cols, int teammatesCount, int myUnum)
        {
            m_teammatesCount = teammatesCount;
            m_myUnum = myUnum;
            m_qTable = new double[rows,cols, rows,cols, rows,cols, rows,cols, 4,
                SoccerAction.GetActionCount(Params.MoveKings, m_teammatesCount)];
        }

        protected override double GetQValue(State s, int ai)
        {
            return m_qTable[s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OurPlayersList[0].Position.Row - 1, s.OurPlayersList[0].Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                s.OppPlayersList[1].Position.Row - 1, s.OppPlayersList[1].Position.Col - 1,
                s.BallOwnerIndex, ai];
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            m_qTable[s.Me.Position.Row - 1, s.Me.Position.Col - 1, 
                s.OurPlayersList[0].Position.Row - 1, s.OurPlayersList[0].Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1, 
                s.OppPlayersList[1].Position.Row - 1, s.OppPlayersList[1].Position.Col - 1,
                s.BallOwnerIndex, ai] = newValue;
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

        public override void Save(TextWriter tw)
        {
            m_qTable.SaveArrayContents(tw);
        }

        public override void Load(TextReader tr)
        {
            m_qTable.LoadDoubleArrayContents(tr);
        }

        public override Array QTableArray
        {
            get { return m_qTable; }
        }

    }
}
