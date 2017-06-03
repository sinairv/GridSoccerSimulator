using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.NeuralNetwork;


namespace SharpNeatLib.Experiments
{
    public class Prey : Agent
    {
        public Predator closestPred = null;
        public float closestPredDist = float.MaxValue;

        public Prey() : base()
        {
            radius = 50;
        }
        public Prey(float x, float y, float w, float h) : base(x, y, w, h)
        {
            radius = 50;
        }
        public Prey(float x, float y, float w, float h, INetwork network)
            : base(x, y, w, h)
        {
            radius = 50;
        }

        //face the opposite direction and flee if the predator is closer than the radius
        public void determineAction()
        {
            if (closestPredDist <= radius)
            {
                setHeading();
                move(World.step);
            }
            closestPred = null;
            closestPredDist = float.MaxValue;
        }

        //forces the prey to face opposite the predator
        private void setHeading()
        {
            float xDist = closestPred.x - x;
            float yDist = closestPred.y - y;

            float desiredAngle = (float)Math.Atan2(yDist, xDist);
            desiredAngle += (float)Math.PI;
            if (desiredAngle < 0)
                desiredAngle += Utilities.twoPi;
            if (desiredAngle > Utilities.twoPi)
                desiredAngle -= Utilities.twoPi;
            heading = desiredAngle;
        }
    }
}
