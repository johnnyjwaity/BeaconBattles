using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Multiplayer : MonoBehaviour {

    public bool connect;
    public bool disconnect;
    public bool inLobby = false;
    public bool gameStarted = false;
    public bool connected = false;
    private TcpClient client = null;
    public List<GameObject> syncQueue;
    private List<GameObject> waitlist;
    public Dictionary<int, GameObject> sync;
    private List<string> queue;
    private List<string> acceptedQueue;
    private List<int> updatedObjects;
    private Dictionary<int, GameObject> networkedObjects = new Dictionary<int, GameObject>();
    public List<string> objectNames;
    public GameObject[] objects;
    private float timeCounter = 1 / 60;
    private string nameUsed;
    public GameObject alertPrefab;
    

	// Use this for initialization
	void Start () {
        queue = new List<string>();
        acceptedQueue = new List<string>();
        waitlist = new List<GameObject>();
        updatedObjects = new List<int>();
        sync = new Dictionary<int, GameObject>();
        DontDestroyOnLoad(this);
        //sync.Add(testId, syncQueue[0]);
        //syncQueue.Clear();
        //GameObjectData d = JsonConvert.DeserializeObject<GameObjectData>(Serialize.sg(sync[0]));
        //GameObjectData.updateGameObject(d, update);
        //printChildren(d);
        Connect();
    }

    public void Connect()
    {
        //connect to server
        connect = false;
        client = new TcpClient();
        //client.Connect("10.0.0.4", 666);
        //client.Connect("24.15.247.118", 666);
        IPAddress address = Dns.GetHostEntry("bb.johnnywaity.com").AddressList[0];
        IPEndPoint point = new IPEndPoint(address, 666);
        client.Connect(point);
        Thread reciveThread = new Thread(recieve);
        reciveThread.IsBackground = true;
        reciveThread.Start();
        connected = true;
    }
    public void CreateGame(string playerName)
    {
        NetData n = new NetData("create_lobby", playerName);
        Debug.Log("Multi :" + playerName);
        nameUsed = playerName;
        string data = JsonConvert.SerializeObject(n);
        Debug.Log(data);
        sendData(data);
    }
    public void JoinGame(string playerName, int code)
    {
        NetData n = new NetData("join_lobby", playerName);
        n.id = code;
        nameUsed = playerName;
        sendData(JsonConvert.SerializeObject(n));
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Connect();
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    CreateGame("Test");
        //}
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    JoinGame(playerName, lobbyCode);
        //}
        if (inLobby)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("game"))
            {
                inLobby = false;
                NetData n = new NetData("ready_start", "");
                sendData(JsonConvert.SerializeObject(n));
            }
        }
        if (!inLobby)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("lobby"))
            {
                inLobby = true;
            }
        }
        
        if (connected)
        {
            acceptedQueue.AddRange(queue);
            queue.Clear();
            updatedObjects.Clear();
            for (int i = acceptedQueue.Count - 1; i >= 0; i--)
            {
                NetData data = JsonConvert.DeserializeObject<NetData>(acceptedQueue[i]);
                if (data.type == "object")
                {
                    if (!updatedObjects.Contains(data.id))
                    {
                        GameObjectData gData = JsonConvert.DeserializeObject<GameObjectData>(data.value);
                        if (networkedObjects.ContainsKey(data.id))
                        {
                            GameObjectData.updateGameObject(gData, networkedObjects[data.id]);
                        }
                        else
                        {
                            Debug.Log(data.key);
                            GameObject g = Instantiate(objects[objectNames.IndexOf(data.key)]);
                            NetObject netObj = g.AddComponent<NetObject>();
                            netObj.id = data.id;
                            networkedObjects.Add(data.id, g);
                            GameObjectData.updateGameObject(gData, g);
                            if (data.key == "player")
                            {
                                string p_name = data.other[0];
                                if (p_name.EndsWith("?"))
                                {
                                    p_name = p_name.Substring(0, p_name.Length - 1);
                                }
                                g.transform.Find("Stats").GetChild(0).GetComponent<TextMeshProUGUI>().text = p_name;
                            }
                        }

                        updatedObjects.Add(data.id);
                    }
                }
                else if (data.type == "object_id")
                {
                    Debug.Log("New Object: " + data.id);
                    NetObject obj = waitlist[0].AddComponent<NetObject>();
                    obj.id = data.id;
                    sync.Add(data.id, waitlist[0]);
                    waitlist.RemoveAt(0);
                }
                else if (data.type == "game_start")
                {
                    gameStarted = true;
                    Debug.Log("Game Started");
                }
                else if (data.type == "animation")
                {
                    Animator anim = networkedObjects[data.id].GetComponent<Animator>();
                    if (data.key == "trigger")
                    {
                        anim.SetTrigger(data.value);
                    }
                    else
                    {
                        anim.SetBool(data.value, bool.Parse(data.key));
                    }
                }
                else if (data.type == "update_roster")
                {
                    string combinedRoster = "";
                    foreach (string n in data.other)
                    {
                        Debug.Log(n);
                        combinedRoster += n + ";";
                    }
                    combinedRoster = combinedRoster.Substring(0, combinedRoster.Length - 1);
                    PlayerPrefs.SetString("last_roster", combinedRoster);
                    Debug.Log(combinedRoster);
                    LobbyManager m = FindObjectOfType<LobbyManager>();
                    if (m != null)
                    {
                        m.UpdatePlayers(data.other);
                        int counter = 0;
                        foreach (string n in data.other)
                        {
                            Debug.Log(n);
                            if (n == nameUsed)
                            {
                                PlayerPrefs.SetInt("color", counter);
                                Debug.Log("Found Name: " + counter);
                                break;
                            }
                            counter++;
                        }
                    }
                }
                else if (data.type == "lobby_joined")
                {
                    Debug.Log("Joined Lobby: " + data.id);
                    PlayerPrefs.SetInt("joinedLobby", data.id);
                    inLobby = true;
                    SceneManager.LoadScene("lobby");
                }
                else if (data.type == "prepare_game")
                {
                    SceneManager.LoadScene("game");
                }
                else if (data.type == "recieve_damage")
                {
                    Debug.Log("Lost " + data.value + " health");
                    Vector3 enemyPosition = networkedObjects[int.Parse(data.key)].transform.position;
                    sync[data.id].GetComponent<PlayerController>().addKnockback(enemyPosition);
                    sync[data.id].GetComponent<PlayerController>().Damage(int.Parse(data.value));
                }
                else if (data.type == "join_failed")
                {
                    GameObject alertCanvas = Instantiate(alertPrefab);
                    Alert alert = alertCanvas.GetComponent<Alert>();
                    alert.ShowInput(false);
                    alert.ConfigureQuestion(data.key);
                    alert.Display(null);
                }
                else if (data.type == "destroy_beacon")
                {
                    Debug.Log("recieved Destroy Message");
                    BeaconManager manager = FindObjectOfType<BeaconManager>();
                    if (manager != null)
                    {
                        manager.DestoryBeacon(data.id);
                    }
                }
            }
            acceptedQueue.Clear();
            if (gameStarted)
            {
                timeCounter -= Time.deltaTime;
                if (timeCounter < 0)
                {
                    runGameSync();
                    timeCounter = 1 / 60;
                }

            }
        }
        if (disconnect)
        {
            Disconnect();
        }

        
    }
    public void Disconnect()
    {
        connected = false;
        client.Close();
    }
    public void ReturnToLobby()
    {
        gameStarted = false;
        sync.Clear();
        queue.Clear();
        acceptedQueue.Clear();
        networkedObjects.Clear();
        updatedObjects.Clear();
        SceneManager.LoadScene("lobby");
    }
    private void OnApplicationQuit()
    {
        Disconnect();
    }
    private void runGameSync()
    {
        foreach (int key in sync.Keys)
        {
            GameObjectData d = Serialize.convertToGameObjectData(sync[key]);
            d.c = new GameObjectData[0];
            NetData n = new NetData("object", JsonConvert.SerializeObject(d));
            n.key = "player";
            n.id = key;
            n.other = new string[1];
            n.other[0] = nameUsed;
            sendData(n.Serialize());
        }
        foreach (GameObject obj in syncQueue)
        {
            NetData req = new NetData("request_object", "");
            sendData(req.Serialize());
            waitlist.Add(obj);
        }
        syncQueue.Clear();
        
    }
    public void sendData(string msg)
    {
        //Debug.Log(msg);
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
            int bytesReadLen = 0;
            while(bytesReadLen < 4)
            {
                byte[] currentSet = new byte[4];

                try
                {
                    int bytesReadCurrent = stream.Read(currentSet, 0, 4 - bytesReadLen);
                    for(int i = 0; i < bytesReadCurrent; i++)
                    {
                        length[bytesReadLen + i] = currentSet[i];
                    }
                    bytesReadLen += bytesReadCurrent;
                }
                catch (IOException e)
                {
                    Debug.Log("Read Error");
                    Debug.Log(e.Message);
                }
            }
            

            int msgLen = int.Parse(encoder.GetString(length), System.Globalization.NumberStyles.HexNumber);
            //Debug.Log(msgLen);
            if (msgLen > 0)
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
