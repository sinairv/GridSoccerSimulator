using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GridSoccer.SampleClient
{
    static class Program
    {
        static SampleClient client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length >= 2)
                client = new SampleClient("127.0.0.1", 5050, args[0], Int32.Parse(args[1]));
            else
                client = new SampleClient();

            client.Start();
        }
    }
}
