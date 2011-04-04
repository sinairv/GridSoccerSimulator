// Copyright (c) 2009 - 2011 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GridSoccer.RLAgentsCommon
{
    public static class ArrayExtensions
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

        public static int[] GetDimensions(this Array array)
        {
            int[] dims = new int[array.Rank];
            for (int i = 0; i < dims.Length; ++i)
            {
                dims[i] = array.GetLength(i);
            }
            return dims;
        }

        /// <summary>
        /// Slow Version
        /// </summary>
        public static object GetValueLinearly(this Array array, long index)
        {
            int[] dims = array.GetDimensions();
            return array.GetValueLinearly(index, dims);
        }

        /// <summary>
        /// Fast Version
        /// </summary>
        public static object GetValueLinearly(this Array array, long index, int[] dims)
        {
            int[] dimIndeces = GetDimentionalIndex(index, dims);
            return array.GetValue(dimIndeces);
        }

        /// <summary>
        /// Slow Version
        /// </summary>
        public static void SetValueLinearly(this Array array, long index, object value)
        {
            int[] dims = array.GetDimensions();
            array.SetValueLinearly(index, value, dims);
        }

        /// <summary>
        /// Fast Version
        /// </summary>
        public static void SetValueLinearly(this Array array, long index, object value, int[] dims)
        {
            int[] dimIndeces = GetDimentionalIndex(index, dims);
            array.SetValue(value, dimIndeces);
        }

        public static void SaveArrayContents(this Array array, TextWriter tw)
        {
            int len = array.Length;
            int[] dims = array.GetDimensions();
            object curObj;
            for(int i = 0; i < len; ++i)
            {
                curObj = array.GetValueLinearly(i, dims);
                if ((i + 1) % 20 == 1)
                {
                    tw.WriteLine();
                    tw.Write(curObj);
                }
                else
                {
                    tw.Write(String.Format(",{0}", curObj));
                }
            }
        }

        public static void LoadInt32ArrayContents(this Array array, TextReader tr)
        {
            int len = array.Length;
            int[] dims = array.GetDimensions();
            char[] splitChars = new char[] { ' ', ',', '\t', '\r', '\n' };

            string line; // holds the current line of the file
            int curIndex = 0; // holds the current linear index of the array
            int n; // holds the current int number read
            bool finishedReading = false;
            while ((line = tr.ReadLine()) != null)
            {
                line = line.Trim();

                if (String.IsNullOrEmpty(line) || line.StartsWith("%") || line.StartsWith("#"))
                    continue;

                string[] strInts = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

                foreach (string stri in strInts)
                {
                    if (!Int32.TryParse(stri, out n))
                        n = 0;
                    array.SetValueLinearly(curIndex, n, dims);
                    curIndex++;

                    if (curIndex >= len)
                    {
                        finishedReading = true;
                        break;
                    }
                }

                if (finishedReading)
                    break;
            }
        }


        public static void LoadDoubleArrayContents(this Array array, TextReader tr)
        {
            int len = array.Length;
            int[] dims = array.GetDimensions();
            char[] splitChars = new char[] { ' ', ',', '\t', '\r', '\n' };

            string line; // holds the current line of the file
            int curIndex = 0; // holds the current linear index of the array
            double d; // holds the current double number read
            bool finishedReading = false;
            while ((line = tr.ReadLine()) != null)
            {
                line = line.Trim();

                if (String.IsNullOrEmpty(line) || line.StartsWith("%") || line.StartsWith("#"))
                    continue;

                string[] strDoubles = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

                foreach (string strd in strDoubles)
                {
                    if (!Double.TryParse(strd, out d))
                        d = 0.0;
                    array.SetValueLinearly(curIndex, d, dims);
                    curIndex++;

                    if (curIndex >= len)
                    {
                        finishedReading = true;
                        break;
                    }
                }

                if (finishedReading)
                    break;
            }
        }
    }
}
