using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.NeuroRLClient
{
    public class ObservationHistory
    {
        public class Observation
        {
            public double[] Features { get; set; }
            public double[] QValues { get; set; }

            public Observation (double[] feats, double[] qvalues)
	        {
                this.Features = feats;
                this.QValues = qvalues;
	        }

            public override bool Equals(object obj)
            {
                if (Object.ReferenceEquals(this, obj))
                    return true;

                if (!(obj is Observation))
                {
                    return false;
                }

                var other = obj as Observation;

                if (this.Features.Length != other.Features.Length)
                    return false;

                for (int i = 0, icount = this.Features.Length; i < icount; i++)
                {
                    if (this.Features[i] != other.Features[i])
                        return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                int sum = 0;
                for (int i = 0, icount = this.Features.Length; i < icount; i++)
                {
                    sum += Features[i].GetHashCode();
                }

                return sum;
            }
        }

        private int m_maxLength = -1;

        private bool m_unique = false;

        private List<Observation> m_queueObs = null;
        private HashSet<Observation> m_keys = null;

        public ObservationHistory(int maxLength)
            : this(maxLength, false)
        {
        }

        public ObservationHistory(int maxLength, bool unique)
        {
            m_unique = unique;
            m_maxLength = maxLength;
            m_queueObs = new List<Observation>(Math.Min(m_maxLength, 1000));
            if (m_unique)
                m_keys = new HashSet<Observation>();
        }

        public void Clear()
        {
            m_queueObs.Clear();
            if (m_keys != null)
                m_keys.Clear();
        }

        public void Add(double[] feats, double[] qValues)
        {
            if (m_queueObs.Count >= m_maxLength)
            {
                Dequeue();
            }

            var obs = new Observation(feats, qValues);
            if (m_unique)
            {
                if (m_keys.Contains(obs))
                {
                    int i = m_queueObs.IndexOf(obs);
                    m_queueObs[i].QValues = qValues;
                }
                else
                {
                    m_queueObs.Add(obs);
                    m_keys.Add(obs);
                }
            }
            else
            {
                m_queueObs.Add(obs);
            }


        }

        public Observation this[int i]
        {
            get
            {
                return m_queueObs[i];
            }
        }

        public int Count
        {
            get
            {
                return m_queueObs.Count;
            }
        }

        public Observation Dequeue()
        {
            Observation obs = m_queueObs[0];
            m_queueObs.RemoveAt(0);
            if(m_unique)
                m_keys.Remove(obs);
            return obs;
        }
    }
}
