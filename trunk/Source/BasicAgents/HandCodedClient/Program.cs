using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;

namespace GridSoccer.HandCodedClient
{
    public class Program
    {
        public static bool IsDefensive = false;
        public static double SensitiveDistance = 2.0;
        public static bool IsRandom = false;
        public static bool IsOffensive = false;

        public static bool IsTrainer = false;
        public static int TrainerPatience = -1;

        private static HandCodedClient client;
        public static void Main(string[] args)
        {
            CommandLineParser cmdParser = new CommandLineParser(args);

            string name;
            int unum;

            if (!cmdParser.IsArgumentProvided("-name", out name))
                name = "HandCoded";

            if (!cmdParser.IsIntArgumentProvided("-n", out unum))
                unum = 1;

            if (cmdParser.IsOptionProvided("-def"))
                IsDefensive = true;
            else if (cmdParser.IsOptionProvided("-rnd"))
                IsRandom = true;
            else if (cmdParser.IsOptionProvided("-off"))
                IsOffensive = true;

            if (cmdParser.IsOptionProvided("-trainer"))
            {
                IsTrainer = true;
                if (!cmdParser.IsIntArgumentProvided("-trainer", out TrainerPatience))
                    TrainerPatience = -1;
            }

            if (!cmdParser.IsDoubleArgumentProvided("-sen", out SensitiveDistance))
                SensitiveDistance = 2.0;


            client = new HandCodedClient(name, unum);

            client.Start();
        }
    }
}
