using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;
using NeuralNetwork;
using GridSoccer.RLAgentsCommon;
using NeuralNetwork.FeedForwardNet;

namespace GridSoccer.NeuroRLClient
{
    class NeuralQTable4 : NeuralQTableBase
    {
        private int m_numActions;
        private FeedForwardNetwork m_ann;
        private FFBackpropLearning m_learning;

        private double m_scalingAlpha = 0.05;

        public NeuralQTable4(RLClientBase client, int numTeammates, int myUnum)
            : base(client, numTeammates, myUnum)
        {
            m_numActions = SoccerAction.GetActionCount(Params.MoveKings, m_numTeammates);

            m_ann = new FeedForwardNetwork(
                new BipolarSigmoidFunction(),  // threshold function
                WeightInitMethods.NormalRandom, 1.0,
                m_numFeatures,  // 2 inputs
                m_numFeatures * 12, m_numFeatures * 10, m_numActions);

            //m_ann.SetLastLayerActivationFunction(new LinearFunction());

            m_learning = new FFBackpropLearning(m_ann);
        }

        protected override double GetQValue(State s, int ai)
        {
            double[] feats = ExtractFeatures(s);
            double[] actValues = m_ann.Compute(feats);
            return NeuralNetwork.MathUtils.BipolarSigmoidInverse(
                actValues[ai], m_scalingAlpha);
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            double[] feats = ExtractFeatures(s);
            double[] actValues = m_ann.Compute(feats);
            actValues[ai] = NeuralNetwork.MathUtils.BipolarSigmoid(
                newValue, m_scalingAlpha);

            double errb, erra;
            m_learning.RunIncremental(feats, actValues, out errb, out erra);
        }

    }
}
