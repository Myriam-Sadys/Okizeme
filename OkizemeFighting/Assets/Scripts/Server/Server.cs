using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;
    public InputField mainInputField;
    public int lengthMax = 100 ;
    public int port = 6324;
    public string address = "127.0.0.1";
    private TcpListener server;
    private bool serverStarted;
    private GameObject[] players;
    private LobbyPlayer testScript;
    private GameObject color;

    public void StartServer()
    {
        mainInputField.characterLimit = lengthMax;
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
        try
        {
            IPAddress localhost = IPAddress.Parse(address);
            server = new TcpListener(localhost, port);
            server.Start();

            StartListening();
            serverStarted = true;
            Debug.Log("Server has been started on port " + port.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
            port++;
            StartServer();
        }
    }

    private void Update()
    {
        if (!serverStarted)
            return;

        //foreach (ServerClient c in clients)
        //{
        for (int i = 0; i < clients.Count; i++)
        {
            ServerClient c = clients[i];
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                clients.RemoveAt(i);
                i--;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }
        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            Broadcast(disconnectList[i].clientName + " has disconnected", clients);
            //clients.Remove(disconnectList[i]);
            //disconnectList.RemoveAt(i);
            disconnectList.Clear();
        }
    }

    private void OnIncomingData(ServerClient c, string data)
    {
        if (GameObject.FindGameObjectsWithTag("Player") != null)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (testScript = player.GetComponent<LobbyPlayer>())
                {
                    if (testScript.playerName == c.clientName)
                        Broadcast("<color=#" + ColorUtility.ToHtmlStringRGBA(testScript.playerColor) + ">" + c.clientName + ": " + "</color>" + data, clients); ;
                }
                else
                    Debug.Log("Fail");
            }
            //color = GameObject.Find("LobbyPanel/PlayerListSubPanel/PlayerList/PlayerInfo(Clone)/Color");
            //Image colorImage = color.GetComponent<Image>();
            //Debug.Log(colorImage.color);
            //Broadcast("<color=#" + ColorUtility.ToHtmlStringRGBA(colorImage.color) + ">" + c.clientName + ": " + "</color>" + data, clients);
        }
    }

    private bool IsConnected(TcpClient tcp)
    {
        try
        {
            if (tcp != null && tcp.Client != null && tcp.Client.Connected)
            {
                if (tcp.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(tcp.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        string name = "Player" + (clients.Count + 1);
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar), name));
        StartListening();
        //Broadcast("%NAME", new List<ServerClient>() { clients[clients.Count - 1] });
        Broadcast(clients[clients.Count - 1].clientName + " has connected.", clients);
    }

    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch(Exception e)
            {
                Debug.Log("Write error : " + e.Message + " to client " + c.clientName);
            }
        }
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket, string name)
    {
        clientName = name;
        tcp = clientSocket;
    }
}