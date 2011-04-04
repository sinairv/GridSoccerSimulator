using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Text;

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
                    var buffer = new byte[1024];
                    tcpSocket.GetStream().Read(buffer, 0, buffer.Length);
                    string rcvd = Encoding.ASCII.GetString(buffer);
                    rcvd = rcvd.Trim('\0');
                    Console.WriteLine("R: {0}", rcvd);
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
            string msg = txtInput.Text;
            var buff = Encoding.ASCII.GetBytes(msg);
            tcpSocket.GetStream().Write(buff, 0, buff.Length);
            txtInput.Text = "";
        }
    }
}
