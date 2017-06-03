start mono ../Bin/GridSoccerSimulator.exe
start /wait Wait.exe 500
start /min mono ../Bin/RandomClient.exe Random 1
start /wait Wait.exe 500
start /min  mono ../Bin/RandomClient.exe Random 2
start /wait Wait.exe 1000
start /min mono ../Bin/HandCodedClient.exe HandCoded 1
start /min mono ../Bin/HandCodedClient.exe HandCoded 2
