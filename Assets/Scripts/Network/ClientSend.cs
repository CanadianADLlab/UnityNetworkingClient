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
            Client.Instance.isConnected = true; // receives response from the server so we connected
            SendTCPData(_packet);
        }
    }

    public static void SendPlayerValues(Vector3 _pos, Quaternion _rot)
    {
        if (Client.Instance && Client.Instance.isConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
            {
                _packet.Write(Client.Instance.MyID);
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendUDPData(_packet);
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
        if (Client.Instance && Client.Instance.isConnected)
        {
            using (Packet _packet = new Packet((int)ClientPackets.objectMovement)) 
            {
                _packet.Write(Client.Instance.MyID); // players id 
                _packet.Write(networkID); // objects id
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendUDPData(_packet);
            }
        }
    }

}
