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
players in the room and theres a cube they can both push around and it will be visible over the network. The whole idea is to keep this as simple as possible to be able to 
use as a template for other future projects. The main two scripts responsible for tracking objects with the server and the NetworkObject and the Network Character which I'll cover later.


<h1>A brief overview of how things work</h1>

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
If you want an object in the scenes movement to be tracked for everyone to see when they are connected just attach the NetworkGameObject.cs Script to it. This just waits for any change in the position and rotation and the object
and if it changes it tells the network to move it for everyone else. Just make sure the NetworkID the object has is unique. I think NetworkGameObject should cover the majority of basic interactions but if you need to handle anything more complicated
you will have to dive in and change the code a bit.
<br><br>

![networkgameobject](https://user-images.githubusercontent.com/39784801/115235427-29dae400-a0e8-11eb-9858-314748ba0fbb.png)

<br>

<h3>The Menu and UI</h3>

The Menu Scene contains a GameObject called NetworkingUI containing all the different UI widgets for connecting to and joining rooms. You can change around the UI but if you want it to work without modifying the code make sure you set all the public objects there and follow the flow of how it originally worked. 


<h1>Adding a new networked function</h1>
So if you want to write a custom function that sends something different than what is already here I'll explain the process.<br>
For this to work correctly both the client and the server will have to be modified, client sends it and the server has to figure out what to do with that data and who to send it to.
<br>
This example function will just be a button pressed that sends a int to the server who then sends it back to every client in the lobby.
<h3>Add Player UI</h3>
Lets add the button to the local player that we can click, so under the localPlayerPrefab add a Canvas and a button under that. 
<br>

![structureofplayer](https://user-images.githubusercontent.com/39784801/115254580-7aa80800-a0fb-11eb-9a58-1f7b2696bb2e.png)
<br>
From there we are going to create a script called PlayerUIManager and attatch it to the root of the localplayerprefab. This will just contain the click event.
Now write a public function that generates a random int.
<br>

![playerui1](https://user-images.githubusercontent.com/39784801/115254767-aa571000-a0fb-11eb-9a9a-967922150904.png)
<br>
Now set the button under the UI to call that event
<br>

![clickeventbutton](https://user-images.githubusercontent.com/39784801/115254971-da061800-a0fb-11eb-9864-507a9ea3524d.png)
<br>
Okay now the front end is set up so let's modify the SendClick() event to actually send that int to the server.
<br>
Open up packet.cs in the unity project and add intRespond to the ServerPackets enum and intSend to the ClientPackets enum.
<br>
![addthese](https://user-images.githubusercontent.com/39784801/115255537-5d276e00-a0fc-11eb-8313-d06a8bbb20b2.png)
<br>
Now open up the server code if you haven't pulled it you can do so from here https://github.com/CanadianADLlab/UnityNetworkingServer and paste the enums over top of the same 
enums in the packets.cs file on the server.
Next in the client unity project add a function called SendInt(int _num) to the ClientSend.cs script (copy the screenshot below).
![packet](https://user-images.githubusercontent.com/39784801/115256420-31f14e80-a0fd-11eb-9f14-9ae23fe3de75.png)











