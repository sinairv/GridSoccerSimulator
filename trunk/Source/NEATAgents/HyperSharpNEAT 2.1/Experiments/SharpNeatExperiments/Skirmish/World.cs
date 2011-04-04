using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
    public class World : Drawable
    {
        public List<Prey> removeThese = new List<Prey>();
        public List<Predator> Player=new List<Predator>();
        public List<Prey> Enemy=new List<Prey>();
        public static int step = 5;
        public static float turn = (float)Math.PI / 5.0f;
        public int agentSize = 10;
        public int time = 0;
        public static float size = 0;

        // bigBrain is the network that models all the agents.
        public FloatFastConcurrentNetwork bigBrain;
        public float distanceFromEnemy = 0;

        public World() 
            : base(0,0,1000,1000)
        {
            color = System.Drawing.Brushes.White;
            size = height;
        }

        public World(INetwork network)
            : this()
        {
            if(network != null)
                bigBrain = (FloatFastConcurrentNetwork)network;
        }


        public override void Draw(Graphics g)
        {
            g.FillRectangle(color, x, y, width, height);
        }

        public void addPlayer(Predator player)
        {
            Player.Add(player);
        }

        public void addEnemy(Prey enemy)
        {
            Enemy.Add(enemy);
        }

        public float go(int howLong)
        {
            time = 0;
            while (timeStep() && time < howLong)
            {
                time++;
            }
            if (Enemy.Count != 0)
                time = howLong;

            return howLong-time;

        }

        public float goMulti(int howLong)
        {
            time = 0;
            while (timeStepMulti() && time < howLong)
            {
                time++;
            }
            if (Enemy.Count != 0)
                time = howLong;

            return howLong-time;

        }

        public bool timeStep()
        {
            int predatorCount = Player.Count;
            if (Enemy.Count == 0)
                return false;
            for (int pred = 0; pred < predatorCount; pred++)
            {
                Predator a = Player[pred];
                a.clearSensors();
                a.fillSensorsFront(Enemy);
            }

            //if (SkirmishNetworkEvaluator.trainingSeed) 
            //distanceFromEnemy += Utilities.Distance(Enemy[0], Player[0]);

            for (int pred = 0; pred < predatorCount; pred++)
                Player[pred].determineAction();

            int preyCount = Enemy.Count;
            for (int prey = 0; prey < preyCount; prey++)
            {
                //mark the dead guys for deletion
                if (Enemy[prey].hitpoints <= 0)
                {
                    removeThese.Add(Enemy[prey]);
                    continue;
                }

                Enemy[prey].determineAction();

            }

            foreach (Prey a in removeThese)
                Enemy.Remove(a);
            removeThese.Clear();
            return true;
        }


        public bool timeStepMulti()
        {
            int predCount = Player.Count;
            float[] inputs;
            inputs = new float[Player.Count * 5];
            if (Enemy.Count == 0)
                return false;
            

            //fill the predators' sensors and then copy those inputs to a big array that will input to the big ANN
            for(int pred=0;pred<predCount;pred++)
            {
                Player[pred].clearSensors();
                
                Player[pred].fillSensorsFront(Enemy);

                //Here we assume 5 sensors per predator
                for (int sense = 0; sense < 5; sense++)
                {
                    inputs[sense + pred * 5] = Player[pred].sensors[sense];
                }
            }
            bigBrain.SetInputSignals(inputs);
            bigBrain.MultipleSteps(2);
            float[] outputs=new float[3];


            for (int agent = 0; agent < predCount; agent++)
            {
                outputs[0] = (float)bigBrain.GetOutputSignal(0 + agent * 3);
                outputs[1] = (float)bigBrain.GetOutputSignal(1 + agent * 3);
                outputs[2] = (float)bigBrain.GetOutputSignal(2 + agent * 3);
                (Player[agent]).determineAction(outputs);
                
                //If you're running the visualization, uncomment this line otherwise the sensor displays will lag by 1 timestep
                //pred.fillSensorsFront(Enemy);
                
            }
            int preyCount=Enemy.Count;
            for(int prey=0;prey<preyCount;prey++)
            {
                //mark the dead guys for deletion
                if (Enemy[prey].hitpoints <= 0)
                {
                    removeThese.Add(Enemy[prey]);
                    continue;
                }

                Enemy[prey].determineAction();
            }

            foreach (Prey a in removeThese)
                Enemy.Remove(a);
            removeThese.Clear();
            return true;
        }


        public void drawWorld(System.Drawing.Graphics g, bool drawPie)
        {
            System.Drawing.Brush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            Draw(g);
            foreach (Predator a in Player)
            {
                if (drawPie)
                {
                    float arc = 180F / a.sensors.Length;
                    float f = a.heading * (360f / (2 * (float)Math.PI)) - 90;
                    for (int i = 0; i < a.sensors.Length; f += arc, i++)
                    {
                        if (a.sensors[i] > 0)
                        {
                            ((SolidBrush)myBrush).Color = Color.FromArgb(255, (byte)(255 * (1 - a.sensors[i])), (byte)(255 * (1 - a.sensors[i])));
                            g.FillPie(myBrush, a.x - a.radius, Utilities.shiftDown+(a.y - a.radius), a.radius * 2, a.radius * 2, f, arc);
                            //g.DrawPie(Pens.Black, a.x - a.radius, a.y - a.radius, a.radius * 2, a.radius * 2, f, arc);
                        }
                        else
                            g.DrawPie(Pens.Black, a.x - a.radius, Utilities.shiftDown + (a.y - a.radius), a.radius * 2, a.radius * 2, f, arc);
                        

                    }
                }
                System.Drawing.Pen p=new Pen(System.Drawing.Color.Red,5f);
                g.DrawLine(p, a.x, Utilities.shiftDown + a.y, a.x + (float)Math.Cos(a.heading) * (25), Utilities.shiftDown + a.y + (float)Math.Sin(a.heading) * (25));
                a.Draw(g);
            }
            foreach(Prey p in Enemy)
            {
                p.Draw(g);
            }
        }
    }
}
