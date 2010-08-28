using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace GridSoccer.Simulator.Net
{
    /// <summary>
    /// Encapsulates information about each network client
    /// </summary>
    public class ClientInfo
    {
        private TcpClient client;
        private BinaryReader br;
        private BinaryWriter bw;

        public TcpClient TcpClient 
        { 
            get
            {
                return client;
            }

            set
            {
                client = value;
                if (client != null)
                {
                    br = new BinaryReader(client.GetStream());
                    bw = new BinaryWriter(client.GetStream());
                }
            }
        }

        public bool DataAvailable
        {
            get
            {
                return client.GetStream().DataAvailable;
            }
        }

        public string ReadString()
        {
            return br.ReadString();
        }

        public void WriteString(string str)
        {
            try
            {
                bw.Write(str);
                //bw.Flush();
            }
            catch
            {
            }
        }

        public void Close()
        {
            client.Close();
            br.Close();
            bw.Close();
        }

        public int PlayerIndex;

    }
}
