using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNetwork.FeedForwardNet;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;
using NeuralNetwork;

namespace GridSoccer.NeuroRLClient
{
    class OnlineNFQTable: NeuralQTableBase
    {
        private int m_numActions;
        private FeedForwardNetwork m_ann;
        private IFeedForwardLearningAlgorithm m_learning;

        private double m_scalingAlpha = 0.05;
        private double m_episodeCountBeforeUpdate = 1000;
        private int m_maxBatchIters = 10000;

        private bool m_isLastLayerLinear = false;

        private ObservationHistory m_obsHistory = new ObservationHistory(Int32.MaxValue, true);

        public OnlineNFQTable(RLClientBase client, int numTeammates, int myUnum)
            : base(client, numTeammates, myUnum)
        {
            m_numActions = SoccerAction.GetActionCount(Params.MoveKings, m_numTeammates);

            m_ann = new FeedForwardNetwork(
                new BipolarSigmoidFunction(),  
                WeightInitMethods.Constant, 0.0,
                m_numFeatures + 1,  
                m_numFeatures * 12, m_numFeatures * 10, 1);

            if(m_isLastLayerLinear)
                m_ann.SetLastLayerActivationFunction(new LinearFunction());

            m_learning = new FFBackpropLearning(m_ann);
        }

        private double[] GetFeatureVector(State s, int ai)
        {
            double[] feats = ExtractFeatures(s);
            double[] inputs = new double[feats.Length + 1];
            Array.Copy(feats, 0, inputs, 0, feats.Length);
            inputs[inputs.Length - 1] = ai;

            return inputs;
        }

        protected override double GetQValue(State s, int ai)
        {
            double[] inputs = GetFeatureVector(s, ai);
            
            double[] qValue = m_ann.Compute(inputs);

            if (m_isLastLayerLinear)
            {
                return qValue[0];
            }
            else
            {
                return NeuralNetwork.MathUtils.BipolarSigmoidInverse(
                    qValue[0], m_scalingAlpha);
            }
        }

        private int m_prevScoresSum = 0;

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            if (s.OurScore + s.OppScore != m_prevScoresSum) // i.e. a new episode
            {
                m_prevScoresSum++;
                if (m_prevScoresSum % m_episodeCountBeforeUpdate == 0)
                {
                    PerformNFQ();
                }
            }

            double[] inputs = GetFeatureVector(s, ai);


            double newQValue = newValue;
            if (m_isLastLayerLinear)
            {
                newQValue = newValue;
            }
            else
            {
                newQValue = NeuralNetwork.MathUtils.BipolarSigmoid(
                    newValue, m_scalingAlpha);
            }

            m_obsHistory.Add(inputs, new double[] { newQValue });

        }

        private double[] GetQValues(State s)
        {
            double[] qValues = new double[m_numActions];
            for (int ai = 0; ai < m_numActions; ai++)
            {
                qValues[ai] =  m_ann.Compute(GetFeatureVector(s, ai))[0];
            }

            return qValues;
        }

        private double GetMaxQValue(State s)
        {
            double maxQvalue;
            base.GetMaxQ(s, out maxQvalue);
            return maxQvalue;
        }

        private void PerformNFQ()
        {
            const double ConvergenceThreshold = 0.000001;
            const int MaxConvergenceIters = 10;

            double curIterErr = 0.0;
            double prevIterErr = Double.MaxValue;

            int curConvergIters = 0;

            double errb, erra; 
            for (int bi = 0; bi < m_maxBatchIters; bi++)
            {
                curIterErr = 0.0;
                for (int oi = 0, oend = m_obsHistory.Count - 1; oi <= oend; oi++)
                {
                    bool update = (oi == oend);
                    m_learning.RunOneIteration(update,
                        m_obsHistory[oi].Features, m_obsHistory[oi].QValues,
                        out errb, out erra);

                    curIterErr += erra;
                }

                if (curIterErr < 5 && Math.Abs(curIterErr - prevIterErr) < ConvergenceThreshold)
                    curConvergIters++;
                else
                    curConvergIters = 0;


                if (curConvergIters >= MaxConvergenceIters)
                    break; // net has converged

                prevIterErr = curIterErr;
            }

            m_obsHistory.Clear();
        }

        public override string MethodName
        {
            get { return "OnlineNFQ"; }
        }
    }
}
