using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLwIM
{
    class Program
    {
        private static RLwIMClient client;
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                client = new RLwIMClient(args[0], Int32.Parse(args[1]));
            }
            else
            {
                client = new RLwIMClient("RLwIM", 1);
            }

            client.Start();
        }
    }
}
