package jsampleclient;

import java.util.ArrayList;
import java.util.Arrays;

/**
 * A client whose functionalities are provided manually,
 * rather than by extension. This is going to be used in Matlab.
 */
public class ManualClient extends ClientBase {

    public ManualClient(String serverAddr, int serverPort, String teamname, int unum) throws Exception
    {
        super(serverAddr, serverPort, teamname, unum);

        if (unum == 1)
            SetHomePosition(2, 2);
        else if (unum == 2)
            SetHomePosition(4, 2);
        else if (unum == 3)
            SetHomePosition(3, 3);
    }

    public SoccerAction StringActionToSoccerAction(String act)
    {
        act = act.trim();
        if(act.length() <= 0)
            return new SoccerAction(ActionTypes.Hold);
        if(Character.toLowerCase(act.charAt(0)) == 'p')
        {
            if(act.length() <= 1)
                return new SoccerAction(ActionTypes.Hold);
            String unumSubstr = act.substring(1).trim();
            int unum = -1;
            try
            {
                unum = Integer.parseInt(unumSubstr);
                return new SoccerAction(ActionTypes.Pass, unum);
            }
            catch (Exception ex)
            {
                return new SoccerAction(ActionTypes.Hold);
            }
        }
        else
        {
            int code = -1;
            try
            {
                code = Integer.parseInt(act);
                return new SoccerAction(ActionTypes.fromInt(code));
            }
            catch (Exception ex)
            {
                return new SoccerAction(ActionTypes.Hold);
            }
        }
    }

    public ArrayList<Position> GetPlayerPositionsList()
    {
        return new ArrayList<Position>(Arrays.asList(this.getPlayerPositions()));
    }

    @Override
    public void Start() throws Exception {
        throw new Exception("A manual client must not call Start. It has to implement main loop itself!");
    }

    @Override
    protected SoccerAction Think() {
        return null;  //To change body of implemented methods use File | Settings | File Templates.
    }

    public int isMySideLeft()
    {
        return this.getMySide() == Sides.Left ? 1 : 0;
    }

}
