using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;
using NeuralNetwork;
using System.IO;

namespace GridSoccer.NeuroRLClient
{
    public abstract class NeuralQTableBase : QTableBase
    {
        public NeuralQTableBase(RLClientBase client, int numTeammates, int myUnum)
            : base(true)
        {
            m_client = client;
            m_numTeammates = numTeammates;
            m_myUnum = myUnum;
        }

        protected RLClientBase m_client;
        protected int m_numTeammates = 1;
        protected int m_myUnum = 1;


        public virtual int NumFeatures
        {
            get
            {
                return m_numFeatures;
            }
        }

        protected int m_numFeatures = 5;
        public virtual double[] ExtractFeatures(State s)
        {
            double[] feats = new double[m_numFeatures];

            int myR = s.Me.Position.Row;
            int myC = s.Me.Position.Col;

            int oppR = s.OppPlayersList[0].Position.Row;
            int oppC = s.OppPlayersList[0].Position.Col;

            feats[0] = myR;
            feats[1] = myC;
            feats[2] = oppR;
            feats[3] = oppC;
            feats[4] = s.AmIBallOwner ? 1.0 : 0.0;
            return feats;
        }

        //protected int m_numFeatures = 7;
        //public virtual double[] ExtractFeatures(State s)
        //{
        //    double[] feats = new double[m_numFeatures];

        //    int myR = s.Me.Position.Row;
        //    int myC = s.Me.Position.Col;

        //    int oppR = s.OppPlayersList[0].Position.Row;
        //    int oppC = s.OppPlayersList[0].Position.Col;

        //    int goalR = m_client.EnvRows / 2;
        //    int ourGoalC = 1;
        //    int oppGoalC = m_client.EnvCols;

        //    // dist to goal row
        //    feats[0] = myR - goalR;

        //    // dist to our goal col
        //    feats[1] = myC - ourGoalC;

        //    // dist to opp goal col
        //    feats[2] = myC - oppGoalC;

        //    // dist to ball
        //    feats[3] = s.AmIBallOwner ? 0 : oppR - myR;
        //    feats[4] = s.AmIBallOwner ? 0 : oppC - myC;

        //    // dist to opp player
        //    feats[5] = oppR - myR;
        //    feats[6] = oppC - myC;

        //    return feats;
        //}

        
        //private int m_numFeatures = 5;
        //private double[] ExtractFeatures(State s)
        //{
            //int goalR = m_client.EnvRows / 2;
            //int ourGoalC = 1;
            //int oppGoalC = m_client.EnvCols;

            //// dist to goal row
            //feats[0] = myR - goalR;

            //// dist to our goal col
            //feats[1] = myC - ourGoalC;

            //// dist to opp goal col
            //feats[2] = myC - oppGoalC;

            //// dist to ball
            //feats[3] = s.AmIBallOwner ? 0: oppR - myR;
            //feats[4] = s.AmIBallOwner ? 0: oppC - myC;

            //// dist to opp player
            //feats[5] = oppR - myR;
            //feats[6] = oppC - myC;
        
            //return feats;
        //}

        public virtual string MethodName { get { return "NeuralRL1P"; } }

        protected override int MyUnum
        {
            get { return m_myUnum; }
        }

        protected override int TeammatesCount
        {
            get { return m_numTeammates; }
        }

        public override void Save(TextWriter tw)
        {
            throw new NotImplementedException();
        }

        public override void Load(TextReader tr)
        {
            throw new NotImplementedException();
        }

        public override Array QTableArray
        {
            get { throw new NotImplementedException(); }
        }
    }
}
