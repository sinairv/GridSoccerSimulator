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

#ifndef CLIENTBASE_H
#define CLIENTBASE_H

#include <vector>
#include <iostream>
#include "TcpConnection.h"
#include "ClientException.h"
#include "Position.h"
#include "MessageInfo.h"
#include "SoccerAction.h"


class ClientBase
{
private:
	TcpConnection m_socket;
	
protected:
	int m_myIndex;
    int m_goalUpperRow;
    int m_goalLowerRow;

	int m_myHomePosRow;
	int m_myHomePosCol;

	Sides m_mySide;
    std::string m_myTeamName;
    int m_myUnum;

    int m_ourScore;
    int m_oppScore;

    int m_cycle;
    int m_lastSeeBall;
    Position m_ballPosition;
    bool m_isGameStarted;

	int m_cycleLength;
    bool m_turboMode;

	std::vector<int> m_lastSeePlayers;
    std::vector<Position> m_playerPositions;
    std::vector<bool> m_playerAvailabilities;

    int m_envRows;
    int m_envCols;
    int m_envPassDistance; 
    int m_envVisibilityDistance; 
    int m_envGoalWidth; 
    int m_envMinPlayers;
    int m_envMaxPlayers;

	bool m_gameIsNotStoped;


public:
	ClientBase(std::string serverAddr, int serverPort, std::string teamName, int unum);
	virtual void Start();
	virtual void OnGameStopped();

private:
	void ParseSettings(std::vector<std::string>& tokens);
	void ParseSeeMessage(std::vector<std::string>& toks);


    bool UpdateFromServer();
	void ThinkBase();
	void SendAction(ActionTypes acType);
	void SendAction(SoccerAction act);
	void Send(std::string msg);
	void SendFormat(const char *fmt, ...);

protected:
	int CalculateGoalUpperRow();
    int CalculateGoalLowerRow();

	virtual SoccerAction Think() = 0;

	void EpisodeTimeoutOurFail();
    void EpisodeTimeoutOurPass();
    void EpisodeTimeoutOppFail();
    void EpisodeTimeoutOppPass();
	void SetHomePosition(Position homePos);
    void SetHomePosition(int r, int c);

	std::vector<int> GetAvailableTeammatesIndeces();
	std::vector<int> GetAvailableOpponentsIndeces();
	std::vector<int> GetAvailableTeammatesUnums();
    std::vector<int> GetAvailableOpponentsUnums();
    int GetPlayerIndex(Sides side, int unum);
    int GetPlayerUnumFromIndex(int index);
    Position GetMyPosition();
    bool AmIBallOwner();
    bool AreWeBallOwner();
    int GetBallOwnerPlayerIndex();

};

#endif