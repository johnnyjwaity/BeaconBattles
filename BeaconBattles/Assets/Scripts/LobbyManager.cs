using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviour {

    public TextMeshProUGUI[] labels;
    public TextMeshProUGUI lobbyCode;
    public GameObject[] podiums;

	// Use this for initialization
	void Start () {
		foreach(GameObject obj in podiums)
        {
            for(int i = 0; i < obj.transform.childCount; i++)
            {
                obj.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        UpdatePlayers(PlayerPrefs.GetString("last_roster").Split(';'));
        lobbyCode.text = "Lobby Code: " + PlayerPrefs.GetInt("joinedLoby");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void UpdatePlayers(string[] names)
    {
        foreach (GameObject obj in podiums)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                obj.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        foreach(TextMeshProUGUI t in labels)
        {
            t.text = "Not Connected";
        }
        int counter = 0;
        foreach(string name in names)
        {
            for (int i = 0; i < podiums[counter].transform.childCount; i++)
            {
                podiums[counter].transform.GetChild(i).gameObject.SetActive(true);
            }
            labels[counter].text = name;
            counter++;
        }
    }
    public void requestStart()
    {
        Multiplayer m = FindObjectOfType<Multiplayer>();
        NetData n = new NetData("request_start", "");
        m.sendData(JsonConvert.SerializeObject(n));
    }
    public void leaveGame()
    {
        Multiplayer m = FindObjectOfType<Multiplayer>();
        m.Disconnect();
        Destroy(m.gameObject);
        SceneManager.LoadScene("Menu");
    }
}
