// Copyright (c) 2009 - 2010 
//  - Sina Iravanian <sina@sinairv.com>
//  - Sahar Araghi   <sahar_araghi@aut.ac.ir>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;

namespace GridSoccer.Common
{
    public class MathUtils
    {
        public static double GetDistancePointFromLine(double px, double py, double lx0, double ly0, double lx1, double ly1)
        {
            double dx = lx1 - lx0;
            double dy = ly1 - ly0;
            return Math.Abs(dx * (ly0 - py) - (lx0 - px) * dy) / Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetDistancePointFromLine(Position pt, Position l0, Position l1)
        {
            return GetDistancePointFromLine(pt.Col, pt.Row, l0.Col, l0.Row, l1.Col, l1.Row);
        }

        public static double GetDistancePointFromPoint(double x0, double y0, double x1, double y1)
        {
            return Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1));
        }

        public static double GetDistancePointFromPoint(Position pos0, Position pos1)
        {
            return GetDistancePointFromPoint(pos0.Col, pos0.Row, pos1.Col, pos1.Row);
        }

        public static bool IsPointBetweenTwoPoints(Position pt1, Position pt2, Position pt)
        {
            if (!IsPointInRectangle(pt1, pt2, pt))
                return false;

            if (pt.Col == pt1.Col && pt1.Col == pt2.Col)
                return (pt1.Row <= pt.Row && pt.Row <= pt2.Row) || (pt2.Row <= pt.Row && pt.Row <= pt1.Row);

            double m1 = (double)(pt.Row - pt1.Row);
            if (pt.Col != pt1.Col)
                m1 /= pt.Col - pt1.Col;
            else
                return false;

            double m2 = (double)(pt2.Row - pt.Row);
            if (pt2.Col != pt.Col)
                m2 /= pt2.Col - pt.Col;
            else
                return false;

            return m1 == m2;
        }

        public static bool IsPointInRectangle(Position pt1, Position pt2, Position pt)
        {
            int temp;
            int r1 = pt1.Row; int r2 = pt2.Row;
            if (r2 < r1) { temp = r1; r1 = r2; r2 = temp; }

            int c1 = pt1.Col; int c2 = pt2.Col;
            if (c2 < c1) { temp = c1; c1 = c2; c2 = temp; }

            return (r1 <= pt.Row && pt.Row <= r2 && c1 <= pt.Col && pt.Col <= c2);
        }



    }
}
