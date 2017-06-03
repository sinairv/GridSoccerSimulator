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
        LastSeePlayers = new int[2 * getEnvMaxPlayers()];

        PlayerPositions = new Position[2 * getEnvMaxPlayers()];
        for (int i = 0; i < getPlayerPositions().length; ++i)
        {
            getPlayerPositions()[i] = new Position();
        }

        PlayerAvailabilities = new boolean[2 * getEnvMaxPlayers()];

        m_myIndex = GetPlayerIndex(getMySide(), getMyUnum());
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
                if (getMySide() == Sides.Left)
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
                this.getPlayerPositions()[getMyIndex()].Set(num1, num2);
                this.getLastSeePlayers()[getMyIndex()] = cycle;
                this.getPlayerAvailabilities()[getMyIndex()] = true;
            }
            else if(toks[i].equals("b"))
            {
                num1 = Integer.parseInt(toks[i + 1]);
                num2 = Integer.parseInt(toks[i + 2]);
                this.LastSeeBall = cycle;
                this.getBallPosition().Set(num1, num2);
            }
            else if(toks[i].equals("l"))
            {
                unum = Integer.parseInt(toks[i + 1]);
                num1 = Integer.parseInt(toks[i + 2]);
                num2 = Integer.parseInt(toks[i + 3]);
                pi = GetPlayerIndex(Sides.Left, unum);
                this.getPlayerPositions()[pi].Set(num1, num2);
                this.getLastSeePlayers()[pi] = cycle;
                this.getPlayerAvailabilities()[pi] = true;
                i++;
            }
            else if(toks[i].equals("r"))
            {
                unum = Integer.parseInt(toks[i + 1]);
                num1 = Integer.parseInt(toks[i + 2]);
                num2 = Integer.parseInt(toks[i + 3]);
                pi = GetPlayerIndex(Sides.Right, unum);
                this.getPlayerPositions()[pi].Set(num1, num2);
                this.getLastSeePlayers()[pi] = cycle;
                this.getPlayerAvailabilities()[pi] = true;
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

    public void SendAction(ActionTypes acType) throws Exception
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

    public void SendAction(SoccerAction act) throws Exception
    {
        if (act.ActionType != ActionTypes.Pass)
            SendAction(act.ActionType);
        else
            Send(String.format("(pass %d)", act.DestinationUnum));
    }

    public boolean UpdateFromServer() throws Exception
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
                isNotGameStopped = false;
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

    public void SetHomePosition(Position homePos)
    {
        SetHomePosition(homePos.Row, homePos.Col);
    }

    public void SetHomePosition(int r, int c)
    {
        Send(String.format("(home %d %d)", r, c));
    }

    protected abstract SoccerAction Think();

    private boolean isNotGameStopped = true;
    public void Start() throws Exception
    {
        while (getIsNotGameStopped())
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
        if(getIsGameStarted())
            SendAction(Think());
    }

    public void EpisodeTimeoutOurFail()
    {
        Send("(episode-timeout our-fail)");
    }

    public void EpisodeTimeoutOurPass()
    {
        Send("(episode-timeout our-pass)");
    }

    public void EpisodeTimeoutOppFail()
    {
        Send("(episode-timeout opp-fail)");
    }

    public void EpisodeTimeoutOppPass()
    {
        Send("(episode-timeout opp-pass)");
    }


    public ArrayList<Integer> GetAvailableTeammatesUnums()
    {
        ArrayList<Integer> rtValue = new ArrayList<Integer>();
        
        int start = this.getMySide() == Sides.Left ? 0 : this.getEnvMaxPlayers();
        int end = this.getMySide() == Sides.Left ? this.getEnvMaxPlayers() - 1 : 2 * this.getEnvMaxPlayers() - 1;
        for (int i = start; i <= end; ++i)
        {
            if (i != getMyIndex() && getPlayerAvailabilities()[i])
                rtValue.add(i - start + 1);
        }
        
        return rtValue;
    }

    public ArrayList<Integer> GetAvailableTeammatesIndeces()
    {
        ArrayList<Integer> rtValue = new ArrayList<Integer>();
        
        int start = this.getMySide() == Sides.Left ? 0 : this.getEnvMaxPlayers();
        int end = this.getMySide() == Sides.Left ? this.getEnvMaxPlayers() - 1 : 2* this.getEnvMaxPlayers() - 1;
        for (int i = start; i <= end; ++i)
        {
            if (i != getMyIndex() && getPlayerAvailabilities()[i])
                rtValue.add(i);
        }
        
        return rtValue;
    }

    public ArrayList<Integer> GetAvailableOpponentsIndeces()
    {
        ArrayList<Integer> rtValue = new ArrayList<Integer>();

        int start = this.getMySide() != Sides.Left ? 0 : this.getEnvMaxPlayers();
        int end = this.getMySide() != Sides.Left ? this.getEnvMaxPlayers() - 1 : 2 * this.getEnvMaxPlayers() - 1;
        for (int i = start; i <= end; ++i)
        {
            if (getPlayerAvailabilities()[i])
                rtValue.add(i);
        }
        
        return rtValue;
    }

    public ArrayList<Integer> GetAvailableOpponentsUnums()
    {
        ArrayList<Integer> rtValue = new ArrayList<Integer>();

        int start = this.getMySide() != Sides.Left ? 0 : this.getEnvMaxPlayers();
        int end = this.getMySide() != Sides.Left ? this.getEnvMaxPlayers() - 1 : 2 * this.getEnvMaxPlayers() - 1;
        for (int i = start; i <= end; ++i)
        {
            if (getPlayerAvailabilities()[i])
                rtValue.add(i - start + 1);
        }
        
        return rtValue;
    }

    public int GetPlayerIndex(Sides side, int unum)
    {
        if (side == Sides.Left)
        {
            return unum - 1;
        }
        else
        {
            return getEnvMaxPlayers() + unum - 1;
        }
    }

    public int GetPlayerUnumFromIndex(int index)
    {
        if (0 <= index && index < getEnvMaxPlayers())
            return index + 1;
        else if (index < 2 * getEnvMaxPlayers())
            return index - getEnvMaxPlayers() + 1;
        else
            return -1;
    }

    public Position GetMyPosition()
    {
        return getPlayerPositions()[getMyIndex()];
    }

    public boolean AmIBallOwner()
    {
        return (getBallPosition() == getPlayerPositions()[getMyIndex()] && getLastSeeBall() == this.getCycle()) ;
    }

    public boolean AreWeBallOwner()
    {
        int bi = GetBallOwnerPlayerIndex();
        int start = this.getMySide() == Sides.Left ? 0 : this.getEnvMaxPlayers();
        int end = this.getMySide() == Sides.Left ? this.getEnvMaxPlayers() - 1 : 2* this.getEnvMaxPlayers() - 1;
        return (start <= bi && bi <= end);
    }

    public int GetBallOwnerPlayerIndex()
    {
        for (int i = 0; i < getPlayerAvailabilities().length; ++i)
        {
            if (getPlayerAvailabilities()[i] && getBallPosition() == getPlayerPositions()[i] && getLastSeePlayers()[i] == getLastSeeBall())
                return i;
        }

        return -1;
    }

    public int CalculateGoalUpperRow()
    {
        return (getEnvRows() - getEnvGoalWidth()) / 2 + 1;
    }

    public int CalculateGoalLowerRow()
    {
        return CalculateGoalUpperRow() + getEnvGoalWidth() - 1;
    }

    public int getGoalUpperRow()
    {
        return m_goalUpperRow;
    }

    public int getGoalLowerRow()
    {
        return m_goalLowerRow;
    }

    public Sides getMySide() {
        return MySide;
    }

    public String getMyTeamName() {
        return MyTeamName;
    }

    public int getMyUnum() {
        return MyUnum;
    }

    public int getOurScore() {
        return OurScore;
    }

    public int getOppScore() {
        return OppScore;
    }

    public int getCycle() {
        return Cycle;
    }

    public int getLastSeeBall() {
        return LastSeeBall;
    }

    public Position getBallPosition() {
        return BallPosition;
    }

    public boolean getIsGameStarted() {
        return IsGameStarted;
    }

    public int getCycleLength() {
        return CycleLength;
    }

    public boolean isTurboMode() {
        return TurboMode;
    }

    public int[] getLastSeePlayers() {
        return LastSeePlayers;
    }

    public Position[] getPlayerPositions() {
        return PlayerPositions;
    }

    public boolean[] getPlayerAvailabilities() {
        return PlayerAvailabilities;
    }

    public int getEnvRows() {
        return EnvRows;
    }

    public int getEnvCols() {
        return EnvCols;
    }

    public int getEnvPassDistance() {
        return EnvPassDistance;
    }

    public int getEnvVisibilityDistance() {
        return EnvVisibilityDistance;
    }

    public int getEnvGoalWidth() {
        return EnvGoalWidth;
    }

    public int getEnvMinPlayers() {
        return EnvMinPlayers;
    }

    public int getEnvMaxPlayers() {
        return EnvMaxPlayers;
    }

    public boolean getIsNotGameStopped() {
        return isNotGameStopped;
    }

    public int getMyIndex() {
        return m_myIndex;
    }
}
