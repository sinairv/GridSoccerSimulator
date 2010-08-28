start ../Bin/GridSoccerSimulator.exe
start /wait Wait.exe 500
start /min ../Bin/RandomClient.exe Random 1
start /wait Wait.exe 500
start /min ../Bin/RandomClient.exe Random 2
start /wait Wait.exe 1000
start /min ../Bin/HandCodedClient.exe HandCoded 1
start /min ../Bin/HandCodedClient.exe HandCoded 2
