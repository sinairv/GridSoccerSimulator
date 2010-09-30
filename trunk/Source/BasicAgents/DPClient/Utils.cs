using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.DPClient
{
    public class Utils
    {
        public static long GetLinearIndex(int[] indices, int[] dims, long length)
        {
            long result = 0L;
            long coef = length;

            for (int i = 0; i < indices.Length - 1; ++i)
            {
                coef /= dims[i];
                result += indices[i] * coef;
            }

            result += indices[indices.Length - 1];
            return result;
        }

        public static int[] GetDimentionalIndex(long linInd, int[] dims)
        {
            int[] result = new int[dims.Length];

            int d = (int)linInd;

            for (int n = dims.Length - 1; n > 0; n--)
            {
                result[n] = d % dims[n];
                d = (d - result[n]) / dims[n];
            }

            result[0] = d;
            return result;
        }


    }
}
