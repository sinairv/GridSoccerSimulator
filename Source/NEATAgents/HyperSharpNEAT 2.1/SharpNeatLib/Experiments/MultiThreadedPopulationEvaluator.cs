using System;
using System.Threading;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.Experiments;

namespace SharpNeatLib.Experiments
{
    /// <summary>
    /// An implementation of IPopulationEvaluator that evaluates all new genomes(EvaluationCount==0)
    /// within the population using multiple threads, using an INetworkEvaluator provided at construction time.
    /// 
    /// This class provides an IPopulationEvaluator for use within the EvolutionAlgorithm by simply
    /// providing an INetworkEvaluator to its constructor. This usage is intended for experiments
    /// where the genomes are evaluated independently of each other (e.g. not simultaneoulsy in 
    /// a simulated world) using a fixed evaluation function that can be described by an INetworkEvaluator.
    /// </summary>
    public class MultiThreadedPopulationEvaluator : IPopulationEvaluator
    {
        INetworkEvaluator networkEvaluator;
        IActivationFunction activationFn;
        private static Semaphore sem = new Semaphore(HyperNEATParameters.numThreads, HyperNEATParameters.numThreads);
        private static Semaphore sem2 = new Semaphore(1, 1);
        
        ulong evaluationCount = 0;

        #region Constructor

        public MultiThreadedPopulationEvaluator(INetworkEvaluator networkEvaluator, IActivationFunction activationFn)
        {
            this.networkEvaluator = networkEvaluator;
            this.activationFn = activationFn;

        }

        #endregion

        #region IPopulationEvaluator Members

        public void EvaluatePopulation(Population pop, EvolutionAlgorithm ea)
        {
            int count = pop.GenomeList.Count;
       
            evalPack e;
            IGenome g;
            int i;

            for (i = 0; i < count; i++)
            {


                sem.WaitOne();
                g = pop.GenomeList[i];
                e = new evalPack(networkEvaluator, activationFn, g);

                ThreadPool.QueueUserWorkItem(new WaitCallback(evalNet), e);
                // Update master evaluation counter.
                evaluationCount++;

            }

            for (int j = 0; j < HyperNEATParameters.numThreads; j++)
            {
                sem.WaitOne();
            }
            for (int j = 0; j < HyperNEATParameters.numThreads; j++)
            {
                sem.Release();
            }


        }



        public ulong EvaluationCount
        {
            get
            {
                return evaluationCount;
            }
        }

        public string EvaluatorStateMessage
        {
            get
            {	// Pass on the network evaluator's message.
                return networkEvaluator.EvaluatorStateMessage;
            }
        }

        public bool BestIsIntermediateChampion
        {
            get
            {	// Only relevant to incremental evolution experiments.
                return false;
            }
        }

        public bool SearchCompleted
        {
            get
            {	// This flag is not yet supported in the main search algorithm.
                return false;
            }
        }

        public static void evalNet(Object input)
        {

            evalPack e = (evalPack)input;

            if (e.g == null )//|| e.g.EvaluationCount != 0)
            {
                sem.Release();
                return;
            }
            sem2.WaitOne();
            INetwork network = e.g.Decode(e.Activation);
            sem2.Release();
            if (network == null)
            {	// Future genomes may not decode - handle the possibility.
                e.g.Fitness = EvolutionAlgorithm.MIN_GENOME_FITNESS;
            }
            else
            {
                e.g.Fitness = Math.Max(e.NetworkEvaluator.threadSafeEvaluateNetwork(network,sem2), EvolutionAlgorithm.MIN_GENOME_FITNESS);
            }

            // Reset these genome level statistics.
            e.g.TotalFitness += e.g.Fitness;
            e.g.EvaluationCount += 1;
            sem.Release();

        }

        #endregion
    }

    class evalPack
    {

        INetworkEvaluator networkEvaluator;
        IActivationFunction activationFn;
        IGenome genome;

        public evalPack(INetworkEvaluator n, IActivationFunction a, IGenome g)
        {

            networkEvaluator = n;
            activationFn = a;
            genome = g;

        }

        public INetworkEvaluator NetworkEvaluator
        {
            get
            {
                return networkEvaluator;
            }
        }

        public IActivationFunction Activation
        {
            get
            {
                return activationFn;
            }
        }

        public IGenome g
        {
            get
            {
                return genome;
            }
        }

    }
}
