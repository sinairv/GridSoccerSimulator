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
        #region Private Fields
        private bool m_waitForAllPlayers;

        private int m_cycleLength;


        private bool m_turboMode;
        private bool m_preWaitForAll;

        private int m_doEventInterval = 2000;

        private SoccerSimulator m_simulator;
        private Server m_server;
        private Timer m_timer;

        private bool m_isGameStopped = false;

        bool m_isPaused = false;

        #endregion 

        #region Public Properties

        [Category("Game Settings")]
        [Description("The length of each time cycle in milli-seconds.")]
        [DisplayName("Game Duration")]
        public long GameDuration { get; set; }

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

        [Browsable(false)]
        public bool TurboMode
        {
            get
            {
                return m_turboMode;
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
                        m_turboMode = true;
                        m_server.SendAllPlayers("(turbo on)");
                        EnableTurbo();
                    }
                }
                else
                {
                    if(m_turboMode)
                    {
                        m_turboMode = false;
                        WaitForAllPlayers = m_preWaitForAll;
                        m_server.SendAllPlayers("(turbo off)");
                        m_timer.Enabled = true;
                    }
                }
            }
        }

        [Category("Turbo Mode Settings")]
        [Description("Sets the period in which Application.DoEvent() is called whenever in Turbo mode.")]
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

        #endregion 

        #region Constructors
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

        #endregion 

        #region events
        public event EventHandler GameCyclesFinished;

        #endregion

        #region Public Methods

        public void BindMonitor(SoccerMonitor monitor)
        {
            monitor.SetSimulator(m_simulator);
        }

        public void UnbindMonitor(SoccerMonitor monitor)
        {
            monitor.UnbindSimulator(m_simulator);
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

        public void Stop()
        {
            m_isGameStopped = true;
            m_timer.Enabled = false;
            m_server.SendAllPlayers("(stop)");
            m_simulator.EndSimulation();
        }

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
                Stop();
        }

        #endregion

        #region Private Methods

        private void m_timer_Tick_accepting_connections(object sender, EventArgs e)
        {
            m_timer.Enabled = false;

            if (m_server.HasConnectionRequest())
                m_server.AcceptConnection();

            m_server.CheckTempClients();
            m_server.CheckConnectedClients();

            m_timer.Enabled = true;
        }

        private void m_timer_Tick_play_on(object sender, EventArgs e)
        {
            if (m_isPaused) return;
            m_timer.Enabled = false;
            if (m_turboMode) return;

            GameStep();

            if (m_simulator.Cycle >= this.GameDuration)
            {
                Stop();
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

        private void EnableTurbo()
        {
            System.Threading.Thread.Sleep(1);
            while (m_turboMode)
            {
                GameStep();

                if (m_simulator.Cycle % m_doEventInterval == 0)
                    Application.DoEvents();

                if (m_simulator.Cycle >= this.GameDuration)
                {
                    Stop();
                    if (GameCyclesFinished != null)
                        GameCyclesFinished(this, new EventArgs());
                    break;
                }

                if (m_isGameStopped)
                    break;
            }

        }

        #endregion
    }
}
