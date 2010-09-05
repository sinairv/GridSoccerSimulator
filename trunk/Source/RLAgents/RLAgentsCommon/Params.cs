// Copyright (c) 2009 - 2010 
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

namespace GridSoccer.RLAgentsCommon
{
    public class Params
    {
        #region enums
        public enum RLMethods
        {
            Q_Zero, SARSA_Zero, SARSA_Lambda, Q_Lambda_Watkins, Q_Lambda_Naive /*, Q_Lambda_Peng*/ 
        }

        public enum EligibilityTraceTypes
        {
            Accumulating, Replacing
        }

        public enum RewardAssignment
        {
            SelfInterested, Collaborative
        }
        #endregion

        public static bool MoveKings = false;
        public static bool IsStateUniformNeutral = false;
        public static RLMethods RLMethod = RLMethods.Q_Zero;
        public static EligibilityTraceTypes TraceType = EligibilityTraceTypes.Replacing;

        public static double Gamma = 0.9;
        public static double Lambda = 0.9; // trace-decay parameter
        public static double Alpha = 0.1;
        public static double Epsillon = 0.1;

        public static double Theta = 0.001; // internal model learning rate

        public static RewardAssignment RewardAssignmentStrategy = RewardAssignment.Collaborative;

        public static double RewardEveryMovment = -2;
        public static double RewardSuccessfulPass = -1;
        public static double RewardHold = -1;
        public static double RewardIllegalMovment = -3;

        public static double RewardTeamCatchBall = 20;
        public static double RewardTeamLooseBall = -20;

        public static double RewardSelfCatchBall = 20;
        public static double RewardSelfLooseBall = -20;

        public static double RewardTeamScoreGoal = 20;
        public static double RewardSelfScoreGoal = 20;
        public static double RewardTeamRecvGoal = -20;

        public static double RewardTeamOwnGoal = -50;
        public static double RewardSelfOwnGoal = -100;
        public static double RewardOpponentOwnGoal = 1;

        public static string GetParamsString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("% MoveKings: {0}", MoveKings));
            sb.AppendLine(String.Format("% IsStateUniformNeutral: {0}", IsStateUniformNeutral));
            sb.AppendLine(String.Format("% RLMethod: {0}", RLMethod));
            sb.AppendLine(String.Format("% EligibilityTraceType: {0}", TraceType));
            sb.AppendLine("%");
            sb.AppendLine(String.Format("% Gamma: {0}", Gamma));
            sb.AppendLine(String.Format("% Lambda: {0}", Lambda));
            sb.AppendLine(String.Format("% Alpha: {0}", Alpha));
            sb.AppendLine(String.Format("% Epsillon: {0}", Epsillon ));
            sb.AppendLine(String.Format("% Theta: {0}", Theta));
            sb.AppendLine("%");
            sb.AppendLine(String.Format("% RewardAssignmentStrategy: {0}", RewardAssignmentStrategy));
            sb.AppendLine("%");
            sb.AppendLine(String.Format("% RewardEveryMovment: {0}", RewardEveryMovment));
            sb.AppendLine(String.Format("% RewardSuccessfulPass: {0}", RewardSuccessfulPass));
            sb.AppendLine(String.Format("% RewardHold: {0}", RewardHold));
            sb.AppendLine(String.Format("% RewardIllegalMovment: {0}", RewardIllegalMovment));
            sb.AppendLine("%");
            sb.AppendLine(String.Format("% RewardTeamCatchBall: {0}", RewardTeamCatchBall));
            sb.AppendLine(String.Format("% RewardTeamLooseBall: {0}", RewardTeamLooseBall));
            sb.AppendLine("%");
            sb.AppendLine(String.Format("% RewardSelfCatchBall: {0}", RewardSelfCatchBall));
            sb.AppendLine(String.Format("% RewardSelfLooseBall: {0}", RewardSelfLooseBall));
            sb.AppendLine("%");
            sb.AppendLine(String.Format("% RewardTeamScoreGoal: {0}", RewardTeamScoreGoal));
            sb.AppendLine(String.Format("% RewardSelfScoreGoal: {0}", RewardSelfScoreGoal));
            sb.AppendLine(String.Format("% RewardTeamRecvGoal: {0}", RewardTeamRecvGoal));
            sb.AppendLine("%");
            sb.AppendLine(String.Format("% RewardTeamOwnGoal: {0}", RewardTeamOwnGoal));
            sb.AppendLine(String.Format("% RewardSelfOwnGoal: {0}", RewardSelfOwnGoal));
            sb.AppendLine(String.Format("% RewardOpponentOwnGoal: {0}", RewardOpponentOwnGoal));

            return sb.ToString();
        }

        public static class DM
        {
            public enum MethodTypes
            {
                Averaging, TopQ, Voting
            }

            public static int K = 1; // as in K-Cyclic NN

            public static double MinSupport = 2000;
            public static double MinConfidence = 0.0;

            public static MethodTypes Method = MethodTypes.Voting;
        }
    }
}
