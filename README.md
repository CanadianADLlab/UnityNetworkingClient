# UnityNetworkingClient 
Quick guide for setting up this example.

<h1>Overview</h1>

This is a modified version of the Unity client from this wonderful tutorial https://www.youtube.com/watch?v=uh8XaC0Y5MA&t=8s. The main change I added was rooms, 
also input is not sent to the server we send position and rotation here. I did this because the application we plan on using this for is training and likely won't have people cheating.

<h1>Using the example scene</h1>

First pull the server and follow the guide for setting it up from here https://github.com/CanadianADLlab/UnityNetworkingServer. Now just open the MenuScene and go to the networking manager
gameobject and configure the port and ip to match the server you have running.
<br>
![networkmanager](https://user-images.githubusercontent.com/39784801/115231529-ad460680-a0e3-11eb-8581-737d4d444b45.png)

From there just run the scene, make a room and then join it on another build for the networking example to work. There's not a whole lot going on here it will just spawn an instance of the 
players in the room and theres a cube they can both push around and it will be visable over the network. The whole idea is to keep this as simple as possible to be able to 
use as a template for other future projects. The main two scripts responsible for tracking objects with the server and the NetworkObject and the Network Character which I'll cover later.


<h1>A breif overview of how things work</h1>

I just want to outline here how to change and modify things to work for your own projects, not like a super in depth guide of how networking works.<br>
<h3>Player Prefabs</h3>
To change the players that get spawned in modify the network gameobjects,localplayerprefab and the player prefab. The localplayer is the player who client is playing as, and the 
player prefab is anyone that the client is playing with. <br>

![networkmanagerscript](https://user-images.githubusercontent.com/39784801/115233035-68bb6a80-a0e5-11eb-8756-a0b55c3ccc6f.png)

<h3>Loading into scenes</h3>
<br>
The scene that gets loaded into after joining a room is a public string called levelName under ClientHandle.cs change this to load into whatever scene you want after joining.
<br>

So the scene the players connect to in this example is called main and it is just a plane and 3 cubes, it can be whatever you want just make sure it is the same layout for everyone using.
<br>

<h3>Tracked gameobjects other than player</h3>
If you want an object in the scenes movement to be tracked for everyone to see when they are connected just attatch the NetworkGameObject.cs Script to it. This just waits for any change in the position and rotation and the object
and if it changes it tells the network to move it for everyone else. Just make sure the NetworkID the object has is unique. I think NetworkGameObject should cover the majority of basic interactions but if you need to handle anything more complicated
you will have to dive in and change the code a bit.
<br><br>

![networkgameobject](https://user-images.githubusercontent.com/39784801/115235427-29dae400-a0e8-11eb-9858-314748ba0fbb.png)

<br>



