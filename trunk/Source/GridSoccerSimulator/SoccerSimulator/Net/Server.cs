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
using System.Text;
using System.Net.Sockets;
using System.Net;
using GridSoccer.Simulator.Properties;
using GridSoccer.Common;

namespace GridSoccer.Simulator.Net
{
    public class Server
    {
        #region Fields

        readonly TcpListener m_listener = null;
        readonly List<ClientInfo> m_tempClients = new List<ClientInfo>();
        readonly ClientInfo[] m_connectedClients;

        readonly SoccerSimulator m_simulator = null;

        #endregion

        #region Constructors

        public Server(SoccerSimulator sim, int port)
        {
            this.m_simulator = sim;
            m_connectedClients = new ClientInfo[Settings.Default.MaxPlayers * 2];

            m_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Settings.Default.PortNumber);
            m_listener.Start();
        }

        #endregion

        #region Methods

        public bool HasConnectionRequest()
        {
            return m_listener.Pending();
        }

        public void AcceptConnection()
        {
            Console.WriteLine("A Client Connected...");

            TcpClient client = m_listener.AcceptTcpClient();
            client.NoDelay = true;

            m_tempClients.Add(new ClientInfo() { PlayerIndex = -1, TcpClient = client });
        }

        public void StopListeningForNewConnections()
        {
            m_listener.Stop();
        }

        private static string GetSettingsMessage()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(" ({0} {1})", "rows", Settings.Default.NumRows);
            sb.AppendFormat(" ({0} {1})", "cols", Settings.Default.NumCols);
            sb.AppendFormat(" ({0} {1})", "goal-width", Settings.Default.GoalWidth);
            sb.AppendFormat(" ({0} {1})", "pass-dist", Settings.Default.PassableDistance);
            sb.AppendFormat(" ({0} {1})", "visible-dist", Settings.Default.VisibleRadius);
            sb.AppendFormat(" ({0} {1})", "min-players", Settings.Default.MinPlayers);
            sb.AppendFormat(" ({0} {1})", "max-players", Settings.Default.MaxPlayers);

            return String.Format("(settings{0})", sb);
        }

        public void FreeTempClients()
        {
            for (int i = m_tempClients.Count - 1; i >= 0; --i)
            {
                m_tempClients[i].Close();
            }

            m_tempClients.Clear();
        }

        public void CheckTempClients()
        {
            for (int i = m_tempClients.Count - 1; i >= 0; --i)
            {
                try
                {
                    if (m_tempClients[i].DataAvailable)
                    {
                        string str = m_tempClients[i].ReadString();

                        IMessageInfo mi = MessageParser.ParseInputMessage(str);
                        if (mi.MessageType == MessageTypes.Init)
                        {
                            var initmsg = mi as InitMessage;
                            int pi = m_simulator.AddPlayer(initmsg.TeamName, initmsg.UNum);
                            if (pi >= 0)
                            {
                                m_tempClients[i].WriteString(String.Format("(init {0} ok)", initmsg.TeamName == m_simulator.LeftTeamName ? "l" : "r"));
                                m_tempClients[i].WriteString(GetSettingsMessage());
                                m_connectedClients[pi] = m_tempClients[i];
                            }
                            else
                            {
                                m_tempClients[i].WriteString("(init error)");
                                m_tempClients[i].Close();
                            }

                            m_tempClients.RemoveAt(i);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public void SendAllPlayers(string msg)
        {
            int count = m_simulator.LeftPlayersCount + m_simulator.RightPlayersCount;
            for (int i = 0; i < count; ++i)
            {
                m_connectedClients[i].WriteString(msg);
            }
        }

        public void CheckConnectedClientsWaitingForAll()
        {
            int count = m_simulator.LeftPlayersCount + m_simulator.RightPlayersCount;

            bool[] recv = new bool[count];

            for (int i = 0; i < count; ++i)
            {
                try
                {
                    while (!recv[i] && !m_simulator.IsGameStopped)
                    {
                        if (m_connectedClients[i].DataAvailable)
                        {
                            recv[i] = true;

                            string str = m_connectedClients[i].ReadString();
                            IMessageInfo mi = MessageParser.ParseInputMessage(str);
                            if (mi.MessageType == MessageTypes.Hold)
                            {
                                var act = new SoccerAction(ActionTypes.Hold, -1);
                                m_simulator.UpdateActionForPlayer(i, act);
                            }
                            else if (mi.MessageType == MessageTypes.Move)
                            {
                                var movmsg = mi as MoveMessage;
                                var act = new SoccerAction(movmsg.ActionType, -1);
                                m_simulator.UpdateActionForPlayer(i, act);
                            }
                            else if (mi.MessageType == MessageTypes.Pass)
                            {
                                var pmsg = mi as PassMessage;
                                m_simulator.UpdateActionForPlayer(i, new SoccerAction(ActionTypes.Pass, pmsg.DstUnum));
                            }
                            else if (mi.MessageType == MessageTypes.Home)
                            {
                                var hmsg = mi as HomeMessage;
                                if(!m_simulator.SetHomePos(i, hmsg.R, hmsg.C))
                                    m_connectedClients[i].WriteString("(error could-not-set-home)");
                            }
                            else if (mi.MessageType == MessageTypes.EpisodeTimeout)
                            {
                                var etmsg = mi as EpisodeTimeoutMessage;
                                m_simulator.EpisodeTimeout(i, etmsg.IsOur, etmsg.IsPass);
                            }
                            else
                            {
                                m_connectedClients[i].WriteString("(error)");
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public void CheckConnectedClients()
        {
            int count = m_simulator.LeftPlayersCount + m_simulator.RightPlayersCount;

            for (int i = 0; i < count; ++i)
            {
                try
                {
                    if (m_connectedClients[i].DataAvailable)
                    {
                        string str = m_connectedClients[i].ReadString();
                        IMessageInfo mi = MessageParser.ParseInputMessage(str);
                        if (mi.MessageType == MessageTypes.Hold)
                        {
                            var act = new SoccerAction(ActionTypes.Hold, -1);
                            m_simulator.UpdateActionForPlayer(i, act);
                        }
                        else if (mi.MessageType == MessageTypes.Move)
                        {
                            var movmsg = mi as MoveMessage;
                            var act = new SoccerAction(movmsg.ActionType, -1);
                            m_simulator.UpdateActionForPlayer(i, act);
                        }
                        else if (mi.MessageType == MessageTypes.Pass)
                        {
                            var pmsg = mi as PassMessage;
                            m_simulator.UpdateActionForPlayer(i, new SoccerAction(ActionTypes.Pass, pmsg.DstUnum));
                        }
                        else if (mi.MessageType == MessageTypes.Home)
                        {
                            var hmsg = mi as HomeMessage;
                            if (!m_simulator.SetHomePos(i, hmsg.R, hmsg.C))
                                m_connectedClients[i].WriteString("(error could-not-set-home)");
                        }
                        else if (mi.MessageType == MessageTypes.EpisodeTimeout)
                        {
                            var etmsg = mi as EpisodeTimeoutMessage;
                            m_simulator.EpisodeTimeout(i, etmsg.IsOur, etmsg.IsPass);
                        }
                        else
                        {
                            m_connectedClients[i].WriteString("(error)");
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public void SendSeeMessages()
        {
            m_simulator.FormSeeLists();
            int count = m_simulator.LeftPlayersCount + m_simulator.RightPlayersCount;

            PlayerInfo curPlayer;

            for (int i = 0; i < count; ++i)
            {
                List<int> visiblePlayers = m_simulator.GetSeeListForPlayer(i);
                curPlayer = m_simulator.Players[i];

                var sb = new StringBuilder();
                sb.AppendFormat("(see {0} (score {1} {2})", m_simulator.Cycle, m_simulator.LeftScore, m_simulator.RightScore);

                if (curPlayer.Side == Sides.Left)
                {
                    sb.AppendFormat(" (self {0})", curPlayer.Position.ToString());

                    foreach (int p in visiblePlayers)
                    {
                        if (p < 0)
                        {
                            sb.AppendFormat(" (b {0})", m_simulator.BallPosition.ToString());
                        }
                        else
                        {
                            PlayerInfo thePlayer = m_simulator.Players[p];
                            sb.AppendFormat(" ({0} {1} {2})", 
                                thePlayer.Side == Sides.Left ? "l" : "r", 
                                thePlayer.PlayerNumber, 
                                thePlayer.Position); 
                        }
                    }
                }
                else if (curPlayer.Side == Sides.Right)
                {
                    sb.AppendFormat(" (self {0})", curPlayer.Position.GetRTL().ToString());

                    foreach (int p in visiblePlayers)
                    {
                        if (p < 0)
                        {
                            sb.AppendFormat(" (b {0})", m_simulator.BallPosition.GetRTL().ToString());
                        }
                        else
                        {
                            PlayerInfo thePlayer = m_simulator.Players[p];
                            sb.AppendFormat(" ({0} {1} {2})",
                                thePlayer.Side == Sides.Left ? "l" : "r",
                                thePlayer.PlayerNumber,
                                thePlayer.Position.GetRTL());
                        }
                    }
                }

                sb.Append(")");

                m_connectedClients[i].WriteString(sb.ToString());
            }
        }

        #endregion
    }
}
