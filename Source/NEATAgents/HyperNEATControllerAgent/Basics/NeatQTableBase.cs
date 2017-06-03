using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.RLAgentsCommon;
using SharpNeatLib.Experiments;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using System.Threading;
using GridSoccer.Common;
using SharpNeatLib.NeatGenome;
using System.Diagnostics;
using System.Xml;
using SharpNeatLib.NeatGenome.Xml;
using System.IO;

namespace GridSoccer.NEATAgentsBase
{
    public abstract class NeatQTableBase : QTableBase
    {
        protected RLClientBase m_client;
        protected PerformanceLogger m_eaLogger;

        protected int m_numTeammates = 1;
        protected int m_myUnum = 1;
        protected int m_rows = 0;
        protected int m_cols = 0;

        protected int m_numActions;

        protected IExperiment m_exp = null;
        protected EvolutionAlgorithm m_evoAlg = null;

        protected object m_lockEvalNet = new object();

        protected INetwork m_netToEvalBallOwner = null;
        protected INetwork m_netToEvalNotBallOwner = null;

        protected SharpNeatLib.NeatGenome.NeatGenome m_curBestGenome = null;
        protected SharpNeatLib.NeatGenome.NeatGenome m_overalBestGenome = null;
        protected double m_overalBestFitness = Double.MinValue;

        protected int m_lastEvalCycle = -1;
        protected double[] m_lastQValues = null;

        protected bool m_shouldQuit = false;

        public AutoResetEvent m_eventNewNetReady = new AutoResetEvent(false);
        public AutoResetEvent m_eventNetFitnessReady = new AutoResetEvent(false);

        protected bool m_isCentralized = false;

        //public NeatQTableBase(RLClientBase client, int numTeammates, int myUnum)
        //    : this(client, numTeammates, myUnum, false)
        //{
        //}

        public NeatQTableBase(RLClientBase client, int numTeammates, int myUnum, bool isCentralized)
        {
            m_isCentralized = isCentralized;

            m_client = client;
            m_rows = client.EnvRows;
            m_cols = client.EnvCols;

            m_numTeammates = numTeammates;
            m_myUnum = myUnum;
            m_numActions = SoccerAction.GetActionCount(Params.MoveKings, m_numTeammates);

            this.PerformanceNetworkToEvaluate = new NeatPlayerPerformanceStats();

            if ((!isCentralized) || (isCentralized && myUnum == 1))
            {
                if (NeatExpParams.SaveFitnessGrowth)
                {
                    m_eaLogger = new PerformanceLogger(String.Format("EALogs/{0}-{1}-{2}",
                        m_client.MyTeamName, m_myUnum, m_client.PerformanceLoggerMethodName), false);

                    m_eaLogger.WriteLine("% Generation  BestFitness MeanFitness AvgComplexity");
                }
            }

            Thread evoThread = new Thread(EvolutionaryThread);
            evoThread.Start();
            
            m_eventNewNetReady.WaitOne();
            
        }

        public SharpNeatLib.NeatGenome.NeatGenome BestGenomeOveral { get { return m_overalBestGenome; } }

        public INetwork NetworkToEvaluateBallOwner
        {
            get
            {
                return m_netToEvalBallOwner;
            }

            set
            {
                // TODO
                lock (m_lockEvalNet)
                {
                    m_netToEvalBallOwner = value;
                }
            }
        }

        public INetwork NetworkToEvaluateNotBallOwner
        {
            get
            {
                return m_netToEvalNotBallOwner;
            }

            set
            {
                // TODO
                lock (m_lockEvalNet)
                {
                    m_netToEvalNotBallOwner = value;
                }
            }
        }

        public NeatPlayerPerformanceStats PerformanceNetworkToEvaluate { get; set; }

        public int Rows
        {
            get { return m_rows; }
        }

        public int Cols
        {
            get { return m_cols; }
        }

        public uint GenerationNo
        {
            get
            {
                return m_evoAlg != null ? m_evoAlg.Generation + 1 : 0;
            }
        }

        public abstract IExperiment CreateExperiment();

        public abstract double[] GetQValuesForNetwork(INetwork networkBallOwner, INetwork networkNotBallOwner, State s);

        private void EvolutionaryThread()
        {
            m_exp = CreateExperiment();

            // only CCEA can continue or Team Learning for agent no. 1 only
            if (m_isCentralized && MyUnum != 1)
            {
                return;
            }

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
                    m_eaLogger.WriteLine(String.Format("{0,-10} {1,-20} {2,-20} {3,-20}",
                        m_evoAlg.Generation,
                        m_evoAlg.BestGenome.Fitness,
                        m_evoAlg.Population.MeanFitness, m_evoAlg.Population.AvgComplexity));
                }

                m_curBestGenome = m_evoAlg.BestGenome as NeatGenome;
                if (m_evoAlg.BestGenome.Fitness > m_overalBestFitness)
                {
                    m_overalBestFitness = m_evoAlg.BestGenome.Fitness;
                    m_overalBestGenome = m_curBestGenome;

                    if (NeatExpParams.SaveEachGenerationChampionCPPN)
                    {
                        try
                        {
                            var doc = new XmlDocument();
                            XmlGenomeWriterStatic.Write(doc, (NeatGenome)m_evoAlg.BestGenome);
                            var oFileInfo = new FileInfo(Path.Combine(
                                NeatExpParams.EALogDir, String.Format("BestIndividual-{0}-{1}.xml", MyUnum, m_evoAlg.Generation.ToString())));
                            doc.Save(oFileInfo.FullName);
                        }
                        catch
                        {
                        }
                    }

                }

                if (EAUpdate != null)
                    EAUpdate.Invoke(this, EventArgs.Empty);
            }
        }

        protected override double GetQValue(State s, int ai)
        {
            if (this.m_client.Cycle > m_lastEvalCycle)
            {
                lock (m_lockEvalNet)
                {
                    m_lastQValues = GetQValuesForNetwork(m_netToEvalBallOwner, m_netToEvalNotBallOwner, s);
                    Debug.Assert(m_lastQValues != null);
                    Debug.Assert(m_lastQValues.Length == m_numActions);
                } // end of lock

                m_lastEvalCycle = this.m_client.Cycle;

            } // end if 

            return m_lastQValues[ai];
        }

        private int lastEpisodeProcessed = 0;
        protected override void UpdateQValue(State s, int ai, double reward)
        {
            this.PerformanceNetworkToEvaluate.PerformanceTime++;
            this.PerformanceNetworkToEvaluate.Reward += reward;
            if (m_curState.OurScore > m_prevState.OurScore)
            {
                if (m_prevState.AreWeBallOwner) // not an opponent's own goal
                    this.PerformanceNetworkToEvaluate.GoalsScoredByTeam++;

                if(m_prevState.AmIBallOwner)
                    this.PerformanceNetworkToEvaluate.GoalsScoredByMe++;
            }

            if (m_curState.OppScore > m_prevState.OppScore)
            {
                this.PerformanceNetworkToEvaluate.GoalsReceived++;
            }

            int episodeNumber = this.m_client.OurScore + this.m_client.OppScore;
            //if (this.m_client.Cycle % NeatExpParams.CyclesPerGenome == 0)
            if (episodeNumber != 0 && episodeNumber % NeatExpParams.EpisodesPerGenome == 0)
            {
                if (episodeNumber != lastEpisodeProcessed)
                {
                    lastEpisodeProcessed = episodeNumber;

                    m_eventNetFitnessReady.Set();

                    m_eventNewNetReady.WaitOne();

                    this.PerformanceNetworkToEvaluate.Reset();
                }
            }
        }

        public RLClientBase NeatClient
        {
            get { return m_client; }
        }

        public event EventHandler EAUpdate;

        
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

        ~NeatQTableBase()
        {
            m_shouldQuit = true;
        }
    }
}
