using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;
using NeuralNetwork;
using GridSoccer.RLAgentsCommon;
using NeuralNetwork.Learning;

namespace GridSoccer.NeuroRLClient
{
    class NeuralQTable3 : NeuralQTableBase
    {
        private int m_numActions;
        private ActivationNetwork m_ann;
        private BackPropagationLearning m_learning;

        //private ObservationHistory m_obsHist;
        private double m_scalingFactor = 20.0;

        public NeuralQTable3(RLClientBase client, int numTeammates, int myUnum)
            : base(client, numTeammates, myUnum)
        {
            m_numActions = SoccerAction.GetActionCount(Params.MoveKings, m_numTeammates);

            m_ann = new ActivationNetwork(
                new SigmoidFunction(),  // threshold function
                m_numFeatures + m_numActions,  
            (m_numFeatures + m_numActions) * 4, 1);

            //m_ann.SetLastLayerActivationFunction(new LinearFunction());

            m_learning = new BackPropagationLearning(m_ann);

            // TODO
            m_learning.LearningRate = Params.Alpha;
            //m_learning.Momentum = 0.01;

            //m_obsHist = new ObservationHistory(10);
        }

        protected override double GetQValue(State s, int ai)
        {
            double[] feats = ExtractFeatures(s);
            double[] inputVec = GetInputVector(feats, ai);
            double[] actValues = m_ann.Compute(inputVec);
            return actValues[0] * m_scalingFactor;
        }

        private double[] GetInputVector(double[] feats, int ai)
        {
            double[] inputVec = new double[feats.Length + m_numActions];
            Array.Copy(feats, 0, inputVec, 0, feats.Length);
            inputVec[ai + feats.Length] = 1.0;
            return inputVec;
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            double[] feats = ExtractFeatures(s);
            double[] inputVec = GetInputVector(feats, ai);
            double[] actValues = m_ann.Compute(inputVec);
            actValues[0] = newValue / m_scalingFactor;

            m_learning.Run(inputVec, actValues);
        }

    }
}
