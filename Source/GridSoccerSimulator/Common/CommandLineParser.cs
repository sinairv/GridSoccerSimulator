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

namespace GridSoccer.Common
{
    public class CommandLineParser
    {
        string[] m_args;
        public CommandLineParser(string[] args)
        {
            m_args = new string[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                m_args[i] = args[i].Trim();

                if (m_args[i].StartsWith("-"))
                    m_args[i] = m_args[i].ToLower();
            }
        }

        /// <summary>
        /// the only one
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public bool IsOptionProvided(string opt)
        {
            return (Find(opt) >= 0);
        }

        /// <summary>
        /// The pair 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool IsArgumentProvided(string arg, out string param)
        {
            int i = Find(arg);
            if (i < 0)
            {
                param = "";
                return false;
            }
            else if (i + 1 < m_args.Length)
            {
                param = m_args[i + 1];
                return (!param.StartsWith("-"));
            }
            else
            {
                param = "";
                return false;
            }
        }

        public bool IsIntArgumentProvided(string arg, out int n)
        {
            n = 0;
            string param;
            if (!IsArgumentProvided(arg, out param))
                return false;

            return Int32.TryParse(param, out n);
        }

        public bool IsDoubleArgumentProvided(string arg, out double d)
        {
            d = 0;
            string param;
            if (!IsArgumentProvided(arg, out param))
                return false;

            return Double.TryParse(param, out d);
        }

        private int Find(string str)
        {
            string key = str.Trim().ToLower();
            for (int i = 0; i < m_args.Length; ++i)
            {
                if (m_args[i] == key)
                    return i;
            }

            return -1;
        }
    }
}
