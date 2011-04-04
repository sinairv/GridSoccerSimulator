using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using SharpNeatLib.Experiments;
using System.Collections;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using GridSoccer.NEATAgentsBase;
using System.Diagnostics;
using System.Threading;

namespace GridSoccer.HyperNEATControllerAgent.FourDFieldSubs
{
    public class FourDFieldSubsExperiment : IExperiment
    {
        private static FourDFieldSubsExperiment s_singleInstance = null;
        private static int numAgentsConnected = 0;
        private static object agentConnectionLock = new object();

        public static FourDFieldSubsExperiment GetExperiment(NeatQTableBase qTable, int agentId)
        {
            lock (agentConnectionLock)
            {
                if (s_singleInstance == null)
                {
                    s_singleInstance = new FourDFieldSubsExperiment(8,
                        NeatExpParams.AddBiasToSubstrate ? 4 : 2);
                }

                s_singleInstance.AddAgent(qTable, agentId);
            }
            return s_singleInstance;
        }

        private AutoResetEvent m_eventAllAgentsRegistered = new AutoResetEvent(false);

        private IPopulationEvaluator populationEvaluator = null;
        private NeatParameters neatParams = null;

        private NeatQTableBase[] m_neatQTables = null;
        private int m_fieldWidth = 1;
        private int m_fieldHeight = 1;

        private int m_numCPPNInputs = -1;
        private int m_numCPPNOutputs = -1;

        private FourDFieldSubsExperiment(int numCPPNInputs, int numCPPNOutputs)
        {
            m_numCPPNInputs = numCPPNInputs;
            m_numCPPNOutputs = numCPPNOutputs;
        }

        private void AddAgent(NeatQTableBase qTable, int agentId)
        {
            if (m_neatQTables == null)
                m_neatQTables = new NeatQTableBase[Program.Teammates];

            //Debug.Assert(qTable != null);
            m_neatQTables[agentId] = qTable;

            m_fieldWidth = qTable.NeatClient.EnvCols;
            m_fieldHeight = qTable.NeatClient.EnvRows;

            numAgentsConnected++;
            if (numAgentsConnected == Program.Teammates)
            {
                s_singleInstance.m_eventAllAgentsRegistered.Set();
            }
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
            m_eventAllAgentsRegistered.WaitOne();

            populationEvaluator = new
                EveryonePopulationEvaluator(new FourDFieldSubsNetworkEvaluator(m_neatQTables));
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
