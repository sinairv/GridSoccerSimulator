using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLClient
{
    class Program
    {
        static RLClient client;
        static void Main(string[] args)
        {
            try
            {
                if (args.Length >= 2)
                {
                    client = new RLClient(args[0], Int32.Parse(args[1]));
                }
                else
                {
                    client = new RLClient("RL", 1);
                }

                client.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
