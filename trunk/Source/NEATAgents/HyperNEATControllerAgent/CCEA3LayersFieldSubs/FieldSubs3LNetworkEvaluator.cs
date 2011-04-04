using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.Experiments;
using GridSoccer.NEATAgentsBase;
using SharpNeatLib.CPPNs;
using SharpNeatLib.NeuralNetwork;
using System.Threading;

namespace GridSoccer.HyperNEATControllerAgent.CCEA3LayersFieldSubs
{
    public class FieldSubs3LNetworkEvaluator : INetworkEvaluator
    {
        private ulong m_evalCount = 0;
        private NeatQTableBase m_neatQTable;

        private ISubstrate m_substrate;

        public FieldSubs3LNetworkEvaluator(NeatQTableBase neatQTable)
        {
            m_neatQTable = neatQTable;
            //Debug.Assert(m_neatQTable != null);
            // we put (cols + 2) columns in substrate to allaw players to go beyond the columns and score a goal
            m_substrate = new MultiLayerSandwichSubstrate(neatQTable.Rows, neatQTable.Cols + 2, 
                NeatExpParams.SubstrateLayers, NeatExpParams.AddBiasToSubstrate, 
                HyperNEATParameters.substrateActivationFunction);
        }

        private object locker = new object();

        public double EvaluateNetwork(INetwork cppn)
        {
            lock (locker)
            {
                m_evalCount++;

                SharpNeatLib.NeatGenome.NeatGenome subsGenome = m_substrate.GenerateGenome(cppn);
                INetwork subsNet = subsGenome.Decode(HyperNEATParameters.substrateActivationFunction);

                m_neatQTable.NetworkToEvaluateBallOwner = subsNet;
                m_neatQTable.NetworkToEvaluateNotBallOwner = subsNet;

                m_neatQTable.m_eventNewNetReady.Set();
                m_neatQTable.m_eventNetFitnessReady.WaitOne();

                double reward = m_neatQTable.PerformanceNetworkToEvaluate.Reward;

                double maxTime = NeatExpParams.EpisodesPerGenome * 3 * (m_neatQTable.Rows + m_neatQTable.Cols);

                double globalFitness = (m_neatQTable.PerformanceNetworkToEvaluate.GoalsScoredByTeam * NeatExpParams.GoalScoreEffect);
                double localFitness = (m_neatQTable.PerformanceNetworkToEvaluate.GoalsScoredByMe * NeatExpParams.GoalScoreEffect);

                double fitness = globalFitness;

                switch (NeatExpParams.CreditAssignment)
                {
                    case CreditAssignments.Global:
                        fitness = globalFitness;
                        break;
                    case CreditAssignments.Local:
                        fitness = localFitness;
                        break;
                    case CreditAssignments.Mid:
                        fitness = (localFitness + globalFitness) * 0.5;
                        break;
                    default:
                        break;
                }

                if(m_neatQTable.PerformanceNetworkToEvaluate.GoalsScoredByTeam > 0)
                    fitness += (maxTime - m_neatQTable.PerformanceNetworkToEvaluate.PerformanceTime);
                    //- m_neatQTable.PerformanceNetworkToEvaluate.GoalsReceived * NeatExpParams.GoalRecvEffect

                if (fitness < 0)
                    fitness = 0;

                Console.WriteLine("G: {0,-3} - Rew: {1,-10:F3} - Fitness: {2,-15:F3}",
                    m_neatQTable.GenerationNo,
                    reward, fitness);

                return fitness;
            }
        }

        public double threadSafeEvaluateNetwork(INetwork network, Semaphore sem)
        {
            // No need to semaphore, the method already uses a lock
            //sem.WaitOne();
            double rtValue = EvaluateNetwork(network);
            //sem.Release();

            return rtValue;
        }

        public string EvaluatorStateMessage
        {
            get { return ""; }
        }

    }
}
