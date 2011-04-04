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
    public class PerformanceLogger
    {
        private string m_fileName;
        private StreamWriter m_sWriter;

        public bool Enabled { get; set; }

        public PerformanceLogger(string fileNameBase)
            : this(fileNameBase, true)
        {
        }

        public PerformanceLogger(string fileNameBase, bool printHeading)
        {
            Enabled = true;
            m_fileName = fileNameBase + String.Format("-{0:yyyy}{0:MM}{0:dd}-{0:HH}{0:mm}{0:ss}.log", DateTime.Now);
            try
            {
                m_sWriter = new StreamWriter(new FileStream(m_fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                m_sWriter.AutoFlush = true;
            }
            catch
            {
                Enabled = false;
            }

            if(printHeading)
                PrintHeading();
        }

        private void PrintHeading()
        {
            if (Enabled)
                m_sWriter.WriteLine("{0,-15} {1,-15} {2,-10} {3,-10} {4,-10}", "%AvgReward", "Reward", "OurScore", "OppScore", "ScoreDiff");
        }

        double m_avgRew = 0.0;
        long m_count = 0L;
        public void Log(int cycle, double reward, int ourScore, int oppScore)
        {
            m_avgRew = (m_avgRew * ((double)m_count / (m_count + 1))) + reward / (m_count + 1);
            m_count++;
            
            if (Enabled)
                m_sWriter.WriteLine("{0,-15:F05} {1,-15:F05} {2,-10} {3,-10} {4,-10}", 
                    m_avgRew, reward, ourScore, oppScore, ourScore - oppScore);
        }

        public void WriteLine(string str)
        {
            if (Enabled)
                m_sWriter.WriteLine(str);
        }

        public void Write(string str)
        {
            if (Enabled)
                m_sWriter.Write(str);
        }

        ~PerformanceLogger()
        {
            if (m_sWriter != null)
            {
                try
                {
                    m_sWriter.Flush();
                    m_sWriter.Close();
                }
                catch
                {
                }
                finally
                {
                    m_sWriter = null;
                }
            }
        }
    }
}
