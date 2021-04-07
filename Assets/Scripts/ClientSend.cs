using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
   private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Debug.Log("Sending data");
        Client.Instance.Tcp.SendData(_packet);
    }
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Debug.Log("Sending data");
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

}
