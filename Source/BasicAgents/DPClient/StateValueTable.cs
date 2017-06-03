using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.DPClient
{
    public class StateValueTable : IValueTable
    {
        private long m_stateLength = 0L;
        private int[] m_stateDims = null;
        private double[] m_valueTable = null;

        public StateValueTable(params int[] stateDims)
        {
            m_stateDims = stateDims;

            m_stateLength = 1;
            foreach (int dim in stateDims)
                m_stateLength *= dim;

            m_valueTable = new double[m_stateLength];
        }

        public StateValueTable(ValueTableInitModes initMode, double value, params int[] stateDims)
            : this(stateDims)
        {
            if (initMode == ValueTableInitModes.Constant)
            {
                for (int s = 0; s < m_stateLength; s++)
                    m_valueTable[s] = value;
            }
            else if (initMode == ValueTableInitModes.UniformRandom)
            {
                Random r = new Random();
                for (int s = 0; s < m_stateLength; s++)
                    m_valueTable[s] = ((r.NextDouble() * 2) - 1) * value;
            }
        }

        public int[] StateDimensions
        {
            get
            {
                return m_stateDims;
            }
        }

        public long NumStates
        {
            get
            {
                return m_stateLength;
            }
        }


        public double GetValue(params int[] stateInds)
        {
            if(stateInds.Length != m_stateDims.Length)
            {
                throw new ArgumentException("The number of indices provided do not match the underlying array's dimensions");
            }

            long linInd = Utils.GetLinearIndex(stateInds, m_stateDims, m_stateLength);
            return m_valueTable[linInd];
        }


        public double GetValueForState(params int[] inds)
        {
            return GetValue(inds);
        }


        public int[] GetDimentionalState(int stateLinInd)
        {
            return Utils.GetDimentionalIndex(stateLinInd, m_stateDims);
        }

        public double GetValueLinear(int stateLinInd)
        {
            if (stateLinInd < 0)
                return 0.0;

            return m_valueTable[stateLinInd];
        }

        /// <summary>
        /// Sets the new value and returns the old value.
        /// </summary>
        public double SetValue(double newValue, params int[] stateInds)
        {
            if (stateInds.Length != m_stateDims.Length)
            {
                throw new ArgumentException("The number of indices provided do not match the underlying array's dimensions");
            }

            long linInd = Utils.GetLinearIndex(stateInds, m_stateDims, m_stateLength);
            double oldValue = m_valueTable[linInd];
            m_valueTable[linInd] = newValue;
            return oldValue;
        }

        public double SetValueLinear(double newValue, int stateLinInd)
        {
            double oldValue = m_valueTable[stateLinInd];
            m_valueTable[stateLinInd] = newValue;
            return oldValue;
        }
    }
}
