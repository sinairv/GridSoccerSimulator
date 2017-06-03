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

#include "Position.h"

Position::Position() 
{
	Row = 0;
	Col = 0;
}

Position::Position(int r, int c)
{
    Row = r;
    Col = c;
}


Position::Position(Position& p) 
{
	Row = p.Row;
	Col = p.Col;
}

void Position::Set(int r, int c)
{
    Row = r;
    Col = c;
}

void Position::Set(Position &pos)
{
    Row = pos.Row;
    Col = pos.Col;
}

Position Position::operator+(Position &rhs)
{
    int r = Row + rhs.Row;
    int c = Col + rhs.Col;
    return Position(r, c);
}

Position Position::operator -(Position &pos)
{
    int r = Row - pos.Row;
    int c = Col - pos.Col;
    return Position(r, c);
}

bool Position::operator ==(Position &pos)
{
    return Equals(pos);
}

bool Position::operator !=(Position &pos)
{
    return !Equals(pos);
}

bool Position::Equals(Position &pos)
{
    if (Row == pos.Row && Col == pos.Col)
        return true;
    return false;
}

std::string Position::ToString()
{
	char szBuff[256];
	sprintf(szBuff, "%d %d", Row, Col);
	return std::string(szBuff);
}

Position Position::Clone()
{
    return Position(Row, Col);
}
