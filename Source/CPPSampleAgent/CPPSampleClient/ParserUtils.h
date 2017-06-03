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


#ifndef PARSERUTILS_H
#define PARSERUTILS_H

#include <string>
#include <vector>

class ParserUtils
{
public:
	static std::vector<std::string> TokenizeString(const std::string& str);
	static double Str2Double (const std::string &str);
	static int Str2Int (const std::string &str);
	static bool TryStr2Int (const std::string &str, int* outNum);
	static std::string FormatString(const char *fmt, va_list args);
};
#endif
