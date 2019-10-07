# war-project
My crash course of learning C# by writing a very simple backend and client for the game of War

What it lacks in polish and finesse, it makes up for in seperation between the backend service and the client. My guiding heuristic
on this was to keep asking myself, "can I still run the whole game from Postman?"

The [WarGameService](GoodOldWar/WarGameService.cs) is still a fun read because it shows an evolution of style from doing things in
a very brute-force way to eventually discovering LINQ and anonymous types.
