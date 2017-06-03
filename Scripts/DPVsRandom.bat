start ../Bin/GridSoccerSimulator.exe
start /wait Wait.exe 500
start /min ../Bin/DPClient.exe DPClient 1
start /wait Wait.exe 1000
start /min ../Bin/RandomClient.exe Random 1
