start ../Bin/GridSoccerSimulator.exe
start /wait Wait.exe 500
start /min ../Bin/HyperNEATControllerAgent.exe -exp FourDFieldSubs -name HyperNEAT4D -team 3
start /wait Wait.exe 1000
start /min ../Bin/HandCodedClient.exe -name OffTrainer -n 1 -off -trainer 40
start /min ../Bin/HandCodedClient.exe -name OffTrainer -n 2 -off 
start /min ../Bin/HandCodedClient.exe -name OffTrainer -n 3 -off 
