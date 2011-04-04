using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SharpNeatLib.NeuralNetwork;
using SharpNeatExperiments.Skirmish;
using SharpNeatLib.Experiments;

namespace SkirmishCmdEvolution
{
    public class WorldSimulator
    {
        private int m_numAgents;
        public Shapes m_currentShape;

        public AutoResetEvent[] EventsNetworkReady;
        public AutoResetEvent[] EventsFitnessReady;

        public INetwork[] NetworksToEvaluate;

        public double Fitness = 0.0;

        public WorldSimulator(int numAgents, string shape)
        {
            m_numAgents = numAgents;

            try
            {
                m_currentShape = (Shapes)Enum.Parse(typeof(Shapes), shape, true);
                Console.WriteLine("Shape: {0}", m_currentShape);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid Shape Entered, Defaulting to Triangle");
                m_currentShape = Shapes.Triangle;
            }


            EventsFitnessReady = new AutoResetEvent[numAgents];
            EventsNetworkReady = new AutoResetEvent[numAgents];
            NetworksToEvaluate = new INetwork[numAgents];

            for(int i = 0; i < m_numAgents; i++)
            {
                EventsFitnessReady[i] = new AutoResetEvent(false);
                EventsNetworkReady[i] = new AutoResetEvent(false);
            }
        }

        private Thread m_mainThread;
        public void Start()
        {
            m_mainThread = new Thread(MainThread);
            m_mainThread.Start();
        }

        public void Stop()
        {
            if (m_mainThread.IsAlive)
            {
                m_mainThread.Abort();
            }
        }

        private void MainThread()
        {
            while (true)
            {
                for (int i = 0; i < m_numAgents; i++)
                {
                    this.EventsNetworkReady[i].WaitOne();
                }

                this.Fitness = DoSimulation();

                for (int i = 0; i < m_numAgents; i++)
                {
                    this.EventsFitnessReady[i].Set();
                }
            }
        }

        private double DoSimulation()
        {
            float timetaken = 0;
            double fitness = 0;
            int startEnemy = 0;
            World w = null;
            INetwork network = null;
            switch (m_currentShape)
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


        ~WorldSimulator()
        {
            Stop();
        }

        #region Simulation Stuff


        private double doEvaluationMulti(INetwork network)
        {
            float timetaken = 0;
            double fitness = 0;
            int startEnemy = 0;
            World w = null;
            switch (m_currentShape)
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
            int startEnemy = 0;
            World w = null;
            switch (m_currentShape)
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

        public void addPredators(World w)
        {
            w.addPlayer(new Predator(300, 500, w.agentSize, w.agentSize, this.NetworksToEvaluate[0]));
            w.addPlayer(new Predator(400, 500, w.agentSize, w.agentSize, this.NetworksToEvaluate[1]));
            w.addPlayer(new Predator(500, 500, w.agentSize, w.agentSize, this.NetworksToEvaluate[2]));
            w.addPlayer(new Predator(600, 500, w.agentSize, w.agentSize, this.NetworksToEvaluate[3]));
            w.addPlayer(new Predator(700, 500, w.agentSize, w.agentSize, this.NetworksToEvaluate[4]));

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

        public World world1(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 5, 5, network));
            w.addEnemy(new Prey(350, 450, w.agentSize, w.agentSize));
            return w;
        }

        public World world2(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 10, 10, network));
            w.addEnemy(new Prey(425, 400, w.agentSize, w.agentSize));
            return w;
        }

        public World world3(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 5, 5, network));
            w.addEnemy(new Prey(500, 350, w.agentSize, w.agentSize));
            return w;
        }

        public World world4(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 5, 5, network));
            w.addEnemy(new Prey(575, 400, w.agentSize, w.agentSize));
            return w;
        }

        public World world5(INetwork network)
        {
            World w = new World(network);
            w.addPlayer(new Predator(500, 500, 5, 5, network));
            w.addEnemy(new Prey(650, 450, w.agentSize, w.agentSize));
            return w;
        }

        public World squareWorldVar(INetwork network, float spacing)
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

        public World lWorldVar(INetwork network, float spacing)
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

        public World diamondWorldVar(INetwork network, float spacing)
        {
            float middleX = 500;
            float middleY = 200;
            World w = new World(network);

            addPredators(w);

            w.addEnemy(new Prey(middleX, middleY + 2f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + spacing, middleY + spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - spacing, middleY + spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 2f * spacing, middleY, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + 2f * spacing, middleY, w.agentSize, w.agentSize));

            w.addEnemy(new Prey(middleX, middleY - 2f * spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - spacing, middleY - spacing, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + spacing, middleY - spacing, w.agentSize, w.agentSize));

            return w;

        }

        public World pointWorldVar(INetwork network, float angle)
        {
            float middleX = 500;
            float bottomY = 400;
            float spacing = 150;
            World w = new World(network);

            addPredators(w);

            w.addEnemy(new Prey(middleX, bottomY, w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + spacing * (float)Math.Cos(angle), bottomY - spacing * (float)Math.Sin(angle), w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - spacing * (float)Math.Cos(angle), bottomY - spacing * (float)Math.Sin(angle), w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX + 2 * spacing * (float)Math.Cos(angle), bottomY - 2 * spacing * (float)Math.Sin(angle), w.agentSize, w.agentSize));
            w.addEnemy(new Prey(middleX - 2 * spacing * (float)Math.Cos(angle), bottomY - 2 * spacing * (float)Math.Sin(angle), w.agentSize, w.agentSize));

            return w;
        }

        #endregion
    }
}
