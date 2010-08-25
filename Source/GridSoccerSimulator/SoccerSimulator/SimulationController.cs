using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Simulator.Properties;
using GridSoccer.Simulator.Net;
using System.Windows.Forms;
using GridSoccer.Simulator.Monitor;
using System.ComponentModel;

namespace GridSoccer.Simulator
{
    public class SimulationController
    {
        [Category("Game Settings")]
        [Description("The length of each time cycle in milli-seconds.")]
        [DisplayName("Game Duration")]
        public long GameDuration { get; set; }

        private bool m_waitForAllPlayers;
        [Category("Game Settings")]
        [Description("If set to true, the simulator waits for each connected player to send their command, before going to the next time-cycle. This prevents the players from loosing cycles (maybe needed for some experiments), but may cause the simulation to become less smooth.")]
        [DisplayName("Wait For All Players")]
        public bool WaitForAllPlayers 
        {
            get
            {
                return m_waitForAllPlayers;
            }

            set
            {
                m_waitForAllPlayers = value;
            }
        }
        
        private int m_cycleLength;

        [Category("Game Settings")]
        [Description("The totoal number of time cycles in a game.")]
        [DisplayName("Cycle Length")]
        public int CycleLength 
        {
            get
            {
                return m_cycleLength;
            }

            set
            {
                m_cycleLength = value;

                if (m_simulator.Cycle > 0)
                {
                    m_server.SendAllPlayers(String.Format("(cycle {0})", value));
                    this.m_timer.Interval = value;
                }
            }
        }

        private bool m_jetMode;
        private bool m_preWaitForAll;

        [Browsable(false)]
        public bool JetMode
        {
            get
            {
                return m_jetMode;
            }

            set
            {
                if (value)
                {
                    if (m_simulator.IsGameStarted)
                    {
                        m_timer.Enabled = false;
                        m_preWaitForAll = WaitForAllPlayers;
                        WaitForAllPlayers = true;
                        m_jetMode = true;
                        m_server.SendAllPlayers("(jet on)");
                        GoJet();
                    }
                }
                else
                {
                    if(m_jetMode)
                    {
                        m_jetMode = false;
                        WaitForAllPlayers = m_preWaitForAll;
                        m_server.SendAllPlayers("(jet off)");
                        m_timer.Enabled = true;
                    }
                }
            }
        }

        private int m_doEventInterval = 2000;
        [Category("Jet Mode Settings")]
        [Description("Sets the period in which Application.DoEvent() is called whenever in jet mode.")]
        [DisplayName("Do-Event Period")]
        public int DoEventInterval
        {
            get
            {
                return m_doEventInterval;
            }

            set
            {
                m_doEventInterval = value;
            }
        }

        [Browsable(false)]
        public bool IsGameStopped
        {
            get
            {
                return m_isGameStopped;
            }
        }

        [Browsable(false)]
        public bool IsGameStarted
        {
            get
            {
                return m_simulator.IsGameStarted;
            }
        }


        [Browsable(false)]
        public bool IsPaused
        {
            get
            {
                return m_isPaused;
            }
        }

        private SoccerSimulator m_simulator;
        private Server m_server;
        private Timer m_timer;

        private bool m_isGameStopped = false;

        public SimulationController(int randomSeed)
        {
            m_simulator = new SoccerSimulator((int)DateTime.Now.Ticks);
            m_server = new Server(m_simulator, Settings.Default.PortNumber);

            GameDuration = Settings.Default.NumCycles;
            CycleLength = Settings.Default.CycleDuration;
            WaitForAllPlayers = Settings.Default.WaitForAllPlayers;

            m_timer = new Timer();
            m_timer.Interval = 100;
            m_timer.Tick += new EventHandler(m_timer_Tick_accepting_connections);
            m_timer.Enabled = true;
        }

        public SimulationController()
            : this((int)DateTime.Now.Ticks)
        {
        }

        public event EventHandler GameCyclesFinished;

        public void BindMonitor(SoccerMonitor monitor)
        {
            monitor.SetSimulator(m_simulator);
        }

        public void UnbindMonitor(SoccerMonitor monitor)
        {
            monitor.UnbindSimulator(m_simulator);
        }

        void m_timer_Tick_accepting_connections(object sender, EventArgs e)
        {
            m_timer.Enabled = false;

            if (m_server.HasConnectionRequest())
                m_server.AcceptConnection();

            m_server.CheckTempClients();
            m_server.CheckConnectedClients();

            m_timer.Enabled = true;
        }

        public void Start()
        {
            if (m_simulator.Cycle > 0)
                return;

            m_timer.Enabled = false;
            m_timer.Tick -= new EventHandler(m_timer_Tick_accepting_connections);
            m_timer.Interval = this.CycleLength;
            m_timer.Tick += new EventHandler(m_timer_Tick_play_on);

            m_server.StopListeningForNewConnections();
            m_server.SendAllPlayers(String.Format("(cycle {0})", CycleLength));
            m_server.SendAllPlayers("(start)");
            m_simulator.BeginSimulation();
            m_server.SendSeeMessages();
            m_simulator.FinishCycle();

            System.Threading.Thread.Sleep(CycleLength);
            m_timer.Enabled = true;
        }

        public void Finish()
        {
            m_isGameStopped = true;
            m_timer.Enabled = false;
            m_server.SendAllPlayers("(stop)");
            m_simulator.EndSimulation();
        }

        void m_timer_Tick_play_on(object sender, EventArgs e)
        {
            if (m_isPaused) return;
            m_timer.Enabled = false;
            if (m_jetMode) return;

            GameStep();

            if (m_simulator.Cycle >= this.GameDuration)
            {
                Finish();
                if (GameCyclesFinished != null)
                    GameCyclesFinished(this, new EventArgs());
            }
            else if (!m_isGameStopped)
                m_timer.Enabled = true;
        }

        private void GameStep()
        {
            m_simulator.BeginUpdateActions();

            if (m_waitForAllPlayers)
                m_server.CheckConnectedClientsWaitingForAll();
            else
                m_server.CheckConnectedClients();

            m_simulator.FinishUpdateActions();
            m_server.SendSeeMessages();
            m_simulator.FinishCycle();
        }

        private void GoJet()
        {
            System.Threading.Thread.Sleep(1);
            while (m_jetMode)
            {
                GameStep();

                if (m_simulator.Cycle % m_doEventInterval == 0)
                    Application.DoEvents();

                if (m_simulator.Cycle >= this.GameDuration)
                {
                    Finish();
                    if (GameCyclesFinished != null)
                        GameCyclesFinished(this, new EventArgs());
                    break;
                }

                if (m_isGameStopped)
                    break;
            }

        }

        bool m_isPaused = false;
        public void TogglePauseResume()
        {
            m_isPaused = !m_isPaused;
            m_timer.Enabled = !m_isPaused;
        }

        public void RunOneStep()
        {
            m_timer.Enabled = false;
            m_isPaused = true;
            GameStep();
            if (m_simulator.Cycle >= this.GameDuration)
                Finish();
        }
    }
}
