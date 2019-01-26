using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Attack : MonoBehaviour {

    private Collider collider;
    public int damage;
    public float startDelay;
    private float startDelayCounter;
    private bool delayed;
    public float duration;
    private float durationCounter;
    private bool running;
    private Multiplayer multi;
    private GameObject player;

    //private List<GameObject> currentCollisions;

	// Use this for initialization
	void Start () {
        collider = GetComponent<Collider>();
        multi = FindObjectOfType<Multiplayer>();
        player = FindObjectOfType<PlayerController>().gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (delayed)
        {
            startDelayCounter -= Time.deltaTime;
            if(startDelayCounter <= 0)
            {
                delayed = false;
                running = true;
                durationCounter = duration;
                collider.enabled = true;
            }
        }
        if (running)
        {
            //Attack cooldowns
            durationCounter -= Time.deltaTime;
            if(durationCounter < 0)
            {
                running = false;
                collider.enabled = false;
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        //Determine if hit enemy or hit beacon
        if(other.tag == "Enemy")
        {
            Debug.Log("Hit Enemy");
            NetData n = new NetData("damage", "" + damage);
            n.id = other.GetComponent<NetObject>().id;
            n.key = "" + player.GetComponent<NetObject>().id;
            multi.sendData(JsonConvert.SerializeObject(n));
        }else if(other.tag == "Beacon")
        {
            NetData n = new NetData("damage_beacon", other.gameObject.name.Split(' ')[0].ToLower());
            n.id = damage;
            multi.sendData(JsonConvert.SerializeObject(n));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
    public void run()
    {
        delayed = true;
        startDelayCounter = startDelay;
    }
}
