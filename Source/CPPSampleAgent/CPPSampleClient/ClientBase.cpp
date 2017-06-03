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

#include "ClientBase.h"
#include "ServerMessageParser.h"
#include "MessageInfo.h"
#include "ParserUtils.h"

using namespace std;

ClientBase::ClientBase(string serverAddr, int serverPort, string teamName, int unum)
		: m_socket(serverAddr, serverPort)
{
	if(teamName.empty())
		throw ClientException("Invalid team name");

	m_socket.SendFormat("(init %s %d)", teamName.c_str(), unum);

	string msg = m_socket.ReceiveString();

	IMessageInfo* mi = ServerMessageParser::ParseInputMessage(msg);
    if (mi->GetMessageType() != MessageTypes::InitOK)
	{
		delete mi;
        throw ClientException("expected init-ok but received " + mi->GetMessageType());
	}

	InitOKMessage* okmi =  dynamic_cast<InitOKMessage*>(mi);

	m_mySide = okmi->Side;
	m_myTeamName = teamName;
    m_myUnum = unum;

	if(mi) delete mi;

	msg = m_socket.ReceiveString();

	mi = ServerMessageParser::ParseInputMessage(msg);
	if (mi->GetMessageType() != MessageTypes::Settings)
	{
		delete mi;
		throw ClientException("expected settings but received " + mi->GetMessageType());
	}

	SettingsMessage* smi = dynamic_cast<SettingsMessage*>(mi);
    ParseSettings(smi->SettingsMsgTokens);
	if(mi) delete mi;

	for(int i = 0; i < 2 * m_envMaxPlayers; i++)
	{
		m_lastSeePlayers.push_back(0);
		m_playerPositions.push_back(Position(0,0));
		m_playerAvailabilities.push_back(false);
	}

	m_myIndex = GetPlayerIndex(m_mySide, m_myUnum);
    m_goalUpperRow = CalculateGoalUpperRow();
    m_goalLowerRow = CalculateGoalLowerRow();

	m_gameIsNotStoped = true;
}

void ClientBase::Start()
{
    while (m_gameIsNotStoped)
    {
        if(UpdateFromServer())
            ThinkBase();
    }
    OnGameStopped();
}

void ClientBase::ThinkBase()
{
    if(m_isGameStarted)
        SendAction(Think());
}

void ClientBase::Send(string msg)
{
    try
    {
		m_socket.SendString(msg);
    }
    catch (...)
    {
		cerr << "Error in sending message: " << msg << endl;
    }
}

void ClientBase::SendFormat(const char *fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	std::string s = ParserUtils::FormatString(fmt, args);
	va_end(args);
	Send(s);
}

void ClientBase::SendAction(ActionTypes acType)
{
    switch (acType)
    {
        case ActionTypes::ActHold:
            Send("(hold)");
            break;
        case ActionTypes::ActMoveEast:
            Send("(move east)");
            break;
        case ActionTypes::ActMoveSouth:
            Send("(move south)");
            break;
        case ActionTypes::ActMoveWest:
            Send("(move west)");
            break;
        case ActionTypes::ActMoveNorth:
            Send("(move north)");
            break;
        case ActionTypes::ActMoveNorthEast:
            Send("(move north-east)");
            break;
        case ActionTypes::ActMoveSouthEast:
            Send("(move south-east)");
            break;
        case ActionTypes::ActMoveSouthWest:
            Send("(move south-west)");
            break;
        case ActionTypes::ActMoveNorthWest:
            Send("(move north-west)");
            break;
        case ActionTypes::ActPass:
            throw ClientException("Action Type cannot be Pass");
        default:
            break;
    }
}

void ClientBase::SendAction(SoccerAction act)
{
	if (act.IsIllegal())
        return;

    if (act.ActionType != ActionTypes::ActPass)
        SendAction(act.ActionType);
    else
        SendFormat("(pass %d)", act.DestinationUnum);
}

void ClientBase::OnGameStopped()
{
	// Do nothing
}

bool ClientBase::UpdateFromServer()
{
	bool rtValue = false;
//    if (DataAvailable())
//    {
        string msg = m_socket.ReceiveString();
        IMessageInfo* pmi = ServerMessageParser::ParseInputMessage(msg);
		switch (pmi->GetMessageType())
        {
		case MessageTypes::Error:
            cout << "Error: " << msg << endl;
            rtValue = false;
			break;
		case MessageTypes::See:
            ParseSeeMessage(((SeeMessage*) pmi)->SeeMsgTokens);
            rtValue = true;
			break;
		case MessageTypes::Start:
			m_isGameStarted = true;
            rtValue = false;
			break;
		case MessageTypes::Stop:
            m_gameIsNotStoped = false;
			rtValue = false;
			break;
		case MessageTypes::Cycle:
			m_cycleLength = ((CycleMessage *) pmi)->CycleLength;
            rtValue = false;
			break;
		case MessageTypes::Turbo:
			m_turboMode = ((TurboMessage *) pmi)->TurboOn;
            rtValue = false;
			break;
        }
		delete pmi;
//    }
    return rtValue;
}


int ClientBase::CalculateGoalUpperRow()
{
    return (m_envRows - m_envGoalWidth) / 2 + 1;
}

int ClientBase::CalculateGoalLowerRow()
{
    return CalculateGoalUpperRow() + m_envGoalWidth - 1;
}


void ClientBase::ParseSeeMessage(vector<string>& toks)
{
	int cycle = ParserUtils::Str2Int(toks[1]);
    m_cycle = cycle;
    int unum, num1, num2;
    int pi;

	for (int i = 2; i < toks.size(); i += 3)
    {
		string toki = toks[i];
        if(toki == "score")
		{
            num1 = ParserUtils::Str2Int(toks[i + 1]);
            num2 = ParserUtils::Str2Int(toks[i + 2]);
            if (m_mySide == Sides::Left)
            {
                m_ourScore = num1;
                m_oppScore = num2;
            }
            else
            {
                m_ourScore = num2;
                m_oppScore = num1;
            }
		}
        else if(toki == "self")
		{
            num1 = ParserUtils::Str2Int(toks[i + 1]);
            num2 = ParserUtils::Str2Int(toks[i + 2]);
            m_playerPositions[m_myIndex].Set(num1, num2);
            m_lastSeePlayers[m_myIndex] = cycle;
            m_playerAvailabilities[m_myIndex] = true;
		}
        else if(toki == "b")
		{
            num1 = ParserUtils::Str2Int(toks[i + 1]);
            num2 = ParserUtils::Str2Int(toks[i + 2]);
            m_lastSeeBall = cycle;
            m_ballPosition.Set(num1, num2);
		}
        else if(toki == "l")
		{
            unum = ParserUtils::Str2Int(toks[i + 1]);
            num1 = ParserUtils::Str2Int(toks[i + 2]);
            num2 = ParserUtils::Str2Int(toks[i + 3]);
            pi = GetPlayerIndex(Sides::Left, unum);
            m_playerPositions[pi].Set(num1, num2);
            m_lastSeePlayers[pi] = cycle;
            m_playerAvailabilities[pi] = true;
            i++;
		}
		else if(toki == "r")
		{
            unum = ParserUtils::Str2Int(toks[i + 1]);
            num1 = ParserUtils::Str2Int(toks[i + 2]);
            num2 = ParserUtils::Str2Int(toks[i + 3]);
            pi = GetPlayerIndex(Sides::Right, unum);
            m_playerPositions[pi].Set(num1, num2);
            m_lastSeePlayers[pi] = cycle;
            m_playerAvailabilities[pi] = true;
            i++;
		}
    }
}

void ClientBase::ParseSettings(vector<string>& tokens)
{
	for (int i = 1; i < tokens.size(); i+=2)
    {
        int value;
		if (ParserUtils::TryStr2Int(tokens[i + 1], &value))
        {
			string toki = tokens[i];
            if(toki == "rows") 
			{
                this->m_envRows = value;
			}
			else if(toki == "cols")
			{
                this->m_envCols = value;
			}
			else if(toki == "goal-width")
			{
                this->m_envGoalWidth = value;
			}
			else if(toki == "pass-dist")
			{
                this->m_envPassDistance = value;
			}
			else if(toki == "visible-dist")
			{
                this->m_envVisibilityDistance = value;
			}
			else if(toki == "min-players")
			{
                this->m_envMinPlayers = value;
			}
			else if(toki == "max-players")
			{
                this->m_envMaxPlayers = value;
			}
        }
    }
}

void ClientBase::EpisodeTimeoutOurFail()
{
    Send("(episode-timeout our-fail)");
}

void ClientBase::EpisodeTimeoutOurPass()
{
    Send("(episode-timeout our-pass)");
}

void ClientBase::EpisodeTimeoutOppFail()
{
    Send("(episode-timeout opp-fail)");
}

void ClientBase::EpisodeTimeoutOppPass()
{
    Send("(episode-timeout opp-pass)");
}

void ClientBase::SetHomePosition(Position homePos)
{
    SetHomePosition(homePos.Row, homePos.Col);
}

void ClientBase::SetHomePosition(int r, int c)
{
    m_myHomePosRow = r;
    m_myHomePosCol = c;

    SendFormat("(home %d %d)", r, c);
}

vector<int> ClientBase::GetAvailableTeammatesUnums()
{
	vector<int> unums;

    int start = m_mySide == Sides::Left ? 0 : m_envMaxPlayers;
    int end = m_mySide == Sides::Left ? m_envMaxPlayers - 1 : 2 * m_envMaxPlayers - 1;
    for (int i = start; i <= end; ++i)
    {
        if (i != m_myIndex && m_playerAvailabilities[i])
			unums.push_back(i - start + 1);
    }

	return unums;
}


vector<int> ClientBase::GetAvailableTeammatesIndeces()
{
	vector<int> inds;
    int start = m_mySide == Sides::Left ? 0 : m_envMaxPlayers;
    int end = m_mySide == Sides::Left ? m_envMaxPlayers - 1 : 2 * m_envMaxPlayers - 1;
    for (int i = start; i <= end; ++i)
    {
        if (i != m_myIndex && m_playerAvailabilities[i])
			inds.push_back(i);
    }
	return inds;
}

vector<int> ClientBase::GetAvailableOpponentsIndeces()
{
	vector<int> inds;
    int start = m_mySide != Sides::Left ? 0 : m_envMaxPlayers;
    int end = m_mySide != Sides::Left ? m_envMaxPlayers - 1 : 2 * m_envMaxPlayers - 1;
    for (int i = start; i <= end; ++i)
    {
        if (m_playerAvailabilities[i])
			inds.push_back(i);
    }

	return inds;
}

vector<int> ClientBase::GetAvailableOpponentsUnums()
{
	vector<int> unums;
    int start = m_mySide != Sides::Left ? 0 : m_envMaxPlayers;
    int end = m_mySide != Sides::Left ? m_envMaxPlayers - 1 : 2 * m_envMaxPlayers - 1;
    for (int i = start; i <= end; ++i)
    {
        if (m_playerAvailabilities[i])
			unums.push_back(i - start + 1);
    }
	return unums;
}

int ClientBase::GetPlayerIndex(Sides side, int unum)
{
    if (side == Sides::Left)
    {
        return unum - 1;
    }
    else
    {
        return m_envMaxPlayers + unum - 1;
    }
}

int ClientBase::GetPlayerUnumFromIndex(int index)
{
    if (0 <= index && index < m_envMaxPlayers)
        return index + 1;
    else if (index < 2 * m_envMaxPlayers)
        return index - m_envMaxPlayers + 1;
    else
        return -1;
}

Position ClientBase::GetMyPosition()
{
    return m_playerPositions[m_myIndex];
}

bool ClientBase::AmIBallOwner()
{
    return (m_ballPosition == m_playerPositions[m_myIndex] && m_lastSeeBall == m_cycle) ;
}

bool ClientBase::AreWeBallOwner()
{
    int bi = GetBallOwnerPlayerIndex();
    int start = m_mySide == Sides::Left ? 0 : m_envMaxPlayers;
    int end = m_mySide == Sides::Left ? m_envMaxPlayers - 1 : 2*m_envMaxPlayers - 1;
    return (start <= bi && bi <= end);
}

int ClientBase::GetBallOwnerPlayerIndex()
{
    for (int i = 0; i < m_playerAvailabilities.size(); ++i)
    {
        if (m_playerAvailabilities[i] && m_ballPosition == m_playerPositions[i] && m_lastSeePlayers[i] == m_lastSeeBall)
            return i;
    }

    return -1;
}
		