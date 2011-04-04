using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;
using System.Threading;
using SharpNeatLib.NeatGenome;
using SharpNeat.Experiments;

namespace SharpNeatExperiments.Skirmish
{
    public class SkirmishNeatCCEANetworkEvaluator : INetworkEvaluator
    {
        //public SkirmishSubstrate m_substrate;
        private int m_agentId = -1;

        public SkirmishNeatCCEANetworkEvaluator(int agentId)
        {
            m_agentId = agentId;
        }

        #region INetworkEvaluator Members
        object locker = new object();
        public double EvaluateNetwork(INetwork network)
        {
            lock (locker)
            {
                double fitness = 0.0;

                Program.TheWorldSimulator.NetworksToEvaluate[m_agentId] = network;
                Program.TheWorldSimulator.EventsNetworkReady[m_agentId].Set();
                Program.TheWorldSimulator.EventsFitnessReady[m_agentId].WaitOne();

                fitness = Program.TheWorldSimulator.Fitness;

                //if (!SkirmishExperiment.multiple)
                //    fitness += doEvaluation(tempNet);
                //else
                //    fitness += doEvaluationMulti(tempNet);
                return fitness;
            }
        }

        public double threadSafeEvaluateNetwork(INetwork network, Semaphore sem)
        {
            return EvaluateNetwork(network);
        }


        public string EvaluatorStateMessage
        {
            get { return ""; }
        }

        #endregion

    }
}
