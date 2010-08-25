using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.Common;

namespace GridSoccer.Simulator
{
    public class ObjectInfo
    {
        public SoccerObjects SoccerObject { get; set; }
        public int PlayerNumber { get; set; }
        public Sides Side { get; set; }
        public Position Position { get; set; }
        public Position HomePosition { get; private set; }
        public bool HasHomePos { get; set; }

        public int Row
        {
            get
            {
                return Position.Row;
            }

            set
            {
                Position.Row = value;
            }
        }

        public int Col
        {
            get
            {
                return Position.Col;
            }

            set
            {
                Position.Col = value;
            }
        }

        public ObjectInfo(SoccerObjects obj, int unum, Sides side, int r, int c, int homer, int homec)
        {
            this.SoccerObject = obj;
            this.PlayerNumber = unum;
            this.Side = side;
            this.Position = new Position(r, c);
            this.HomePosition = new Position(homer, homec);
            this.HasHomePos = false;
        }

        public ObjectInfo()
            : this(SoccerObjects.Player, 0, Sides.Left, 0, 0, 0, 0)
        {
        }
    }
}
