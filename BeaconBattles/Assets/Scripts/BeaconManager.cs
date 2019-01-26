using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconManager : MonoBehaviour {

    public GameObject[] beacons;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void DestoryBeacon(int id)
    {
        Debug.Log("Destorying");
        beacons[id].SetActive(false);
    }
    public bool IsBeaconAlive(string color)
    {
        if (beacons[ColorToIndex(color)].activeSelf)
        {
            return true;
        }
        return false;
    }
    public bool IsBeaconAlive(int color)
    {
        if (beacons[color].activeSelf)
        {
            return true;
        }
        return false;
    }
    public int GetBeaconCount()
    {
        int amount = 0;
        foreach(GameObject beacon in beacons)
        {
            if (beacon.activeSelf)
            {
                amount++;
            }
        }
        return amount;
    }
    public int GetWinningBeacon()
    {
        if(GetBeaconCount() == 1)
        {
            int count = 0;
            foreach(GameObject b in beacons)
            {
                if (b.activeSelf)
                {
                    return count;
                }
                count++;
            }
        }else if(GetBeaconCount() == 0)
        {
            return -2;
        }
        return -1;
    }
    private int ColorToIndex(string color)
    {
        switch (color)
        {
            case "red":
                return 0;
            case "green":
                return 1;
            case "yellow":
                return 2;
            case "blue":
                return 3;
        }
        return -1;
    }
}
