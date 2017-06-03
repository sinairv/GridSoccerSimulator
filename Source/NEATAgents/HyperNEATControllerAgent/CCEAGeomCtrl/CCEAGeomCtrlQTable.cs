using System;
using GridSoccer.RLAgentsCommon;
using GridSoccer.Common;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeuralNetwork;
using GridSoccer.NEATAgentsBase;

namespace GridSoccer.HyperNEATControllerAgent.CCEAGeomCtrl
{
    /// <summary>
    /// This class is left INCOMPLETE!
    /// </summary>
    public class CCEAGeomCtrlQTable : NeatQTableBase
    {
        private bool loadingBehaviour = false;
        private INetwork m_loadedNetworkBallOwner = null;
        private INetwork m_loadedNetworkNotBallOwner = null;

        public CCEAGeomCtrlQTable(RLClientBase client, int numTeammates, int myUnum)
            : base(client, numTeammates, myUnum, false)
        {
            //if (Program.LoadingGenome)
            //{
            //    try
            //    {
            //        XmlDocument doc = new XmlDocument();
            //        doc.Load(Program.LoadedGenomePath);
            //        var readCPPN = XmlNeatGenomeReaderStatic.Read(doc);

            //        var substrateBallOwner = new TwoLayerSandwichSubstrate(Rows, Cols + 2,
            //            NeatExpParams.AddBiasToSubstrate, HyperNEATParameters.substrateActivationFunction, 0);
            //        var substrateNotBallOwner = new TwoLayerSandwichSubstrate(Rows, Cols + 2,
            //            NeatExpParams.AddBiasToSubstrate, HyperNEATParameters.substrateActivationFunction,
            //            NeatExpParams.AddBiasToSubstrate ? 2 : 1);

            //        SharpNeatLib.NeatGenome.NeatGenome tempGenomeBallOwner = substrateBallOwner.GenerateGenome(readCPPN.Decode(HyperNEATParameters.substrateActivationFunction));
            //        m_loadedNetworkBallOwner = tempGenomeBallOwner.Decode(HyperNEATParameters.substrateActivationFunction);

            //        SharpNeatLib.NeatGenome.NeatGenome tempGenomeNotBallOwner = substrateNotBallOwner.GenerateGenome(readCPPN.Decode(HyperNEATParameters.substrateActivationFunction));
            //        m_loadedNetworkNotBallOwner = tempGenomeNotBallOwner.Decode(HyperNEATParameters.substrateActivationFunction);

            //        loadingBehaviour = true;
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //        loadingBehaviour = false;
            //    }

            //}

        }

        public override IExperiment CreateExperiment()
        {
            // if the substrate contains bias nodes, then we should
            // have CPPNs with 2 outputs
            return new CCEAGeomCtrlExperiment(this, 4,
                NeatExpParams.AddBiasToSubstrate ? 4 : 2);
        }


        // maximum signal strength when a player is not ball owner
        private const double m_maxNBOSignal = 0.4;

        public override double[] GetQValuesForNetwork(INetwork networkBallOwner, INetwork networkNotBallOwner, State s)
        {
            if (this.loadingBehaviour)
            {
                networkBallOwner = m_loadedNetworkBallOwner;
                networkNotBallOwner = m_loadedNetworkNotBallOwner;
            }


            INetwork networkToEval = networkBallOwner;

            if (!s.AmIBallOwner)//(!s.AreWeBallOwner)
                networkToEval = networkNotBallOwner;

            networkToEval.ClearSignals();
            float[] inputSignals = new float[networkToEval.InputNeuronCount];

            // No need to do this, it seems that ClearSignals sets all inputs to 0.0

            //for (int i = 0, len = network.InputNeuronCount; i < len; i++)
            //{
            //    network.SetInputSignals() = 0.0;
            //}

            // add info of players to the input signal array
            for (int oi = 0, oicount = s.OppPlayersList.Count; oi < oicount; oi++)
            {
                var pos = s.OppPlayersList[oi].Position;
                var posDiff = pos - s.Me.Position;
                int visionIdx = PosDiffToVisionIndex(posDiff.Row, posDiff.Col);
                double dist = PosDiffToDistance(posDiff.Row, posDiff.Col);
                double signalVal = (1.0 - dist) * m_maxNBOSignal;
                if (s.OppPlayersList[oi].IsBallOwner)
                    signalVal += 0.5;

                // because the player belongs to the opp team
                signalVal *= -1.0;

                if (Math.Abs(signalVal) > Math.Abs(inputSignals[visionIdx]))
                    inputSignals[visionIdx] = (float)signalVal;
            }

            // add info for ours
            for (int ti = 0, ticount = s.OurPlayersList.Count; ti < ticount; ti++)
            {
                var pos = s.OurPlayersList[ti].Position;
                var posDiff = pos - s.Me.Position;
                int visionIdx = PosDiffToVisionIndex(posDiff.Row, posDiff.Col);
                double dist = PosDiffToDistance(posDiff.Row, posDiff.Col);
                double signalVal = (1.0 - dist) * m_maxNBOSignal;
                if (s.OurPlayersList[ti].IsBallOwner)
                    signalVal += 0.5;

                // because the player belongs to our team
                //signalVal *= +1.0;

                if (Math.Abs(signalVal) > Math.Abs(inputSignals[visionIdx]))
                    inputSignals[visionIdx] = (float)signalVal;
            }

            // TODO: we can add a node in the center and set it to 0.5 
            // if I am ball owner, and 0.0 if otherwise

            //// add info for me
            //do // just create a block
            //{
            //    var pos = s.Me.Position;
            //    var posDiff = pos - s.Me.Position;
            //    int visionIdx = PosDiffToVisionIndex(posDiff.Row, posDiff.Col);
            //    double dist = PosDiffToDistance(posDiff.Row, posDiff.Col);
            //    double signalVal = (1.0 - dist) * m_maxNBOSignal;
            //    if (s.Me.IsBallOwner)
            //        signalVal += 0.5;

            //    // because the player belongs to our team
            //    //signalVal *= +1.0;

            //    if (Math.Abs(signalVal) > Math.Abs(inputSignals[visionIdx]))
            //        inputSignals[visionIdx] = (float)signalVal;
            //} while (false);

            networkToEval.SetInputSignals(inputSignals);

            //if(network.RelaxNetwork(2, 0.0))
            networkToEval.MultipleSteps(2);

            // See which position gets the maximum activation
            var maxOutput = Single.MinValue;
            int maxOutIndex = -1;
            for (int osi = 0, oscount = networkToEval.OutputNeuronCount; osi < oscount; osi++)
            {
                var curValue = networkToEval.GetOutputSignal(osi);
                if (curValue > maxOutput)
                {
                    maxOutput = curValue;
                    maxOutIndex = osi;
                }
            }

            // Now interpret the position of max activation

            int hsAct = InterpretHighestSignal(s, maxOutIndex);

            double[] qValues = new double[m_numActions];
            qValues[hsAct] = 1.0;
            return qValues;
        }

        private double PosDiffToDistance(int dr, int dc)
        {
            double relDr = (double)dr / Rows;
            double relDc = (double)dc / Cols;

            double dist = Math.Sqrt((relDc * relDc) + (relDr * relDr));
            return dist;
        }

        /*
         *      *  2  *
         *        * *
                3  O  1
         *        * *
         *      *  0  *
         * */ 
        private int PosDiffToVisionIndex(int dr, int dc)
        {
            if (dr >= dc) // it is either 0 or 3
            {
                if (dr > -dc) // it is 0
                {
                    return 0;
                }
                else
                {
                    return 3;
                }
            }
            else // it is either 1 or 2
            {
                if (dr > -dc)
                    return 1;
                else
                    return 2;
            }
        }
       
        /// <summary>
        /// i is 0-based, but r and c are 1-based
        /// </summary>
        private void SignalIndexToRowCol(int i, out int r, out int c)
        {
            // the "+2" and "c-=1" are for the mapping between 
            // real field coords and the field with padded 2 cols
            r = (i / (m_cols + 2)) + 1;
            c = (i % (m_cols + 2)) + 1;
            c -= 1; 
        }

        /// <summary>
        /// return value is 0-based, but r and c are 1-based
        /// </summary>
        private int RowColToSignalIndex(int r, int c)
        {
            c++; // now c referes to the 1-based col index in the padded field
            return ((r - 1) * (m_cols + 2)) + (c - 1);
        }

        /*         7
         *      *  3  *
         *        * *
              8 4  0  2 6
         *        * *
         *      *  1  *
         *         5    
         * 
         * */ 
        private int InterpretHighestSignal(State s, int maxOutIndex)
        {
            // if I am not a ballowner then I cannot pass
            if(!s.AmIBallOwner && maxOutIndex >= 5)
                maxOutIndex -= 4;

            if (0 <= maxOutIndex && maxOutIndex <= 4)
            {
                ActionTypes actType = ActionTypes.Hold;
                switch (maxOutIndex)
                {
                    case 0:
                        actType = ActionTypes.Hold;
                        break;
                    case 1:
                        actType = ActionTypes.MoveSouth;
                        break;
                    case 2:
                        actType = ActionTypes.MoveEast;
                        break;
                    case 3:
                        actType = ActionTypes.MoveNorth;
                        break;
                    case 4:
                        actType = ActionTypes.MoveWest;
                        break;
                }
                return SoccerAction.GetIndexFromAction(new SoccerAction(actType), Params.MoveKings, MyUnum);
            }
            else if (5 <= maxOutIndex && maxOutIndex <= 8)
            {
                Double minDist = Double.MaxValue;
                PlayerInfo pTeammateToPass = null;

                foreach (var pi in s.OurPlayersList)
                {
                    var posDiff = pi.Position - s.Me.Position;
                    int posRegIdx = PosDiffToVisionIndex(posDiff.Row, posDiff.Col) + 5;
                    if (posRegIdx == maxOutIndex)
                    {
                        double dist = posDiff.Row * posDiff.Row + posDiff.Col * posDiff.Col;
                        if (dist < minDist)
                        {
                            minDist = dist;
                            pTeammateToPass = pi;
                        }
                    }
                }

                if (pTeammateToPass != null)
                {
                    return SoccerAction.GetIndexFromAction(new SoccerAction(ActionTypes.Pass, pTeammateToPass.Unum), Params.MoveKings, MyUnum);
                }
                else
                {
                    // if no proper passees were found then just make a movement accordingly
                    ActionTypes actType = ActionTypes.Hold;
                    switch (maxOutIndex)
                    {
                        case 5:
                            actType = ActionTypes.MoveSouth;
                            break;
                        case 6:
                            actType = ActionTypes.MoveEast;
                            break;
                        case 7:
                            actType = ActionTypes.MoveNorth;
                            break;
                        case 8:
                            actType = ActionTypes.MoveWest;
                            break;
                        default:
                            throw new Exception("Unknown max out index");
                    }

                    return SoccerAction.GetIndexFromAction(new SoccerAction(actType), Params.MoveKings, MyUnum);
                }
            }
            else
            {
                throw new Exception("Cannot interpret the index of the highest action!");
            }
        }

        //private int InterpretHighestSignal(State s, int maxr, int maxc)
        //{
        //    var myPos = s.Me.Position;
        //    var rPos = new Position(maxr, maxc);
        //    int rDiff = maxr - myPos.Row;
        //    int cDiff = maxc - myPos.Col;

        //    foreach (var player in s.OurPlayersList)
        //    {
        //        if (player.Position.Equals(rPos))
        //            return SoccerAction.GetIndexFromAction(new SoccerAction(ActionTypes.Pass, player.Unum), 
        //                Params.MoveKings, this.MyUnum);
        //    }

        //    if (rDiff == 0 && cDiff == 0)
        //    {
        //        return SoccerAction.GetIndexFromAction(new SoccerAction(ActionTypes.Hold), Params.MoveKings, this.MyUnum);
        //    }
        //    else if (rDiff == 0 || (Math.Abs(rDiff) <= Math.Abs(cDiff)))
        //    {
        //        return SoccerAction.GetIndexFromAction(new SoccerAction(
        //            cDiff > 0 ? ActionTypes.MoveEast : ActionTypes.MoveWest
        //            ), Params.MoveKings, this.MyUnum);
        //    }
        //    else if (cDiff == 0 || (Math.Abs(rDiff) > Math.Abs(cDiff)))
        //    {
        //        return SoccerAction.GetIndexFromAction(new SoccerAction(
        //            rDiff > 0 ? ActionTypes.MoveSouth : ActionTypes.MoveNorth
        //            ), Params.MoveKings, this.MyUnum);
        //    }

        //    return 0;
        //}

    }
}
