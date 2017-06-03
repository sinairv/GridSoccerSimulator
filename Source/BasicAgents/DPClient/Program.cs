using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.DPClient
{
    class Program
    {
        static DPClient client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 2)
                client = new DPClient("127.0.0.1", 5050, args[0], Int32.Parse(args[1]));
            else
                client = new DPClient("127.0.0.1", 5050, "DPClient", 1);

            client.Start();
        }
    }
}
