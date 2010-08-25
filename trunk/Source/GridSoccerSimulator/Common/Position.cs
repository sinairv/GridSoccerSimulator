using System;

namespace GridSoccer.Common
{
    public class Position
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Position(int r, int c)
        {
            this.Row = r;
            this.Col = c;
        }

        public Position()
            : this(0, 0)
        {
        }

        public Position(Position p)
            : this(p.Row, p.Col)
        {
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

        public static Position operator +(Position pos1, Position pos2)
        {
            int r = pos1.Row + pos2.Row;
            int c = pos1.Col + pos2.Col;
            return new Position(r, c);
        }

        public static bool operator ==(Position pos1, Position pos2)
        {
            return pos1.Equals(pos2);
        }

        public static bool operator !=(Position pos1, Position pos2)
        {
            return !pos1.Equals(pos2);
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;
            Position pos = obj as Position;
            if (this.Row == pos.Row && this.Col == pos.Col)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode() + Col.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", Row, Col);
        }

        public Position Clone()
        {
            return new Position(this);
        }
    }
}
