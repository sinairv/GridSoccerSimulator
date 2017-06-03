function agent1()
% change the following values accordingly
teamname = 'Our';
myUnum = 1;
serverHost = '127.0.0.1';
serverPort = 5050;

% leave the rest of the function as is
jarpath = 'gsjar/JavaSampleClient.jar';
javaaddpath(jarpath);
me = jsampleclient.ManualClient(serverHost, serverPort, teamname, myUnum);
fprintf('Starting player...');

theAgent = Agent();
theEnv = Env();
theEnv = theEnv.Update(me);
%theEnv.Display();
while (me.getIsNotGameStopped())
    if(me.UpdateFromServer())
        if(me.getIsGameStarted())
            theAgent = theAgent.Update(me);
            me.SendAction(me.StringActionToSoccerAction(Think(theAgent, theEnv)));
        end
    end
end

fprintf('Stopping player and clearing resources.');
me.OnGameStopped();

clear me
javarmpath(jarpath);

function act = Think(agent, env)
% Change this function to implement your own strategy
% see classes Agent, Env, and Commands 
% to see what you have at your disposal

% agent.SayWhatYouSee();
% env.Display();

r = rand(); 
if r < 0.2
    act = Commands.GoEast;
elseif r < 0.4
    act = Commands.GoWest;
elseif r < 0.6
    act = Commands.GoSouth;
elseif r < 0.8
    act = Commands.GoNorth;
else
    if agent.AmIBallOwner && ~isempty(agent.TeamMatesUnum)
        destUnum = agent.TeamMatesUnum(1);
        fprintf('Passing to teammate: %d at [%d]\n', destUnum, agent.Cycle);
        act = Commands.Pass(destUnum);
    else
        act = Commands.Hold;
    end
end
