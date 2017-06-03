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

namespace GridSoccer.RLAgentsCommon
{
    public class EnvironmentModeler
    {
        public static double GetReward(State prevState, State curState, ActionTypes prevAct)
        {
            bool preAmIBallOwner = prevState.AmIBallOwner;
            bool preAreWeBallOwner = prevState.AreWeBallOwner;
            bool curAmIBallOwner = curState.AmIBallOwner;
            bool curAreWeBallOwner = curState.AreWeBallOwner;
            bool isCollaborative = Params.RewardAssignmentStrategy == Params.RewardAssignment.Collaborative;

            if (curState.OurScore > prevState.OurScore) // if we scored a goal
            {
                if (preAmIBallOwner)
                    return Params.RewardSelfScoreGoal;
                else if (preAreWeBallOwner)
                {
                    if(isCollaborative)
                        return Params.RewardTeamScoreGoal;
                }
                else
                    return Params.RewardOpponentOwnGoal;
            }

            if (curState.OppScore > prevState.OppScore) // if we received a goal
            {
                if (preAmIBallOwner)
                    return Params.RewardSelfOwnGoal;
                else if (preAreWeBallOwner)
                {
                    if(isCollaborative)
                        return Params.RewardTeamOwnGoal;
                }
                else
                    return Params.RewardTeamRecvGoal;
            }


            //if (!isCollaborative && curAmIBallOwner && !preAmIBallOwner && prevAct != ActionTypes.Pass)
            //    return Params.RewardSelfCatchBall;
            if (curAmIBallOwner && !preAreWeBallOwner)
                return Params.RewardSelfCatchBall;
            if (curAreWeBallOwner && !preAreWeBallOwner && isCollaborative)
                return Params.RewardTeamCatchBall;
            if (!curAreWeBallOwner && preAmIBallOwner)
                return Params.RewardSelfLooseBall;
            if (!curAreWeBallOwner && preAreWeBallOwner && isCollaborative)
                return Params.RewardTeamLooseBall;

            if (prevAct == ActionTypes.Pass && preAmIBallOwner && !curAmIBallOwner && curAreWeBallOwner)
                return Params.RewardSuccessfulPass;

            if (prevAct == ActionTypes.Hold)
                return Params.RewardHold;

            if (prevAct != ActionTypes.Hold && prevAct != ActionTypes.Pass && prevState.Me.Position == curState.Me.Position)
                return Params.RewardIllegalMovment;

            return Params.RewardEveryMovment;
        }

        public static int GuessTeammateAction(State prevState, State curState, int teammateIndex)
        {
            int doerUnum;
            SoccerAction soc = GuessTeammateAction(prevState, curState, teammateIndex, out doerUnum);
            return SoccerAction.GetIndexFromAction(soc, Params.MoveKings, doerUnum);
        }

        private static SoccerAction GuessTeammateAction(State prevState, State curState, int teammateIndex, out int doerUnum)
        {
            PlayerInfo prevInfo = prevState.OurPlayersList[teammateIndex];
            PlayerInfo curInfo = curState.OurPlayersList[teammateIndex];
            Position prevPos = prevInfo.Position;
            Position curPos = curInfo.Position;
            doerUnum = curInfo.Unum;

            if (prevState.OurScore != curState.OurScore || prevState.OppScore != curState.OppScore)
                return new SoccerAction(ActionTypes.Hold);

            if (prevPos == curPos) // i.e. it is either hold or pass
            {
                if (prevInfo.IsBallOwner && !curInfo.IsBallOwner) // it is a pass
                {
                    if (curState.Me.IsBallOwner)
                        return new SoccerAction(ActionTypes.Pass, curState.Me.Unum);

                    for (int i = 0; i < curState.OurPlayersList.Count; ++i)
                    {
                        if (curState.OurPlayersList[i].IsBallOwner)
                            return new SoccerAction(ActionTypes.Pass, curState.OurPlayersList[i].Unum);
                    }

                    return new SoccerAction(ActionTypes.Hold);
                }
                else // i.e. it was a hold
                {
                    return new SoccerAction(ActionTypes.Hold);
                }
            }
            else // i.e. it had made a move
            {
                int r0 = prevPos.Row; int r1 = curPos.Row;
                int c0 = prevPos.Col; int c1 = curPos.Col;

                if (r0 == r1)
                {
                    if(c1 > c0)
                        return new SoccerAction(ActionTypes.MoveEast);
                    else
                        return new SoccerAction(ActionTypes.MoveWest);
                }

                if (c1 == c0)
                {
                    if (r1 > r0)
                        return new SoccerAction(ActionTypes.MoveSouth);
                    else
                        return new SoccerAction(ActionTypes.MoveNorth);
                }

                if (c1 > c0)
                {
                    if (r1 > r0)
                        return new SoccerAction(ActionTypes.MoveSouthEast);
                    else
                        return new SoccerAction(ActionTypes.MoveNorthEast);
                }
                else
                {
                    if (r1 > r0)
                        return new SoccerAction(ActionTypes.MoveSouthWest);
                    else
                        return new SoccerAction(ActionTypes.MoveNorthWest);
                }
            }
        }

        public static int GuessOpponentAction(State prevState, State curState, int oppIndex)
        {
            int doerUnum;
            SoccerAction soc = GuessOpponentAction(prevState, curState, oppIndex, out doerUnum);
            return SoccerAction.GetIndexFromAction(soc, Params.MoveKings, doerUnum);
        }

        private static SoccerAction GuessOpponentAction(State prevState, State curState, int oppIndex, out int doerUnum)
        {
            PlayerInfo prevInfo = prevState.OppPlayersList[oppIndex];
            PlayerInfo curInfo = curState.OppPlayersList[oppIndex];
            Position prevPos = prevInfo.Position;
            Position curPos = curInfo.Position;
            doerUnum = curInfo.Unum;

            if (prevState.OurScore != curState.OurScore || prevState.OppScore != curState.OppScore)
                return new SoccerAction(ActionTypes.Hold);

            if (prevPos == curPos) // i.e. it is either hold or pass
            {
                if (prevInfo.IsBallOwner && !curInfo.IsBallOwner) // it is a pass
                {
                    for (int i = 0; i < curState.OppPlayersList.Count; ++i)
                    {
                        if (curState.OppPlayersList[i].IsBallOwner)
                            return new SoccerAction(ActionTypes.Pass, curState.OppPlayersList[i].Unum);
                    }

                    return new SoccerAction(ActionTypes.Hold);
                }
                else // i.e. it was a hold
                {
                    return new SoccerAction(ActionTypes.Hold);
                }
            }
            else // i.e. it had made a move
            {
                int r0 = prevPos.Row; int r1 = curPos.Row;
                int c0 = prevPos.Col; int c1 = curPos.Col;

                if (r0 == r1)
                {
                    if (c1 > c0)
                        return new SoccerAction(ActionTypes.MoveWest);
                    else
                        return new SoccerAction(ActionTypes.MoveEast);
                }

                if (c1 == c0)
                {
                    if (r1 > r0)
                        return new SoccerAction(ActionTypes.MoveNorth);
                    else
                        return new SoccerAction(ActionTypes.MoveSouth);
                }

                if (c1 > c0)
                {
                    if (r1 > r0)
                        return new SoccerAction(ActionTypes.MoveNorthWest);
                    else
                        return new SoccerAction(ActionTypes.MoveSouthWest);
                }
                else
                {
                    if (r1 > r0)
                        return new SoccerAction(ActionTypes.MoveNorthEast);
                    else
                        return new SoccerAction(ActionTypes.MoveSouthEast);
                }
            }
        }

    }
}
