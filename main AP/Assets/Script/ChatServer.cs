using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;

public class ChatServer : MonoBehaviour
{
    Socket server;
    byte[] buffer = new byte[1024];
    string fromNetThread = "";
    List<Socket> clients = new List<Socket>();

    private void Awake()
    {
        if(this.gameObject.activeInHierarchy)
           SetServer();
    }

    void Start()
    {
    
    }

    void SetServer()
    {
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Any, 10000));
        server.Listen(10);

        server.BeginAccept(AcceptCallback, null);
    }

    void AcceptCallback(IAsyncResult result)
    {
        Socket client = server.EndAccept(result);
        IPEndPoint addr = ((IPEndPoint)client.RemoteEndPoint);

        print(string.Format("{0}, {1}", addr.ToString(), addr.Port.ToString()));

        client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecvCallback, client);

        clients.Add(client);

        server.BeginAccept(AcceptCallback, null);
    }

    void RecvCallback(IAsyncResult result)
    {
        Socket client = (Socket)result.AsyncState;
        int len = client.EndReceive(result);

        if (len > 0)
        {
            string recv = System.Text.ASCIIEncoding.ASCII.GetString(this.buffer, 0, len);
            IPEndPoint addr = ((IPEndPoint)client.RemoteEndPoint);
            fromNetThread = string.Format("{0}, : {1} : {2}", addr.Address, addr.Port, recv);
            print("Server : " + fromNetThread);

            for (int i = 0; i < clients.Count; i++)
                clients[i].BeginSend(buffer, 0, len,
                    SocketFlags.None, SendCallback, client);
        }
        client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecvCallback, client);
    }

    void SendCallback(IAsyncResult result)
    {
        Socket client = (Socket)result.AsyncState;
        int len = client.EndSend(result);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
