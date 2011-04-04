using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GridSoccer.DPClient;

namespace GridSoccer.NeuroRLClient
{
    public partial class FormMain : Form
    {
        private NeuralClient m_client = null;
        private Thread m_clientThread = null;

        public FormMain()
        {
            InitializeComponent();

            string teamName = "NeuralRL";
            int unum = 1;

            string[] cmdArgs = Environment.GetCommandLineArgs();
            if (cmdArgs.Length > 1)
            {
                teamName = cmdArgs[1];
            }

            if (cmdArgs.Length > 2)
            {
                if (!Int32.TryParse(cmdArgs[2], out unum))
                {
                    unum = 1;
                }
            }

            this.Text = String.Format("{0} - {1}", teamName, unum);

            m_client = new NeuralClient(teamName, unum);
            m_clientThread = new Thread(() => { m_client.Start(); });
            m_clientThread.Start();
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            ValueTableVisualizer frm = new ValueTableVisualizer((m_client as NeuralClient).NeuralQTable, m_client);
            frm.Show(this);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_clientThread.IsAlive)
            {
                m_clientThread.Abort();
            }
        }
    }
}
