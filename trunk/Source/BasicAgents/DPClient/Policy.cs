using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.DPClient
{
    public class Policy
    {
        private long m_stateLength = 0L;
        private int[] m_stateDims = null;
        private int[] m_valueTable = null;

        public Policy(params int[] stateDims)
        {
            m_stateDims = stateDims;

            m_stateLength = 1;
            foreach (int dim in stateDims)
                m_stateLength *= dim;

            m_valueTable = new int[m_stateLength];
        }

        public Policy(ValueTableInitModes initMode, int value, params int[] stateDims)
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
                    m_valueTable[s] = (int)(r.NextDouble() * value);
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


        public int GetValue(params int[] stateInds)
        {
            if(stateInds.Length != m_stateDims.Length)
            {
                throw new ArgumentException("The number of indices provided do not match the underlying array's dimensions");
            }

            long linInd = Utils.GetLinearIndex(stateInds, m_stateDims, m_stateLength);
            return m_valueTable[linInd];
        }


        public int GetValueForState(params int[] inds)
        {
            return GetValue(inds);
        }


        public int[] GetDimentionalState(int stateLinInd)
        {
            return Utils.GetDimentionalIndex(stateLinInd, m_stateDims);
        }

        public int GetValueLinear(int stateLinInd)
        {
            if (stateLinInd < 0)
                return 0;

            return m_valueTable[stateLinInd];
        }

        /// <summary>
        /// Sets the new value and returns the old value.
        /// </summary>
        public int SetValue(int newAction, params int[] stateInds)
        {
            if (stateInds.Length != m_stateDims.Length)
            {
                throw new ArgumentException("The number of indices provided do not match the underlying array's dimensions");
            }

            long linInd = Utils.GetLinearIndex(stateInds, m_stateDims, m_stateLength);
            var oldValue = m_valueTable[linInd];
            m_valueTable[linInd] = newAction;
            return oldValue;
        }

        public int SetValueLinear(int newValue, int stateLinInd)
        {
            var oldValue = m_valueTable[stateLinInd];
            m_valueTable[stateLinInd] = newValue;
            return oldValue;
        }
    }
}
