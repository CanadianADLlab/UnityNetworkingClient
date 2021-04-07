using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        Debug.Log("Is this ever called");
        Debug.Log("Message from server " + _msg);
        Client.Instance.MyID = _myId;
        ClientSend.WelcomeReceived();
        Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.Socket.Client.LocalEndPoint).Port);
    }

    public static void UDPTest(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Debug.Log("Message from server " + _msg);
        ClientSend.UDPTestReceived();
    }
}
