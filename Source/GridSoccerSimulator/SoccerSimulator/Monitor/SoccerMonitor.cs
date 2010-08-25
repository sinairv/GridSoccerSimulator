using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GridSoccer.Simulator.Monitor
{
    public partial class SoccerMonitor : UserControl
    {
        public SoccerMonitor()
        {
            InitializeComponent();
            //SoccerMonitor_Resize(this, null);
        }

        private SoccerSimulator m_simulator = null;

        public void SetSimulator(SoccerSimulator sim)
        {
            m_simulator = sim;
            m_simulator.Changed += new EventHandler(m_simulator_Changed);
            m_simulator.ScoreChanged += new EventHandler(m_simulator_ScoreChanged);
            this.soccerField.SetSimulator(sim);
            m_simulator_ScoreChanged(this, new EventArgs());
        }

        public void UnbindSimulator(SoccerSimulator sim)
        {
            m_simulator.Changed -= new EventHandler(m_simulator_Changed);
            m_simulator.ScoreChanged -= new EventHandler(m_simulator_ScoreChanged);
            this.soccerField.UnbindSimulator(sim);
        }

        void m_simulator_ScoreChanged(object sender, EventArgs e)
        {
            ShowScores();
        }

        void m_simulator_Changed(object sender, EventArgs e)
        {
            if (m_simulator.Cycle <= 0)
            {
                ShowScores();
            }

            lblCycle.Text = m_simulator.Cycle.ToString();
        }

        private void ShowScores()
        {
            if (!String.IsNullOrEmpty(m_simulator.LeftTeamName))
                lblLeftTeamName.Text = String.Format("{1} - {0}", m_simulator.LeftScore, m_simulator.LeftTeamName);
            if (!String.IsNullOrEmpty(m_simulator.RightTeamName))
                lblRightTeamName.Text = String.Format("{1} - {0}", m_simulator.RightTeamName, m_simulator.RightScore);
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
