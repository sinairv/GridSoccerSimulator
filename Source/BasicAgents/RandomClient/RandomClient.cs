using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridSoccer.ClientBasic;
using GridSoccer.Common;

namespace GridSoccer.RandomClient
{
    public class RandomClient : ClientBase
    {
        public RandomClient(string serverAddr, int serverPort, string teamname, int unum)
            : base(serverAddr, serverPort, teamname, unum)
        {
            if (unum == 1)
                SetHomePosition(2, 2);
            else if (unum == 2)
                SetHomePosition(4, 2);
            else if (unum == 3)
                SetHomePosition(3, 3);
        }

        private static Random rnd = new Random();
        protected override SoccerAction Think()
        {
            int dst = -1;
            ActionTypes at = ActionTypes.Hold;

            at = (ActionTypes)rnd.Next(0, 10);
            if (at == ActionTypes.Pass)
            {
                int[] teammates = this.GetAvailableTeammatesUnums().ToArray();
                if (teammates.Length <= 0)
                    at = ActionTypes.Hold;
                else
                    dst = teammates[rnd.Next(0, teammates.Length)];
            }

            return new SoccerAction(at, dst);
        }
    }
}
