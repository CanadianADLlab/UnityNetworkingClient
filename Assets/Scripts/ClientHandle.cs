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


    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        GameManager.Instance.SpawnPlayer(_id, _username, _pos, _rot);
    }

    public static void MovePlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        GameManager.Instance.MovePlayer(_id,_pos,_rot);
    }


}
