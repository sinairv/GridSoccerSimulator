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
using System.Windows.Forms;
using System.Threading;

namespace GridSoccer.Simulator.Monitor
{
    public partial class SoccerMonitor : UserControl
    {
        private System.Threading.Timer m_timerUpdateUI = null;
        private SoccerSimulator m_simulator = null;
        private int m_timerInterval = 500;
        private bool m_isBound = false;

        private bool m_gameStateChanged = false;
        private bool m_gameScoreChanged = true;
        private object m_eventLock = new object();

        public SoccerMonitor()
        {
            InitializeComponent();

            m_timerUpdateUI = new System.Threading.Timer(TimerUpdateUICallBack, null,
                Timeout.Infinite, Timeout.Infinite);
        }

        public int IntervalUpdateUI
        {
            get
            {
                return m_timerInterval;
            }

            set
            {
                m_timerInterval = Math.Max(value, 5);
            }
        }

        private void DisableTimer()
        {
            m_timerUpdateUI.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void EnableTimer()
        {
            m_timerUpdateUI.Change(0, m_timerInterval);
        }

        private void TimerUpdateUICallBack(object args)
        {
            //DisableTimer();

            lock (m_eventLock)
            {
                if (m_gameScoreChanged)
                {
                    ShowScores();
                    m_gameScoreChanged = false;
                }
            }

            lock (m_eventLock)
            {
                if (m_gameStateChanged)
                {
                    var act = new Action(() =>
                            {
                                lblCycle.Text = m_simulator.Cycle.ToString();
                                this.soccerField.UpdateField();
                            }
                        );
                    if (this.InvokeRequired)
                        this.Invoke(act);
                    else
                        act.Invoke();

                    m_gameStateChanged = false;
                }
            }

            //if(m_isBound)
            //    EnableTimer();
        }

        public void BindToSimulator(SoccerSimulator sim)
        {
            var act = new Action(() =>
                                     {
                                         m_simulator = sim;
                                         this.soccerField.SetSimulator(sim);
                                         m_simulator.Changed += new EventHandler(m_simulator_Changed);
                                         m_simulator.ScoreChanged += new EventHandler(m_simulator_ScoreChanged);
                                         m_isBound = true;

                                         //this.soccerField.BindToSimulator(sim);
                                         //m_simulator_ScoreChanged(this, new EventArgs());

                                         ForceUpdateMonitor();
                                         EnableTimer();
                                     }
                );

            if(this.InvokeRequired)
                this.Invoke(act);
            else
                act.Invoke();
        }

        public void ForceUpdateMonitor()
        {
            m_gameStateChanged = true;
            m_gameScoreChanged = true;
            TimerUpdateUICallBack(null);
        }

        public void UnbindFromSimulator(SoccerSimulator sim)
        {
            var act = new Action(() =>
                                     {
                                         m_isBound = false;
                                         DisableTimer();
                                         this.soccerField.SetSimulator(null);
                                         m_simulator.Changed -= new EventHandler(m_simulator_Changed);
                                         m_simulator.ScoreChanged -= new EventHandler(m_simulator_ScoreChanged);
                                     }
                );

            if(this.InvokeRequired)
                this.Invoke(act);
            else
                act.Invoke();
        }

        void m_simulator_ScoreChanged(object sender, EventArgs e)
        {
            lock (m_eventLock)
            {
                m_gameScoreChanged = true;
            }
        }

        void m_simulator_Changed(object sender, EventArgs e)
        {
            lock (m_eventLock)
            {
                m_gameStateChanged = true;
            }
        }

        private void ShowScores()
        {
            var act = new Action(() =>
                                     {
                                         if (!String.IsNullOrEmpty(m_simulator.LeftTeamName))
                                             lblLeftTeamName.Text = String.Format("{1} - {0}", m_simulator.LeftScore,
                                                                                  m_simulator.LeftTeamName);
                                         if (!String.IsNullOrEmpty(m_simulator.RightTeamName))
                                             lblRightTeamName.Text = String.Format("{1} - {0}",
                                                                                   m_simulator.RightTeamName,
                                                                                   m_simulator.RightScore);
                                     }
                );

            if(this.InvokeRequired)
                this.Invoke(act);
            else
                act.Invoke();
        }

        private void SoccerMonitor_Resize(object sender, EventArgs e)
        {
            int wthird = this.Width / 3;
            this.lblLeftTeamName.Width = wthird;
            this.lblRightTeamName.Width = wthird;
            this.lblCycle.Width = wthird;
            this.lblLeftTeamName.Left = 0;
            this.lblCycle.Left = wthird + 1;
            this.lblRightTeamName.Left = 2 * wthird + 2;
        }

    }
}
