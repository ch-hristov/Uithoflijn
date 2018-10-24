Uithoflijn simulation

1. How to build the project?
- Please use the latest version of .NET Core (2.1). This can be downloaded from the official website.

2. What additional libraries are used in this project?
- We use a library for graph structures(Quickgraph) in order to represent our tram track, 
a library for sampling from distributions(MathNet.Numerics) and a library for the priority queue(C5). 
All of these packages do not influence the logic of the simulation and are used as implementations of 
popular operations which are not present by default in the .NET Core framework.

3. Debugging and understanding the data
- You can also use the DEBUG variable in the Program.cs which produces debug files in the folders 
stat, debug and vis. The stat produces statistics, debug produces a history of the event queue operations
and the vis folder contains a 'visualization' of how the track looks like in both directions.

4. Note: We have tested the code on 8 cores. You can modify the parallelism options in the Program.cs file

5. We build the timetables in the file Terrain.cs on line 110.

6. Input/Output analysis and production of plots
- This can be found in the folders input_analysis and output_analysis and can be ran in R