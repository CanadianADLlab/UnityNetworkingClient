using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public static int DataBufferSize = 4096;
    public string IP = "127.0.0.1";

    public int Port = 90;
    public int MyID;
    public bool IsVR = false;
    public bool IsConnected = false;
    public bool IsHost = false; // theres not actually a host but I guess the host is like the owner of the room

    public string Name; // name set from the ui
    public int RoomID = -99; // room id the client is in (only gets set when level is loaded)
    [HideInInspector]
    public TCP Tcp;

    [HideInInspector]
    public UDP Udp;

  
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    void Start()
    { 
        Tcp = new TCP();
        Udp = new UDP();
    }

    public void Disconnect()
    {
        Tcp.Disconnect();
        Udp.Disconnect();
        Tcp = null;
        Udp = null;
        IsConnected = false;
    }
    public void ConnectToServer()
    {
        InitializeClientData();
        Tcp.Connect();
    }
    public void SetIsVR(Toggle _vrToggle)
    {
        IsVR = _vrToggle.isOn;
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
        public void Disconnect()
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Close(); // close the stream from this
                stream = null;
            }
            if (Socket != null)
            {
                Socket.Close();
                Socket = null;
            }
            receiveBuffer = null;
            receivedData = null;
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
            Socket.BeginConnect(Instance.IP, Instance.Port, ConnectCallBack, Socket);
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
                if (stream != null)
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
    public class UDP
    {
        public UdpClient Socket;
        public IPEndPoint EndPoint;

        public void Disconnect()
        {
            EndPoint = null; // there might be a way to dc a udp im not sure but for now fuck it
        }
        public UDP()
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(Instance.IP), Instance.Port);
        }

        public void Connect(int _localPort)
        {
            Socket = new UdpClient(_localPort);
            Socket.Connect(EndPoint);
            Socket.BeginReceive(ReceiveCallBack, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }
        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(Instance.MyID);
                if(Socket != null)
                {
                    Socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void ReceiveCallBack(IAsyncResult _result)
        {
            try
            {
                byte[] _data = Socket.EndReceive(_result, ref EndPoint);
                Socket.BeginReceive(ReceiveCallBack, null);

                if(_data.Length < 4)
                {
                    return;
                }
                HandleData(_data);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }
    }
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome,ClientHandle.Welcome},
            {(int)ServerPackets.spawnPlayer,ClientHandle.SpawnPlayer},
            {(int)ServerPackets.playerMovement,ClientHandle.MovePlayer},
            {(int)ServerPackets.objectMovement,ClientHandle.MoveObject},
            {(int)ServerPackets.playerDisconnect,ClientHandle.RemovePlayer},
            {(int)ServerPackets.roomCreated,ClientHandle.RoomCreatedSuccess},
            {(int)ServerPackets.sendRooms,ClientHandle.RoomsReceived},
            {(int)ServerPackets.roomJoined,ClientHandle.RoomJoined },
            {(int)ServerPackets.roomJoinFailed,ClientHandle.RoomJoinedFailed },
            {(int)ServerPackets.setObjectLocation,ClientHandle.SetObjectLocation },
            {(int)ServerPackets.newHostSelected,ClientHandle.SetHost }
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
        DontDestroyOnLoad(this.gameObject);
    }

    #endregion
}
