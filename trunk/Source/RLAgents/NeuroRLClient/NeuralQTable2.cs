using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;
using NeuralNetwork;
using NeuralNetwork.Learning;

namespace GridSoccer.NeuroRLClient
{
    /// <summary>
    /// 
    /// </summary>
    public class NeuralQTable2 : NeuralQTableBase
    {
        private int m_numActions;
        private ActivationNetwork m_ann;
        private BackPropagationLearning m_learning;

        private ObservationHistory m_obsHist;
        private double m_scalingAlpha = 0.05;

        //private double m_scalingFactor = 1;//40.0;

        public NeuralQTable2(RLClientBase client, int numTeammates, int myUnum)
            : base(client, numTeammates, myUnum)
        {
            m_numActions = SoccerAction.GetActionCount(Params.MoveKings, m_numTeammates);

            m_ann = new ActivationNetwork(
                new BipolarSigmoidFunction(2),  // threshold function
                m_numFeatures,  // 2 inputs
            m_numFeatures * 2, m_numActions);

            //m_ann.SetLastLayerActivationFunction(new LinearFunction());

            m_learning = new BackPropagationLearning(m_ann);

            // TODO
            m_learning.LearningRate = Params.Alpha;
            //m_learning.Momentum = 0.01;

            m_obsHist = new ObservationHistory(1);
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

            m_learning.Run(feats, actValues);

            //m_obsHist.Add(feats, actValues);

            //const int maxEpoch = 1;
            //const double minError = 0.001;

            //double curError = Double.MaxValue;

            //for (int i = 0; i < maxEpoch && curError > minError; i++)
            //{
            //    curError = 0.0;

            //    for (int oi = m_obsHist.Count - 1; oi >= 0; oi--)
            //    {
            //        var obs = m_obsHist[oi];
            //        double e = m_learning.Run(obs.Features, obs.QValues);

            //        curError = Math.Max(curError, e);
            //    }

            //}
        }

    }
}
