using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.NeuralNetwork;


namespace SharpNeatLib.Experiments
{
    public abstract class Agent : Drawable
    {
        public float heading;
        public float radius=125;
        public int hitpoints = 1;

        public Agent() : base()
        {
            color = System.Drawing.Brushes.Blue;
            heading = 0;
        }
        public Agent(float x, float y, float w, float h) : base(x, y, w, h)
        {
            color = System.Drawing.Brushes.Blue;
            heading = (float)(3*Math.PI/2.0);
        }
        public Agent(float x, float y, float w, float h, INetwork n)
            : base(x, y, w, h)
        {
            color = System.Drawing.Brushes.Black;
            heading = (float)(3 * Math.PI / 2.0);
        }

        public void turn(float radians)
        {
            heading += radians;
            //keep the heading between 0 and 2PI
            if(heading>=Utilities.twoPi)
                heading -= Utilities.twoPi;
            if (heading < 0)
                heading += Utilities.twoPi;
        }

        public void move(float speed)
        {
            x += speed * (float) Math.Cos(heading);
            y += speed * (float) Math.Sin(heading);
        }

        public virtual bool isActionable(Agent a)
        {
            return Utilities.Distance(a, this) < radius;
        }
    }
}
