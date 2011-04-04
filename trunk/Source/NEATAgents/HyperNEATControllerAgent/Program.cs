using System;
using System.Threading;
using System.Windows.Forms;
using GridSoccer.Common;

namespace GridSoccer.HyperNEATControllerAgent
{
    public class Program
    {
        public static int Teammates = 1;

        public static bool LoadingGenome = false;
        public static string LoadedGenomePath = "";

        public static ExperimentTypes ExpType = ExperimentTypes.CCEAFieldSubs;


        private static bool s_showUI = false;

        private static HyperNEATClient NeatClinet = null;
        private static AutoResetEvent s_eventClientCreated = new AutoResetEvent(false);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (s_showUI)
            {
                var th = new Thread(MainConsoleApp);
                th.Start();

                s_eventClientCreated.WaitOne();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());

            }
            else
            {
                MainConsoleApp();
            }
        }


        public static void MainConsoleApp()
        {
            string[] args = Environment.GetCommandLineArgs();

            var cmdParser = new CommandLineParser(args);

            string name = "HyperNEAT";
            int unum = 1;
            int teamMembers = 1;
            bool isConcurrentMode = false;
            int concurrentTeamMembers = 3;

            if (!cmdParser.IsArgumentProvided("-name", out name))
                name = "HyperNEAT";

            string strExpType;

            if (cmdParser.IsArgumentProvided("-exp", out strExpType))
            {
                ExperimentTypes curExpType;
                if (!Enum.TryParse(strExpType, true, out curExpType))
                {
                    Console.WriteLine("Invalid Experiment type! defaulting To CCEAFieldSubs.");
                    curExpType = ExperimentTypes.CCEAFieldSubs;
                }

                Program.ExpType = curExpType;
            }
            else
            {
                Program.ExpType = ExperimentTypes.CCEAFieldSubs;
            }

            Console.WriteLine("Experiment is: {0}", Program.ExpType);

            if (cmdParser.IsIntArgumentProvided("-team", out concurrentTeamMembers))
            {
                isConcurrentMode = true;
                Program.Teammates = concurrentTeamMembers;
            }
            else
            {
                isConcurrentMode = false;
            }

            if (!isConcurrentMode)
            {
                if (!cmdParser.IsIntArgumentProvided("-n", out unum))
                    unum = 1;

                if (!cmdParser.IsIntArgumentProvided("-t", out teamMembers))
                    teamMembers = 1;


                Program.Teammates = teamMembers;

                if (cmdParser.IsArgumentProvided("-load", out LoadedGenomePath))
                {
                    LoadingGenome = true;
                }
            }


            if (isConcurrentMode)
            {
                
                var concClients = new HyperNEATClient[concurrentTeamMembers];
                var clientThreads = new Thread[concurrentTeamMembers];

                for (int i = 0; i < concClients.Length; i++)
                {
                    int myUnum = i + 1;
                    concClients[i] = new HyperNEATClient(name, myUnum, concurrentTeamMembers);
                }

                for (int i = 0; i < concClients.Length; i++)
                {
                    int loci = i;
                    clientThreads[i] = new Thread(() =>
                        {
                            int myUnum = loci + 1;
                            try
                            {
                                //concClients[myUnum - 1] = new HyperNEATClient(name, myUnum, concurrentTeamMembers);
                                concClients[myUnum - 1].Start();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    );

                    clientThreads[i].Start();
                }

                s_eventClientCreated.Set();


                foreach (var t in clientThreads)
                {
                    t.Join();
                }
            }
            else
            {
                try
                {
                    Program.NeatClinet = new HyperNEATClient(name, unum, teamMembers);

                    s_eventClientCreated.Set();

                    Program.NeatClinet.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.ReadLine();
                }
            }
        }
      
    }
}
