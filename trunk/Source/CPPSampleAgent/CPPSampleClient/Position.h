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


#ifndef POSITION_H
#define POSITION_H

#include <string>

class Position
{
public:
	int Row;
    int Col;

	Position();
	Position(int r, int c);
    Position(Position& p);
    void Set(int r, int c);
    void Set(Position &pos);
	Position operator+(Position &rhs);
    Position operator-(Position &pos);
    bool operator ==(Position &pos);
    bool operator !=(Position &pos);
    bool Equals(Position &pos);
    std::string ToString();
    Position Clone();
};

#endif
