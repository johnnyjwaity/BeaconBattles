using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class Multiplayer : MonoBehaviour {

    public bool connect;
    public bool disconnect;
    private bool connected = false;
    private TcpClient client = null;
    public List<GameObject> syncQueue;
    private List<GameObject> waitlist;
    public List<GameObject> sync;
    public GameObject update;
    private List<string> queue;
    private List<string> acceptedQueue;

	// Use this for initialization
	void Start () {
        queue = new List<string>();
        acceptedQueue = new List<string>();
        waitlist = new List<GameObject>();
        //GameObjectData d = JsonConvert.DeserializeObject<GameObjectData>(Serialize.sg(sync[0]));
        //GameObjectData.updateGameObject(d, update);
        //printChildren(d);
    }

    // Update is called once per frame
    void Update() {

        if (connect)
        {
            connect = false;
            client = new TcpClient();
            client.Connect("24.15.247.118", 666);
            Thread reciveThread = new Thread(recieve);
            reciveThread.IsBackground = true;
            reciveThread.Start();
            connected = true;
            
            //sendData(Serialize.sg(sync[0]));
        }
        if (connected)
        {

            foreach(GameObject obj in sync)
            {
                NetData n = new NetData("object", Serialize.sg(obj));
                sendData(n.Serialize());
            }
            foreach(GameObject obj in syncQueue)
            {
                NetData req = new NetData("request_object", "");
                sendData(req.Serialize());
                waitlist.Add(obj);
            }
            syncQueue.Clear();

            acceptedQueue.AddRange(queue);
            queue.Clear();
            foreach(string q in acceptedQueue)
            {
                NetData data = JsonConvert.DeserializeObject<NetData>(q);
                if(data.type == "object")
                {
                    GameObjectData gData = JsonConvert.DeserializeObject<GameObjectData>(data.value);
                    GameObjectData.updateGameObject(gData, update);
                }else if(data.type == "object_id")
                {
                    Debug.Log("New Object: " + data.value);
                    sync.Add(waitlist[0]);
                    waitlist.RemoveAt(0);
                }
            }
            acceptedQueue.Clear();
        }
        if (disconnect)
        {
            connected = false;
            client.Close();
        }
        
    }
    private void OnApplicationQuit()
    {
        if (connected)
        {
            client.Close();
        }
    }
    void sendData(string msg)
    {
        NetworkStream stream = client.GetStream();
        ASCIIEncoding encoder = new ASCIIEncoding();
        byte[] data = encoder.GetBytes(msg);
        stream.Write(encoder.GetBytes(data.Length.ToString("X4")), 0, 4);
        stream.Write(data, 0, data.Length);
    }
    void recieve()
    {
        NetworkStream stream = client.GetStream();
        ASCIIEncoding encoder = new ASCIIEncoding();
        while (connected)
        {
            byte[] length = new byte[4];
            try
            {
                stream.Read(length, 0, length.Length);
            }
            catch
            {

            }
            int msgLen = int.Parse(encoder.GetString(length), System.Globalization.NumberStyles.HexNumber);
            //Debug.Log(msgLen);
            if(msgLen > 0)
            {
                byte[] message = new byte[0];
                int bytesRead = 0;
                while(bytesRead < msgLen)
                {
                    try
                    {
                        byte[] currentSet = new byte[msgLen - bytesRead];
                        bytesRead += stream.Read(currentSet, 0, currentSet.Length);
                        Array.Resize(ref currentSet, bytesRead - message.Length);
                        message = JoinArrays(message, currentSet);
                    }
                    catch
                    {

                    }
                }
                string recieved = encoder.GetString(message, 0, bytesRead);
                //Debug.Log(recieved);
                queue.Add(recieved);
                

            }
        }
    }

    byte[] JoinArrays(byte[] arrayA, byte[] arrayB)
    {
        byte[] outputBytes = new byte[arrayA.Length + arrayB.Length];
        Buffer.BlockCopy(arrayA, 0, outputBytes, 0, arrayA.Length);
        Buffer.BlockCopy(arrayB, 0, outputBytes, arrayA.Length, arrayB.Length);
        return outputBytes;
    }
}
