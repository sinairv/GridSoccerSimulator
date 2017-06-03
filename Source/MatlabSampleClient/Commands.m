classdef Commands
    % Commands encapsulates commands to be sent to the server
    % with comprehensible names. There's no need to instantiate
    % from this class. All methods, and fields are static.
    
    properties(Constant)
        GoEast = '1';
        GoSouth = '2';
        GoWest = '3';
        GoNorth = '4';
        GoNorthEast = '5';
        GoSouthEast = '6';
        GoSouthWest = '7';
        GoNorthWest = '8';
        Hold = '0';
    end
    methods(Static)
        function pstr = Pass(unum)
            pstr = strcat({'p '}, num2str(unum));
        end
    end
end
