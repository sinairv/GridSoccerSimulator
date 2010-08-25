using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GridSoccer.RandomClient
{
    static class Program
    {
        static RandomClient client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length >= 2)
                client = new RandomClient("127.0.0.1", 5050, args[0], Int32.Parse(args[1]));
            else
                client = new RandomClient("127.0.0.1", 5050, "RandomClient", 1);

            client.Start();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
