start ../Bin/GridSoccerSimulator.exe
start /wait Wait.exe 500
start /min ../Bin/RandomClient.exe Random 1
start /wait Wait.exe 500
start /min ../Bin/RandomClient.exe Random 2
start /wait Wait.exe 1000
rem start /min ../Bin/RandomClient.exe Fandom 1
rem start /min ../Bin/RandomClient.exe Fandom 2
start /min ../Bin/HandCodedClient.exe -name HandCoded -n 1
start /min ../Bin/HandCodedClient.exe -name HandCoded -n 2
