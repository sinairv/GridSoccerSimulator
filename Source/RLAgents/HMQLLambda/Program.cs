using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.HMQLLambda
{
    class Program
    {
        public static int s_numModules = 0;

        private static HMRLLambdaClient client;
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                if (args.Length >= 3)
                {
                    string arg3 = args[2].ToLower();
                    if (arg3 == "-3m")
                        s_numModules = 3;
                    else if (arg3 == "-5m")
                        s_numModules = 5;
                    else
                        s_numModules = 0; // i.e. the default
                }

                client = new HMRLLambdaClient(args[0], Int32.Parse(args[1]));
            }
            else
            {
                client = new HMRLLambdaClient("HMQLLambda", 1);
            }

            client.Start();
        }
    }
}
