using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;

using UnityEngine.UI;

public class ChatClient : MonoBehaviour
{
    public InputField inputField;

    Socket client;
    string fromNetThread = "";
    byte[] buffer = new byte[1024];


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (fromNetThread.Length > 0)
        {
            GameObject.Find("UIText").GetComponent<UnityEngine.UI.Text>().text +=  fromNetThread + "\n";

            fromNetThread = "";
        }
    }


    public void BtnConnect()
    {
        {
            print("클라이언트 접속중");
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect("127.0.0.1", 10000, ConnctCallback, null);
        }
    }

    void ConnctCallback(IAsyncResult result)
    {
        client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecvCallback, null);
    }

    public void sendMessage()
    {
        {
            string _Message = inputField.text;

            // Byte[] _buffer;
            var _buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(_Message);
            client.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, SendCallback, null);
            GameObject.Find("InputField").GetComponent<UnityEngine.UI.InputField>().text = "";
        }
    }

    void SendCallback(IAsyncResult result)
    {
        int len = client.EndSend(result);

        print("보낸결과 : " + len);

        //client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecvCallback, null);
    }


    void RecvCallback(IAsyncResult result)
    {
        int len = client.EndReceive(result);
        if (len > 0)
        {
            string recv = System.Text.ASCIIEncoding.ASCII.GetString(this.buffer, 0, len);
            
            fromNetThread = recv;
        }

        client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecvCallback, null);
    }
}
