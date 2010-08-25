using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GridSoccer.Simulator.Net;
using GridSoccer.Simulator.Properties;

namespace GridSoccer.Simulator
{
    public partial class FormMain : Form
    {
        SimulationController simController = new SimulationController();

        public FormMain()
        {
            InitializeComponent();

            simController.GameCyclesFinished += new EventHandler(simController_GameCyclesFinished);
            propController.SelectedObject = simController;
            simController.BindMonitor(soccerMonitor1);
        }

        void simController_GameCyclesFinished(object sender, EventArgs e)
        {
            btnPause.Enabled = false;
            btnStep.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            btnGoJet.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            simController.Start();
            btnStop.Enabled = true;
            btnPause.Enabled = true;
            btnStep.Enabled = true;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!simController.IsGameStopped)
            {
                bool wasPaused = simController.IsPaused;
                if (!wasPaused)
                    simController.TogglePauseResume();

                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to stop the game and exit?", "Exit?", MessageBoxButtons.YesNo))
                {
                    simController.Finish();
                }
                else
                {
                    e.Cancel = true;
                    if (!wasPaused)
                        simController.TogglePauseResume();
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bool wasPaused = simController.IsPaused;
            if(!wasPaused)
                simController.TogglePauseResume();

            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to stop the game?", "Stop the game?", MessageBoxButtons.YesNo))
            {
                simController.Finish();
                btnPause.Enabled = false;
                btnStep.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                btnGoJet.Enabled = false;
            }
            else
            {
                if (!wasPaused)
                    simController.TogglePauseResume();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            simController.TogglePauseResume();
            SetPauseButtonCaption();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            simController.RunOneStep();
            SetPauseButtonCaption();
        }

        private void SetPauseButtonCaption()
        {
            if (simController.IsPaused)
                btnPause.Text = "Resume";
            else
                btnPause.Text = "Pause";
        }

        private void btnBindMonitor_Click(object sender, EventArgs e)
        {
            simController.BindMonitor(soccerMonitor1);
            btnUnbindMonitor.Enabled = true;
            btnBindMonitor.Enabled = false;
        }

        private void btnUnbindMonitor_Click(object sender, EventArgs e)
        {
            simController.UnbindMonitor(soccerMonitor1);
            btnUnbindMonitor.Enabled = false;
            btnBindMonitor.Enabled = true;
        }

        private void btnGoJet_Click(object sender, EventArgs e)
        {
            if (simController.JetMode)
            {
                simController.JetMode = false;
                if (!simController.JetMode)
                {
                    btnGoJet.BackColor = Color.Red;
                    btnGoJet.Text = "Go Jet";
                    btnStep.Enabled = true;
                    btnPause.Enabled = true;
                }
            }
            else
            {
                if (simController.IsGameStarted)
                {
                    btnGoJet.BackColor = Color.Cyan;
                    btnGoJet.Text = "Go Normal";
                    btnStep.Enabled = false;
                    btnPause.Enabled = false;

                    Application.DoEvents();

                    simController.JetMode = true;
                }
            }
        }
    }
}
