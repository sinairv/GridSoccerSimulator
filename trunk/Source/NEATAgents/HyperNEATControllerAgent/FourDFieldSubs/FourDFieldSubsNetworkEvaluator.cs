using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GridSoccer.RLAgentsCommon;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;
using System.Threading;
using GridSoccer.NEATAgentsBase;
using SharpNeatLib.CPPNs;
using GridSoccer.Common;

namespace GridSoccer.HyperNEATControllerAgent.FourDFieldSubs
{
    public class FourDFieldSubsNetworkEvaluator : INetworkEvaluator
    {
        private ulong m_evalCount = 0;
        private NeatQTableBase[] m_neatQTables;

        private ISubstrate[] m_substratesBallOwner;
        private ISubstrate[] m_substratesNotBallOwner;

        public FourDFieldSubsNetworkEvaluator(NeatQTableBase[] neatQTables)
        {
            m_neatQTables = neatQTables;

            m_substratesBallOwner = new ISubstrate[m_neatQTables.Length];
            m_substratesNotBallOwner = new ISubstrate[m_neatQTables.Length];

            //Debug.Assert(m_neatQTable != null);
            // we put (cols + 2) columns in substrate to allaw players to go beyond the columns and score a goal
            for (int i = 0; i < m_neatQTables.Length; i++)
            {
                //Position homPos = GetHomePos(i + 1, m_neatQTables[i].Rows);


                float homex = ((float)m_neatQTables[i].NeatClient.MyHomePosCol / ((float)m_neatQTables[i].Cols * 0.5f));
                float homey = ((float)m_neatQTables[i].NeatClient.MyHomePosRow / (float)m_neatQTables[i].Rows);


                m_substratesBallOwner[i] = new FourDFieldSubstrate(homex, homey, m_neatQTables[i].Rows, m_neatQTables[i].Cols + 2,
                    NeatExpParams.AddBiasToSubstrate, HyperNEATParameters.substrateActivationFunction, 0);
                m_substratesNotBallOwner[i] = new FourDFieldSubstrate(homex, homey, m_neatQTables[i].Rows, m_neatQTables[i].Cols + 2,
                    NeatExpParams.AddBiasToSubstrate, HyperNEATParameters.substrateActivationFunction,
                    NeatExpParams.AddBiasToSubstrate ? 2 : 1);
            }
        }

        private object locker = new object();

        //private Position GetHomePos(int unum, int r)
        //{
        //    if (unum == 1)
        //        return new Position(2, 2);
        //    else if (unum == 2)
        //        return new Position(r - 1, 2);
        //    else if (unum == 3)
        //        return new Position(r / 2 + 1, 4);
        //    else 
        //        return new Position(0, 0);
        //}

        public double EvaluateNetwork(INetwork cppn)
        {
            // lazy loading
            //if (m_substratesBallOwner == null)
            //{
            //    m_substratesBallOwner = new ISubstrate[m_neatQTables.Length];
            //    m_substratesNotBallOwner = new ISubstrate[m_neatQTables.Length];

            //    //Debug.Assert(m_neatQTable != null);
            //    // we put (cols + 2) columns in substrate to allaw players to go beyond the columns and score a goal
            //    for (int i = 0; i < m_neatQTables.Length; i++)
            //    {
            //        //Position homPos = GetHomePos(i + 1, m_neatQTables[i].Rows);
                    

            //        float homex = ((float) m_neatQTables[i].NeatClient.MyHomePosCol / ((float)m_neatQTables[i].Cols * 0.5f));
            //        float homey = ((float)m_neatQTables[i].NeatClient.MyHomePosRow / (float)m_neatQTables[i].Rows);


            //        m_substratesBallOwner[i] = new FourDFieldSubstrate(homex, homey, m_neatQTables[i].Rows, m_neatQTables[i].Cols + 2,
            //            NeatExpParams.AddBiasToSubstrate, HyperNEATParameters.substrateActivationFunction, 0);
            //        m_substratesNotBallOwner[i] = new FourDFieldSubstrate(homex, homey, m_neatQTables[i].Rows, m_neatQTables[i].Cols + 2,
            //            NeatExpParams.AddBiasToSubstrate, HyperNEATParameters.substrateActivationFunction,
            //            NeatExpParams.AddBiasToSubstrate ? 2 : 1);
            //    }

            //} // end of lazy loading

            

            lock (locker)
            {
                m_evalCount++;

                for (int i = 0; i < m_neatQTables.Length; i++)
                {

                    SharpNeatLib.NeatGenome.NeatGenome tempGenomeBallOwner = m_substratesBallOwner[i].GenerateGenome(cppn);
                    INetwork tempNetBallOwner = tempGenomeBallOwner.Decode(HyperNEATParameters.substrateActivationFunction);

                    SharpNeatLib.NeatGenome.NeatGenome tempGenomeNotBallOwner = m_substratesNotBallOwner[i].GenerateGenome(cppn);
                    INetwork tempNetNotBallOwner = tempGenomeNotBallOwner.Decode(HyperNEATParameters.substrateActivationFunction);


                    m_neatQTables[i].NetworkToEvaluateBallOwner = tempNetBallOwner;
                    m_neatQTables[i].NetworkToEvaluateNotBallOwner = tempNetNotBallOwner;

                    m_neatQTables[i].m_eventNewNetReady.Set();
                }

                double[] fitnesses = new double[m_neatQTables.Length];

                for (int i = 0; i < m_neatQTables.Length; i++)
                {

                    m_neatQTables[i].m_eventNetFitnessReady.WaitOne();

                    double reward = m_neatQTables[i].PerformanceNetworkToEvaluate.Reward;

                    double maxTime = NeatExpParams.EpisodesPerGenome * 3 * (m_neatQTables[i].Rows + m_neatQTables[i].Cols);

                    double globalFitness = (m_neatQTables[i].PerformanceNetworkToEvaluate.GoalsScoredByTeam * NeatExpParams.GoalScoreEffect);
                    double localFitness = (m_neatQTables[i].PerformanceNetworkToEvaluate.GoalsScoredByMe * NeatExpParams.GoalScoreEffect);

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

                    if (m_neatQTables[i].PerformanceNetworkToEvaluate.GoalsScoredByTeam > 0)
                        fitness += (maxTime - m_neatQTables[i].PerformanceNetworkToEvaluate.PerformanceTime);
                    //- m_neatQTable.PerformanceNetworkToEvaluate.GoalsReceived * NeatExpParams.GoalRecvEffect

                    if (fitness < 0)
                        fitness = 0;

                    fitnesses[i] = fitness;

                    Console.WriteLine("[{0}] G: {1,-3} - Rew: {2,-10:F3} - Fitness: {3,-15:F3}",
                        m_neatQTables[i].NeatClient.MyUnum,
                        m_neatQTables[i].GenerationNo,
                        reward, fitness);
                }

                
                Console.WriteLine("These are all fitnesses:");
                foreach(var f in fitnesses)
                    Console.Write("{0} ", f);
                Console.WriteLine("::::::::::::::::::::::::::::::::");
                Console.WriteLine();

                return fitnesses[0];
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
