using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.HandCodedClient
{
    class Program
    {
        static HandCodedClient client;
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                client = new HandCodedClient(args[0], Int32.Parse(args[1]));
            }
            else
            {
                client = new HandCodedClient("HCMP", 1);
            }

            client.Start();
        }
    }
}
