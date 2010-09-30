using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridSoccer.DPClient
{
    public class StateActionValueTable : IValueTable
    {
        private long m_stateLength = 0L;
        private int[] m_stateDims = null;
        private int m_numActions = 0;
        private double[,] m_valueTable = null;

        public StateActionValueTable(int numActions, params int[] stateDims)
        {
            m_stateDims = stateDims;
            m_numActions = numActions;

            m_stateLength = 1;
            foreach (int dim in stateDims)
                m_stateLength *= dim;

            m_valueTable = new double[m_stateLength, m_numActions];
        }

        public StateActionValueTable(ValueTableInitModes initMode, double value, int numActions, params int[] stateDims)
            : this(numActions, stateDims)
        {
            if (initMode == ValueTableInitModes.Constant)
            {
                for (int s = 0; s < m_stateLength; s++)
                    for (int a = 0; a < m_numActions; a++)
                        m_valueTable[s, a] = value;
            }
            else if(initMode == ValueTableInitModes.UniformRandom)
            {
                Random r = new Random();
                for (int s = 0; s < m_stateLength; s++)
                    for (int a = 0; a < m_numActions; a++)
                        m_valueTable[s, a] = ((r.NextDouble() * 2) - 1) * value;
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

        public int NumActions
        {
            get
            {
                return m_numActions;
            }
        }

        public double GetValue(int actInd, params int[] stateInds)
        {
            if(stateInds.Length != m_stateDims.Length)
            {
                throw new ArgumentException("The number of indices provided do not match the underlying array's dimensions");
            }

            long linInd = Utils.GetLinearIndex(stateInds, m_stateDims, m_stateLength);
            return m_valueTable[linInd, actInd];
        }

        public double GetValueForState(params int[] inds)
        {
            double maxValue = Double.MinValue;
            for (int a = 0; a < m_numActions; a++)
            {
                double curValue = GetValue(a, inds);
                if (curValue > maxValue)
                {
                    maxValue = curValue;
                }
            }

            return maxValue;
        }


        public int[] GetDimentionalState(int stateLinInd)
        {
            return Utils.GetDimentionalIndex(stateLinInd, m_stateDims);
        }

        public double GetValueLinear(int actInd, int stateLinInd)
        {
            return m_valueTable[stateLinInd, actInd];
        }

        /// <summary>
        /// Sets the new value and returns the old value.
        /// </summary>
        public double SetValue(double newValue, int actInd, params int[] stateInds)
        {
            if (stateInds.Length != m_stateDims.Length)
            {
                throw new ArgumentException("The number of indices provided do not match the underlying array's dimensions");
            }

            long linInd = Utils.GetLinearIndex(stateInds, m_stateDims, m_stateLength);
            double oldValue = m_valueTable[linInd, actInd];
            m_valueTable[linInd, actInd] = newValue;
            return oldValue;
        }

        public double SetValueLinear(double newValue, int actInd, int stateLinInd)
        {
            double oldValue = m_valueTable[stateLinInd, actInd];
            m_valueTable[stateLinInd, actInd] = newValue;
            return oldValue;
        }

    }
}
