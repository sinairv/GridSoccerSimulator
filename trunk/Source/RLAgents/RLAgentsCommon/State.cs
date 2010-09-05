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
using GridSoccer.Common;

namespace GridSoccer.RLAgentsCommon
{
    public class State
    {
        public PlayerInfo Me { get; private set; }
        public List<PlayerInfo> OurPlayersList { get; private set; }
        public List<PlayerInfo> OppPlayersList { get; private set; }
        public int OurScore = 0;
        public int OppScore = 0;

        private bool m_amIBallOwner = false;
        private bool m_areWeBallOwner = false;

        public State(PlayerInfo myInfo)
        {
            OurPlayersList = new List<PlayerInfo>();
            OppPlayersList = new List<PlayerInfo>();
            SetMe(myInfo);
        }

        public State GetDecomposedState(int[] teammateIndeces, int[] oppIndececs)
        {
            State s = new State(Me.Clone());
            if (OurPlayersList.Count > 0)
            {
                for (int i = 0; i < teammateIndeces.Length; ++i)
                    s.OurPlayersList.Add(OurPlayersList[teammateIndeces[i]].Clone());
            }

            if (OppPlayersList.Count > 0)
            {
                for (int i = 0; i < oppIndececs.Length; ++i)
                    s.OppPlayersList.Add(OppPlayersList[oppIndececs[i]].Clone());
            }

            s.OurScore = OurScore;
            s.OppScore = OppScore;

            s.m_amIBallOwner = m_amIBallOwner;
            s.m_areWeBallOwner = m_areWeBallOwner;

            return s;
        }

        private void SetMe(PlayerInfo pi)
        {
            Me = pi;
            m_amIBallOwner = Me.IsBallOwner;
            m_areWeBallOwner = Me.IsBallOwner;
        }

        public void AddOurPlayer(PlayerInfo pi)
        {
            OurPlayersList.Add(pi);
            if (pi.IsBallOwner)
                m_areWeBallOwner = true;

            if (Params.IsStateUniformNeutral)
                OppPlayersList.Sort(UnumNeutralComparison);
            else
                OppPlayersList.Sort(UnumBasedComparison);
        }

        public void AddOppPlayer(PlayerInfo pi)
        {
            OppPlayersList.Add(pi);
            if (Params.IsStateUniformNeutral)
                OppPlayersList.Sort(UnumNeutralComparison);
            else
                OppPlayersList.Sort(UnumBasedComparison);
        }

        private int UnumBasedComparison(PlayerInfo pi1, PlayerInfo pi2)
        {
            return pi1.Unum - pi2.Unum;
        }

        private int UnumNeutralComparison(PlayerInfo pi1, PlayerInfo pi2)
        {
            // TODO: fix me
            double d1 = MathUtils.GetDistancePointFromPoint(pi1.Position, new Position(1, 1));
            double d2 = MathUtils.GetDistancePointFromPoint(pi2.Position, new Position(1, 1));
            return Math.Sign( d1 - d2 );
        }

        public int BallOwnerIndex
        {
            get
            {
                if (m_amIBallOwner)
                {
                    return 0;
                }
                else if (m_areWeBallOwner)
                {
                    for (int i = 0; i < OurPlayersList.Count; ++i)
                        if (OurPlayersList[i].IsBallOwner)
                            return i + 1;
                }
                else
                {
                    for (int i = 0; i < OppPlayersList.Count; ++i)
                        if (OppPlayersList[i].IsBallOwner)
                            return i + OurPlayersList.Count + 1;
                }

                return -1;
                //throw new Exception("Ball Owner Index == -1");
                //return -1; // this should not happen
            }
        }

        public bool AreWeBallOwner
        {
            get
            {
                return m_areWeBallOwner;
            }
        }

        public bool AmIBallOwner
        {
            get
            {
                return m_amIBallOwner;
            }
        }

        public string GetStateString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GetStateStringForPlayerInfo(this.Me));

            if (OurPlayersList.Count > 0)
            {
                sb.Append("OUR");

                foreach (PlayerInfo pi in OurPlayersList)
                    sb.Append(GetStateStringForPlayerInfo(pi));
            }

            if (OppPlayersList.Count > 0)
            {
                sb.Append("OPP");

                foreach (PlayerInfo pi in OppPlayersList)
                    sb.Append(GetStateStringForPlayerInfo(pi));
            }

            return sb.ToString();
        }

        private string GetStateStringForPlayerInfo(PlayerInfo pi)
        {
            Position pos = pi.Position;
            string r = pos.Row.ToString();
            if (r.Length == 1)
                r = "00" + r;
            else if (r.Length == 2)
                r = "0" + r;

            string c = pos.Col.ToString();
            if (c.Length == 1)
                c = "00" + c;
            else if (c.Length == 2)
                c = "0" + c;

            if (pi.IsBallOwner)
                c += "B";

            return r + c;
        }
        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Me.ToString());
            sb.Append("{our");
            foreach (var info in OurPlayersList)
            {
                sb.Append(info.ToString());
            }
            sb.Append("}{opp");
            foreach (var info in OppPlayersList)
            {
                sb.Append(info.ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
