classdef Env
    %ENV Stores environment settings
    properties(GetAccess = 'public', SetAccess = 'private')
        Rows;
        Cols;
        PassDistance;
        VisibilityDistance;
        GoalWidth; 
        MinPlayers;
        MaxPlayers;
        
        GoalUpperRow;
        GoalLowerRow;

        MySide;
        MyTeamName;
        MyUnum;
    end
    
    methods(Access = 'public')
        function this = Env()
            this.Rows = -1;
            this.Cols = -1;
        end
        
        function this = Update(this, jplayer)
            this.Rows = jplayer.getEnvRows();
            this.Cols = jplayer.getEnvCols();
            this.PassDistance = jplayer.getEnvPassDistance();
            this.VisibilityDistance = jplayer.getEnvVisibilityDistance();
            this.GoalWidth = jplayer.getEnvGoalWidth();
            this.MinPlayers = jplayer.getEnvMinPlayers();
            this.MaxPlayers = jplayer.getEnvMaxPlayers();
            isLeft = jplayer.isMySideLeft();
            if isLeft == 1
                this.MySide = 'Left';
            else
                this.MySide = 'Right';
            end
            this.MyTeamName = char(jplayer.getMyTeamName());
            this.MyUnum = jplayer.getMyUnum();

            this.GoalUpperRow = jplayer.getGoalUpperRow();
            this.GoalLowerRow = jplayer.getGoalLowerRow();
        end
        
        function Display(this)
            fprintf('Env Settings\n');
            fprintf('Rows: %d\n', this.Rows);
            fprintf('Cols: %d\n', this.Cols);
            fprintf('PassDistance: %d\n', this.PassDistance);
            fprintf('VisibilityDistance: %d\n', this.VisibilityDistance);
            fprintf('GoalWidth : %d\n', this.GoalWidth );
            fprintf('MinPlayers: %d\n', this.MinPlayers);
            fprintf('MaxPlayers: %d\n', this.MaxPlayers);
            fprintf('GoalUpperRow: %d\n', this.GoalUpperRow);
            fprintf('GoalLowerRow: %d\n', this.GoalLowerRow);
            fprintf('MySide: %s\n', this.MySide);
            fprintf('MyTeamName: %s\n', this.MyTeamName);
            fprintf('MyUnum: %d\n', this.MyUnum);
        end
    end
end

