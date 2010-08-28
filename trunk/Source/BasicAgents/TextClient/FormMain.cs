using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace GridSoccer.TextClient
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private TcpClient tcpSocket = null;

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (tcpSocket == null)
            {
                tcpSocket = new TcpClient("127.0.0.1", 5050);
                tcpSocket.NoDelay = true;
                txtInput.Enabled = true;
                btnAccept.Text = "Send";
            }
            else
            {
                SendData();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            if (tcpSocket != null)
            {
                if (tcpSocket.GetStream().DataAvailable)
                {
                    BinaryReader br = new BinaryReader(tcpSocket.GetStream());
                    Console.WriteLine("R: {0}", br.ReadString());
                }
            }

            timer1.Enabled = true;
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendData();
            }
        }

        private void SendData()
        {
            BinaryWriter bw = new BinaryWriter(tcpSocket.GetStream());
            bw.Write(txtInput.Text);
            txtInput.Text = "";
        }
    }
}
