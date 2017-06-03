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

// Almost all networking material in this code is acquired form
// AutSoccer, by Babak Behsaz, and Arash Rahimi - Amirkabir University of Technology

#include <iostream>
#include <io.h>
#include "TcpConnection.h"
#include "ClientException.h"
#include "ParserUtils.h"

using namespace std;

TcpConnection::TcpConnection(std::string serverAddress, int port)
	: serverAddress(serverAddress), port(port)
{
	if(serverAddress.empty())
		throw ClientException("Server address is empty");

	DWORD version = MAKEWORD(1, 1);
	WSADATA data;
	int given = WSAStartup(version, &data);

	host = ::gethostbyname(serverAddress.c_str());

	if (host == NULL)
		throw string("Could not resolve the given hostname");

	m_socket = ::socket(AF_INET, SOCK_STREAM, 0);

	if (socket < 0)
		throw string("Error creating a TCP socket");

	Connect();
}

TcpConnection::~TcpConnection()
{
	::closesocket(m_socket);
}

void TcpConnection::Connect()
{
	sockaddr_in s;
	memset(&s, 0, sizeof(s));
	s.sin_family = AF_INET;
	s.sin_addr.s_addr = htonl(INADDR_ANY);
	s.sin_addr.s_addr = ((struct in_addr *)(host->h_addr))->s_addr;
	s.sin_port = htons(port);

	int result = ::connect(m_socket, (sockaddr *) &s, sizeof(s));

	if (result < 0)
		throw string("Error Connecting To Server");
}

void TcpConnection::SendString(std::string value)
{
	int len = (int) value.size();
	//cout << "sending: " << value << endl;
	const char *buf = value.c_str();

	int curSent = ::send(m_socket, buf, len, 0);
	if (curSent < 1)
		throw ClientException("Error Sending Data To Server");
}

void TcpConnection::SendFormat(const char *fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	std::string s = ParserUtils::FormatString(fmt, args);
	va_end(args);

	SendString(s);
}

string TcpConnection::ReceiveString()
{
	static char szBuff[1024];
	szBuff[0] = 0;

	int rcvd = ::recv(m_socket, szBuff, 1024, 0);
	if (rcvd < 1)
		throw ClientException("Error Reading Data From Server");
	
	szBuff[rcvd] = 0;
	
	//cout << "recv: " << string(szBuff) << endl;
	return string(szBuff);
}
