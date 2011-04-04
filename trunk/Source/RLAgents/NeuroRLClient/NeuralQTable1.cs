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
    public class NeuralQTable1 : NeuralQTableBase
    {
        private int m_numActions;

        private ActivationNetwork[] m_anns;
        private BackPropagationLearning[] m_learnings;
        
        public NeuralQTable1(RLClientBase client, int numTeammates, int myUnum)
            : base(client, numTeammates, myUnum)
        {
            m_numActions = SoccerAction.GetActionCount(Params.MoveKings, m_numTeammates);

            m_anns = new ActivationNetwork[m_numActions];
            m_learnings = new BackPropagationLearning[m_numActions];

            for(int i = 0; i < m_anns.Length; i++)
            {
                m_anns[i] = new ActivationNetwork(
                    new SigmoidFunction(),  // threshold function
                    this.NumFeatures,  // 2 inputs
                this.NumFeatures * 2, 1);

                m_anns[i].SetLastLayerActivationFunction(new LinearFunction());

                m_learnings[i] = new BackPropagationLearning(m_anns[i]);

                m_learnings[i].LearningRate = Params.Alpha;
                m_learnings[i].Momentum = 0.0;
            }
        }

        protected override double GetQValue(State s, int ai)
        {
            double[] feats = ExtractFeatures(s);
            return m_anns[ai].Compute(feats)[0];
        }

        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            double[] feats = ExtractFeatures(s);
            m_learnings[ai].Run(feats, new double[] { newValue });
            //return m_anns[ai].Compute(feats)[0];
        }

    }
}
