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
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace GridSoccer.Simulator.Net
{
    /// <summary>
    /// Encapsulates information about each network client
    /// </summary>
    public class ClientInfo
    {
        private TcpClient m_client;

        public TcpClient TcpClient 
        { 
            get
            {
                return m_client;
            }

            set
            {
                m_client = value;
            }
        }

        public bool DataAvailable
        {
            get
            {
                return m_client.GetStream().DataAvailable;
            }
        }

        public string ReadString()
        {
            var readBuffer = new byte[1024];
            m_client.GetStream().Read(readBuffer, 0, readBuffer.Length);
            string rcvd = Encoding.ASCII.GetString(readBuffer);
            rcvd = rcvd.Trim('\0');
            //Console.WriteLine("RCVD: " + rcvd);
            return rcvd;
        }

        public void WriteString(string str)
        {
            try
            {
                byte[] bs = Encoding.ASCII.GetBytes(str);
                m_client.GetStream().Write(bs, 0, bs.Length);
                //m_client.GetStream().Flush();
                //Thread.Sleep(0);
                //Console.WriteLine("Sent: " + str);
            }
            catch
            {
            }
        }

        public void Close()
        {
            m_client.Close();
        }

        public int PlayerIndex;

    }
}
