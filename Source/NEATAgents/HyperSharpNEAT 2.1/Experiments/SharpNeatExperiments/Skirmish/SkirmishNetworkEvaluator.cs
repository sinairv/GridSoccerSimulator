using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.CPPNs;
using System.Threading;
using SharpNeatExperiments.Skirmish;

namespace SharpNeatLib.Experiments 
{
    public class SkirmishNetworkEvaluator : INetworkEvaluator
    {
        public static SkirmishSubstrate substrate;
        uint numAgents;
        public static bool trainingSeed = false;
        public Shapes currentShape;

        public SkirmishNetworkEvaluator(uint agents, string shape)
        {
            numAgents = agents;
            substrate = new SkirmishSubstrate(5 * agents, 3 * agents, 5 * agents, HyperNEATParameters.substrateActivationFunction);
            try
            {
                currentShape = (Shapes)Enum.Parse(typeof(Shapes), shape, true);
            }
            catch(ArgumentException)
            {
                Console.WriteLine("Invalid Shape Entered, Defaulting to Triangle");
                currentShape = Shapes.Triangle;
            }
        }

        #region INetworkEvaluator Members

        public double threadSafeEvaluateNetwork(INetwork network, Semaphore sem)
        {
            double fitness = 0;
            INetwork tempNet = null;
            NeatGenome.NeatGenome tempGenome = null;
            int count = network.InputNeuronCount;

            if (!SkirmishExperiment.multiple)
                tempGenome = substrate.GenerateGenome(network);
            else
                tempGenome = substrate.generateMultiGenomeModulus(network, numAgents);
            

            //Currently decoding genomes is NOT thread safe, so we have to do that single file
            sem.WaitOne();
            tempNet = tempGenome.Decode(null);
            sem.Release();

            if (!SkirmishExperiment.multiple)
                fitness += doEvaluation(tempNet);
            else
                fitness += doEvaluationMulti(tempNet);
            return fitness;
        }

        // used by the visualizer
        public static INetwork[] NetworkPerPred = null;

        public static void addPredators(World w)
        {
            if (NetworkPerPred != null)
            {
                w.addPlayer(new Predator(300, 500, w.agentSize, w.agentSize, NetworkPerPred[0]));
                w.addPlayer(new Predator(400, 500, w.agentSize, w.agentSize, NetworkPerPred[1]));
                w.addPlayer(new Predator(500, 500, w.agentSize, w.agentSize, NetworkPerPred[2]));
                w.addPlayer(new Predator(600, 500, w.agentSize, w.agentSize, NetworkPerPred[3]));
                w.addPlayer(new Predator(700, 500, w.agentSize, w.agentSize, NetworkPerPred[4]));
            }
            else
            {
                //I cheat and store a copy of the ANN in everything so I don't have to make special cases for
                //heterogeneous and homogeneous Worlds
                w.addPlayer(new Predator(300, 500, w.agentSize, w.agentSize, w.bigBrain));
                w.addPlayer(new Predator(400, 500, w.agentSize, w.agentSize, w.bigBrain));
                w.addPlayer(new Predator(500, 500, w.agentSize, w.agentSize, w.bigBrain));
                w.addPlayer(new Predator(600, 500, w.agentSize, w.agentSize, w.bigBrain));
                w.addPlayer(new Predator(700, 500, w.agentSize, w.agentSize, w.bigBrain));
            }
        }

        private double doSeedTrain(INetwork network)
        {
            float timetaken = 0;
            double fitness = 0;
            World w = null;

            w = world1(network);
            timetaken += w.go(1000);
            fitness += 1000 - (w.distanceFromEnemy / 1000f);

            w = world2(network);
            timetaken += w.go(1000);
            fitness += 1000 - (w.distanceFromEnemy / 1000f);

            w = world3(network);
            timetaken += w.go(1000);
            fitness += 1000 - (w.distanceFromEnemy / 1000f);

            w = world4(network);
            timetaken += w.go(1000);
            fitness += 1000 - (w.distanceFromEnemy / 1000f);

            w = world5(network);
            timetaken += w.go(1000);
            fitness += 1000f - (w.distanceFromEnemy / 1000f);

            return fitness;
        }

        public double EvaluateNetwork(INetwork network)
        {
            return 1;
        }

        public string EvaluatorStateMessage
        {
            get { return ""; }
        }

        #endregion

        private double doEvaluationMulti(INetwork network)
        {
            float timetaken = 0;
            double fitness = 0;
            int startEnemy = 0;
            World w = null;
            switch (currentShape)
            {
                case Shapes.Triangle:
                    w = pointWorldVar(network, (float)Math.PI / 8.0f);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.goMulti(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;

                    w = pointWorldVar(network, 3 * (float)Math.PI / 8.0f);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.goMulti(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;
                    break;

                case Shapes.Diamond:
                    w = diamondWorldVar(network, 75);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.goMulti(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;

                    w = diamondWorldVar(network, 125);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.goMulti(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;
                    break;

                case Shapes.Square:
                    w = squareWorldVar(network, 75);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.goMulti(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;

                    w = squareWorldVar(network, 125);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.goMulti(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;
                    break;

                case Shapes.L:
                    w = lWorldVar(network, 75);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.goMulti(500);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;

                    w = lWorldVar(network, 125);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.goMulti(500);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;
                    break;

            }
            return fitness;
        }

        private double doEvaluation(INetwork network)
        {
            float timetaken = 0;
            double fitness = 0;
            int startEnemy=0;
            World w = null;
            switch (currentShape)
            {
                case Shapes.Triangle:

                    w = pointWorldVar(network, (float)Math.PI / 8.0f);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.go(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;

                    w = pointWorldVar(network, 3 * (float)Math.PI / 8.0f);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.go(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;
                    break;

                case Shapes.Diamond:
                    w = diamondWorldVar(network, 75);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.go(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;

                    w = diamondWorldVar(network, 125);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.go(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;
                    break;

                case Shapes.Square:
                    w = squareWorldVar(network, 75);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.go(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;

                    w = squareWorldVar(network, 125);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.go(1000);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;
                    break;

                case Shapes.L:
                    w = lWorldVar(network, 75);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.go(500);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;

                    w = lWorldVar(network, 125);
                    startEnemy = w.Enemy.Count;
                    timetaken = w.go(500);
                    fitness += 10000 * (startEnemy - w.Enemy.Count) + timetaken;
                    break;

            }
            return fitness;
        }

        public static World world1(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 5, 5, network));
            w.addEnemy(new Prey(350, 450, w.agentSize, w.agentSize));
            return w;
        }

        public static World world2(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 10, 10, network));
            w.addEnemy(new Prey(425, 400, w.agentSize, w.agentSize));
            return w;
        }

        public static World world3(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 5, 5,network));
            w.addEnemy(new Prey(500, 350, w.agentSize, w.agentSize));
            return w;
        }

        public static World world4(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 5, 5, network));
            w.addEnemy(new Prey(575, 400, w.agentSize, w.agentSize));
            return w;
        }

        public static World world5(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 5, 5, network));
            w.addEnemy(new Prey(650, 450, w.agentSize, w.agentSize));
            return w;
        }

        public static World squareWorldVar(INetwork network, float spacing)
        {
            float middleX = 500;
            float middleY = 250;
            World w = new World(network);


            addPredators(w);

            w.addEnemy(new Prey(middleX - 1.5f * spacing, middleY + 1.5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - .5f * spacing, middleY + 1.5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + .5f * spacing, middleY + 1.5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + 1.5f * spacing, middleY + 1.5f * spacing, w.agentSize, w.agentSize));

            w.addEnemy(new Prey(middleX - 1.5f * spacing, middleY + .5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 1.5f * spacing, middleY - .5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 1.5f * spacing, middleY - 1.5f * spacing, w.agentSize, w.agentSize));

            w.addEnemy(new Prey(middleX + 1.5f * spacing, middleY + .5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + 1.5f * spacing, middleY - .5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + 1.5f * spacing, middleY - 1.5f * spacing, w.agentSize, w.agentSize));

            w.addEnemy(new Prey(middleX - .5f * spacing, middleY - 1.5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + .5f * spacing, middleY - 1.5f * spacing, w.agentSize, w.agentSize));

            return w;

        }

        public static World lWorldVar(INetwork network, float spacing)
        {
            float middleX = 500;
            float middleY = 250;
            World w = new World(network);

            addPredators(w);

            w.addEnemy(new Prey(middleX - 1.5f * spacing, middleY + 1.5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 1.5f * spacing, middleY + .5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 1.5f * spacing, middleY - .5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 1.5f * spacing, middleY - 1.5f * spacing, w.agentSize, w.agentSize));

            w.addEnemy(new Prey(middleX - .5f * spacing, middleY - 1.5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + .5f * spacing, middleY - 1.5f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + 1.5f * spacing, middleY - 1.5f * spacing, w.agentSize, w.agentSize));

            return w;

        }

        public static World diamondWorldVar(INetwork network, float spacing)
        {
            float middleX = 500;
            float middleY = 200;
            World w = new World(network);

            addPredators(w);

            w.addEnemy(new Prey(middleX , middleY + 2f*spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + spacing, middleY + spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX -  spacing, middleY + spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 2f * spacing, middleY, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + 2f * spacing, middleY, w.agentSize, w.agentSize));

            w.addEnemy(new Prey(middleX, middleY - 2f*spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - spacing, middleY - spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + spacing, middleY - spacing, w.agentSize, w.agentSize));

            return w;

        }

        public static World pointWorldVar(INetwork network, float angle)
        {
            float middleX = 500;
            float bottomY = 400;
            float spacing = 150;
            World w = new World(network);

            addPredators(w);

            w.addEnemy(new Prey(middleX, bottomY, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + spacing * (float)Math.Cos(angle), bottomY - spacing * (float)Math.Sin(angle), w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - spacing * (float)Math.Cos(angle), bottomY - spacing * (float)Math.Sin(angle), w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + 2 * spacing * (float)Math.Cos(angle), bottomY - 2*spacing * (float)Math.Sin(angle), w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 2 * spacing * (float)Math.Cos(angle), bottomY - 2 * spacing * (float)Math.Sin(angle), w.agentSize, w.agentSize));

            return w;
        }
    }
}
