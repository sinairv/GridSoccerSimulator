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
using GridSoccer.Simulator.Properties;
using GridSoccer.Common;

namespace GridSoccer.Simulator
{
    /// <summary>
    /// The Grid-Soccer Simulator class
    /// </summary>
    public class SoccerSimulator
    {
        #region Fields

        /// <summary>
        /// Gets or sets the current cycle (i.e., time step) of the game
        /// </summary>
        /// <value>the current cycle (i.e., time step) of the game.</value>
        public long Cycle { get; private set; }

        /// <summary>
        /// Gets or sets the score of the left team.
        /// </summary>
        /// <value>The score of the left team.</value>
        public int LeftScore { get; private set; }

        /// <summary>
        /// Gets or sets the score of the right team.
        /// </summary>
        /// <value>The score of the right team.</value>
        public int RightScore { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game has been started.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the game has been started; otherwise, <c>false</c>.
        /// </value>
        public bool IsGameStarted { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game has been stopped
        /// </summary>
        public bool IsGameStopped { get; private set; }

        /// <summary>
        /// Gets or sets the name of the left team.
        /// </summary>
        /// <value>The name of the left team.</value>
        public string LeftTeamName { get; private set; }

        /// <summary>
        /// Gets or sets the name of the right team.
        /// </summary>
        /// <value>The name of the right team.</value>
        public string RightTeamName { get; private set; }

        /// <summary>
        /// Gets or sets the information related to all availbale players in the field.
        /// </summary>
        public PlayerInfo[] Players { get; private set; }

        /// <summary>
        /// Gets or sets the position of the ball.
        /// </summary>
        /// <value>The position of the ball.</value>
        public Position BallPosition { get; private set; }

        /// <summary>
        /// Gets or sets the number of players of the left team.
        /// </summary>
        /// <value>The number of players of the left team.</value>
        public int LeftPlayersCount { get; private set; }

        /// <summary>
        /// Gets or sets the number of players of the right team.
        /// </summary>
        /// <value>The number of players of the right team.</value>
        public int RightPlayersCount { get; private set; }

        /// <summary>
        /// An instance of random generator
        /// </summary>
        private readonly Random m_randomGenerator;

        /// <summary>
        /// The unum of the left player which will hold the ball upon game start
        /// </summary>
        private int m_defaultLeftPlayer = -1;

        /// <summary>
        /// The unum of the right player which will hold the ball upon game start
        /// </summary>
        private int m_defaultRightPlayer = -1;

        /// <summary>
        /// The distance into which the players can see objects, if set to zero, 
        /// the players can see all the field
        /// </summary>
        private readonly int m_visibleDistance;

        /// <summary>
        /// The distance into which the players can pass, if set to zero,
        /// the players can pass to any other teammate in any position of the field.
        /// </summary>
        private readonly int m_passableDistance;

        private bool m_isUpdating = false;
        private PendingActionsInfo[] m_pendingActions = null;
        private bool m_isGoalScored = false;
        private Sides m_sideReceivingGoal = Sides.Left;
        private int m_ballOwnerIndex = -1;
        private RandomCandidateChooser<int> m_rndCandidChooser = null;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SoccerSimulator"/> class.
        /// </summary>
        /// <param name="randomSeed">The random seed.</param>
        public SoccerSimulator(int randomSeed)
        {
            Players = new PlayerInfo[Settings.Default.MaxPlayers * 2];
            BallPosition = new Position(0, 0);
            Cycle = 0;
            LeftScore = 0;
            RightScore = 0;
            LeftPlayersCount = 0;
            RightPlayersCount = 0;
            IsGameStarted = false;

            m_visibleDistance = Settings.Default.VisibleRadius;
            if (m_visibleDistance <= 0) m_visibleDistance = Math.Max(Settings.Default.NumRows, Settings.Default.NumCols);

            m_passableDistance = Settings.Default.PassableDistance;
            if (m_passableDistance <= 0) m_passableDistance = Math.Max(Settings.Default.NumRows, Settings.Default.NumCols);

            m_randomGenerator = new Random(randomSeed);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the state of the game changes (including every cycle completion).
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        private void RaiseChangedEvent()
        {
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        /// <summary>
        /// Occurs when the score of the eaither teams has changed.
        /// </summary>
        public event EventHandler ScoreChanged;

        /// <summary>
        /// Raises the score-changed event.
        /// </summary>
        private void RaiseScoreChangedEvent()
        {
            if (ScoreChanged != null)
                ScoreChanged(this, new EventArgs());
        }

        #endregion

        /// <summary>
        /// Adds a player to the game, with the specified uniform number, belonging
        /// to the team with the specified name.
        /// </summary>
        /// <param name="teamName">Name of the team.</param>
        /// <param name="unum">The uniform number.</param>
        /// <returns></returns>
        public int AddPlayer(string teamName, int unum)
        {
            if (IsGameStarted)
                return -1;

            if(unum > Settings.Default.MaxPlayers)
                return -1;

            Sides side;
            if (LeftTeamName == null || teamName == LeftTeamName)
                side = Sides.Left;
            else if (RightTeamName == null || teamName == RightTeamName)
                side = Sides.Right;
            else
                return -1;

            if (side == Sides.Left && LeftPlayersCount >= Settings.Default.MaxPlayers)
                return -1;

            if (side == Sides.Right && RightPlayersCount >= Settings.Default.MaxPlayers)
                return -1;

            int homec = ((unum - 1) / Settings.Default.NumRows) + 1;
            int homer = unum % Settings.Default.NumRows;

            var homePos = new Position(homer, homec);
            if(side == Sides.Right)
                homePos = homePos.GetRTL();

            for (int i = 0; i < LeftPlayersCount + RightPlayersCount; ++i)
            {
                if (Players[i].Side == side)
                {
                    if (Players[i].PlayerNumber == unum)
                        return -1;
                    if (Players[i].HomePosition == homePos)
                        return -1;
                }
            }

            Players[LeftPlayersCount + RightPlayersCount] =
                new PlayerInfo(unum, side, homePos.Row, homePos.Col, homePos.Row, homePos.Col);
            if (side == Sides.Left)
            {
                LeftTeamName = teamName;
                LeftPlayersCount++;
            }
            else
            {
                RightTeamName = teamName;
                RightPlayersCount++;
            }

            ChooseDefaultPlayers();

            if(m_defaultLeftPlayer >= 0)
                BallPosition.Set(Players[m_defaultLeftPlayer].HomePosition);
            else if(m_defaultRightPlayer >= 0)
                BallPosition.Set(Players[m_defaultRightPlayer].HomePosition);

            RaiseChangedEvent();
            RaiseScoreChangedEvent();

            return LeftPlayersCount + RightPlayersCount - 1;
        }

        public bool EpisodeTimeout(int pi, bool isOur, bool isPass)
        {
            if (!IsGameStarted || !Settings.Default.AllowEpisodeTimeout)
                return false;

            m_isGoalScored = true;

            Sides ourSide = Players[pi].Side;
            Sides oppSide = Players[pi].Side == Sides.Left ? Sides.Right : Sides.Left;

            if (isPass)
            {
                m_sideReceivingGoal = isOur ? oppSide : ourSide;
            }
            else
            {
                m_sideReceivingGoal = isOur ? ourSide : oppSide;
            }

            return true;
        }


        /// <summary>
        /// Sets the home position for the player with the specified player index.
        /// </summary>
        /// <param name="pi">The player index.</param>
        /// <param name="r">The row.</param>
        /// <param name="c">The column.</param>
        /// <returns></returns>
        public bool SetHomePos(int pi, int r, int c)
        {
            // You cannot set a player's home position during play-on
            if (IsGameStarted)
                return false;

            // Change it to right-to-left coordination if the player 
            // is playing for the right-hand-side team
            var newHomePosition = new Position(r, c);
            if (Players[pi].Side == Sides.Right)
                newHomePosition = newHomePosition.GetRTL();

            // see if there's another player already at the specified home-position
            int otherPi = FindPlayerWithHomePos(newHomePosition);
            if(otherPi < 0)
            {
                // if there isn't such player then set the newly 
                // added player's home position successfully
                Players[pi].HomePosition.Set(newHomePosition);
                Players[pi].Position.Set(newHomePosition);
                Players[pi].HasHomePos = true;
            }
            else
            {
                // if there's another player with the same home position
                if (otherPi != pi)
                {
                    if (Players[otherPi].HasHomePos)
                        return false;
                    else
                    {
                        Players[otherPi].HomePosition.Set(Players[pi].HomePosition);
                        Players[otherPi].Position.Set(Players[pi].HomePosition);
                        Players[pi].HomePosition.Set(newHomePosition);
                        Players[pi].Position.Set(newHomePosition);
                        Players[pi].HasHomePos = true;
                    }
                }
            }


            ChooseDefaultPlayers();

            // Give the ownership of the ball to the default player
            if (m_defaultLeftPlayer >= 0)
                BallPosition.Set(Players[m_defaultLeftPlayer].HomePosition);
            else if (m_defaultRightPlayer >= 0)
                BallPosition.Set(Players[m_defaultRightPlayer].HomePosition);

            RaiseChangedEvent();
            return true;
        }

        /// <summary>
        /// Finds the player with the given home posisiton 
        /// and returns the corresponding player index.
        /// </summary>
        /// <param name="pos">The position to look for.</param>
        /// <returns></returns>
        private int FindPlayerWithHomePos(Position pos)
        {
            for (int i = 0; i < LeftPlayersCount + RightPlayersCount; ++i)
            {
                if (Players[i].HomePosition == pos)
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// Chooses the default players from each team based upon their 
        /// distance from the center of the field.
        /// Default players are players that will own the ball when their team 
        /// is going to start the game.
        /// </summary>
        private void ChooseDefaultPlayers()
        {
            var center = new Position(Settings.Default.NumRows / 2, Settings.Default.NumCols / 2);
            double minRightDist = Double.PositiveInfinity;
            double minLeftDist = Double.PositiveInfinity;
            double dist;

            for (int i = 0; i < LeftPlayersCount + RightPlayersCount; ++i)
            {
                dist = MathUtils.GetDistancePointFromPoint(Players[i].HomePosition, center);

                if (Players[i].Side == Sides.Left)
                {
                    if (dist < minLeftDist)
                    {
                        minLeftDist = dist;
                        m_defaultLeftPlayer = i;
                    }
                }
                else if(Players[i].Side == Sides.Right)
                {
                    if (dist < minRightDist)
                    {
                        minRightDist = dist;
                        m_defaultRightPlayer = i;
                    }
                }
            }
        }

        /// <summary>
        /// Begins the simulation.
        /// </summary>
        public void BeginSimulation()
        {
            m_pendingActions = new PendingActionsInfo[LeftPlayersCount + RightPlayersCount];
            m_seeLists = new List<int>[LeftPlayersCount + RightPlayersCount];
            for (int i = 0; i < m_pendingActions.Length; ++i)
            {
                m_pendingActions[i] = new PendingActionsInfo();
                m_seeLists[i] = new List<int>();
            }

            m_rndCandidChooser = new RandomCandidateChooser<int>(this.m_randomGenerator);
            IsGameStarted = true;
        }

        public void EndSimulation()
        {
            IsGameStopped = true;
        }

        #region See Message Stuff

        private List<int>[] m_seeLists;

        public void FormSeeLists()
        {
            int count = LeftPlayersCount + RightPlayersCount;

            for (int i = 0; i < count; ++i)
            {
                m_seeLists[i].Clear();
                if (CanSeeBall(i))
                    m_seeLists[i].Add(-1);
            }

            for (int i = 0; i < count - 1; ++i)
            {
                for (int j = i + 1; j < count; ++j)
                {
                    if (CanSeeEachOther(i, j))
                    {
                        m_seeLists[i].Add(j);
                        m_seeLists[j].Add(i);
                    }
                }
            }

        }

        public List<int> GetSeeListForPlayer(int i)
        {
            return m_seeLists[i];
        }

        private bool CanSeeBall(int i)
        {
            var pos1 = Players[i].Position;
            var pos2 = BallPosition;

            return !(Math.Abs(pos1.Col - pos2.Col) > m_visibleDistance || Math.Abs(pos1.Row - pos2.Row) > m_visibleDistance);
        }

        private bool CanSeeEachOther(int i, int j)
        {
            var pos1 = Players[i].Position;
            var pos2 = Players[j].Position;

            return !(Math.Abs(pos1.Col - pos2.Col) > m_visibleDistance || Math.Abs(pos1.Row - pos2.Row) > m_visibleDistance);
        }

        #endregion

        #region Action Update Stuff

        public void BeginUpdateActions()
        {
            m_isUpdating = true;
            m_isGoalScored = false;
            m_ballOwnerIndex = -1;
            ClearPendingActions();
            //m_rndCandidChooser.Clear();
        }

        public void UpdateActionForPlayer(int pi, SoccerAction act)
        {
            if (pi < 0)
                return;

            if (!m_isUpdating)
                return;

            if (m_isGoalScored) // no need to update anything
                return;

            Sides side = Players[pi].Side;

            if (m_pendingActions[pi].Updated)
                return;

            if (side == Sides.Right)
                act = GetActionForRightSide(act);

            bool hasBall = BallPosition == Players[pi].Position;

            if (hasBall) m_ballOwnerIndex = pi;

            Position newPos = GetMovementDir(act.ActionType) + Players[pi].Position;

            // check if a goal is scored
            if (hasBall)
            {
                Sides recvSide;
                if (IsBallBehindGoal(Players[pi].Position, newPos, out recvSide))
                {
                    m_isGoalScored = true;
                    m_sideReceivingGoal = recvSide;
                    return;
                }
            }

            if (IsPositionOutsideField(newPos))
                newPos = NormalizePosition(newPos);

            m_pendingActions[pi].DesiredPos.Set(newPos);
            m_pendingActions[pi].NewPos.Set(newPos);
            m_pendingActions[pi].Action.Set(act);
            m_pendingActions[pi].Updated = true;
        }

        public void FinishUpdateActions()
        {
            PerformPendingUpdates();
        }

        public void FinishCycle()
        {
            m_isUpdating = false;
            this.Cycle++;
            RaiseChangedEvent();
        }

        private void ClearPendingActions()
        {
            for (int i = 0; i < m_pendingActions.Length; ++i)
                m_pendingActions[i].Updated = false;
        }

        private void PerformPendingUpdates()
        {
            if (this.m_isGoalScored)
            {
                if (m_sideReceivingGoal == Sides.Left)
                    this.RightScore++;
                else
                    this.LeftScore++;

                for (int i = 0; i < LeftPlayersCount + RightPlayersCount; ++i)
                    Players[i].Position.Set(Players[i].HomePosition);

                if (m_sideReceivingGoal == Sides.Right)
                {
                    if (m_defaultRightPlayer >= 0)
                        BallPosition.Set(Players[m_defaultRightPlayer].HomePosition);
                    else if (m_defaultLeftPlayer >= 0)
                        BallPosition.Set(Players[m_defaultLeftPlayer].HomePosition);
                }
                else if (m_sideReceivingGoal == Sides.Left)
                {
                    if (m_defaultLeftPlayer >= 0)
                        BallPosition.Set(Players[m_defaultLeftPlayer].HomePosition);
                    else if (m_defaultRightPlayer >= 0)
                        BallPosition.Set(Players[m_defaultRightPlayer].HomePosition);
                }

                RaiseScoreChangedEvent();
            }
            else
            {
                if (m_ballOwnerIndex < 0)
                    m_ballOwnerIndex = FindPlayerAtLocation(BallPosition);

                bool conflictsFound = false;

                do
                {
                    conflictsFound = false;
                    for (int i = 0; i < m_pendingActions.Length; ++i)
                    {
                        if (!m_pendingActions[i].Updated)
                        {
                            m_pendingActions[i].NewPos.Set(Players[i].Position);
                            m_pendingActions[i].DesiredPos.Set(Players[i].Position);
                        }

                        int ci = FindNewPosConflict(m_pendingActions[i].NewPos, i); // short for conflicting index
                        if (ci >= 0)
                        {
                            conflictsFound = true;

                            if (Players[i].Position != m_pendingActions[i].NewPos &&
                                Players[ci].Position != m_pendingActions[ci].NewPos)
                            {
                                if (m_randomGenerator.NextDouble() < 0.5)
                                    m_pendingActions[i].NewPos.Set(Players[i].Position);
                                else
                                    m_pendingActions[ci].NewPos.Set(Players[ci].Position);

                            }
                            else if (Players[i].Position == m_pendingActions[i].NewPos &&
                                Players[ci].Position != m_pendingActions[ci].NewPos)
                            {
                                m_pendingActions[ci].NewPos.Set(Players[ci].Position);
                            }
                            else if (Players[i].Position != m_pendingActions[i].NewPos &&
                                Players[ci].Position == m_pendingActions[ci].NewPos)
                            {
                                m_pendingActions[i].NewPos.Set(Players[i].Position);
                            }
                        }
                    }
                } while (conflictsFound);


                if (m_ballOwnerIndex >= 0)
                {
                    // first form the action type
                    ActionTypes at = ActionTypes.Hold;
                    if (m_pendingActions[m_ballOwnerIndex].Updated)
                    {
                        at = m_pendingActions[m_ballOwnerIndex].Action.ActionType;
                        if (at == ActionTypes.Pass)
                        {
                            int passeeIndex = FindPlayerIndex(Players[m_ballOwnerIndex].Side, m_pendingActions[m_ballOwnerIndex].Action.DestinationUnum);
                            if (passeeIndex == m_ballOwnerIndex || passeeIndex < 0)
                                at = ActionTypes.Hold;
                            else if (!PlayersAreWithinPassableDistance(m_ballOwnerIndex, passeeIndex))
                                at = ActionTypes.Hold;
                        }
                    }
                    else // i.e. the player was holding
                        at = ActionTypes.Hold;

                    int nextBO = m_ballOwnerIndex; // will hold the next ball owner

                    if (at == ActionTypes.Hold)
                    {
                        m_rndCandidChooser.Clear();
                        m_rndCandidChooser.AddCandidate(m_ballOwnerIndex, 4.0);

                        for (int i = 0; i < m_pendingActions.Length; ++i)
                        {
                            if (i != m_ballOwnerIndex && m_pendingActions[i].Updated &&
                                m_pendingActions[i].DesiredPos == BallPosition)
                                m_rndCandidChooser.AddCandidate(i, 1.0);
                        }

                        nextBO = m_rndCandidChooser.ChooseRandomly();
                    }
                    else if (at == ActionTypes.Pass)
                    {
                        int passee = FindPlayerIndex(Players[m_ballOwnerIndex].Side, m_pendingActions[m_ballOwnerIndex].Action.DestinationUnum);
                        Position passerPos = m_pendingActions[m_ballOwnerIndex].NewPos;
                        Position passeePos = m_pendingActions[passee].NewPos;
                        Position curPos;

                        if (PlayersAreInEightMainDirs(m_ballOwnerIndex, passee))
                        {
                            nextBO = passee;
                            double minDist = MathUtils.GetDistancePointFromPoint(passeePos, passerPos);
                            double curDist;

                            for (int i = 0; i < m_pendingActions.Length; ++i)
                            {
                                curPos = m_pendingActions[i].NewPos;
                                if (i != m_ballOwnerIndex && i != passee &&
                                    MathUtils.IsPointBetweenTwoPoints(passerPos, passeePos, curPos))
                                {
                                    curDist = MathUtils.GetDistancePointFromPoint(curPos, passerPos);
                                    if (curDist < minDist)
                                    {
                                        minDist = curDist;
                                        nextBO = i;
                                    }
                                }
                            }
                        }
                        else // if they are not in the eight main directions
                        {
                            double curDist;

                            nextBO = passee;

                            m_rndCandidChooser.Clear();
                            m_rndCandidChooser.AddCandidate(passee, 4.0);

                            for (int i = 0; i < m_pendingActions.Length; ++i)
                            {
                                curPos = m_pendingActions[i].NewPos;
                                if (i != m_ballOwnerIndex && i != passee &&
                                    MathUtils.IsPointInRectangle(passerPos, passeePos, curPos))
                                {
                                    curDist = MathUtils.GetDistancePointFromLine(curPos, passerPos, passeePos);
                                    if (0 < curDist && curDist < 1.5)
                                    {
                                        m_rndCandidChooser.AddCandidate(i, 1.0);
                                    }
                                    else if (curDist == 0.0)
                                    {
                                        if (MathUtils.GetDistancePointFromPoint(curPos, passerPos) <
                                            MathUtils.GetDistancePointFromPoint(m_pendingActions[nextBO].NewPos, passerPos))
                                            nextBO = i;
                                    }
                                }
                            }

                            if(nextBO == passee)
                                nextBO = m_rndCandidChooser.ChooseRandomly();
                        }
                    }
                    else // One of the Moves
                    {
                        if (m_pendingActions[m_ballOwnerIndex].NewPos != m_pendingActions[m_ballOwnerIndex].DesiredPos)
                        {
                            int plAtDes = FindPlayerAtLocation(m_pendingActions[m_ballOwnerIndex].DesiredPos);
                            if (plAtDes >= 0 && plAtDes != m_ballOwnerIndex && PlayerWasHolding(plAtDes))
                            {
                                nextBO = plAtDes;
                            }
                            else
                            {
                                m_rndCandidChooser.Clear();
                                m_rndCandidChooser.AddCandidate(m_ballOwnerIndex, 4.0);
                                for (int i = 0; i < m_pendingActions.Length; ++i)
                                {
                                    if (i != m_ballOwnerIndex && m_pendingActions[i].Updated &&
                                        m_pendingActions[i].DesiredPos == m_pendingActions[m_ballOwnerIndex].DesiredPos)
                                        m_rndCandidChooser.AddCandidate(i, 1.0);
                                }

                                nextBO = m_rndCandidChooser.ChooseRandomly();
                            }
                        }
                    }

                    if(nextBO >= 0)
                        BallPosition.Set(m_pendingActions[nextBO].NewPos);
                }

                // in the end put the players at their new position
                for (int i = 0; i < LeftPlayersCount + RightPlayersCount; ++i)
                {
                    if(m_pendingActions[i].Updated)
                        Players[i].Position.Set(m_pendingActions[i].NewPos);
                }
            }
        }

        private int FindNewPosConflict(Position newpos, int pi)
        {
            for (int i = 0; i < m_pendingActions.Length; ++i)
            {
                if (i != pi)
                {
                    if((m_pendingActions[i].Updated && m_pendingActions[i].NewPos == newpos) ||
                        (!m_pendingActions[i].Updated && Players[i].Position == newpos))
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Utils

        private bool PlayersAreInEightMainDirs(int p1, int p2)
        {
            Position pos1 = m_pendingActions[p1].NewPos;
            Position pos2 = m_pendingActions[p2].NewPos;

            return (pos1.Row == pos2.Row || pos1.Col == pos2.Col || Math.Abs(pos1.Row - pos2.Row) == Math.Abs(pos1.Col - pos2.Col));
        }

        private bool PlayersAreWithinPassableDistance(int p1, int p2)
        {
            Position pos1 = m_pendingActions[p1].NewPos;
            Position pos2 = m_pendingActions[p2].NewPos;

            return !(Math.Abs(pos1.Col - pos2.Col) > m_passableDistance || Math.Abs(pos1.Row - pos2.Row) > m_passableDistance);
        }

        private bool PlayerWasHolding(int playerIndex)
        {
            if (!m_pendingActions[playerIndex].Updated)
                return true;
            return (Players[playerIndex].Position == m_pendingActions[playerIndex].NewPos);
        }

        private Position NormalizePosition(Position pos)
        {
            int r = pos.Row;
            int c = pos.Col;
            if (r < 1) r = 1;
            if (c < 1) c = 1;
            if (r > Settings.Default.NumRows) r = Settings.Default.NumRows;
            if (c > Settings.Default.NumCols) c = Settings.Default.NumCols;

            return new Position(r, c);
        }

        private bool IsPositionOutsideField(Position pos)
        {
            if (pos.Row < 1 || pos.Col < 1 || pos.Row > Settings.Default.NumRows || pos.Col > Settings.Default.NumCols)
                return true;
            else
                return false;
        }

        private bool IsBallBehindGoal(Position oldPos, Position newPos, out Sides sideReceiving)
        {
            int rowStartGoal = (Settings.Default.NumRows - Settings.Default.GoalWidth) / 2 + 1;
            if(rowStartGoal <= newPos.Row && newPos.Row <= rowStartGoal + Settings.Default.GoalWidth - 1 &&
                rowStartGoal <= oldPos.Row && oldPos.Row <= rowStartGoal + Settings.Default.GoalWidth - 1)
            {
                if (newPos.Col < 1)
                {
                    sideReceiving = Sides.Left;
                    return true;
                }
                else if (newPos.Col > Settings.Default.NumCols)
                {
                    sideReceiving = Sides.Right;
                    return true;
                }
            }

            sideReceiving = Sides.Left;
            return false;
        }

        private Position GetMovementDir(ActionTypes act)
        {
            int r = 0, c = 0;
            switch (act)
            {
                case ActionTypes.Hold:
                    r = 0; c = 0;
                    break;
                case ActionTypes.MoveEast:
                    r = 0; c = 1;
                    break;
                case ActionTypes.MoveSouth:
                    r = 1; c = 0;
                    break;
                case ActionTypes.MoveWest:
                    r = 0; c = -1;
                    break;
                case ActionTypes.MoveNorth:
                    r = -1; c = 0;
                    break;
                case ActionTypes.MoveNorthEast:
                    r = -1; c = 1;
                    break;
                case ActionTypes.MoveSouthEast:
                    r = 1; c = 1;
                    break;
                case ActionTypes.MoveSouthWest:
                    r = 1; c = -1;
                    break;
                case ActionTypes.MoveNorthWest:
                    r = -1; c = -1;
                    break;
                case ActionTypes.Pass:
                    r = 0; c = 0;
                    break;
                default:
                    break;
            }

            return new Position(r, c);
        }

        private SoccerAction GetActionForRightSide(SoccerAction act)
        {
            ActionTypes actType = act.ActionType;
            int dst = -1;

            switch (act.ActionType)
            {
                case ActionTypes.Hold:
                    actType = ActionTypes.Hold;
                    break;
                case ActionTypes.MoveEast:
                    actType = ActionTypes.MoveWest;
                    break;
                case ActionTypes.MoveSouth:
                    actType = ActionTypes.MoveNorth;
                    break;
                case ActionTypes.MoveWest:
                    actType = ActionTypes.MoveEast;
                    break;
                case ActionTypes.MoveNorth:
                    actType = ActionTypes.MoveSouth;
                    break;
                case ActionTypes.MoveNorthEast:
                    actType = ActionTypes.MoveSouthWest;
                    break;
                case ActionTypes.MoveSouthEast:
                    actType = ActionTypes.MoveNorthWest;
                    break;
                case ActionTypes.MoveSouthWest:
                    actType = ActionTypes.MoveNorthEast;
                    break;
                case ActionTypes.MoveNorthWest:
                    actType = ActionTypes.MoveSouthEast;
                    break;
                case ActionTypes.Pass:
                    actType = ActionTypes.Pass;
                    dst = act.DestinationUnum;
                    break;
                default:
                    break;
            }

            return new SoccerAction(actType, dst);
        }

        private int FindPlayerAtLocation(Position pos)
        {
            return FindPlayerAtLocation(pos.Row, pos.Col);
        }
    
        private int FindPlayerAtLocation(int r, int c)
        {
            for (int i = 0; i < LeftPlayersCount + RightPlayersCount; ++i)
                if (Players[i].Row == r && Players[i].Col == c)
                    return i;
            return -1;
        }

        private int FindPlayerIndex(Sides side, int unum)
        {
            for (int i = 0; i < LeftPlayersCount + RightPlayersCount; ++i)
                if (Players[i].Side == side && Players[i].PlayerNumber == unum)
                    return i;
            return -1;
        }

        #endregion 
    
    
        
    }
}
