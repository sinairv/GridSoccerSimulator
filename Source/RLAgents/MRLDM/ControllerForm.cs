using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GridSoccer.RLAgentsCommon;
using System.IO;
using GridSoccer.RLAgentsCommon.Modules;

namespace MRLDM
{
    public partial class ControllerForm : Form
    {
        RLClientBase m_client = null;
        public ControllerForm()
        {
            InitializeComponent();
        }

        public ControllerForm(RLClientBase client)
        {
            InitializeComponent();

            foreach (string name in Enum.GetNames(typeof(Params.DM.MethodTypes)))
                comboMethods.Items.Add(name);

            comboMethods.SelectedItem = Params.DM.Method.ToString();

            SetClient(client);
            cbIsOnline_CheckedChanged(this, null);
        }

        private void SetClient(RLClientBase client)
        {
            m_client = client;
            this.Text = String.Format("{0} {1}", client.MyTeamName, client.MyUnum);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = m_client.MyTeamName + m_client.MyUnum;
            dlg.Filter = "dat files|*.dat|All files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = dlg.FileName;

                StreamWriter sw = File.CreateText(fileName);
                m_client.SaveQTable(sw);
                sw.Close();
                MessageBox.Show("Saving Done!");
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "dat files|*.dat|All files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = dlg.FileName;

                StreamReader sr = File.OpenText(fileName);
                m_client.LoadQTable(sr);

                sr.Close();
                this.Text += " - " + Path.GetFileName(fileName);
                MessageBox.Show("Loading Done!");
            }
        }

        double prevAlpha = Params.Alpha;
        double prevEpsilon = Params.Epsillon;
        private void cbIsOnline_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIsOnline.Checked)
            {
                Params.Alpha = 0.0;
                //Params.Epsillon = 0.0001;
            }
            else
            {
                Params.Alpha = prevAlpha;
                //Params.Epsillon = prevEpsilon;
            }
        }

        private void btnPerformDM_Click(object sender, EventArgs e)
        {
            ChangeK();
            ChangeMinConf();
            ChangeSupp();

            int numChanges = ((MRLDMClient)m_client).PerformDM();
            MessageBox.Show(String.Format("Made {0} updates!", numChanges));
        }

        private void txtK_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChangeK();
                MessageBox.Show("K Changed to: " + Params.DM.K);
            }
        }

        private void ChangeK()
        {
            int k;
            if (Int32.TryParse(txtK.Text, out k))
            {
                Params.DM.K = k;
            }
        }

        private void txtMinSupp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChangeSupp();
                MessageBox.Show("Min Supp Changed to: " + Params.DM.MinSupport);
            }
        }

        private void ChangeSupp()
        {
            double minSupp;
            if (Double.TryParse(txtMinSupp.Text, out minSupp))
            {
                Params.DM.MinSupport = minSupp;
            }
        }

        private void txtMinConf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChangeMinConf();
                MessageBox.Show("Min conf Changed to: " + Params.DM.MinConfidence);
            }
        }

        private void ChangeMinConf()
        {
            double minConf;
            if (Double.TryParse(txtMinConf.Text, out minConf))
            {
                Params.DM.MinConfidence = minConf;
            }
        }

        private void btnShowStats_Click(object sender, EventArgs e)
        {
            double[, ] stats = ((MRLDMClient)m_client).GetQTableStats();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("      {1,-10} {2,-10} {3,-10}",
                    0, "MaxSupp", "MinSupp", "MeanSupp"));
            for(int i = 0; i < stats.GetLength(0); ++i)
            {
                sb.AppendLine(String.Format("[{0}] {1,-10:F5} {2,-10:F5} {3,-10:F5}",
                    i, stats[i,0], stats[i,2], stats[i,1]));
            }

            MessageBox.Show(sb.ToString());
        }

        private void comboMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            Params.DM.Method = (Params.DM.MethodTypes) Enum.Parse(typeof(Params.DM.MethodTypes), (string)comboMethods.SelectedItem);
            MessageBox.Show("" + Params.DM.Method);
        }

        private void btnBatch_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            string path = dlg.SelectedPath;
            string baseFileName = m_client.MyTeamName + m_client.MyUnum;

            Params.DM.MethodTypes oldMethod = Params.DM.Method;
            int oldK = Params.DM.K;
            double oldMinSupp = Params.DM.MinSupport;


            // save the base 
            SaveClient(Path.Combine(path, baseFileName));

            int[] kValues = new int[] { 1, 2, 5 };
            int[] minSuppValues = new int[] { 1500, 2500, 10000, 20000 };

            for(int mi = 0; mi <= 2; ++mi)
                foreach(int k in kValues)
                    foreach (int minSupp in minSuppValues)
                    {
                        switch (mi)
                        {
                            case 0:
                                Params.DM.Method = Params.DM.MethodTypes.Averaging;
                                break;
                            case 1:
                                Params.DM.Method = Params.DM.MethodTypes.TopQ;
                                break;
                            case 2:
                            default:
                                Params.DM.Method = Params.DM.MethodTypes.Voting;
                                break;
                        }

                        Params.DM.K = k;
                        Params.DM.MinSupport = minSupp;

                        ((MRLDMClient)m_client).PerformDM();

                        SaveClient(Path.Combine(path, String.Format("{0}-{1}-{2}-{3}", 
                            baseFileName, 
                            mi == 0? "avg" : (mi == 1 ? "topq" : "voting"), 
                            k, 
                            minSupp)));
                        LoadClient(Path.Combine(path, baseFileName));
                    }

            Params.DM.Method = oldMethod;
            Params.DM.K = oldK;
            Params.DM.MinSupport = oldMinSupp;

            MessageBox.Show("Batch DM Done!");
        }

        private void SaveClient(string fileName)
        {
            StreamWriter sw = File.CreateText(fileName+".dat");
            m_client.SaveQTable(sw);
            sw.Close();
        }

        private void LoadClient(string fileName)
        {
            StreamReader sr = File.OpenText(fileName+".dat");
            m_client.LoadQTable(sr);
            sr.Close();
        }

    }
}
