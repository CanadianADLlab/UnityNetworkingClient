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
            SendTCPData(_packet);
        }
    }

    public static void SendPlayerValues(Vector3 _pos, Quaternion _rot)
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
