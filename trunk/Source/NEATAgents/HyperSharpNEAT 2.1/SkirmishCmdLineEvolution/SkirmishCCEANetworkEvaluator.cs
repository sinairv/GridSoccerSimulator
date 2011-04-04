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
    public class SkirmishCCEANetworkEvaluator : INetworkEvaluator
    {
        public SkirmishSubstrate m_substrate;
        private int m_agentId = -1;

        public SkirmishCCEANetworkEvaluator(int agentId)
        {
            m_agentId = agentId;
            m_substrate = new SkirmishSubstrate(5, 3, 5, HyperNEATParameters.substrateActivationFunction);
        }

        #region INetworkEvaluator Members
        object locker = new object();
        public double EvaluateNetwork(INetwork cppn)
        {
            lock (locker)
            {
                NeatGenome tempGenome = m_substrate.GenerateGenome(cppn);
                INetwork tempSubsNet = tempGenome.Decode(null);
                double fitness = 0.0;

                Program.TheWorldSimulator.NetworksToEvaluate[m_agentId] = tempSubsNet;
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
