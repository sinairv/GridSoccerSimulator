using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using GridSoccer.Common;
using System.Threading;

namespace GridSoccer.ClientBasic
{
    public abstract class ClientBase
    {
        private TcpClient socket = null;
        private BinaryWriter binWriter = null;
        private BinaryReader binReader = null;

        protected int m_myIndex = -1;
        private int m_goalUpperRow = -1;
        private int m_goalLowerRow = -1;

        public Sides MySide { get; private set; }
        public string MyTeamName { get; private set; }
        public int MyUnum { get; private set; }

        public int OurScore { get; private set; }
        public int OppScore { get; private set; }

        public int Cycle { get; private set; }
        public int LastSeeBall { get; private set; }
        public Position BallPosition { get; private set; }
        public bool IsGameStarted { get; private set; }
        private int CycleLength { get; set; }
        private bool JetMode { get; set; }

        public int[] LastSeePlayers { get; private set; }
        public Position[] PlayerPositions { get; private set; }
        public bool[] PlayerAvailabilities { get; private set; }

        public int EnvRows { get; private set; }
        public int EnvCols { get; private set; }
        public int EnvPassDistance { get; private set; }
        public int EnvVisibilityDistance { get; private set; }
        public int EnvGoalWidth { get; private set; }
        public int EnvMinPlayers { get; private set; }
        public int EnvMaxPlayers { get; private set; }

        public ClientBase(string serverAddr, int serverPort, string teamname, int unum)
        {
            if (String.IsNullOrEmpty(teamname))
                throw new Exception("Invalid team name", null);

            socket = new TcpClient(serverAddr, serverPort);
            socket.NoDelay = true;
            binWriter = new BinaryWriter(socket.GetStream());
            binReader = new BinaryReader(socket.GetStream());

            Send(String.Format("(init {0} {1})", teamname, unum));

            string msg = Read();
            IMessageInfo mi = ServerMessageParser.ParseInputMessage(msg);
            if (mi.MessageType != MessageTypes.InitOK)
                throw new Exception("expected init-ok but received " + mi.MessageType.ToString());

            InitOKMessage okmi =  mi as InitOKMessage;
            this.MySide = okmi.Side;
            this.MyTeamName = teamname;
            this.MyUnum = unum;

            msg = Read();
            mi = ServerMessageParser.ParseInputMessage(msg);
            if (mi.MessageType != MessageTypes.Settings)
                throw new Exception("expected settings but received " + mi.MessageType.ToString());
            ParseSettings((mi as SettingsMessage).SettingsMsgTokens);
            
            BallPosition = new Position();
            LastSeePlayers = new int[2 * EnvMaxPlayers];

            PlayerPositions = new Position[2 * EnvMaxPlayers];
            for (int i = 0; i < PlayerPositions.Length; ++i)
            {
                PlayerPositions[i] = new Position();
            }

            PlayerAvailabilities = new bool[2 * EnvMaxPlayers];

            m_myIndex = GetPlayerIndex(MySide, MyUnum);
            m_goalUpperRow = CalculateGoalUpperRow();
            m_goalLowerRow = CalculateGoalLowerRow();
        }

        #region Communication Stuff
        private void ParseSettings(string[] tokens)
        {
            int value;
            for (int i = 1; i < tokens.Length; i+=2)
            {
                if (Int32.TryParse(tokens[i + 1], out value))
                {
                    switch (tokens[i])
                    {
                        case "rows":
                            this.EnvRows = value;
                            break;
                        case "cols":
                            this.EnvCols = value;
                            break;
                        case "goal-width":
                            this.EnvGoalWidth = value;
                            break;
                        case "pass-dist":
                            this.EnvPassDistance = value;
                            break;
                        case "visible-dist":
                            this.EnvVisibilityDistance = value;
                            break;
                        case "min-players":
                            this.EnvMinPlayers = value;
                            break;
                        case "max-players":
                            this.EnvMaxPlayers = value;
                            break;
                    }
                }
            }
        }

        private void ParseSeeMessage(string[] toks)
        {
            int cycle = Int32.Parse(toks[1]);
            this.Cycle = cycle;
            int unum, num1, num2;
            int pi;

            for (int i = 2; i < toks.Length; i += 3)
            {
                switch (toks[i])
                {
                    case "score":
                        num1 = Int32.Parse(toks[i + 1]);
                        num2 = Int32.Parse(toks[i + 2]);
                        if (MySide == Sides.Left)
                        {
                            this.OurScore = num1;
                            this.OppScore = num2;
                        }
                        else
                        {
                            this.OurScore = num2;
                            this.OppScore = num1;
                        }
                        break;
                    case "self":
                        num1 = Int32.Parse(toks[i + 1]);
                        num2 = Int32.Parse(toks[i + 2]);
                        this.PlayerPositions[m_myIndex].Set(num1, num2);
                        this.LastSeePlayers[m_myIndex] = cycle;
                        this.PlayerAvailabilities[m_myIndex] = true;
                        break;
                    case "b":
                        num1 = Int32.Parse(toks[i + 1]);
                        num2 = Int32.Parse(toks[i + 2]);
                        this.LastSeeBall = cycle;
                        this.BallPosition.Set(num1, num2);
                        break;
                    case "l":
                        unum = Int32.Parse(toks[i + 1]);
                        num1 = Int32.Parse(toks[i + 2]);
                        num2 = Int32.Parse(toks[i + 3]);
                        pi = GetPlayerIndex(Sides.Left, unum);
                        this.PlayerPositions[pi].Set(num1, num2);
                        this.LastSeePlayers[pi] = cycle;
                        this.PlayerAvailabilities[pi] = true;
                        i++;
                        break;
                    case "r":
                        unum = Int32.Parse(toks[i + 1]);
                        num1 = Int32.Parse(toks[i + 2]);
                        num2 = Int32.Parse(toks[i + 3]);
                        pi = GetPlayerIndex(Sides.Right, unum);
                        this.PlayerPositions[pi].Set(num1, num2);
                        this.LastSeePlayers[pi] = cycle;
                        this.PlayerAvailabilities[pi] = true;
                        i++;
                        break;
                    default:
                        break;
                }
            }
        }

        private void Send(string msg)
        {
            try
            {
                binWriter.Write(msg);
            }
            catch
            {
            }
        }

        private string Read()
        {
            return binReader.ReadString();
        }

        private bool DataAvailable()
        {
            return socket.GetStream().DataAvailable;
        }

        private void SendAction(ActionTypes acType)
        {
            switch (acType)
            {
                case ActionTypes.Hold:
                    Send("(hold)");
                    break;
                case ActionTypes.MoveEast:
                    Send("(move east)");
                    break;
                case ActionTypes.MoveSouth:
                    Send("(move south)");
                    break;
                case ActionTypes.MoveWest:
                    Send("(move west)");
                    break;
                case ActionTypes.MoveNorth:
                    Send("(move north)");
                    break;
                case ActionTypes.MoveNorthEast:
                    Send("(move north-east)");
                    break;
                case ActionTypes.MoveSouthEast:
                    Send("(move south-east)");
                    break;
                case ActionTypes.MoveSouthWest:
                    Send("(move south-west)");
                    break;
                case ActionTypes.MoveNorthWest:
                    Send("(move north-west)");
                    break;
                case ActionTypes.Pass:
                    throw new Exception("Action Type cannot be Pass");
                default:
                    break;
            }
        }

        private void SendAction(SoccerAction act)
        {
            if (act.ActionType != ActionTypes.Pass)
                SendAction(act.ActionType);
            else
                Send(String.Format("(pass {0})", act.DestinationUnum));
        }

        private bool UpdateFromServer()
        {
            if (DataAvailable())
            {
                string msg = Read();
                IMessageInfo imi = ServerMessageParser.ParseInputMessage(msg);
                switch (imi.MessageType)
                {
                    case MessageTypes.Error:
                        Console.WriteLine("Error: " + msg);
                        return false;
                    case MessageTypes.See:
                        ParseSeeMessage((imi as SeeMessage).SeeMsgTokens);
                        return true;
                    case MessageTypes.Start:
                        IsGameStarted = true;
                        return false;
                    case MessageTypes.Stop:
                        gameIsNotStoped = false;
                        return false;
                    case MessageTypes.Cycle:
                        CycleLength = (imi as CycleMessage).CycleLength;
                        return false;
                    case MessageTypes.Jet:
                        JetMode = (imi as JetMessage).JetOn;
                        return false;
                }
            }
            return false;
        }

        protected void SetHomePosition(Position homePos)
        {
            SetHomePosition(homePos.Row, homePos.Col);
        }

        protected void SetHomePosition(int r, int c)
        {
            Send(String.Format("(home {0} {1})", r, c));
        }

        #endregion

        protected abstract SoccerAction Think();

        private bool gameIsNotStoped = true;
        public void Start()
        {
            while (gameIsNotStoped)
            {
                if(UpdateFromServer())
                    ThinkBase();
                //if(!JetMode)
                Thread.Sleep(1);
            }
            OnGameStopped();
        }

        virtual public void OnGameStopped()
        {
            // Nothing
        }

        private void ThinkBase()
        {
            if(IsGameStarted)
                SendAction(Think());
        }

        #region Utilities

        protected IEnumerable<int> GetAvailableTeammatesUnums()
        {
            int start = this.MySide == Sides.Left ? 0 : this.EnvMaxPlayers;
            int end = this.MySide == Sides.Left ? this.EnvMaxPlayers - 1 : 2 * this.EnvMaxPlayers - 1;
            for (int i = start; i <= end; ++i)
            {
                if (i != m_myIndex && PlayerAvailabilities[i])
                    yield return i - start + 1;
            }
        }

        protected IEnumerable<int> GetAvailableTeammatesIndeces()
        {
            int start = this.MySide == Sides.Left ? 0 : this.EnvMaxPlayers;
            int end = this.MySide == Sides.Left ? this.EnvMaxPlayers - 1 : 2*this.EnvMaxPlayers - 1;
            for (int i = start; i <= end; ++i)
            {
                if (i != m_myIndex && PlayerAvailabilities[i])
                    yield return i;
            }
        }

        protected IEnumerable<int> GetAvailableOpponentsIndeces()
        {
            int start = this.MySide != Sides.Left ? 0 : this.EnvMaxPlayers;
            int end = this.MySide != Sides.Left ? this.EnvMaxPlayers - 1 : 2 * this.EnvMaxPlayers - 1;
            for (int i = start; i <= end; ++i)
            {
                if (PlayerAvailabilities[i])
                    yield return i;
            }
        }

        protected IEnumerable<int> GetAvailableOpponentsUnums()
        {
            int start = this.MySide != Sides.Left ? 0 : this.EnvMaxPlayers;
            int end = this.MySide != Sides.Left ? this.EnvMaxPlayers - 1 : 2 * this.EnvMaxPlayers - 1;
            for (int i = start; i <= end; ++i)
            {
                if (PlayerAvailabilities[i])
                    yield return i - start + 1;
            }
        }

        protected int GetPlayerIndex(Sides side, int unum)
        {
            if (side == Sides.Left)
            {
                return unum - 1;
            }
            else
            {
                return EnvMaxPlayers + unum - 1;
            }
        }

        protected int GetPlayerUnumFromIndex(int index)
        {
            if (0 <= index && index < EnvMaxPlayers)
                return index + 1;
            else if (index < 2 * EnvMaxPlayers)
                return index - EnvMaxPlayers + 1;
            else
                return -1;
        }

        protected Position GetMyPosition()
        {
            return PlayerPositions[m_myIndex];
        }

        protected bool AmIBallOwner()
        {
            return (BallPosition == PlayerPositions[m_myIndex] && LastSeeBall == this.Cycle) ;
        }

        protected bool AreWeBallOwner()
        {
            int bi = GetBallOwnerPlayerIndex();
            int start = this.MySide == Sides.Left ? 0 : this.EnvMaxPlayers;
            int end = this.MySide == Sides.Left ? this.EnvMaxPlayers - 1 : 2*this.EnvMaxPlayers - 1;
            return (start <= bi && bi <= end);
        }

        protected int GetBallOwnerPlayerIndex()
        {
            for (int i = 0; i < PlayerAvailabilities.Length; ++i)
            {
                if (PlayerAvailabilities[i] && BallPosition == PlayerPositions[i] && LastSeePlayers[i] == LastSeeBall)
                    return i;
            }

            return -1;
        }

        private int CalculateGoalUpperRow()
        {
            return (EnvRows - EnvGoalWidth) / 2 + 1;
        }

        private int CalculateGoalLowerRow()
        {
            return CalculateGoalUpperRow() + EnvGoalWidth - 1;
        }

        protected int GoalUpperRow
        {
            get
            {
                return m_goalUpperRow;
            }
        }

        protected int GoalLowerRow
        {
            get
            {
                return m_goalLowerRow;
            }
        }

        #endregion
    }
}
