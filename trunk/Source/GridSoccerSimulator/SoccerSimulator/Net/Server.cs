using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using GridSoccer.Simulator.Properties;
using System.IO;
using GridSoccer.Common;
using System.Threading;

namespace GridSoccer.Simulator.Net
{
    public class Server
    {
        #region Fields

        TcpListener listener = null;
        List<ClientInfo> tempClients = new List<ClientInfo>();
        ClientInfo[] ConnectedClients;

        SoccerSimulator simulator = null;

        #endregion

        #region Constructors

        public Server(SoccerSimulator sim, int port)
        {
            this.simulator = sim;
            ConnectedClients = new ClientInfo[Settings.Default.MaxPlayers * 2];

            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Settings.Default.PortNumber);
            listener.Start();
        }

        #endregion

        #region Methods

        public bool HasConnectionRequest()
        {
            return listener.Pending();
        }

        public void AcceptConnection()
        {
            TcpClient client = listener.AcceptTcpClient();
            client.NoDelay = true;


            tempClients.Add(new ClientInfo() { PlayerIndex = -1, TcpClient = client });
        }

        public void StopListeningForNewConnections()
        {
            listener.Stop();
        }

        private string GetSettingsMessage()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(" ({0} {1})", "rows", Settings.Default.NumRows);
            sb.AppendFormat(" ({0} {1})", "cols", Settings.Default.NumCols);
            sb.AppendFormat(" ({0} {1})", "goal-width", Settings.Default.GoalWidth);
            sb.AppendFormat(" ({0} {1})", "pass-dist", Settings.Default.PassableDistance);
            sb.AppendFormat(" ({0} {1})", "visible-dist", Settings.Default.VisibleRadius);
            sb.AppendFormat(" ({0} {1})", "min-players", Settings.Default.MinPlayers);
            sb.AppendFormat(" ({0} {1})", "max-players", Settings.Default.MaxPlayers);

            return String.Format("(settings{0})", sb.ToString());
        }

        public void FreeTempClients()
        {
            for (int i = tempClients.Count - 1; i >= 0; --i)
            {
                tempClients[i].Close();
            }

            tempClients.Clear();
        }

        public void CheckTempClients()
        {
            for (int i = tempClients.Count - 1; i >= 0; --i)
            {
                try
                {
                    if (tempClients[i].DataAvailable)
                    {
                        string str = tempClients[i].ReadString();
                        IMessageInfo mi = MessageParser.ProcessInputMessage(str);
                        if (mi.MessageType == MessageTypes.Init)
                        {
                            InitMessage initmsg = mi as InitMessage;
                            int pi = simulator.AddPlayer(initmsg.TeamName, initmsg.UNum);
                            if (pi >= 0)
                            {
                                tempClients[i].WriteString(String.Format("(init {0} ok)", initmsg.TeamName == simulator.LeftTeamName ? "l" : "r"));
                                tempClients[i].WriteString(GetSettingsMessage());
                                ConnectedClients[pi] = tempClients[i];
                            }
                            else
                            {
                                tempClients[i].WriteString("(init error)");
                                tempClients[i].Close();
                            }

                            tempClients.RemoveAt(i);
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
            int count = simulator.LeftPlayersCount + simulator.RightPlayersCount;
            for (int i = 0; i < count; ++i)
            {
                ConnectedClients[i].WriteString(msg);
            }
        }

        public void CheckConnectedClientsWaitingForAll()
        {
            int count = simulator.LeftPlayersCount + simulator.RightPlayersCount;

            bool[] recv = new bool[count];

            for (int i = 0; i < count; ++i)
            {
                try
                {
                    while (!recv[i])
                    {
                        if (ConnectedClients[i].DataAvailable)
                        {
                            recv[i] = true;

                            string str = ConnectedClients[i].ReadString();
                            IMessageInfo mi = MessageParser.ProcessInputMessage(str);
                            if (mi.MessageType == MessageTypes.Hold)
                            {
                                SoccerAction act = new SoccerAction(ActionTypes.Hold, -1);
                                simulator.UpdateActionForPlayer(i, act);
                            }
                            else if (mi.MessageType == MessageTypes.Move)
                            {
                                MoveMessage movmsg = mi as MoveMessage;
                                SoccerAction act = new SoccerAction(movmsg.ActionType, -1);
                                simulator.UpdateActionForPlayer(i, act);
                            }
                            else if (mi.MessageType == MessageTypes.Pass)
                            {
                                PassMessage pmsg = mi as PassMessage;
                                simulator.UpdateActionForPlayer(i, new SoccerAction(ActionTypes.Pass, pmsg.DstUnum));
                            }
                            else if (mi.MessageType == MessageTypes.Home)
                            {
                                HomeMessage hmsg = mi as HomeMessage;
                                if(!simulator.SetHomePos(i, hmsg.R, hmsg.C))
                                    ConnectedClients[i].WriteString("(error could-not-set-home)");
                            }
                            else
                            {
                                ConnectedClients[i].WriteString("(error)");
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
            int count = simulator.LeftPlayersCount + simulator.RightPlayersCount;

            for (int i = 0; i < count; ++i)
            {
                try
                {
                    if (ConnectedClients[i].DataAvailable)
                    {
                        string str = ConnectedClients[i].ReadString();
                        IMessageInfo mi = MessageParser.ProcessInputMessage(str);
                        if (mi.MessageType == MessageTypes.Hold)
                        {
                            SoccerAction act = new SoccerAction(ActionTypes.Hold, -1);
                            simulator.UpdateActionForPlayer(i, act);
                        }
                        else if (mi.MessageType == MessageTypes.Move)
                        {
                            MoveMessage movmsg = mi as MoveMessage;
                            SoccerAction act = new SoccerAction(movmsg.ActionType, -1);
                            simulator.UpdateActionForPlayer(i, act);
                        }
                        else if (mi.MessageType == MessageTypes.Pass)
                        {
                            PassMessage pmsg = mi as PassMessage;
                            simulator.UpdateActionForPlayer(i, new SoccerAction(ActionTypes.Pass, pmsg.DstUnum));
                        }
                        else if (mi.MessageType == MessageTypes.Home)
                        {
                            HomeMessage hmsg = mi as HomeMessage;
                            if (!simulator.SetHomePos(i, hmsg.R, hmsg.C))
                                ConnectedClients[i].WriteString("(error could-not-set-home)");
                        }
                        else
                        {
                            ConnectedClients[i].WriteString("(error)");
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
            simulator.FormSeeLists();
            int count = simulator.LeftPlayersCount + simulator.RightPlayersCount;

            List<int> visiblePlayers;
            ObjectInfo curPlayer;

            for (int i = 0; i < count; ++i)
            {
                visiblePlayers = simulator.GetSeeListForPlayer(i);
                curPlayer = simulator.Players[i];

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("(see {0} (score {1} {2})", simulator.Cycle, simulator.LeftScore, simulator.RightScore);

                if (curPlayer.Side == Sides.Left)
                {
                    sb.AppendFormat(" (self {0})", curPlayer.Position.ToString());

                    foreach (int p in visiblePlayers)
                    {
                        if (p < 0)
                        {
                            sb.AppendFormat(" (b {0})", simulator.Ball.Position.ToString());
                        }
                        else
                        {
                            ObjectInfo thePlayer = simulator.Players[p];
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
                            sb.AppendFormat(" (b {0})", simulator.Ball.Position.GetRTL().ToString());
                        }
                        else
                        {
                            ObjectInfo thePlayer = simulator.Players[p];
                            sb.AppendFormat(" ({0} {1} {2})",
                                thePlayer.Side == Sides.Left ? "l" : "r",
                                thePlayer.PlayerNumber,
                                thePlayer.Position.GetRTL());
                        }
                    }
                }

                sb.Append(")");

                ConnectedClients[i].WriteString(sb.ToString());
            }


        }

        #endregion
    }
}
