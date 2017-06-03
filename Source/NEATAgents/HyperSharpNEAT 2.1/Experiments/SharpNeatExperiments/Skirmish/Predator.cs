using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.NeuralNetwork;


namespace SharpNeatLib.Experiments
{
    public class Predator : Agent
    {
        public float[] sensors;
        public float angledelta;
        public float halfangledelta;
        public List<Agent> sensedAgents = new List<Agent>();
        public FloatFastConcurrentNetwork brain;

        public Predator() : base()
        {
            radius = 200;
            sensors = new float[5];
            angledelta = (float)Math.PI / sensors.Length;
            halfangledelta = angledelta / 2.0f;
        }

        public Predator(float x, float y, float w, float h) : base(x, y, w, h)
        {
            radius = 200;
            sensors = new float[5];
            angledelta = (float)Math.PI / sensors.Length;
            halfangledelta = angledelta / 2.0f;
        }

        public Predator(float x, float y, float w, float h,INetwork network)
            : base(x, y, w, h, network)
        {
            radius = 200;
            sensors = new float[5];
            angledelta = (float)Math.PI / sensors.Length;
            halfangledelta = angledelta / 2.0f;
            brain = (FloatFastConcurrentNetwork)network;
        }

        public bool attack(Agent a)
        {
            //this function was much more complex, but I moved all the checks to other areas
            a.hitpoints--;
            return true;
        }

        //checks if the an agent is within striking distance of the predator
        public override bool isActionable(Agent a)
        {
            if (Utilities.Distance(a, this) > Utilities.predStrike)
                return false;
            return true;
        }

        //takes the left edge of the sensors in radians and checks if the given angle is inside that sensor
        private bool isInSensor(float left, float angle)
        {
            float tempAngleMin = left;
            float tempAngleMax = left + angledelta;

            if (tempAngleMin < 0)
            {
                tempAngleMin += Utilities.twoPi;
            }
            else if (tempAngleMin > Utilities.twoPi)
            {
                tempAngleMin -= Utilities.twoPi;
            }
            if (tempAngleMax < 0)
            {
                tempAngleMax += Utilities.twoPi;
            }
            else if (tempAngleMax > Utilities.twoPi)
            {
                tempAngleMax -= Utilities.twoPi;
            }

            if (tempAngleMax < tempAngleMin)//sensor spans the 0 line
            {
                if ((angle >= tempAngleMin && angle <= Utilities.twoPi) || (angle >= 0 && angle <= tempAngleMax))
                {
                    return true;
                }
            }
            else
            {
                if (angle >= tempAngleMin && angle <= tempAngleMax)
                {
                    return true;
                }
            }
            return false;
        }

        public void clearSensors()
        {
            for (int sense = 0; sense < sensors.Length; sense++)
            {
                sensors[sense] = 0;
            }
            sensedAgents.Clear();
        }

        public void fillSensorsFront(List<Prey> agents)
        {
            float testangle;
            float distance;
            
            for (int prey = 0; prey < agents.Count; prey++)
            {
                Prey a = agents[prey];

                
                distance = Utilities.Distance(this, a);

                //while going through prey, they save the closest predator to save time later
                if (distance < a.closestPredDist)
                {
                    a.closestPredDist = distance;
                    a.closestPred = this;
                }

                if (distance > radius)
                    continue;
               
                testangle = heading - Utilities.piOverTwo;
                float angle = (float)Math.Atan2(a.y - y, a.x - x);
                if (angle < 0)
                {
                    angle += Utilities.twoPi;
                }
                for (int j = 0; j < sensors.Length; j++, testangle += angledelta)
                {
                    if (isInSensor(testangle, angle))
                    {
                        //if the agent is in the middle sensor save for later
                        if (j == 2)
                            sensedAgents.Add(a);
                        float dist = 1 - (distance / (float)radius);
                        sensors[j] += dist * dist;
                        break;
                    }
                }
            }
        }

        //called for homogeneous agents, each one runs thier sensors through the network
        public void determineAction()
        {
            brain.SetInputSignals(sensors);
            brain.MultipleSteps(2);
            float[] outputs = new float[3];
            outputs[0] = (float)brain.GetOutputSignal(0);
            outputs[1] = (float)brain.GetOutputSignal(1);
            outputs[2] = (float)brain.GetOutputSignal(2);

            //the rest is the same as the heterogeneous, so call that fucntion
            determineAction(outputs);

            
        }

        //called by itself for heterogeneous agents, they alreay have their outputs from the timestep function
        public void determineAction(float[] outputs)
        {
            float left = outputs[0];
            float straight = outputs[1];
            float right = outputs[2];

            foreach (Agent a in sensedAgents)
                if (isActionable(a) && attack(a))
                {
                        return; //changed from break to make moving and attack exclusive actions
                }

            turn((float)(right - left) * World.turn);
            move((float)straight * World.step);
        }
    }
}
