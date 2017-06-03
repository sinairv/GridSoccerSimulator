#define EUCLID  //Swap between eculidean and manhattan, manhattan is WAY faster but the experiment is not set up for it (sensor values go over 1)

using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNeatLib.Experiments
{
    class Utilities
    {
        public const float twoPi = 2 * (float)Math.PI;
        public const float piOverTwo = (float)Math.PI / 2.0f;
        public static Random r = new Random();
        public static int shiftDown = 20;
        public static int predStrike = 25;

        public static float Distance(Drawable a, Drawable b)
        {
            float xDist = a.x - b.x;
            float yDist = a.y - b.y;
#if EUCLID
            return (float)Math.Sqrt(xDist*xDist + yDist*yDist);
#else
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
#endif
        }
    }
}
