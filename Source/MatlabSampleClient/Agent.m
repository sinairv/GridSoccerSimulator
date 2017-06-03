classdef Agent
    % Agent encapsulates the world model of a Grid-Soccer agent
    properties(GetAccess = 'public', SetAccess = 'private')
        Cycle;
        MyRow;
        MyCol;
        
        TeamMatesRow;
        TeamMatesCol;
        TeamMatesUnum;
        
        OpponentsRow;
        OpponentsCol;
        OpponentsUnum;
        
        BallRow;
        BallCol;
        
        LastSeeBall; % last cycle in which ball has been observed
        
        AmIBallOwner;
        AreWeBallOwner;
        AreTheyBallOwner;        
        
        OurScore;
        OppScore;
    end
    
    methods(Access = 'public')
        function this = Agent()
            this.Cycle = -1;
            this.MyRow = -1;
            this.MyCol = -1;
        end
        
        function this = Update(this, jplayer)
            this.Cycle = jplayer.getCycle();
            jMyPos = jplayer.GetMyPosition();
            this.MyRow = jMyPos.Row;
            this.MyCol = jMyPos.Col;

            jPlPos = jplayer.GetPlayerPositionsList();
            
            jTeammateInds = jplayer.GetAvailableTeammatesIndeces();
            tmCount = jTeammateInds.size();
            this.TeamMatesRow = zeros(1, tmCount);
            this.TeamMatesCol = zeros(1, tmCount);
            this.TeamMatesUnum = zeros(1, tmCount);
            
            for i=0:tmCount - 1
                pind = jTeammateInds.get(i);
                jpos = jPlPos.get(pind);
                this.TeamMatesRow(1, i+1) = jpos.Row;
                this.TeamMatesCol(1, i+1) = jpos.Col;
                this.TeamMatesUnum(1, i+1) = jplayer.GetPlayerUnumFromIndex(pind);
            end
            
            jOppInds = jplayer.GetAvailableOpponentsIndeces();
            tmCount = jOppInds.size();
            this.OpponentsRow = zeros(1, tmCount);
            this.OpponentsCol = zeros(1, tmCount);
            this.OpponentsUnum = zeros(1, tmCount);
            
            for i=0:tmCount - 1
                pind = jOppInds.get(i);
                jpos = jPlPos.get(pind);
                this.OpponentsRow(1, i+1) = jpos.Row;
                this.OpponentsCol(1, i+1) = jpos.Col;
                this.OpponentsUnum(1, i+1) = jplayer.GetPlayerUnumFromIndex(pind);
            end

            jBallPos = jplayer.getBallPosition();
            this.BallRow = jBallPos.Row;
            this.BallCol = jBallPos.Col;
            
            this.LastSeeBall = jplayer.getLastSeeBall();
            this.AmIBallOwner = this.getAmIBallOwner(); % jplayer.AmIBallOwnerAsInt();
            this.AreWeBallOwner = this.getAreWeBallOwner();
            this.AreTheyBallOwner = this.getAreTheyBallOwner();
            
            this.OurScore = jplayer.getOurScore();
            this.OppScore = jplayer.getOppScore();
        end
        
        % TODO rewrite amibo, arewebo, add last see players, teamm, opps
        % Create the env class as so
        % implement hand-coded client
        
        function res = getAmIBallOwner(this)
            res = 0;
            if this.LastSeeBall == this.Cycle && ...
                    this.MyRow == this.BallRow && ...
                    this.MyCol == this.BallCol
                res = 1;
            end            
        end
        
        function res = getAreWeBallOwner(this)
            res = 0;
            if this.LastSeeBall == this.Cycle
                if this.MyRow == this.BallRow && this.MyCol == this.BallCol
                    res = 1;
                else
                    for i=1:length(this.TeamMatesRow)
                        if(this.TeamMatesRow(i) == this.BallRow && ...
                            this.TeamMatesCol(i) == this.BallCol)
                            res = 1;
                            break;
                        end
                    end % end for each teammate pos
                end % if MyPos == BallPos
            end % if lastSeeBall
        end

        function res = getAreTheyBallOwner(this)
            res = 0;
            if this.LastSeeBall == this.Cycle
                for i=1:length(this.OpponentsRow)
                    if(this.OpponentsRow(i) == this.BallRow && ...
                        this.OpponentsCol(i) == this.BallCol)
                        res = 1;
                        break;
                    end
                end % end for each opponent pos
            end % if lastSeeBall
        end

        function DisplayMyPos(this)
            fprintf('[%d] at (%d, %d)\n', this.Cycle, this.MyRow, this.MyCol);
        end
        
        function SayWhatYouSee(this)
            fprintf('Cycle: %d\nOur: %d, Opp: %d\n', ...
                    this.Cycle, this.OurScore, this.OppScore);
            fprintf('Me: %d, %d\n', ...
                    this.MyRow, this.MyCol);
            fprintf('Ball: %d, %d  last seen at %d\n', ...
                this.BallRow, this.BallCol, this.LastSeeBall);
            fprintf('AmIBallOwner: %d\nAreWeBallOwner: %d\nAreTheyBallOwner: %d\n', ...
                this.AmIBallOwner, this.AreWeBallOwner, this.AreTheyBallOwner);

            fprintf('TeammatesUnum: %s\nTeamMatesRow: %s\nTeamMatesCol: %s\n', ...
                    mat2str(this.TeamMatesUnum), ...
                    mat2str(this.TeamMatesRow), ...
                    mat2str(this.TeamMatesCol));
            fprintf('OpponentsUnum: %s\nOpponentsRow: %s\nOpponentsCol: %s\n', ...
                    mat2str(this.OpponentsUnum), ...
                    mat2str(this.OpponentsRow), ...
                    mat2str(this.OpponentsCol));
            fprintf(':::::::::::::::::::::::::::::::::::::::::::::\n\n');
        end
    end
end
