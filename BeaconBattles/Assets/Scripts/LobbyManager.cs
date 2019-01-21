using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class LobbyManager : MonoBehaviour {

    public Text[] labels;
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
        foreach(Text t in labels)
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
}
