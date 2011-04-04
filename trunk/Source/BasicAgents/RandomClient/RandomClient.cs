using System;
using System.Linq;
using GridSoccer.ClientBasic;
using GridSoccer.Common;

namespace GridSoccer.RandomClient
{
    public class RandomClient : ClientBase
    {
        private readonly bool MoveKings = false;
        private static Random rnd = new Random();

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

        protected override SoccerAction Think()
        {
            int[] teammates = this.GetAvailableTeammatesUnums().ToArray();
            int numActions = SoccerAction.GetActionCount(this.MoveKings, teammates.Length + 1);
            int ai = rnd.Next(0, numActions);
            return SoccerAction.GetActionFromIndex(ai, this.MoveKings, this.MyUnum);

            //int dst = -1;
            //ActionTypes at = ActionTypes.Hold;


            //SoccerAction.GetActionTypeFromIndex(ai);
            //at = (ActionTypes)
            //if (at == ActionTypes.Pass)
            //{
            //    if (teammates.Length <= 0)
            //        at = ActionTypes.Hold;
            //    else
            //        dst = teammates[rnd.Next(0, teammates.Length)];
            //}

            //return new SoccerAction(at, dst);
        }
    }
}
