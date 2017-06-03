//#define COMPACTED
#define LONGER
#define MOREBOTH

using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib;
using SharpNeatLib.AppConfig;
using SharpNeatLib.Evolution;
using SharpNeatLib.Evolution.Xml;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeuralNetwork.Xml;
using System.Xml;
using System.IO;
using System.Threading;
using SharpNeatExperiments.Skirmish;
using SkirmishCmdEvolution;

namespace SharpNeat.Experiments
{
    public class Program
    {
        public static int PopulationSize = 150;
        public static int NumPredators = 5;
        public static string Shape = "triangle";
        public static string LogFolder = "";

        public static int MaxGenerations = 1000;

        public static WorldSimulator TheWorldSimulator;
        private static CommandLineParser cmdParser;

        public static void Main(string[] args)
        {
            cmdParser = new CommandLineParser(args);

            if (cmdParser.IsOptionProvided("-ccea"))
                CCEAHyperNEAT();
            else if (cmdParser.IsOptionProvided("-3d"))
                ThreeDHyperNEAT();
            else if (cmdParser.IsOptionProvided("-neat"))
                CCEANeat();
            else
                StandardExp(args);

        }

        public static void ThreeDHyperNEAT()
        {
            if (!cmdParser.IsArgumentProvided("-shape", out Program.Shape))
                Program.Shape = "triangle";

            if (!cmdParser.IsArgumentProvided("-folder", out Program.LogFolder))
                Program.LogFolder = "TestLogs";

            if (!Directory.Exists(Program.LogFolder))
                Directory.CreateDirectory(Program.LogFolder);

            if (!cmdParser.IsIntArgumentProvided("-gens", out Program.MaxGenerations))
                Program.MaxGenerations = 1000;



            double maxFitness = 0;
            int maxGenerations = Program.MaxGenerations;
            int populationSize = Program.PopulationSize;
            IExperiment exp = new Skirmish3DExperiment(5, Program.Shape, Program.PopulationSize);
            StreamWriter sw = File.CreateText(Path.Combine(Program.LogFolder, 
                String.Format("{0}-logfile.log", Program.LogFolder)));

            sw.AutoFlush = true;

            XmlDocument doc;
            FileInfo oFileInfo;
            IdGenerator idgen;
            EvolutionAlgorithm ea;
            idgen = new IdGenerator();
            ea = new EvolutionAlgorithm(new Population(idgen, GenomeFactory.CreateGenomeList(exp.DefaultNeatParameters, idgen, exp.InputNeuronCount, exp.OutputNeuronCount, exp.DefaultNeatParameters.pInitialPopulationInterconnections, populationSize)), exp.PopulationEvaluator, exp.DefaultNeatParameters);

            for (int j = 0; j < maxGenerations; j++)
            {
                DateTime dt = DateTime.Now;
                ea.PerformOneGeneration();
                if (ea.BestGenome.Fitness > maxFitness)
                {
                    maxFitness = ea.BestGenome.Fitness;
                    doc = new XmlDocument();
                    XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome);
                    oFileInfo = new FileInfo(Path.Combine(Program.LogFolder, "BestGenome-" + j.ToString() + ".xml"));
                    doc.Save(oFileInfo.FullName);

                    // This will output the substrate, uncomment if you want that
                    /* doc = new XmlDocument();
                     XmlGenomeWriterStatic.Write(doc, (NeatGenome) SkirmishNetworkEvaluator.substrate.generateMultiGenomeModulus(ea.BestGenome.Decode(null),5));
                     oFileInfo = new FileInfo(folder + "bestNetwork" + j.ToString() + ".xml");
                     doc.Save(oFileInfo.FullName);
                     */

                   
                }
                Console.WriteLine(ea.Generation.ToString() + " " + ea.BestGenome.Fitness + " " + (DateTime.Now.Subtract(dt)));
                //Do any post-hoc stuff here

                sw.WriteLine("{0} {1} {2}", ea.Generation, maxFitness, ea.Population.MeanFitness);
                sw.Flush();

            }
            sw.Close();

            doc = new XmlDocument();
            XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, ActivationFunctionFactory.GetActivationFunction("NullFn"));
            oFileInfo = new FileInfo(Path.Combine(Program.LogFolder, "BestGenome.xml"));
            doc.Save(oFileInfo.FullName);
        
        }

        private static void CCEANeat()
        {
            if (!cmdParser.IsArgumentProvided("-shape", out Program.Shape))
                Program.Shape = "triangle";

            if (!cmdParser.IsArgumentProvided("-folder", out Program.LogFolder))
                Program.LogFolder = "TestLogs";

            if (!Directory.Exists(Program.LogFolder))
                Directory.CreateDirectory(Program.LogFolder);

            if (!cmdParser.IsIntArgumentProvided("-gens", out Program.MaxGenerations))
                Program.MaxGenerations = 1000;

            TheWorldSimulator = new WorldSimulator(Program.NumPredators, Program.Shape);
            TheWorldSimulator.Start();

            Thread[] predatorThreads = new Thread[Program.NumPredators];

            for (int i = 0; i < Program.NumPredators; i++)
            {
                predatorThreads[i] = new Thread(PredatorNeatMainThread);
                predatorThreads[i].Start(i);
            }

            for (int i = 0; i < Program.NumPredators; i++)
            {
                predatorThreads[i].Join();
            }

            TheWorldSimulator.Stop();
        }

        private static void PredatorNeatMainThread(object oAgentId)
        {
            int agentId = (int)oAgentId;
            double maxFitness = 0;
            int maxGenerations = Program.MaxGenerations;
            int populationSize = Program.PopulationSize;
            IExperiment exp = new SkirmishNeatCCEAExperiment(agentId, Program.PopulationSize);

            StreamWriter sw = File.CreateText(Path.Combine(Program.LogFolder,
                String.Format("{1}-logfile-{0}.log", agentId, Program.LogFolder)));
            sw.AutoFlush = true;

            XmlDocument doc;
            FileInfo oFileInfo;

            IdGenerator idgen = new IdGenerator();
            EvolutionAlgorithm ea = new EvolutionAlgorithm(
                new Population(idgen,
                    GenomeFactory.CreateGenomeList(exp.DefaultNeatParameters,
                        idgen, exp.InputNeuronCount, exp.OutputNeuronCount,
                        exp.DefaultNeatParameters.pInitialPopulationInterconnections,
                        populationSize)),
                exp.PopulationEvaluator, exp.DefaultNeatParameters);

            for (int j = 0; j < maxGenerations; j++)
            {
                DateTime dt = DateTime.Now;
                ea.PerformOneGeneration();

                if (ea.BestGenome.Fitness > maxFitness)
                {
                    maxFitness = ea.BestGenome.Fitness;
                    doc = new XmlDocument();
                    XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome);
                    oFileInfo = new FileInfo(Path.Combine(LogFolder,
                        String.Format("BestGenome-{0}-{1}.xml", agentId, j)));
                    doc.Save(oFileInfo.FullName);

                    // This will output the substrate, uncomment if you want that
                    /* doc = new XmlDocument();
                     XmlGenomeWriterStatic.Write(doc, (NeatGenome) SkirmishNetworkEvaluator.substrate.generateMultiGenomeModulus(ea.BestGenome.Decode(null),5));
                     oFileInfo = new FileInfo(folder + "bestNetwork" + j.ToString() + ".xml");
                     doc.Save(oFileInfo.FullName);
                     */
                }

                Console.WriteLine("[{0}] {1} {2} {3}", agentId, ea.Generation, ea.BestGenome.Fitness, (DateTime.Now.Subtract(dt)));
                //Do any post-hoc stuff here

                sw.WriteLine("{0} {1} {2}", ea.Generation, maxFitness, ea.Population.MeanFitness);
                sw.Flush();
            }
            sw.Close();

            //doc = new XmlDocument();
            //XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, ActivationFunctionFactory.GetActivationFunction("NullFn"));
            //oFileInfo = new FileInfo(Path.Combine(LogFolder, "BestGenome.xml"));
            //doc.Save(oFileInfo.FullName);

            Environment.Exit(0);
        }


        public static void CCEAHyperNEAT()
        {
            if(!cmdParser.IsArgumentProvided("-shape", out Program.Shape))
                Program.Shape = "triangle";

            if (!cmdParser.IsArgumentProvided("-folder", out Program.LogFolder))
                Program.LogFolder = "CCEAHyperNEATTestLogs";

            if (!Directory.Exists(Program.LogFolder))
                Directory.CreateDirectory(Program.LogFolder);

            if (!cmdParser.IsIntArgumentProvided("-gens", out Program.MaxGenerations))
                Program.MaxGenerations = 1000;

            TheWorldSimulator = new WorldSimulator(Program.NumPredators, Program.Shape);
            TheWorldSimulator.Start();

            Thread[] predatorThreads = new Thread[Program.NumPredators];

            for (int i = 0; i < Program.NumPredators; i++)
            {
                predatorThreads[i] = new Thread(PredatorCCEAHNMainThread);
                predatorThreads[i].Start(i);
            }

            for (int i = 0; i < Program.NumPredators; i++)
            {
                predatorThreads[i].Join();
            }

            TheWorldSimulator.Stop();
        }

        private static void PredatorCCEAHNMainThread(object oAgentId)
        {
            int agentId = (int)oAgentId;
            double maxFitness = 0;
            int maxGenerations = Program.MaxGenerations;
            int populationSize = Program.PopulationSize;
            IExperiment exp = new SkirmishCCEAExperiment(agentId, Program.PopulationSize);

            StreamWriter sw = File.CreateText(Path.Combine(Program.LogFolder, 
                String.Format("{0}-logfile-{1}.log", Program.LogFolder, agentId)));
            sw.AutoFlush = true;
            
            XmlDocument doc;
            FileInfo oFileInfo;

            IdGenerator idgen = new IdGenerator();
            EvolutionAlgorithm ea = new EvolutionAlgorithm(
                new Population(idgen,
                    GenomeFactory.CreateGenomeList(exp.DefaultNeatParameters, 
                        idgen, exp.InputNeuronCount, exp.OutputNeuronCount, 
                        exp.DefaultNeatParameters.pInitialPopulationInterconnections, 
                        populationSize)),
                exp.PopulationEvaluator, exp.DefaultNeatParameters);

            for (int j = 0; j < maxGenerations; j++)
            {
                DateTime dt = DateTime.Now;
                ea.PerformOneGeneration();

                if (ea.BestGenome.Fitness > maxFitness)
                {
                    maxFitness = ea.BestGenome.Fitness;
                    doc = new XmlDocument();
                    XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome);
                    oFileInfo = new FileInfo(Path.Combine(LogFolder, 
                        String.Format("BestGenome-{0}-{1}.xml", agentId, j)));
                    doc.Save(oFileInfo.FullName);

                    // This will output the substrate, uncomment if you want that
                    /* doc = new XmlDocument();
                     XmlGenomeWriterStatic.Write(doc, (NeatGenome) SkirmishNetworkEvaluator.substrate.generateMultiGenomeModulus(ea.BestGenome.Decode(null),5));
                     oFileInfo = new FileInfo(folder + "bestNetwork" + j.ToString() + ".xml");
                     doc.Save(oFileInfo.FullName);
                     */
                }

                Console.WriteLine("[{0}] {1} {2} {3}", agentId, ea.Generation, ea.BestGenome.Fitness, (DateTime.Now.Subtract(dt)));
                //Do any post-hoc stuff here

                sw.WriteLine("{0} {1} {2}", ea.Generation, maxFitness, ea.Population.MeanFitness);
                sw.Flush();
            }
            sw.Close();

            //doc = new XmlDocument();
            //XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, ActivationFunctionFactory.GetActivationFunction("NullFn"));
            //oFileInfo = new FileInfo(Path.Combine(LogFolder, "BestGenome.xml"));
            //doc.Save(oFileInfo.FullName);

            Environment.Exit(0);
        }

        public static void StandardExp(string[] args)
        {
            string folder = "";
            NeatGenome seedGenome = null;
            string seedFilename = null;
            string shape = "triangle";
            bool isMulti = false;

            for (int j = 0; j < args.Length; j++)
            {
                if(j <= args.Length - 2)
                    switch (args[j])
                    {
                        case "-seed": seedFilename = args[++j];
                            Console.WriteLine("Attempting to use seed from file " + seedFilename);
                            break;
                        case "-folder": folder = args[++j];
                            Console.WriteLine("Attempting to output to folder " + folder);
                            break;
                        case "-shape": shape = args[++j];
                            Console.WriteLine("Attempting to do experiment with shape " + shape);
                            break;
                        case "-multi": isMulti = Boolean.Parse(args[++j]);
                            Console.WriteLine("Experiment is heterogeneous? " + isMulti);
                            break;
                    }
            }

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (!cmdParser.IsIntArgumentProvided("-gens", out Program.MaxGenerations))
                Program.MaxGenerations = 1000;


            if(seedFilename!=null)
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(seedFilename);
                    seedGenome = XmlNeatGenomeReaderStatic.Read(document);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Problem loading genome. \n" + e.Message);
                }
            }

            double maxFitness = 0;
            int maxGenerations = Program.MaxGenerations;
            int populationSize = Program.PopulationSize;
            int inputs = 4;
            IExperiment exp = new SkirmishExperiment(inputs, 1, isMulti, shape);
            StreamWriter sw;
            sw = File.CreateText(Path.Combine(folder, 
                String.Format("{0}-logfile.log", Program.LogFolder)));
            sw.AutoFlush = true;

            XmlDocument doc;
            FileInfo oFileInfo;
            IdGenerator idgen;
            EvolutionAlgorithm ea;
            if (seedGenome == null)
            {
                idgen = new IdGenerator();
                ea = new EvolutionAlgorithm(new Population(idgen, GenomeFactory.CreateGenomeList(exp.DefaultNeatParameters, idgen, exp.InputNeuronCount, exp.OutputNeuronCount, exp.DefaultNeatParameters.pInitialPopulationInterconnections, populationSize)), exp.PopulationEvaluator, exp.DefaultNeatParameters);

            }
            else
            {
                idgen = new IdGeneratorFactory().CreateIdGenerator(seedGenome);
                ea = new EvolutionAlgorithm(new Population(idgen, GenomeFactory.CreateGenomeList(seedGenome, populationSize, exp.DefaultNeatParameters, idgen)), exp.PopulationEvaluator, exp.DefaultNeatParameters);
            }

            for (int j = 0; j < maxGenerations; j++)
            {
                DateTime dt = DateTime.Now;
                ea.PerformOneGeneration();
                if (ea.BestGenome.Fitness > maxFitness)
                {
                    maxFitness = ea.BestGenome.Fitness;
                    doc = new XmlDocument();
                    XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome);
                    oFileInfo = new FileInfo(Path.Combine(folder, "BestGenome-" + j.ToString() + ".xml"));
                    doc.Save(oFileInfo.FullName);

                    // This will output the substrate, uncomment if you want that
                    /* doc = new XmlDocument();
                     XmlGenomeWriterStatic.Write(doc, (NeatGenome) SkirmishNetworkEvaluator.substrate.generateMultiGenomeModulus(ea.BestGenome.Decode(null),5));
                     oFileInfo = new FileInfo(folder + "bestNetwork" + j.ToString() + ".xml");
                     doc.Save(oFileInfo.FullName);
                     */

                   
                }
                Console.WriteLine(ea.Generation.ToString() + " " + ea.BestGenome.Fitness + " " + (DateTime.Now.Subtract(dt)));
                //Do any post-hoc stuff here

                sw.WriteLine("{0} {1} {2}", ea.Generation, maxFitness, ea.Population.MeanFitness);
                sw.Flush();

            }
            sw.Close();

            doc = new XmlDocument();
            XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, ActivationFunctionFactory.GetActivationFunction("NullFn"));
            oFileInfo = new FileInfo(Path.Combine(folder, "BestGenome.xml"));
            doc.Save(oFileInfo.FullName);

        }

    }
}
