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
        {
            Enabled = true;
            m_fileName = fileNameBase + String.Format("-{0:yyyy}{0:MM}{0:dd}-{0:hh}{0:mm}{0:ss}.log", DateTime.Now);
            try
            {
                m_sWriter = File.CreateText(m_fileName);
                m_sWriter.AutoFlush = true;
            }
            catch
            {
                Enabled = false;
            }

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
                m_sWriter.Flush();
                m_sWriter.Close();
            }
        }
    }
}
