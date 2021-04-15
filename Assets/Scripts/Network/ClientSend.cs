using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.Instance.Tcp.SendData(_packet);
    }
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.Instance.Udp.SendData(_packet);
    }

    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.Instance.MyID);
            _packet.Write(UIManager.Instance.UserNameField.text);
          
            Client.Instance.IsConnected = true; // receives response from the server so we connected
            SendTCPData(_packet);
        }
    }

    public static void GetRooms()
    {
        using (Packet _packet = new Packet((int)ClientPackets.getRooms))
        {
            _packet.Write(Client.Instance.MyID);
            _packet.Write(UIManager.Instance.UserNameField.text);
            Client.Instance.IsConnected = true; // receives response from the server so we connected
            SendTCPData(_packet);
        }
    }
    
    public static void CreateRoom(string roomName,int roomSize)
    {
        if (Client.Instance && Client.Instance.IsConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.createRoom))
            {
                _packet.Write(Client.Instance.MyID);
                _packet.Write(roomName);
                _packet.Write(roomSize);
            
                SendUDPData(_packet);
            }
        }
    }

    public static void JoinRoom(int _roomID)
    {
        if (Client.Instance && Client.Instance.IsConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.joinRoom))
            {
                _packet.Write(Client.Instance.MyID);
                _packet.Write(_roomID);
              
                SendUDPData(_packet);
            }
        }
    }

    public static void SendLevelLoaded()
    {
        if (Client.Instance && Client.Instance.IsConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.levelLoaded))
            {
                _packet.Write(Client.Instance.MyID);
                _packet.Write(Client.Instance.Name) ;
                _packet.Write(Client.Instance.RoomID);
                _packet.Write(Client.Instance.IsVR);
                SendTCPData(_packet);
            }
        }
    }

    public static void SendPlayerValues(Vector3 _pos, Quaternion _rot, bool lerp = true)
    {
        if (Client.Instance && Client.Instance.IsConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
            {
                _packet.Write(Client.Instance.MyID);
                _packet.Write(Client.Instance.RoomID);

                _packet.Write(_pos);
                _packet.Write(_rot);

                _packet.Write(lerp);
                _packet.Write(Client.Instance.IsVR);
                SendUDPData(_packet);
            }
        }
    }

    public static void SendPlayerValues(Vector3 _pos, Quaternion _rot, Vector3 _leftHandPos, Quaternion _leftHandRot, Vector3 _rightHandPos, Quaternion _rightHandRot, bool lerp = true)
    {
        if (Client.Instance && Client.Instance.IsConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
            {
                _packet.Write(Client.Instance.MyID);
                _packet.Write(Client.Instance.RoomID);

                _packet.Write(_pos);
                _packet.Write(_rot);

                _packet.Write(lerp);
                _packet.Write(Client.Instance.IsVR);

                _packet.Write(_leftHandPos);
                _packet.Write(_leftHandRot);

                _packet.Write(_rightHandPos);
                _packet.Write(_rightHandRot);

                SendUDPData(_packet);
            }
        }
    }

    public static void SendDisconnect() // removes player from the server
    {
        if (Client.Instance && Client.Instance.IsConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.playerDisconnect))
            {
                _packet.Write(Client.Instance.MyID);
                _packet.Write(Client.Instance.RoomID);
                print(Client.Instance.RoomID);
                SendTCPData(_packet); // it should be tcp because we don't wanna lose this
            }
        }
    }
 
    /// <summary>
    ///  Sends object movement to the server
    /// </summary>
    /// <param name="networkID">network id of the object</param>
    /// <param name="_pos">Vector3 position</param>
    /// <param name="_rot">Quat rotation</param>
    public static void SendGameObjectMovedValues(int networkID,Vector3 _pos, Quaternion _rot)
    {
        if (Client.Instance && Client.Instance.IsConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.objectMovement)) 
            {
                _packet.Write(Client.Instance.MyID); // players id 
                _packet.Write(Client.Instance.RoomID);
                _packet.Write(networkID); // objects id
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendUDPData(_packet);
            }
        }
    }

    public static void SetObjectPosition(int _clientID,int networkID, Vector3 _pos, Quaternion _rot) // behaves simular to the objectmoved but forces a position with no lerp
    {
        if (Client.Instance && Client.Instance.IsConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.objectLocationSet))
            {
                _packet.Write(_clientID); // players id 
                _packet.Write(Client.Instance.RoomID);
                _packet.Write(networkID); // objects id
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendUDPData(_packet);
            }
        }
    }    

}
