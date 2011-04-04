// Copyright (c) 2009 - 2011 
//  - Sina Iravanian <sina@sinairv.com>
//  - Sahar Araghi   <sahar_araghi@aut.ac.ir>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Simulator.Properties;
using GridSoccer.Simulator.Net;
using GridSoccer.Simulator.Monitor;
using System.Threading;

namespace GridSoccer.Simulator
{
    public class SimulationController
    {
        #region Internal Types
        private enum ControllerActionTypes
        {
            Start, Stop, Step, BindMonitor, UnbindMonitor, TogglePauseResume,
            EnableTurbo, DisableTurbo
        }
        #endregion

        #region Private Fields
        
        private bool m_waitForAllPlayers;

        private int m_cycleLength;


        private bool m_turboMode;
        private bool m_preWaitForAll;

        private SoccerSimulator m_simulator;
        private Server m_server;
        private readonly SoccerMonitor m_mainMonitor = null;
        //private Timer m_timer;

        private bool m_isGameStopped = false;

        bool m_isPaused = false;

        private Queue<KeyValuePair<ControllerActionTypes, object>> m_queueActions =
            new Queue<KeyValuePair<ControllerActionTypes, object>>();


        private AutoResetEvent m_eventInitialized = new AutoResetEvent(false);
        private Thread m_simThread;

        #endregion 

        #region Public Properties

        public long GameDuration { get; set; }

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

        public int CycleLength 
        {
            get
            {
                return m_cycleLength;
            }

            set
            {
                if (value > 0)
                {
                    if(m_mainMonitor != null)
                    {
                        m_mainMonitor.IntervalUpdateUI = value;
                    }
                    m_cycleLength = value;
                    m_server.SendAllPlayers(String.Format("(cycle {0})", value));

                }
            }
        }

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
                    EnqueAction(ControllerActionTypes.EnableTurbo, null);
                }
                else
                {
                    EnqueAction(ControllerActionTypes.DisableTurbo, null);
                }
            }
        }

        public bool IsGameStopped
        {
            get
            {
                return m_isGameStopped;
            }
        }

        public bool IsGameStarted
        {
            get
            {
                return m_simulator.IsGameStarted;
            }
        }


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
            m_simThread = new Thread(ThreadCallBack);
            m_simThread.Name = "SimThread"; // good for debugging purposes
            m_simThread.Start();

            // do not return until initialization is finished
            m_eventInitialized.WaitOne();
        }

        public SimulationController()
            : this((int)DateTime.Now.Ticks)
        {
        }

        #endregion

        #region Thread Related Methods

        private void ThreadCallBack()
        {
            m_simulator = new SoccerSimulator((int)DateTime.Now.Ticks);
            m_server = new Server(m_simulator, Settings.Default.PortNumber);

            GameDuration = Settings.Default.NumCycles;
            CycleLength = Settings.Default.CycleDuration;
            WaitForAllPlayers = Settings.Default.WaitForAllPlayers;

            m_eventInitialized.Set();

            while (!m_isGameStopped)
            {
                // Process the queue of all actions
                lock (m_queueActions)
                {
                    while (m_queueActions.Count > 0)
                    {
                        var pair = m_queueActions.Dequeue();
                        switch (pair.Key)
                        {
                            case ControllerActionTypes.Start:
                                ActionStart();
                                break;
                            case ControllerActionTypes.Stop:
                                ActionStop();
                                break;
                            case ControllerActionTypes.Step:
                                ActionStep();
                                break;
                            case ControllerActionTypes.TogglePauseResume:
                                ActionTogglePauseResume();
                                break;
                            case ControllerActionTypes.BindMonitor:
                                ActionBindMonitor(pair.Value);
                                break;
                            case ControllerActionTypes.UnbindMonitor:
                                ActionUnbindMonitor(pair.Value);
                                break;
                            case ControllerActionTypes.EnableTurbo:
                                ActionEnableTurbo();
                                break;
                            case ControllerActionTypes.DisableTurbo:
                                ActionDisableTurbo();
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (IsGameStarted)
                {
                    PlayOnStep();
                }
                else
                {
                    AcceptConnectionStep();
                }

                if (!m_turboMode)
                {
                    Thread.Sleep(m_cycleLength);
                }
                //else
                //{
                //    Thread.Sleep(1);
                //}
            }
        }

        private void EnqueAction(ControllerActionTypes action, object args)
        {
            lock (m_queueActions)
            {
                m_queueActions.Enqueue(new KeyValuePair<ControllerActionTypes, object>(action, args));
            }
        }

        #endregion 

        #region events
        public event EventHandler GameCyclesFinished;

        #endregion

        #region Action... Methods

        private void ActionBindMonitor(object arg)
        {
            var monitor = arg as SoccerMonitor;
            if (monitor != null)
            {
                monitor.IntervalUpdateUI = this.CycleLength;
                monitor.BindToSimulator(m_simulator);
            }
        }

        private void ActionUnbindMonitor(object arg)
        {
            var monitor = arg as SoccerMonitor;
            if (monitor != null)
            {
                monitor.UnbindFromSimulator(m_simulator);
            }
        }

        private void ActionStart()
        {
            if (m_simulator.Cycle > 0)
                return;

            m_server.StopListeningForNewConnections();
            m_server.SendAllPlayers(String.Format("(cycle {0})", CycleLength));
            m_server.SendAllPlayers("(start)");
            m_simulator.BeginSimulation();
            m_server.SendSeeMessages();
            m_simulator.FinishCycle();

            Thread.Sleep(CycleLength);

        }

        private void ActionStop()
        {
            m_isGameStopped = true;
            m_server.SendAllPlayers("(stop)");
            m_simulator.EndSimulation();
        }

        private void ActionTogglePauseResume()
        {
            m_isPaused = !m_isPaused;
            //m_timer.Enabled = !m_isPaused;
        }

        //private void ActionPause()
        //{
        //    m_isPaused = true;
        //    GameStep();
        //    if (m_simulator.Cycle >= this.GameDuration)
        //        ActionStop();
        //}

        private void ActionStep()
        {
            m_isPaused = true;
            GameStep();
            if (m_simulator.Cycle >= this.GameDuration)
                ActionStop();
        }

        private void ActionEnableTurbo()
        {
            if (m_simulator.IsGameStarted)
            {
                m_preWaitForAll = WaitForAllPlayers;
                WaitForAllPlayers = true;
                m_server.SendAllPlayers("(turbo on)");
                m_turboMode = true;
            }
        }

        private void ActionDisableTurbo()
        {
            if (m_turboMode)
            {
                m_turboMode = false;
                WaitForAllPlayers = m_preWaitForAll;
                m_server.SendAllPlayers("(turbo off)");
            }
        }

        #endregion

        #region Public Methods

        public void BindMonitor(SoccerMonitor monitor)
        {
            EnqueAction(ControllerActionTypes.BindMonitor, monitor);
            if (this.IsGameStopped)
            {
                monitor.ForceUpdateMonitor();
            }
        }

        public void UnbindMonitor(SoccerMonitor monitor)
        {
            EnqueAction(ControllerActionTypes.UnbindMonitor, monitor);
        }

        public void Start()
        {
            EnqueAction(ControllerActionTypes.Start, null);
        }

        public void Stop()
        {
            // this is a mandatory call which forcefully sets a flag which prevents other threads from remaining alive
            m_simulator.EndSimulation();
            EnqueAction(ControllerActionTypes.Stop, null);
        }

        public void TogglePauseResume()
        {
            EnqueAction(ControllerActionTypes.TogglePauseResume, null);
        }

        public void RunOneStep()
        {
            EnqueAction(ControllerActionTypes.Step, null);
        }

        #endregion

        #region Private Methods

        private void AcceptConnectionStep()
        {
            if (m_server.HasConnectionRequest())
                m_server.AcceptConnection();

            m_server.CheckTempClients();
            m_server.CheckConnectedClients();
        }

        private void PlayOnStep()
        {
            if (m_isPaused || m_isGameStopped) 
                return;

            GameStep();

            if (m_simulator.Cycle >= this.GameDuration)
            {
                ActionStop();
                if (GameCyclesFinished != null)
                    GameCyclesFinished(this, new EventArgs());
            }
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

        #endregion
    }
}
