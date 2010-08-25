using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLClient1P
{
    class Program
    {
        private static RLClient1P client;
        public static void Main(string[] args)
        {
            if (args.Length >= 2)
                client = new RLClient1P(args[0], Int32.Parse(args[1]));
            else
                client = new RLClient1P("RL1P", 1);

            client.Start();
        }
    }
}
