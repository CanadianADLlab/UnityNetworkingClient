using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static int DataBufferSize = 4096;
    public string IPAddress = "127.0.0.1";

    public int Port = 90;
    public int MyID;
     
    [HideInInspector]
    public TCP Tcp;


    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    void Start()
    { 
        Tcp = new TCP();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        Tcp.Connect();
    }

    public class TCP
    {
        public TcpClient Socket;

        private NetworkStream stream;
        private byte[] receiveBuffer;
        private Packet receivedData;

        public TCP()
        {
        }
        public void SendData(Packet _packet)
        {
            try
            {
                if (Socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending data to player " + e);
            }
        }

        public void Connect()
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };
            receiveBuffer = new byte[DataBufferSize];
            Socket.BeginConnect(Instance.IPAddress, Instance.Port, ConnectCallBack, Socket);
        }
        private void ConnectCallBack(IAsyncResult _result)
        {
            Socket.EndConnect(_result);

            if (!Socket.Connected)
            {
                return;
            }
            stream = Socket.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallBack, null);
        }
        private void ReceiveCallBack(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    return;
                }
                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallBack, null);
            }
            catch (Exception e)
            {
                Debug.Log("Error receiving packet " + e);
            }
        }
        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);
            if(receivedData.UnreadLength() >= 4) // means an id was sent because an int contains 4 bytes
            {
                _packetLength = receivedData.ReadInt();
                if(_packetLength <= 0)
                {
                    return true;
                }
            }

            while(_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });
                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4) // means an id was sent because an int contains 4 bytes
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if(_packetLength <= 1)
            {
                return true;
            }
            return false;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome,ClientHandle.Welcome}
        };
        Debug.Log("Client data inited");
    }
    #region singleton
    public static Client Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Client script already exist destorying the one attatched to " + transform.name);
            Destroy(this);
        } 
    }

    #endregion
}
