using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GridSoccer.ClientBasic;
using System.Diagnostics;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;

namespace GridSoccer.DPClient
{
    public partial class ValueTableVisualizer : Form
    {
        QTableBase m_valueTable;
        ClientBase m_client;

        public ValueTableVisualizer(QTableBase valueTable, ClientBase client)
            : this()
        {
            m_valueTable = valueTable;
            m_client = client;

            UpdateValues();
        }

        public ValueTableVisualizer()
        {
            InitializeComponent();
            meshPlotter1.PlotterView.Density = 1.0;
            meshPlotter1.PlotterView.PenColor = Color.Green;
            meshPlotter1.PlotterView.BaseColor = Color.Green;


            meshPlotter1.PlotterView.ObservatorX = 15;
            meshPlotter1.PlotterView.ObservatorY = 0;
            meshPlotter1.PlotterView.ObservatorZ = 20;

            meshPlotter1.PlotterView.ScreenX = 700;
            meshPlotter1.PlotterView.ScreenY = 500;

            this.propertyGrid1.SelectedObject = meshPlotter1.PlotterView;
        }

        private void txtOppRow_Leave(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void txtOppRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateValues();
            }

        }

        private void tbtnBOwner_Click(object sender, EventArgs e)
        {
            tbtnBOwner.Checked = !tbtnBOwner.Checked;
            UpdateValues();
        }

        private void UpdateValues()
        {
            int oppRow, oppCol;
            if (!Int32.TryParse(txtOppRow.Text, out oppRow))
                oppRow = 1;

            if (!Int32.TryParse(txtOppCol.Text, out oppCol))
                oppCol = 1;

            bool AmIBallOwner = tbtnBOwner.Checked;

            double[,] mesh = new double[m_client.EnvRows, m_client.EnvCols];

            for (int mr = 1; mr <= m_client.EnvRows; mr++)
            {
                for (int mc = 1; mc <= m_client.EnvCols; mc++)
                {
                    double maxQ;
                    State curState = new State(new PlayerInfo() { Position = new Position(mr, mc), Unum = 1, IsBallOwner = AmIBallOwner });
                    curState.AddOppPlayer(new PlayerInfo() { Position = new Position(oppRow, oppCol), IsBallOwner = !AmIBallOwner, Unum = 1 }); 
                    m_valueTable.GetMaxQ(curState, out maxQ);
                    mesh[mr - 1, mc - 1] = maxQ;
                        //.m_valueTable.GetValueForState(mr, mc, oppRow - 1, oppCol - 1, AmIBallOwner ? 0 : 1);
                }

            }

            this.meshPlotter1.SetMesh(mesh);
            this.meshPlotter1.Invalidate();
        }


    }
}
