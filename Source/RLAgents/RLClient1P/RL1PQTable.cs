using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;
using System.IO;

namespace RLClient1P
{
    public class RL1PQTable : QTableBase
    {
        private double[, , , ,,] m_qTable;
        int m_myUnum;

        public RL1PQTable(int rows, int cols, int unum)
        {
            m_qTable = new double[rows,cols,rows,cols,2,
                SoccerAction.GetActionCount(Params.MoveKings, TeammatesCount)];
            m_myUnum = unum;
        }

        protected override double GetQValue(State s, int ai)
        {
            return m_qTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                s.BallOwnerIndex, ai];
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            m_qTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                s.BallOwnerIndex, ai] = newValue;
        }

        protected override int MyUnum
        {
            get { return m_myUnum; }
        }

        protected override int TeammatesCount
        {
            get { return 1; }
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
