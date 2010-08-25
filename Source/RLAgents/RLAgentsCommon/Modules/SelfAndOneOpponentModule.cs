using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;
using GridSoccer.RLAgentsCommon;
using System.IO;

namespace GridSoccer.RLAgentsCommon.Modules
{
    public class SelfAndOneOpponentModule : QTableBase
    {
        private int m_myUnum = -1;
        private int m_oppToMonitor = -1;
        private int m_teammatesCount;

        private double[, , , , ,] m_QTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfAndOneOpponentModule"/> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The cols.</param>
        /// <param name="teammatesCount">The teammates count.</param>
        /// <param name="myUnum">My unum.</param>
        /// <param name="opponentToMonitor">0-based index of the opponent to monitor.</param>
        public SelfAndOneOpponentModule(int rows, int cols, int teammatesCount, int myUnum, int opponentToMonitor)
        {
            m_myUnum = myUnum;
            m_oppToMonitor = opponentToMonitor;
            m_teammatesCount = teammatesCount;

            m_QTable = new double[
                rows, cols,              // my position 
                rows, cols,              // one of opponents's position
                4,                       // ball owner player's index (0: Me) (1: We)(2: The Opponent) (3: They)
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

        //(0: Me) (1: We)(2: The Opponent) (3: They)
        private int GetBallOwnerIndex(State s)
        {
            if (s.AmIBallOwner)
                return 0;
            else if (s.AreWeBallOwner)
                return 1;
            else
            {
                if (s.OppPlayersList[0].IsBallOwner)
                    return 2;
                else
                    return 3;
            }
        }

        protected override double GetQValue(State s, int ai)
        {
            return m_QTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                GetBallOwnerIndex(s), ai];
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            m_QTable[
                s.Me.Position.Row - 1, s.Me.Position.Col - 1,
                s.OppPlayersList[0].Position.Row - 1, s.OppPlayersList[0].Position.Col - 1,
                GetBallOwnerIndex(s), ai] = newValue;
        }

        protected override State DicomposeState(State s)
        {
            return s.GetDecomposedState(new int[0], new int[] { m_oppToMonitor });
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
