using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public  int ID { get; set; } 
    public  string UserName { get; set; }

    public void FixedUpdate()
    {
        ClientSend.SendPlayerValues(transform.position,transform.rotation);
    }
}
