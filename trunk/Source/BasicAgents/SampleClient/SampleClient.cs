using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.ClientBasic;
using GridSoccer.Common;

namespace GridSoccer.SampleClient
{
    public class SampleClient : ClientBase
    {
        public SampleClient(string serverAddr, int serverPort, string teamname, int unum)
            : base(serverAddr, serverPort, teamname, unum)
        {
        }

        public SampleClient() : base("127.0.0.1", 5050, "SampleClient", 1)
        {
        }

        protected override SoccerAction Think()
        {
            for (int i = 0; i < PlayerPositions.Length; ++i)
            {
                if(LastSeePlayers[i] == Cycle)
                    Console.WriteLine(PlayerPositions[i].ToString());
            }
            Console.WriteLine("------------");

            ActionTypes at = ActionTypes.Hold;
            Position pos = GetMyPosition();
            if (pos.Row == 1 && pos.Col != EnvCols)
                at = ActionTypes.MoveEast;
            else if (pos.Col == 1)
                at = ActionTypes.MoveNorth;
            else if (pos.Row == EnvRows)
                at = ActionTypes.MoveWest;
            else if (pos.Col == EnvCols)
                at = ActionTypes.MoveSouth;
            return new SoccerAction(at);
        }
    }
}
