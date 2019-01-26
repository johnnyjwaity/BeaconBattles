using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {

    private AudioSource source;

    // Use this for initialization
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        source.volume = PlayerPrefs.GetFloat("sound_volume");
        if (PlayerPrefs.GetInt("mute") == 1)
        {
            source.volume = 0;
        }
    }
}
