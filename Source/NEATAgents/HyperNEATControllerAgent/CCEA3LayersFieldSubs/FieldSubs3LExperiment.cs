using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.Experiments;
using SharpNeatLib.Evolution;
using GridSoccer.NEATAgentsBase;
using System.Collections;
using SharpNeatLib.NeuralNetwork;

namespace GridSoccer.HyperNEATControllerAgent.CCEA3LayersFieldSubs
{
    public class FieldSubs3LExperiment : IExperiment
    {
        IPopulationEvaluator populationEvaluator = null;
        NeatParameters neatParams = null;

        private NeatQTableBase m_neatQTable = null;
        private int m_fieldWidth = 1;
        private int m_fieldHeight = 1;

        private int m_numCPPNInputs = -1;
        private int m_numCPPNOutputs = -1;

        public FieldSubs3LExperiment(NeatQTableBase qTable, int numCPPNInputs, int numCPPNOutputs)
        {
            //Debug.Assert(qTable != null);
            m_neatQTable = qTable;

            m_numCPPNInputs = numCPPNInputs;
            m_numCPPNOutputs = numCPPNOutputs;

            m_fieldWidth = qTable.NeatClient.EnvCols;
            m_fieldHeight = qTable.NeatClient.EnvRows;
        }

        public void LoadExperimentParameters(Hashtable parameterTable)
        {
            // Do nothing
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
            populationEvaluator = new
                EveryonePopulationEvaluator(new FieldSubs3LNetworkEvaluator(m_neatQTable));
        }

        public int InputNeuronCount
        {
            get { return m_numCPPNInputs; }
        }

        public int OutputNeuronCount
        {
            get { return m_numCPPNOutputs; }
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
                    np.pMutateConnectionWeights = .96;
                    if (!NeatExpParams.DoSimplifyingPhase)
                    {
                        np.pMutateDeleteConnection = 0;
                        np.pMutateDeleteSimpleNeuron = 0;
                    }
                    np.activationProbabilities = new double[4];
                    np.activationProbabilities[0] = .25;
                    np.activationProbabilities[1] = .25;
                    np.activationProbabilities[2] = .25;
                    np.activationProbabilities[3] = .25;
                    np.populationSize = NeatExpParams.PopulationSize;

                    if (!NeatExpParams.DoSimplifyingPhase)
                    {
                        np.pruningPhaseBeginComplexityThreshold = float.MaxValue;
                        np.pruningPhaseBeginFitnessStagnationThreshold = int.MaxValue;
                        np.pruningPhaseEndComplexityStagnationThreshold = int.MinValue;
                    }

                    np.pInitialPopulationInterconnections = 1;
                    np.elitismProportion = .2;
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
            get { return "HyperNAET Grid-Soccer Player!"; }
        }
    }
}
