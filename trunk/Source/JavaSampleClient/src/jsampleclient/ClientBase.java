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

package jsampleclient;

import java.io.IOException;
import java.net.Socket;
import java.util.ArrayList;
import java.util.StringTokenizer;

public abstract class ClientBase
{
    private ArrayList<String> m_qReadMessages;
    private Socket socket = null;

    protected int m_myIndex = -1;
    private int m_goalUpperRow = -1;
    private int m_goalLowerRow = -1;
    
    protected  Sides MySide;
    protected  String MyTeamName;
    protected  int MyUnum;

    protected  int OurScore;
    protected  int OppScore;

    protected  int Cycle;
    protected  int LastSeeBall;

    protected  Position BallPosition;
    protected  boolean IsGameStarted;
    protected  int CycleLength;
    protected  boolean TurboMode;

    protected  int[] LastSeePlayers;
    protected  Position[] PlayerPositions;
    protected  boolean[] PlayerAvailabilities;

    protected  int EnvRows;
    protected  int EnvCols;
    protected  int EnvPassDistance;
    protected  int EnvVisibilityDistance;
    protected  int EnvGoalWidth; 
    protected  int EnvMinPlayers;
    protected  int EnvMaxPlayers;

    public ClientBase(String serverAddr, int serverPort, String teamname, int unum) throws Exception
    {
        if (teamname == null || teamname.length() <= 0)
            throw new Exception("Invalid team name");

        m_qReadMessages = new ArrayList<String>();
        socket = new Socket(serverAddr, serverPort);
        socket.setTcpNoDelay(true);

        Send(String.format("(init %s %d)", teamname, unum));

        String msg = Read();
        IMessageInfo mi = ServerMessageParser.ParseInputMessage(msg);
        if (mi.MessageType != MessageTypes.InitOK)
            throw new Exception("expected init-ok but received " + mi.MessageType.toString());

        InitOKMessage okmi =  (InitOKMessage)mi;
        this.MySide = okmi.Side;
        this.MyTeamName = teamname;
        this.MyUnum = unum;

        msg = Read();
        mi = ServerMessageParser.ParseInputMessage(msg);
        if (mi.MessageType != MessageTypes.Settings)
            throw new Exception("expected settings but received " + mi.MessageType.toString());
        ParseSettings(((SettingsMessage)mi).SettingsMsgTokens);
        
        BallPosition = new Position();
        LastSeePlayers = new int[2 * EnvMaxPlayers];

        PlayerPositions = new Position[2 * EnvMaxPlayers];
        for (int i = 0; i < PlayerPositions.length; ++i)
        {
            PlayerPositions[i] = new Position();
        }

        PlayerAvailabilities = new boolean[2 * EnvMaxPlayers];

        m_myIndex = GetPlayerIndex(MySide, MyUnum);
        m_goalUpperRow = CalculateGoalUpperRow();
        m_goalLowerRow = CalculateGoalLowerRow();
    }

    private void ParseSettings(String[] tokens)
    {
        int value;
        for (int i = 1; i < tokens.length; i+=2)
        {
            try
            {
                value = Integer.parseInt(tokens[i + 1]);
                if(tokens[i].equals("rows"))
                {
                    this.EnvRows = value;
                }
                else if(tokens[i].equals("cols"))
                {
                    this.EnvCols = value;
                }
                else if(tokens[i].equals("goal-width"))
                {
                    this.EnvGoalWidth = value;
                }
                else if(tokens[i].equals("pass-dist"))
                {
                    this.EnvPassDistance = value;
                }
                else if(tokens[i].equals("visible-dist"))
                {
                    this.EnvVisibilityDistance = value;
                }
                else if(tokens[i].equals("min-players"))
                {
                    this.EnvMinPlayers = value;
                }
                else if(tokens[i].equals("max-players"))
                {
                    this.EnvMaxPlayers = value;
                }
            }
            catch(Exception ex)
            {
            }
        }
    }

    private void ParseSeeMessage(String[] toks)
    {
        int cycle = Integer.parseInt(toks[1]);
        this.Cycle = cycle;
        int unum, num1, num2;
        int pi;

        for (int i = 2; i < toks.length; i += 3)
        {
            if(toks[i].equals("score"))
            {
                num1 = Integer.parseInt(toks[i + 1]);
                num2 = Integer.parseInt(toks[i + 2]);
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
            }
            else if(toks[i].equals("self"))
            {
                num1 = Integer.parseInt(toks[i + 1]);
                num2 = Integer.parseInt(toks[i + 2]);
                this.PlayerPositions[m_myIndex].Set(num1, num2);
                this.LastSeePlayers[m_myIndex] = cycle;
                this.PlayerAvailabilities[m_myIndex] = true;
            }
            else if(toks[i].equals("b"))
            {
                num1 = Integer.parseInt(toks[i + 1]);
                num2 = Integer.parseInt(toks[i + 2]);
                this.LastSeeBall = cycle;
                this.BallPosition.Set(num1, num2);
            }
            else if(toks[i].equals("l"))
            {
                unum = Integer.parseInt(toks[i + 1]);
                num1 = Integer.parseInt(toks[i + 2]);
                num2 = Integer.parseInt(toks[i + 3]);
                pi = GetPlayerIndex(Sides.Left, unum);
                this.PlayerPositions[pi].Set(num1, num2);
                this.LastSeePlayers[pi] = cycle;
                this.PlayerAvailabilities[pi] = true;
                i++;
            }
            else if(toks[i].equals("r"))
            {
                unum = Integer.parseInt(toks[i + 1]);
                num1 = Integer.parseInt(toks[i + 2]);
                num2 = Integer.parseInt(toks[i + 3]);
                pi = GetPlayerIndex(Sides.Right, unum);
                this.PlayerPositions[pi].Set(num1, num2);
                this.LastSeePlayers[pi] = cycle;
                this.PlayerAvailabilities[pi] = true;
                i++;
            }
        }
    }

    private void Send(String msg)
    {
        try
        {
            msg = msg + '\0';
            socket.getOutputStream().write(msg.getBytes());
            socket.getOutputStream().flush();
        }
        catch(Exception ex)
        {
            System.err.println("Could not send message: " + msg);
        }
    }

    private String Read() throws IOException
    {
        if(m_qReadMessages.size() > 0)
        {
            String msg = m_qReadMessages.get(0);
            m_qReadMessages.remove(0);
            return msg;
        }
        else
        {
            byte[] inputBuffer = new byte[1024];
            socket.getInputStream().read(inputBuffer);
            String msg = new String(inputBuffer);
            //System.out.println("Read: " + msg);
        
            StringTokenizer toker = new StringTokenizer(msg, "\0");
            while(toker.hasMoreTokens())
            {
                String curTok = toker.nextToken();
                //System.out.println("Tok: " + curTok);
                m_qReadMessages.add(curTok);
            }
            
            String curmsg = m_qReadMessages.get(0);
            m_qReadMessages.remove(0);
            return curmsg;
        }
    }

    private void SendAction(ActionTypes acType) throws Exception
    {
        switch (acType)
        {
            case Hold:
                Send("(hold)");
                break;
            case MoveEast:
                Send("(move east)");
                break;
            case MoveSouth:
                Send("(move south)");
                break;
            case MoveWest:
                Send("(move west)");
                break;
            case MoveNorth:
                Send("(move north)");
                break;
            case MoveNorthEast:
                Send("(move north-east)");
                break;
            case MoveSouthEast:
                Send("(move south-east)");
                break;
            case MoveSouthWest:
                Send("(move south-west)");
                break;
            case MoveNorthWest:
                Send("(move north-west)");
                break;
            case Pass:
                throw new Exception("Action Type cannot be Pass");
            default:
                break;
        }
    }

    private void SendAction(SoccerAction act) throws Exception
    {
        if (act.ActionType != ActionTypes.Pass)
            SendAction(act.ActionType);
        else
            Send(String.format("(pass %d)", act.DestinationUnum));
    }

    private boolean UpdateFromServer() throws Exception
    {
        String msg = Read();
        IMessageInfo imi = ServerMessageParser.ParseInputMessage(msg);
        switch (imi.MessageType)
        {
            case Error:
                System.out.println("Error: " + msg);
                return false;
            case See:
                ParseSeeMessage(((SeeMessage)imi).SeeMsgTokens);
                return true;
            case Start:
                IsGameStarted = true;
                return false;
            case Stop:
                gameIsNotStoped = false;
                return false;
            case Cycle:
                CycleLength = ((CycleMessage)imi).CycleLength;
                return false;
            case Turbo:
                TurboMode = ((TurboMessage)imi).TurboOn;
                return false;
        }
        return false;
    }

    protected void SetHomePosition(Position homePos)
    {
        SetHomePosition(homePos.Row, homePos.Col);
    }

    protected void SetHomePosition(int r, int c)
    {
        Send(String.format("(home %d %d)", r, c));
    }

    protected abstract SoccerAction Think();

    private boolean gameIsNotStoped = true;
    public void Start() throws Exception
    {
        while (gameIsNotStoped)
        {
            if(UpdateFromServer())
                ThinkBase();
            //if(!TurboMode)
            //Thread.sleep(1);
        }
        OnGameStopped();
    }

    public void OnGameStopped()
    {
        // Nothing
    }

    private void ThinkBase() throws Exception
    {
        if(IsGameStarted)
            SendAction(Think());
    }
    
    protected void EpisodeTimeoutOurFail()
    {
        Send("(episode-timeout our-fail)");
    }

    protected void EpisodeTimeoutOurPass()
    {
        Send("(episode-timeout our-pass)");
    }

    protected void EpisodeTimeoutOppFail()
    {
        Send("(episode-timeout opp-fail)");
    }

    protected void EpisodeTimeoutOppPass()
    {
        Send("(episode-timeout opp-pass)");
    }


    protected ArrayList<Integer> GetAvailableTeammatesUnums()
    {
        ArrayList<Integer> rtValue = new ArrayList<Integer>();
        
        int start = this.MySide == Sides.Left ? 0 : this.EnvMaxPlayers;
        int end = this.MySide == Sides.Left ? this.EnvMaxPlayers - 1 : 2 * this.EnvMaxPlayers - 1;
        for (int i = start; i <= end; ++i)
        {
            if (i != m_myIndex && PlayerAvailabilities[i])
                rtValue.add(i - start + 1);
        }
        
        return rtValue;
    }

    protected ArrayList<Integer> GetAvailableTeammatesIndeces()
    {
        ArrayList<Integer> rtValue = new ArrayList<Integer>();
        
        int start = this.MySide == Sides.Left ? 0 : this.EnvMaxPlayers;
        int end = this.MySide == Sides.Left ? this.EnvMaxPlayers - 1 : 2*this.EnvMaxPlayers - 1;
        for (int i = start; i <= end; ++i)
        {
            if (i != m_myIndex && PlayerAvailabilities[i])
                rtValue.add(i);
        }
        
        return rtValue;
    }

    protected ArrayList<Integer> GetAvailableOpponentsIndeces()
    {
        ArrayList<Integer> rtValue = new ArrayList<Integer>();

        int start = this.MySide != Sides.Left ? 0 : this.EnvMaxPlayers;
        int end = this.MySide != Sides.Left ? this.EnvMaxPlayers - 1 : 2 * this.EnvMaxPlayers - 1;
        for (int i = start; i <= end; ++i)
        {
            if (PlayerAvailabilities[i])
                rtValue.add(i);
        }
        
        return rtValue;
    }

    protected ArrayList<Integer> GetAvailableOpponentsUnums()
    {
        ArrayList<Integer> rtValue = new ArrayList<Integer>();

        int start = this.MySide != Sides.Left ? 0 : this.EnvMaxPlayers;
        int end = this.MySide != Sides.Left ? this.EnvMaxPlayers - 1 : 2 * this.EnvMaxPlayers - 1;
        for (int i = start; i <= end; ++i)
        {
            if (PlayerAvailabilities[i])
                rtValue.add(i - start + 1);
        }
        
        return rtValue;
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

    protected boolean AmIBallOwner()
    {
        return (BallPosition == PlayerPositions[m_myIndex] && LastSeeBall == this.Cycle) ;
    }

    protected boolean AreWeBallOwner()
    {
        int bi = GetBallOwnerPlayerIndex();
        int start = this.MySide == Sides.Left ? 0 : this.EnvMaxPlayers;
        int end = this.MySide == Sides.Left ? this.EnvMaxPlayers - 1 : 2*this.EnvMaxPlayers - 1;
        return (start <= bi && bi <= end);
    }

    protected int GetBallOwnerPlayerIndex()
    {
        for (int i = 0; i < PlayerAvailabilities.length; ++i)
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

    protected int getGoalUpperRow()
    {
        return m_goalUpperRow;
    }

    protected int getGoalLowerRow()
    {
        return m_goalLowerRow;
    }
}
