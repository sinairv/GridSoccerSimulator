using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
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



        public SoccerMonitor()
        {
            InitializeComponent();

            m_timerUpdateUI = new System.Threading.Timer(TimerUpdateUICallBack, null,
                System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
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
            m_timerUpdateUI.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        private void EnableTimer()
        {
            m_timerUpdateUI.Change(0, m_timerInterval);
        }

        private void TimerUpdateUICallBack(object args)
        {
            //DisableTimer();
            //timerUpdate.Enabled = false;

            if (m_gameScoreChanged)
            {
                ShowScores();
                m_gameScoreChanged = false;
            }

            if (m_gameStateChanged)
            {
                this.Invoke(new Action(() =>
                    {
                        lblCycle.Text = m_simulator.Cycle.ToString();
                        this.soccerField.UpdateField();
                    }
                ));
                m_gameStateChanged = false;
            }

            //if(m_isBound)
            //    EnableTimer();
            //timerUpdate.Enabled = m_isBound;
        }

        public void BindToSimulator(SoccerSimulator sim)
        {
            this.Invoke(new Action(() =>
                {
                    m_simulator = sim;
                    this.soccerField.SetSimulator(sim);
                    m_simulator.Changed += new EventHandler(m_simulator_Changed);
                    m_simulator.ScoreChanged += new EventHandler(m_simulator_ScoreChanged);
                    m_isBound = true;
                    //this.soccerField.BindToSimulator(sim);
                    m_simulator_ScoreChanged(this, new EventArgs());
                    EnableTimer();
                }
            ));
        }

        public void UnbindFromSimulator(SoccerSimulator sim)
        {
            this.Invoke(new Action(() =>
                {
                    m_isBound = false;
                    DisableTimer();
                    this.soccerField.SetSimulator(null);
                    m_simulator.Changed -= new EventHandler(m_simulator_Changed);
                    m_simulator.ScoreChanged -= new EventHandler(m_simulator_ScoreChanged);
                }
            ));

        }

        void m_simulator_ScoreChanged(object sender, EventArgs e)
        {
            m_gameScoreChanged = true;
        }

        void m_simulator_Changed(object sender, EventArgs e)
        {
            m_gameStateChanged = true;
        }

        private void ShowScores()
        {
            this.Invoke(new Action(() =>
            {
                if (!String.IsNullOrEmpty(m_simulator.LeftTeamName))
                    lblLeftTeamName.Text = String.Format("{1} - {0}", m_simulator.LeftScore, m_simulator.LeftTeamName);
                if (!String.IsNullOrEmpty(m_simulator.RightTeamName))
                    lblRightTeamName.Text = String.Format("{1} - {0}", m_simulator.RightTeamName, m_simulator.RightScore);
            }
            ));
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
