#Matlab sample agent for Grid-Soccer simulator

## How to create your own agent:
* Copy this folder structure to your own workplace
* Make sure you have the `gsjar` folder as well as the `JavaSampleClient.jar` within.
* Create a copy of `agent1.m` and rename it as well as the `agent1` function inside it, as you wish.
* Change the first 4 lines of `agent1` function to reflect your agent team and uniform-number as wel  as connectivity information to Grid-Soccer simulator.
* Change the implementation of the `Think` function to reflect your own desired strategy.
* Acquire environment properties from the `env` variable, which is an instance of the `Env` class.
* Acquire the world-model in each cycle from the `agent` variable, which is an instance of the  `Agent` class.
* Create actions to be performed by the agent using static fields, and functions of the `Commands`  class.

