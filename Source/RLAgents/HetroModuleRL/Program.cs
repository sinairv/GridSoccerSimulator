using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.HetroModuleRL
{
    class Program
    {
        private static HetroModuleRLClient client;
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                client = new HetroModuleRLClient(args[0], Int32.Parse(args[1]));
            }
            else
            {
                client = new HetroModuleRLClient("HMQL", 1);
            }

            client.Start();
        }
    }
}
