using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.Experiments;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatExperiments.Skirmish
{
    public class SkirmishNeatCCEAExperiment : IExperiment
    {
        int inputs;
        int outputs;
        IPopulationEvaluator populationEvaluator = null;
        NeatParameters neatParams = null;
        int m_popSize = 150;
        int m_agentId = -1;

        public SkirmishNeatCCEAExperiment(int agentId, int popSize)
        {
            // TODO: change here for various experiments
            m_agentId = agentId;
            inputs = 5;
            outputs = 3;
            m_popSize = popSize;
        }

        #region IExperiment Members

        public void LoadExperimentParameters(System.Collections.Hashtable parameterTable)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public IPopulationEvaluator PopulationEvaluator
        {
            get
            {
                if (populationEvaluator == null)
                    ResetEvaluator(HyperNEATParameters.substrateActivationFunction);

                return populationEvaluator;
            }
        }

        public void ResetEvaluator(IActivationFunction activationFn)
        {
            populationEvaluator = new EveryonePopulationEvaluator(
                new SkirmishNeatCCEANetworkEvaluator(m_agentId));
            //populationEvaluator = new SkirmishPopulationEvaluator(new SkirmishNetworkEvaluator(1, shape));
        }

        public int InputNeuronCount
        {
            get { return inputs; }
        }

        public int OutputNeuronCount
        {
            get { return outputs; }
        }

        public NeatParameters DefaultNeatParameters
        {
            get
            {
                if (neatParams == null)
                {
                    NeatParameters np = new NeatParameters();
                    np.connectionWeightRange = 3;
                    np.pMutateAddConnection = .03;
                    np.pMutateAddNode = .01;
                    np.pMutateConnectionWeights =  .96;
                    np.pMutateDeleteConnection = 0;
                    np.pMutateDeleteSimpleNeuron = 0;
                    np.populationSize = m_popSize;
                    np.pruningPhaseBeginComplexityThreshold = float.MaxValue;
                    np.pruningPhaseBeginFitnessStagnationThreshold = int.MaxValue;
                    np.pruningPhaseEndComplexityStagnationThreshold = int.MinValue;
                    np.pInitialPopulationInterconnections = 1;
                    np.elitismProportion = .1;
                    np.targetSpeciesCountMax = np.populationSize / 10;
                    np.targetSpeciesCountMin = np.populationSize / 10 - 2;
                    np.selectionProportion = .8;
                    neatParams = np;
                }
                return neatParams;
            }
        }

        public IActivationFunction SuggestedActivationFunction
        {
            get { return HyperNEATParameters.substrateActivationFunction; }
        }

        public AbstractExperimentView CreateExperimentView()
        {
            return null;
        }

        public string ExplanatoryText
        {
            get { return "A NEAT experiemnt for multiagent predator-prey"; }
        }

        #endregion

    }
}
