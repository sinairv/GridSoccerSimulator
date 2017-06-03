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

namespace GridSoccer.Simulator
{
    public partial class FormMain : Form
    {
        private SimulationController m_simController = new SimulationController();
        private bool m_isMonitorBinded = false;

        public FormMain()
        {
            InitializeComponent();

            // TODO before development
            tbtnSettings.Visible = false;

            // Init Image Icons
            tbtnBindMonitor.Visible = false;
            tbtnNormalSpeed.Visible = false;
            tbtnPause.Visible = false;

            // make some buttons disabled
            tbtnStep.Enabled = false;
            tbtnStop.Enabled = false;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // other control initializations, for these need the window handle to be created
            m_simController.GameCyclesFinished += new EventHandler(simController_GameCyclesFinished);
            propController.SelectedObject = new SimulationControllerProperties(m_simController);
            m_simController.BindMonitor(soccerMonitor);
            m_isMonitorBinded = true;
        }

        void simController_GameCyclesFinished(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                tbtnStep.Enabled = false;
                tbtnStartResume.Enabled = false;
                tbtnStop.Enabled = false;
                tbtnTurbo.Enabled = false;
            }));
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_simController.IsGameStopped)
            {
                bool wasPaused = m_simController.IsPaused;
                if (!wasPaused)
                    m_simController.TogglePauseResume();

                if (DialogResult.Yes == MessageBox.Show(
                    "Are you sure you want to stop the game and exit?", 
                    "Exit?", 
                    MessageBoxButtons.YesNo))
                {
                    m_simController.Stop();
                }
                else
                {
                    e.Cancel = true;
                    if (!wasPaused)
                        m_simController.TogglePauseResume();
                }
            }
        }

        private void ToggleBindUnbindButtonUI(bool toBind)
        {
            tbtnBindMonitor.Visible = toBind;
            tbtnUnbindMonitor.Visible = !toBind;
        }

        private void ToggleTurboNormalButtonUI(bool toTurbo)
        {
            tbtnTurbo.Visible = toTurbo;
            tbtnNormalSpeed.Visible = !toTurbo;
        }

        private void ToggleStartPauseButtonUI(bool toPause)
        {
            tbtnPause.Visible = toPause;
            tbtnStartResume.Visible = !toPause;
        }

        #region Actions

        private void StartResumeAction()
        {
            if (!m_simController.IsGameStarted) 
            {
                tbtnStop.Enabled = true;
                tbtnStep.Enabled = true;
                tbtnSettings.Visible = false;
                m_simController.Start();
            }
            else if(m_simController.IsPaused)
            {
                m_simController.TogglePauseResume();
            }
                
            ToggleStartPauseButtonUI(true);
        }

        private void PauseAction()
        {
            if(!m_simController.IsPaused)
            {
                m_simController.TogglePauseResume();
                ToggleStartPauseButtonUI(false);
            }
        }

        private void StopAction()
        {
            bool wasPaused = m_simController.IsPaused;
            if (!wasPaused)
                m_simController.TogglePauseResume();

            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to stop the game?", "Stop the game?", MessageBoxButtons.YesNo))
            {
                m_simController.Stop();
                tbtnStep.Enabled = false;
                tbtnStartResume.Enabled = false;
                tbtnStop.Enabled = false;
                tbtnTurbo.Enabled = false;
            }
            else
            {
                if (!wasPaused)
                    m_simController.TogglePauseResume();
            }
        }

        private void StepAction()
        {
            m_simController.RunOneStep();
            ToggleStartPauseButtonUI(false);
        }

        private void BindUnbindMonitorAction()
        {
            if (m_isMonitorBinded)
            {
                m_simController.UnbindMonitor(soccerMonitor);
                ToggleBindUnbindButtonUI(true);
                m_isMonitorBinded = false;
            }
            else
            {
                m_simController.BindMonitor(soccerMonitor);
                ToggleBindUnbindButtonUI(false);
                m_isMonitorBinded = true;
            }
        }

        private void TurboToggleAction()
        {
            if (m_simController.TurboMode)
            {
                m_simController.TurboMode = false;
                ToggleTurboNormalButtonUI(true);
            }
            else
            {
                if (m_simController.IsGameStarted)
                {
                    ToggleTurboNormalButtonUI(false);
                    m_simController.TurboMode = true;
                }
            }
        }

        private void SettingsAction()
        {
            if (m_simController.IsGameStarted)
            {
                return;
            }

            using (SettingsDialog frm = new SettingsDialog())
            {
                frm.ShowDialog();
            }
        }

        private void AboutAction()
        {
            bool wasPaused = m_simController.IsPaused;
            if (!wasPaused)
                m_simController.TogglePauseResume();

            using (AboutBoxGridSoccer frm = new AboutBoxGridSoccer())
            {
                frm.ShowDialog();
            }

            if (!wasPaused)
                m_simController.TogglePauseResume();
        }

        #endregion

        #region ToolBarButtons Events
        private void tbtnStartResume_Click(object sender, EventArgs e)
        {
            StartResumeAction();
        }

        private void tbtnStep_Click(object sender, EventArgs e)
        {
            StepAction();
        }

        private void tbtnStop_Click(object sender, EventArgs e)
        {
            StopAction();
        }

        private void tbtnPause_Click(object sender, EventArgs e)
        {
            PauseAction();
        }

        private void tbtnTurbo_Click(object sender, EventArgs e)
        {
            TurboToggleAction();
        }

        private void tbtnNormalSpeed_Click(object sender, EventArgs e)
        {
            TurboToggleAction();
        }

        private void tbtnBindMonitor_Click(object sender, EventArgs e)
        {
            BindUnbindMonitorAction();
        }

        private void tbtnUnbindMonitor_Click(object sender, EventArgs e)
        {
            BindUnbindMonitorAction();
        }

        private void tbtnSettings_Click(object sender, EventArgs e)
        {
            SettingsAction();
        }

        private void tbtnAbout_Click(object sender, EventArgs e)
        {
            AboutAction();
        }

        #endregion



    }
}
