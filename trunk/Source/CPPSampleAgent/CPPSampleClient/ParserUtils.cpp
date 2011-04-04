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

#include "ParserUtils.h"

#include <sstream>
using namespace std;

vector<string> ParserUtils::TokenizeString(const string& str)
{
	vector<string> vecToks;
	char* toks;
	toks = strtok((char *)str.c_str(), " ()\t\r\n");
	while(toks != NULL)
	{
		vecToks.push_back(string(toks));
		toks = strtok(NULL, " ()\t\r\n");
	}

	return vecToks;
}

double ParserUtils::Str2Double (const string &str) 
{
	stringstream ss(str);
	double num;
	if((ss >> num).fail())
	{ 
		//ERROR 
	}
	return num;
}

int ParserUtils::Str2Int (const string &str) 
{
	stringstream ss(str);
	int num;
	if((ss >> num).fail())
	{ 
		//ERROR 
	}
	return num;
}

bool ParserUtils::TryStr2Int (const string &str, int* outNum) 
{
	stringstream ss(str);
	int num;
	if((ss >> num).fail())
	{ 
		return false;
	}
	*outNum = num;
	return true;
}

string ParserUtils::FormatString(const char *fmt, va_list args)
{    
	if (!fmt) return "";    
	int   result = -1, length = 256;    
	char *buffer = 0;    
	while (result == -1)    
	{        
		if (buffer) delete [] buffer;        
		buffer = new char [length + 1];        
		memset(buffer, 0, length + 1);
		result = _vsnprintf(buffer, length, fmt, args);
		length *= 2;    
	}
	std::string s(buffer);
	delete [] buffer;
	return s;
}
