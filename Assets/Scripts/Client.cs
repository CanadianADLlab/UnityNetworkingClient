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

    void Start()
    { 
        Tcp = new TCP();
    }

    public void ConnectToServer()
    {
        Tcp.Connect();
    }

    public class TCP
    {
        public TcpClient Socket;

        private NetworkStream stream;
        private byte[] receiveBuffer;

        public TCP()
        {
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

                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallBack, null);
            }
            catch (Exception e)
            {
                Debug.Log("Error receiving packet " + e);
            }
        }
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
