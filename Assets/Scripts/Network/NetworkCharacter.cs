using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour
{
    public void FixedUpdate()
    {
        ClientSend.SendPlayerValues(transform.position, transform.rotation);
    }
}
