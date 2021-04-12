using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour
{
    public static string levelName = "Main";
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


    public static void RoomCreatedSuccess(Packet _packet)
    {
     
        int _myId = _packet.ReadInt();
        int _roomID = _packet.ReadInt();
        Client.Instance.RoomID = _roomID;
        Debug.Log("Room was created with no issues! " + _roomID);
        SceneManager.LoadScene(levelName);
    }
    public static void RoomsReceived(Packet _packet)
    {
        int _roomID = _packet.ReadInt();
        string _roomName = _packet.ReadString();
        int _roomSize = _packet.ReadInt();
        int _spacesOccupiedInRoom = _packet.ReadInt();

        Debug.Log("Room callback recevied here " + _roomName);


    }


    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        NetworkManager.Instance.SpawnPlayer(_id, _username, _pos, _rot);
    }

    public static void MovePlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        NetworkManager.Instance.MovePlayer(_id,_pos,_rot);
    }

    public static void MoveObject(Packet _packet)
    { 
        int _id = _packet.ReadInt(); // player who sent id 
        int _objectNetID = _packet.ReadInt(); // net id
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        NetworkManager.Instance.MoveObject(_id, _objectNetID, _pos, _rot);
    }

    public static void RemovePlayer(Packet _packet)
    {
        int _id = _packet.ReadInt(); // player who is removed
        NetworkManager.Instance.RemovePlayer(_id);
    }




}
