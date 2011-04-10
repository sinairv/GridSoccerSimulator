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

public abstract class IMessageInfo
{
    public MessageTypes MessageType;

    public static IMessageInfo GetIllegalMessage()
    {
        return new IllegalMessage();
    }
}

class IllegalMessage extends IMessageInfo
{
    public IllegalMessage()
    {
        MessageType = MessageTypes.Illegal;
    }
}

class InitOKMessage extends IMessageInfo
{
    public InitOKMessage()
    {
        MessageType = MessageTypes.InitOK;
    }

    public Sides Side;
}

class InitErrorMessage extends IMessageInfo
{
    public InitErrorMessage()
    {
        MessageType = MessageTypes.InitError;
    }
}

class ErrorMessage extends IMessageInfo
{
    public ErrorMessage()
    {
        MessageType = MessageTypes.Error;
    }
}

class StartMessage extends IMessageInfo
{
    public StartMessage()
    {
        MessageType = MessageTypes.Start;
    }
}

class StopMessage extends IMessageInfo
{
    public StopMessage()
    {
        MessageType = MessageTypes.Stop;
    }
}

class CycleMessage extends IMessageInfo
{
    public CycleMessage()
    {
        MessageType = MessageTypes.Cycle;
    }

    public int CycleLength;
}

class SeeMessage extends IMessageInfo
{
    public SeeMessage()
    {
        MessageType = MessageTypes.See;
    }

    public String[] SeeMsgTokens;
}

class SettingsMessage extends IMessageInfo
{
    public SettingsMessage ()
    {
        MessageType = MessageTypes.Settings;
    }

    public String[] SettingsMsgTokens;
}

class TurboMessage extends IMessageInfo
{
    public TurboMessage()
    {
        MessageType = MessageTypes.Turbo;
    }

    public boolean TurboOn;
}
