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


#ifndef MESSAGEINFO_H
#define MESSAGEINFO_H

#include <string>
#include <vector>

enum MessageTypes
{
    Illegal, Init, Move, Pass, Hold, Home, EpisodeTimeout,
    Settings, InitOK, InitError, Error, See, Start, Stop, Cycle, Turbo
};

enum Sides
{
    Left, Right
};


class IMessageInfo
{
protected:
	MessageTypes m_messageType;

public:
	IMessageInfo() {}
	virtual MessageTypes GetMessageType()
	{
		return m_messageType;
	}

    static IMessageInfo* GetIllegalMessage();
};

class IllegalMessage : public IMessageInfo
{
public: 
	IllegalMessage();
};

class InitOKMessage : public IMessageInfo
{
public:
	InitOKMessage(Sides side)
    {
        m_messageType = MessageTypes::InitOK;
		Side = side;
    }

public:
	Sides Side;
};

class InitErrorMessage : public IMessageInfo
{
public:
	InitErrorMessage()
    {
        m_messageType = MessageTypes::InitError;
    }
};

class ErrorMessage : public IMessageInfo
{
public:
	ErrorMessage()
    {
        m_messageType = MessageTypes::Error;
    }
};

class StartMessage : public IMessageInfo
{
public:
	StartMessage()
    {
        m_messageType = MessageTypes::Start;
    }
};

class StopMessage : public IMessageInfo
{
public:
	StopMessage()
    {
        m_messageType = MessageTypes::Stop;
    }
};

class CycleMessage : public IMessageInfo
{
public:
	CycleMessage(int cycleLength)
    {
        m_messageType = MessageTypes::Cycle;
		this->CycleLength = cycleLength;
    }

    int CycleLength;
};

class SeeMessage : public IMessageInfo
{
public:
	SeeMessage(std::vector<std::string> toks)
		: SeeMsgTokens(toks)
    {
        m_messageType = MessageTypes::See;
    }

	std::vector<std::string> SeeMsgTokens;
};

class SettingsMessage : public IMessageInfo
{
public:
	SettingsMessage (std::vector<std::string> toks)
		: SettingsMsgTokens(toks)
    {
        m_messageType = MessageTypes::Settings;
    }

    std::vector<std::string> SettingsMsgTokens;
};

class TurboMessage : public IMessageInfo
{
public:
	TurboMessage(bool isTurboOn)
    {
        m_messageType = MessageTypes::Turbo;
		TurboOn = isTurboOn;
    }

    bool TurboOn;
};

#endif
