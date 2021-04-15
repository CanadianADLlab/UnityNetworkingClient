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
    public static void SetHost(Packet _packet)
    {
        Debug.Log("I am zee host");
        Client.Instance.IsHost = true;
    }
    public static void RoomJoined(Packet _packet)
    {
        int _myId = _packet.ReadInt();
        int _roomID = _packet.ReadInt();
        Client.Instance.RoomID = _roomID;
        Debug.Log("Room was created with no issues! " + _roomID);
        SceneManager.LoadScene(levelName);
    }

    public static void RoomJoinedFailed(Packet _packet)
    {
        int _myId = _packet.ReadInt();
        int _roomID = _packet.ReadInt();
        string message = _packet.ReadString();
        UIManager.Instance.ErrorText.text = message;
        Debug.Log("Room failed to join");
    }
    public static void RoomsReceived(Packet _packet)
    {
        int _roomID = _packet.ReadInt();
        string _roomName = _packet.ReadString();
        int _roomSize = _packet.ReadInt();
        int _spacesOccupiedInRoom = _packet.ReadInt();

        UIManager.Instance.AddRoomToDropDown(_roomID,_roomName,_roomSize,_spacesOccupiedInRoom);
    }


    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();
        bool _isVR = _packet.ReadBool();

        NetworkManager.Instance.SpawnPlayer(_id, _username, _pos, _rot, _isVR);
    }

    public static void MovePlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();
        bool _lerp = _packet.ReadBool();
        bool _isVR = _packet.ReadBool();

        if (!_isVR)
        {
            NetworkManager.Instance.MovePlayer(_id, _pos, _rot, _lerp);
        }
        else
        {
            Vector3 _leftHandPos = _packet.ReadVector3();
            Quaternion _leftHandRot = _packet.ReadQuaternion();
            Vector3 _rightHandPos = _packet.ReadVector3();
            Quaternion _rightHandRot = _packet.ReadQuaternion();
            NetworkManager.Instance.MovePlayer(_id, _pos, _rot, _leftHandPos, _leftHandRot, _rightHandPos, _rightHandRot, _lerp);
        }
    }

    public static void MoveObject(Packet _packet)
    { 
        int _id = _packet.ReadInt(); // player who sent id 
        int _objectNetID = _packet.ReadInt(); // net id
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        NetworkManager.Instance.MoveObject(_id, _objectNetID, _pos, _rot);
    }

    public static void SetObjectLocation(Packet _packet)
    {
        int _id = _packet.ReadInt(); // player who sent id 
        int _objectNetID = _packet.ReadInt(); // net id
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        NetworkManager.Instance.SetObjectLocation(_id, _objectNetID, _pos, _rot);
    }
    public static void RemovePlayer(Packet _packet)
    {
        int _id = _packet.ReadInt(); // player who is removed
        NetworkManager.Instance.RemovePlayer(_id);
    }




}
