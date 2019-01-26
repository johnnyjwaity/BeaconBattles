using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    private AudioSource source;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        source.volume = PlayerPrefs.GetFloat("music_volume");
        if(PlayerPrefs.GetInt("mute") == 1)
        {
            source.volume = 0;
        }
	}
}
