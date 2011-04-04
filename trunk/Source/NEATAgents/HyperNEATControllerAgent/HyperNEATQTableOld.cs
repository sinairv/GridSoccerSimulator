using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;
using System.Threading;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;
using GridSoccer.NEATAgentsBase;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using System.Xml;
using System.IO;

namespace GridSoccer.HyperNEATControllerAgent
{
    public class HyperNEATQTable : QTableBase, INEATQTable
    {
        protected RLClientBase m_client;
        protected PerformanceLogger m_eaLogger;

        protected int m_numTeammates = 1;
        protected int m_myUnum = 1;
        private int m_rows = 0;
        private int m_cols = 0;

        private int m_numActions;

        private IExperiment m_exp = null;
        private EvolutionAlgorithm m_evoAlg = null;

        private object m_lockEvalNet = new object();

        private INetwork m_netToEval = null;
        private double m_netToEvalFitness = 0.0;


        private SharpNeatLib.NeatGenome.NeatGenome m_curBestGenome = null;
        private SharpNeatLib.NeatGenome.NeatGenome m_overalBestGenome = null;
        private double m_overalBestFitness = Double.MinValue;

        private bool m_shouldQuit = false;

        public AutoResetEvent m_eventNewNetReady = new AutoResetEvent(false);
        public AutoResetEvent m_eventNetFitnessReady = new AutoResetEvent(false);

        public HyperNEATQTable(RLClientBase client, int numTeammates, int myUnum)
        {
            m_client = client;
            m_rows = client.EnvRows;
            m_cols = client.EnvCols;

            m_numTeammates = numTeammates;
            m_myUnum = myUnum;
            m_numActions = SoccerAction.GetActionCount(Params.MoveKings, m_numTeammates);

            if (NeatExpParams.SaveFitnessGrowth)
            {
                m_eaLogger = new PerformanceLogger(String.Format("EALogs/{0}-{1}",
                    m_client.MyTeamName, m_myUnum), false);

                m_eaLogger.WriteLine("% generation  bestfitness");
            }

            Thread evoThread = new Thread(EvolutionaryThread);
            evoThread.Start();

            m_eventNewNetReady.WaitOne();
        }

        public SharpNeatLib.NeatGenome.NeatGenome BestGenomeOveral { get { return m_overalBestGenome; } }

        private void EvolutionaryThread()
        {
            m_exp = new HyperNEATPlayerExperiment(this, 4, 1);
            var idgen = new IdGenerator();
            m_evoAlg = new EvolutionAlgorithm(
                new Population(idgen,
                    GenomeFactory.CreateGenomeList(m_exp.DefaultNeatParameters, idgen, 
                        m_exp.InputNeuronCount, m_exp.OutputNeuronCount, 
                        m_exp.DefaultNeatParameters.pInitialPopulationInterconnections, 
                        NeatExpParams.PopulationSize)),
                    m_exp.PopulationEvaluator, m_exp.DefaultNeatParameters);

            while (!m_shouldQuit)
            {
                Console.WriteLine("::::: Performing one generation");
                Console.WriteLine();

                m_evoAlg.PerformOneGeneration();

                if (NeatExpParams.SaveFitnessGrowth)
                {
                    m_eaLogger.WriteLine(String.Format("{0,-10} {1,-20}",
                        m_evoAlg.Generation,
                        m_evoAlg.BestGenome.Fitness));
                }

                m_curBestGenome = m_evoAlg.BestGenome as NeatGenome;
                if (m_evoAlg.BestGenome.Fitness > m_overalBestFitness)
                {
                    m_overalBestFitness = m_evoAlg.BestGenome.Fitness;
                    m_overalBestGenome = m_curBestGenome;

                    if (NeatExpParams.SaveEachGenerationChampionCPPN)
                    {
                        var doc = new XmlDocument();
                        XmlGenomeWriterStatic.Write(doc, (NeatGenome)m_evoAlg.BestGenome);
                        var oFileInfo = new FileInfo(Path.Combine(
                            NeatExpParams.EALogDir, "BestCPPN" + m_evoAlg.Generation.ToString() + ".xml"));
                        doc.Save(oFileInfo.FullName);
                    }

                }

                if (EAUpdate != null)
                    EAUpdate.Invoke(this, EventArgs.Empty);
            }
        }

        public INetwork NetworkToEvaluate
        {
            get
            {
                return m_netToEval;
            }

            set
            {
                // TODO
                lock (m_lockEvalNet)
                {
                    m_netToEval = value;
                }
            }
        }

        public double FitnessNetworkToEvaluate
        {
            get
            {
                return m_netToEvalFitness;
            }

            set
            {
                // TODO
                m_netToEvalFitness = value;
            }
        }

        public int GoalDiffGainedByNetworkToEvaluate
        {
            get;
            set; 
        }

        /// <summary>
        /// i is 0-based, but r and c are 1-based
        /// </summary>
        private void SignalIndexToRowCol(int i, out int r, out int c)
        {
            r = (i / m_cols) + 1;
            c = (i % m_cols) + 1;
        }

        /// <summary>
        /// return value is 0-based, but r and c are 1-based
        /// </summary>
        private int RowColToSignalIndex(int r, int c)
        {
            return ((r - 1) * m_cols) + (c - 1);
        }

        private int m_lastEvalCycle = -1;
        private double[] m_lastQValues = null;
        protected override double GetQValue(State s, int ai)
        {
            if (this.m_client.Cycle > m_lastEvalCycle)
            {
                lock (m_lockEvalNet)
                {
                //    m_netToEval.ResetState();

                //    for (int i = 0, len = m_netToEval.InputSignalArray.Length; i < len; i++)
                //    {
                //        m_netToEval.InputSignalArray[i] = 0.0;
                //    }

                //    // add info of players to the input signal array
                //    for (int oi = 0, oicount = s.OppPlayersList.Count; oi < oicount; oi++)
                //    {
                //        var pos = s.OppPlayersList[oi].Position;
                //        int signali = RowColToSignalIndex(pos.Row, pos.Col);
                //        int signalval = 2;
                //        if (s.OppPlayersList[oi].IsBallOwner)
                //            signalval+=5;

                //        m_netToEval.InputSignalArray[signali] = signalval;
                //    }

                //    // add info for ours
                //    for (int ti = 0, ticount = s.OurPlayersList.Count; ti < ticount; ti++)
                //    {
                //        var pos = s.OurPlayersList[ti].Position;
                //        int signali = RowColToSignalIndex(pos.Row, pos.Col);
                //        int signalval = +1;
                //        if (s.OurPlayersList[ti].IsBallOwner)
                //            signalval += 5;

                //        m_netToEval.InputSignalArray[signali] = signalval;
                //    }

                //    // add info for me
                //    do // just create a block
                //    {
                //        var pos = s.Me.Position;
                //        int signali = RowColToSignalIndex(pos.Row, pos.Col);
                //        int signalval = 10;
                //        if (s.Me.IsBallOwner)
                //            signalval += 5;

                //        m_netToEval.InputSignalArray[signali] = signalval;
                        
                //    } while (false);

                //    //

                //    m_netToEval.Activate();

                //    if (!m_netToEval.IsStateValid)
                //    {
                //        return -1000000;
                //    }

                //    // See which position gets the maximum activation
                //    var outputs = m_netToEval.OutputSignalArray;
                //    double maxOutput = Double.MinValue;
                //    int maxOutIndex = -1;
                //    for (int osi = 0, oscount = outputs.Length; osi < oscount; osi++)
                //    {
                //        var curValue = outputs[osi];
                //        if (curValue > maxOutput)
                //        {
                //            maxOutput = curValue;
                //            maxOutIndex = osi;
                //        }
                //    }

                //    // Now interpret the position of max activation
                //    int maxr, maxc;
                //    SignalIndexToRowCol(maxOutIndex, out maxr, out maxc);

                //    m_lastQValues = new double[m_numActions];
                //    m_lastQValues[
                //        InterpretHighestSignal(s, maxr, maxc)
                //        ] = 1.0;
                } // end of lock

                m_lastEvalCycle = this.m_client.Cycle;

            } // end if 

            return m_lastQValues[ai];
        }

        private int InterpretHighestSignal(State s, int maxr, int maxc)
        {
            var myPos = s.Me.Position;
            int rDiff = maxr - myPos.Row;
            int cDiff = maxc - myPos.Col;

            if (rDiff == 0 && cDiff == 0)
            {
                return SoccerAction.GetIndexFromAction(new SoccerAction(ActionTypes.Hold), Params.MoveKings, this.MyUnum);
            }
            else if (rDiff == 0 || (Math.Abs(rDiff) <= Math.Abs(cDiff)))
            {
                return SoccerAction.GetIndexFromAction(new SoccerAction(
                    cDiff > 0 ? ActionTypes.MoveEast : ActionTypes.MoveWest
                    ), Params.MoveKings, this.MyUnum);
            }
            else if (cDiff == 0 || (Math.Abs(rDiff) > Math.Abs(cDiff)))
            {
                return SoccerAction.GetIndexFromAction(new SoccerAction(
                    rDiff > 0 ? ActionTypes.MoveSouth : ActionTypes.MoveNorth
                    ), Params.MoveKings, this.MyUnum);
            }

            return 0;
        }

        double m_genomeSumRewards = 0.0;
        private int m_prevGeneration = 0;
        private int m_prevGoalDiff = 0;
        protected override void UpdateQValue(State s, int ai, double newValue)
        {
            double reward = EnvironmentModeler.GetReward(base.m_prevState, base.m_curState, 
                SoccerAction.GetActionTypeFromIndex(ai, Params.MoveKings));

            double dummy;
            int prevStateGreedyAct = GetMaxQ(base.m_prevState, out dummy);

            if(prevStateGreedyAct == ai) // add reward to fitness only if it was a greedy action
                m_genomeSumRewards += reward;

            if (this.m_client.Cycle % NeatExpParams.CyclesPerGenome == 0)
            {
                Console.Write("Gener: {0}, ", m_evoAlg != null ? m_evoAlg.Generation : 0);

                int curGoalDiff = m_client.OurScore - m_client.OppScore;

                this.GoalDiffGainedByNetworkToEvaluate = curGoalDiff - m_prevGoalDiff;
                this.FitnessNetworkToEvaluate = m_genomeSumRewards;

                m_prevGoalDiff = curGoalDiff;
                m_eventNetFitnessReady.Set();

                m_genomeSumRewards = 0.0;

                m_eventNewNetReady.WaitOne();
            }
        }

        //private double GetMaxQValue(State s)
        //{
        //    double maxQvalue;
        //    base.GetMaxQ(s, out maxQvalue);
        //    return maxQvalue;
        //}

        protected override int MyUnum
        {
            get { return m_myUnum; }
        }

        protected override int TeammatesCount
        {
            get { return m_numTeammates; }
        }

        public override void Save(System.IO.TextWriter tw)
        {
            throw new NotImplementedException();
        }

        public override void Load(System.IO.TextReader tr)
        {
            throw new NotImplementedException();
        }

        public override Array QTableArray
        {
            get { throw new NotImplementedException(); }
        }

        public RLClientBase NeatClient
        {
            get { return m_client; }
        }

        public event EventHandler EAUpdate;

        ~HyperNEATQTable()
        {
            m_shouldQuit = true;
        }
    }
}
