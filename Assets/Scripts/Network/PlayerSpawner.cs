using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Tells server that I have loaded so we can spawn
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
  
    void Awake()
    {
        ClientSend.SendLevelLoaded();
        Debug.Log("Sending level loaded to the server");
    }

}
