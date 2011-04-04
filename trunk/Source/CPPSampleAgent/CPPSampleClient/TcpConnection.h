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

#ifndef TCPCONNECTION_H
#define TCPCONNECTION_H

#include <winsock.h>
#include <windows.h>
#include <windowsx.h>
#include <string>

class TcpConnection
{
protected:
	SOCKET m_socket;

	std::string serverAddress;
	int port;
	hostent *host;

public:
	TcpConnection(std::string serverAddress, int port);
	virtual ~TcpConnection();

	void Connect();
	void SendString(std::string value);
	void SendFormat(const char *fmt, ...);

	std::string ReceiveString();
};

#endif
