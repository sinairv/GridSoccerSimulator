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

public class Position
{
    public int Row;
    public int Col;
    
    public Position(int r, int c)
    {
        this.Row = r;
        this.Col = c;
    }
    
    public Position()
    {
        this(0, 0);
    }
    
    public Position(Position p)
    {
        this(p.Row, p.Col);
    }
    
    public void Set(int r, int c)
    {
        this.Row = r;
        this.Col = c;
    }
    
    public void Set(Position pos)
    {
        this.Row = pos.Row;
        this.Col = pos.Col;
    }
    
    public static Position Add(Position pos1, Position pos2)
    {
        int r = pos1.Row + pos2.Row;
        int c = pos1.Col + pos2.Col;
        return new Position(r, c);
    }
    
    public static boolean IsEqual(Position pos1, Position pos2)
    {
        return pos1.equals(pos2);
    }
    
    public static boolean IsNotEqual(Position pos1, Position pos2)
    {
        return !pos1.equals(pos2);
    }
    
    @Override
    public  boolean equals(Object obj)
    {
        if (this == obj)
            return true;
        Position pos = (Position)obj;
        if (this.Row == pos.Row && this.Col == pos.Col)
            return true;
        return false;
    }
    
    @Override
    public int hashCode()
    {
        return Row * 10 + Col;
    }
    
    @Override
    public String toString()
    {
        return String.format("%d %d", Row, Col);
    }
    
    @Override
    public Position clone()
    {
        return new Position(this);
    }
}
