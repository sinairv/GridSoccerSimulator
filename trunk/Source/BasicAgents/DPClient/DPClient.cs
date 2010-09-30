using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.ClientBasic;
using GridSoccer.Common;
using System.Diagnostics;

namespace GridSoccer.DPClient
{
    public class DPClient : ClientBase
    {
        public DPClient(string serverAddr, int serverPort, string teamname, int unum)
            : base(serverAddr, serverPort, teamname, unum)
        {
            if (unum == 1)
                SetHomePosition(2, 2);
            else if (unum == 2)
                SetHomePosition(4, 2);
            else if (unum == 3)
                SetHomePosition(3, 3);

            Console.WriteLine("Performing DP Calculations. Please wait...");
            ComputeDP();
            Console.WriteLine("Finished. You may now start the game.");

            ValueTableVisualizer frm = new ValueTableVisualizer(m_valueTable, this);
            frm.ShowDialog();
        }

        private IValueTable m_valueTable = null;
        private int m_numActions = SoccerAction.GetActionCount(false, 1);
        private DPMethods m_dpMethod = DPMethods.PolicyIteration;
        private ValueTableInitModes m_initMode = ValueTableInitModes.Constant;
        private double m_initValue = 0.0;
        
        private const double Theta = 0.001;
        private const double Gamma = 0.9;


        private void ComputeDP()
        {
            // Check for the selected method, and call the corresponding method
            switch (m_dpMethod)
            {
                case DPMethods.ValueIteration:
                    ValueIteration();
                    break;
                case DPMethods.QValueIteration:
                    QValueIteration();
                    break;
                case DPMethods.PolicyIteration:
                    PolicyIteration();
                    break;
                default:
                    break;
            }
        }

        private Policy m_piPolicy = null;
        private void PolicyIteration()
        {
            StateValueTable valueTable = new StateValueTable(m_initMode, m_initValue,
                this.EnvRows, this.EnvCols, // my pos
                this.EnvRows, this.EnvCols, // opp pos
                2); // ball ownership status

            m_valueTable = valueTable;

            Policy policy = new Policy(m_initMode, m_initMode == ValueTableInitModes.Constant ? 0 : m_numActions,
                this.EnvRows, this.EnvCols, // my pos
                this.EnvRows, this.EnvCols, // opp pos
                2); // ball ownership status

            m_piPolicy = policy;

            long stateCounts = m_valueTable.NumStates;


            while (true)
            {
                // Policy Evaluation

                double delta = 0.0;
                do
                {
                    delta = 0.0;

                    // foreach state
                    for (int s = 0; s < stateCounts; s++)
                    {
                        double v = valueTable.GetValueLinear(s);
                        double newV = EstimateNewValueUsingPolicy(s, policy);
                        valueTable.SetValueLinear(newV, s);
                        delta = Math.Max(delta, Math.Abs(v - newV));
                    }
                } while (delta > Theta);

                // Policy Improvement
                bool policyStable = true;

                for (int s = 0; s < stateCounts; s++)
                {
                    int b = policy.GetValueLinear(s);
                    int bestAct;
                    EstimateNewValue((int)s, out bestAct);

                    policy.SetValueLinear(bestAct, s);
                    if (b != bestAct)
                    {
                        policyStable = false;
                    }
                }

                if (policyStable)
                    break;
            }

        }

        private void ValueIteration()
        {
            StateValueTable valueTable = new StateValueTable(m_initMode, m_initValue,
                this.EnvRows, this.EnvCols, // my pos
                this.EnvRows, this.EnvCols, // opp pos
                2); // ball ownership status

            m_valueTable = valueTable;

            long stateCounts = m_valueTable.NumStates;

            double delta = 0.0;
            do
            {
                delta = 0.0;

                // foreach state
                for (int s = 0; s < stateCounts; s++)
                {
                    double v = valueTable.GetValueLinear(s);
                    double newV = EstimateNewValue(s);
                    valueTable.SetValueLinear(newV, s);
                    delta = Math.Max(delta, Math.Abs(v - newV));
                }
            } while (delta > Theta);

        }

        private double EstimateNewValue(int s)
        {
            int a;
            return EstimateNewValue(s, out a);
        }

        private double EstimateNewValue(int s, out int maxA)
        {
            StateValueTable valueTable = m_valueTable as StateValueTable;
            if (valueTable == null)
            {
                throw new Exception("A ValueTable needed!");
            }

            List<int> nextStates;
            List<double> nextStateProbs;
            List<double> nextStateRew;

            double maxV = Double.MinValue;
            maxA = -1;

            for (int a = 0; a < m_numActions; a++)
            {
                GetPossibleNextStates(s, a, out nextStates, out nextStateProbs, out nextStateRew);

                double nextV = 0.0;
                for (int i = 0; i < nextStates.Count; i++)
                {
                    nextV += nextStateProbs[i] * (nextStateRew[i] + (Gamma * valueTable.GetValueLinear(nextStates[i])));
                }

                if (nextV > maxV)
                {
                    maxV = nextV;
                    maxA = a;
                }
            }

            return maxV;
        }

        private double EstimateNewValueUsingPolicy(int s, Policy policy)
        {
            StateValueTable valueTable = m_valueTable as StateValueTable;
            if (valueTable == null)
            {
                throw new Exception("A ValueTable needed!");
            }

            List<int> nextStates;
            List<double> nextStateProbs;
            List<double> nextStateRew;

            int a = policy.GetValueLinear(s);

            GetPossibleNextStates(s, a, out nextStates, out nextStateProbs, out nextStateRew);

            double nextV = 0.0;
            for (int i = 0; i < nextStates.Count; i++)
            {
                nextV += nextStateProbs[i] * (nextStateRew[i] + (Gamma * valueTable.GetValueLinear(nextStates[i])));
            }

            return nextV;
        }

        private void QValueIteration()
        {
            StateActionValueTable qValueTable = new StateActionValueTable(m_initMode, m_initValue,
                5, // actions
                this.EnvRows, this.EnvCols, // my pos
                this.EnvRows, this.EnvCols, // opp pos
                2); // ball ownership status

            m_valueTable = qValueTable;

            long stateCounts = qValueTable.NumStates;
            int actionCounts = qValueTable.NumActions;

            double delta = 0.0;
            do
            {
                delta = 0.0;

                // foreach state
                for (int s = 0; s < stateCounts; s++)
                {
                    // foreach action
                    for (int a = 0; a < actionCounts; a++)
                    {
                        double q = qValueTable.GetValueLinear(a, s);
                        double newQ = EstimateNewQValue(s, a);
                        qValueTable.SetValueLinear(newQ, a, s);
                        delta = Math.Max(delta, Math.Abs(q - newQ));
                    }
                }
            } while (delta > Theta);

        }

        private double EstimateNewQValue(int s, int a)
        {
            List<int> nextStates;
            List<double> nextStateProbs;
            List<double> nextStateRew;

            GetPossibleNextStates(s, a, out nextStates, out nextStateProbs, out nextStateRew);

            double nextQ = 0.0;
            for (int i = 0; i < nextStates.Count; i++)
            {
                nextQ += nextStateProbs[i] * (nextStateRew[i] + (Gamma * GetMaxQForState(nextStates[i]) ) );
            }

            return nextQ;
        }

        private void GetPossibleNextStates(int s, int a, out List<int> nextStates, 
            out List<double> nextStateProbs, out List<double> nextStateRew)
        {
            nextStates = new List<int>();
            
            var rawNextStates = new List<int>();
            var rawNextStateProbs = new List<double>();
            var rawNextStateRew = new List<double>();

            State curState = new State(m_valueTable.GetDimentionalState(s));
            ActionTypes curAction = SoccerAction.GetActionTypeFromIndex(a, false);

            // check for invalid states and return empty data-structures
            if (curState.MyRow == curState.OppRow && curState.MyCol == curState.OppCol)
            {
                nextStates = new List<int>();
                nextStateProbs = new List<double>();
                nextStateRew = new List<double>();
                return;
            }

            // iterate through all possible actions of the opponent
            for (int oppActi = 0; oppActi < m_numActions; oppActi++)
            {
                ActionTypes oppAction = SoccerAction.GetActionTypeFromIndex(oppActi, false);

                List<int> evalNextStates;
                List<double> evalNextStateProbs;
                List<double> evalNextStateRews;

                EvaluateStateAndActions(curState, curAction, oppAction, 
                    out evalNextStates, out evalNextStateProbs, out evalNextStateRews);

                rawNextStates.AddRange(evalNextStates);
                rawNextStateProbs.AddRange(evalNextStateProbs);
                rawNextStateRew.AddRange(evalNextStateRews);
            }

            // now refine the raw results
            
            // calculate the sum of probs (weights) to normalize them further
            double sumProbs = 0.0;
            foreach (double p in rawNextStateProbs)
                sumProbs += p;

            Dictionary<int, double> stateProbs = new Dictionary<int, double>();
            Dictionary<int, double> stateRews = new Dictionary<int, double>();
            Dictionary<int, double> stateSumRews = new Dictionary<int, double>();

            for (int si = 0; si < rawNextStates.Count; si++)
            {
                double curProb = rawNextStateProbs[si] / sumProbs;
                int curSLinInd = rawNextStates[si];

                if (stateProbs.ContainsKey(curSLinInd))
                {
                    stateProbs[curSLinInd] += curProb;
                    stateSumRews[curSLinInd] += curProb * rawNextStateRew[si];
                    stateRews[curSLinInd] = stateSumRews[curSLinInd] / stateProbs[curSLinInd];
                }
                else
                {
                    stateProbs.Add(curSLinInd, curProb);
                    stateSumRews.Add(curSLinInd, curProb * rawNextStateRew[si]);
                    stateRews.Add(curSLinInd, stateSumRews[curSLinInd] / stateProbs[curSLinInd]);
                }
            }

            // now add up the results
            nextStates = stateProbs.Keys.ToList();

            nextStateProbs = new List<double>();
            nextStateRew = new List<double>();

            for (int i = 0; i < nextStates.Count; i++)
            {
                nextStateProbs.Add(stateProbs[nextStates[i]]);
                nextStateRew.Add(stateRews[nextStates[i]]);
            }
        }

        private bool NormalizePositions(ref int row, ref int col)
        {
            bool isChanged = false;

            if (row <= 0)
            {
                row = 1;
                isChanged = true;
            }

            if (row > EnvRows)
            {
                row = EnvRows;
                isChanged = true;
            }

            if (col <= 0)
            {
                col = 1;
                isChanged = true;
            }

            if (col > EnvCols)
            {
                col = EnvCols;
                isChanged = true;
            }

            return isChanged;
        }

        private void EvaluateStateAndActions(State state, ActionTypes myAct, ActionTypes oppAct,
            out List<int> nextStates, out List<double> nextStateProbs, out List<double> nextStateRew)
        {
            nextStates = new List<int>();
            nextStateProbs = new List<double>();
            nextStateRew = new List<double>();


            int newOppRow, newOppCol;
            GetNextPosition(state.OppRow, state.OppCol, oppAct, true, out newOppRow, out newOppCol);
            bool isNewOppPosOut = NormalizePositions(ref newOppRow, ref newOppCol);
            bool didOppMove = !((newOppRow == state.OppRow) && (newOppCol == state.OppCol));

            int newMyRow, newMyCol;
            GetNextPosition(state.MyRow, state.MyCol, myAct, false, out newMyRow, out newMyCol);
            bool isNewMyPosOut = NormalizePositions(ref newMyRow, ref newMyCol);
            bool didIMove = !((newMyRow == state.MyRow) && (newMyCol == state.MyCol));


            // detect goal scores
            // if I had scored a goal
            if (state.AmIBallOwner && myAct == ActionTypes.MoveEast && 
                    this.GoalUpperRow <= state.MyRow && state.MyRow <= this.GoalLowerRow && state.MyCol == this.EnvCols)
            {
                nextStates.Add(-1);
                nextStateProbs.Add(1.0);
                nextStateRew.Add(Rewards.ScoreGoal);
            }
            // I scored an own-goal
            else if (state.AmIBallOwner && myAct == ActionTypes.MoveWest &&
                    this.GoalUpperRow <= state.MyRow && state.MyRow <= this.GoalLowerRow && state.MyCol == 1)
            {
                nextStates.Add(-1);
                nextStateProbs.Add(1.0);
                nextStateRew.Add(Rewards.ReceiveGoal);
            }
            // else if Opp had scored a goal
            else if (!state.AmIBallOwner && oppAct == ActionTypes.MoveEast &&
                this.GoalUpperRow <= state.OppRow && state.OppRow <= this.GoalLowerRow && state.OppCol == 1)
            {
                nextStates.Add(-1);
                nextStateProbs.Add(1.0);
                nextStateRew.Add(Rewards.ReceiveGoal);
            }
            // else if Opp had scored an own-goal
            else if (!state.AmIBallOwner && oppAct == ActionTypes.MoveWest &&
                this.GoalUpperRow <= state.OppRow && state.OppRow <= this.GoalLowerRow && state.OppCol == this.EnvCols)
            {
                nextStates.Add(-1);
                nextStateProbs.Add(1.0);
                nextStateRew.Add(Rewards.ScoreGoal);
            }
            //else if none has scored a goal
            else
            {
                bool conflictsFound = ((newMyCol == newOppCol) && (newMyRow == newOppRow));

                if (!conflictsFound)
                {
                    // with prob 1.0 they will move to their desired position

                    State newState = new State(state) { MyCol = newMyCol, MyRow = newMyRow, OppRow = newMyRow, OppCol = newOppCol };

                    nextStates.Add(newState.GetLinearIndex(m_valueTable));
                    nextStateProbs.Add(1.0);
                    nextStateRew.Add(isNewMyPosOut ? Rewards.InvalidMove : Rewards.Movement);
                }
                // if(there's conflicts)
                else
                {
                    if (didIMove && didOppMove)
                    {
                        // with prob 0.5 each player moves to the given position
                        // with prob 0.8 bowner will not change with 0.2 will change

                        State stateIMove = new State(state) { MyRow = newMyRow, MyCol = newMyCol };
                        State stateOppMove = new State(state) { OppRow = newOppRow, OppCol = newOppCol };

                        nextStates.Add(stateIMove.GetLinearIndex(m_valueTable));
                        nextStateProbs.Add(0.4);
                        nextStateRew.Add(Rewards.Movement);

                        nextStates.Add(stateOppMove.GetLinearIndex(m_valueTable));
                        nextStateProbs.Add(0.4);
                        nextStateRew.Add(Rewards.Movement);

                        // now change the ball owners
                        stateIMove.AmIBallOwner = !stateIMove.AmIBallOwner;
                        stateOppMove.AmIBallOwner = !stateOppMove.AmIBallOwner;

                        nextStates.Add(stateIMove.GetLinearIndex(m_valueTable));
                        nextStateProbs.Add(0.1);
                        nextStateRew.Add(stateIMove.AmIBallOwner ? Rewards.CatchBall : Rewards.LooseBall);

                        nextStates.Add(stateOppMove.GetLinearIndex(m_valueTable));
                        nextStateProbs.Add(0.1);
                        nextStateRew.Add(stateOppMove.AmIBallOwner ? Rewards.CatchBall : Rewards.LooseBall);
                    }
                    else if (didIMove && !didOppMove)
                    {
                        // with prop 1 opp should keep the pos && I should go back to my prev pos
                        // if(Opp was bowner) with prob 0.8 will remain bowner and with prob 0.2 will loose the ball
                        // otherwise opp will own the ball with prob 1.0

                        State s = new State(state); // { OppRow = newOppRow, OppCol = newOppCol };

                        if (s.AmIBallOwner)
                        {
                            s.AmIBallOwner = !s.AmIBallOwner;

                            nextStates.Add(s.GetLinearIndex(m_valueTable));
                            nextStateProbs.Add(1.0);
                            nextStateRew.Add(Rewards.LooseBall);
                        }
                        else // if(opp was ball owner)
                        {
                            nextStates.Add(s.GetLinearIndex(m_valueTable));
                            nextStateProbs.Add(0.8);
                            nextStateRew.Add(Rewards.InvalidMove);

                            s.AmIBallOwner = !s.AmIBallOwner;

                            nextStates.Add(s.GetLinearIndex(m_valueTable));
                            nextStateProbs.Add(0.2);
                            nextStateRew.Add(Rewards.CatchBall);
                        }
                    }
                    else if (!didIMove && didOppMove)
                    {
                        // with prop 1 I should keep the pos && opp should go back to his prev pos
                        // if(I was bowner) with prob 0.8 will remain bowner and with prob 0.2 will loose the ball
                        // otherwise I will own the ball with prob 1.0

                        State s = new State(state); // { MyRow = newMyRow, MyCol = newMyCol };

                        if (!s.AmIBallOwner)
                        {
                            s.AmIBallOwner = !s.AmIBallOwner;

                            nextStates.Add(s.GetLinearIndex(m_valueTable));
                            nextStateProbs.Add(1.0);
                            nextStateRew.Add(Rewards.CatchBall);
                        }
                        else // if(I was ball owner)
                        {
                            nextStates.Add(s.GetLinearIndex(m_valueTable));
                            nextStateProbs.Add(0.8);
                            nextStateRew.Add(isNewMyPosOut ? Rewards.InvalidMove : Rewards.Movement);

                            s.AmIBallOwner = !s.AmIBallOwner;

                            nextStates.Add(s.GetLinearIndex(m_valueTable));
                            nextStateProbs.Add(0.2);
                            nextStateRew.Add(Rewards.LooseBall);
                        }
                    }
                    else
                    {
                        // this must not cause a conflict
                        Debug.Assert(false);
                    }
                }
            } // end of else (none scored a goal)
        }

        private void GetNextPosition(int row, int col, ActionTypes act, bool isOpp, out int newRow, out int newCol)
        {
            newRow = row;
            newCol = col;

            if (isOpp)
            {
                switch (act)
                {
                    case ActionTypes.MoveEast:
                        newCol--;
                        break;
                    case ActionTypes.MoveSouth:
                        newRow--;
                        break;
                    case ActionTypes.MoveWest:
                        newCol++;
                        break;
                    case ActionTypes.MoveNorth:
                        newRow++;
                        break;
                }
            }
            else
            {
                switch (act)
                {
                    case ActionTypes.MoveEast:
                        newCol++;
                        break;
                    case ActionTypes.MoveSouth:
                        newRow++;
                        break;
                    case ActionTypes.MoveWest:
                        newCol--;
                        break;
                    case ActionTypes.MoveNorth:
                        newRow--;
                        break;
                }
            }
        }

        //private static Random rnd = new Random();
        protected override SoccerAction Think()
        {
            int oppIndex = this.GetAvailableOpponentsIndeces().ToArray()[0];
            int[] state = new int[] { 
                this.PlayerPositions[m_myIndex].Row - 1, this.PlayerPositions[m_myIndex].Col - 1,
                this.PlayerPositions[oppIndex].Row - 1, this.PlayerPositions[oppIndex].Col - 1,
                this.AmIBallOwner() ? 0 : 1};

            long linStateInd = Utils.GetLinearIndex(state, m_valueTable.StateDimensions,
                m_valueTable.NumStates);

            int maxA;

            if (m_dpMethod == DPMethods.QValueIteration)
            {
                GetMaxQForState(linStateInd, out maxA);
                return SoccerAction.GetActionFromIndex(maxA, false, this.MyUnum);
            }
            else if (m_dpMethod == DPMethods.ValueIteration)
            {
                EstimateNewValue((int)linStateInd, out maxA);
                return SoccerAction.GetActionFromIndex(maxA, false, this.MyUnum);
            }
            else if (m_dpMethod == DPMethods.PolicyIteration)
            {
                maxA = m_piPolicy.GetValueLinear((int)linStateInd);
                return SoccerAction.GetActionFromIndex(maxA, false, this.MyUnum);
            }
            else 
            {
                throw new Exception("Unknown DP method");
            }
        }

        private double GetMaxQForState(long linStateInd)
        {
            if (linStateInd < 0)
                return 0.0;

            int maxA;
            return GetMaxQForState(linStateInd, out maxA);
        }

        private double GetMaxQForState(long linStateInd, out int maxA)
        {
            StateActionValueTable qTable = m_valueTable as StateActionValueTable;
            if (qTable == null)
                throw new Exception("value table is not a Q-Table");

            maxA = -1;
            double maxQ = Double.MinValue;

            for (int a = 0; a < m_numActions; a++)
            {
                double curQ = qTable.GetValueLinear(a, (int)linStateInd);
                if (curQ > maxQ)
                {
                    maxQ = curQ;
                    maxA = a;
                }
            }

            return maxQ;
        }
    }

}
