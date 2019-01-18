using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour {

    public GameObject target;//the target object
    public float speedMod = 10.0f;//a speed modifier
    private Vector3 point;//the coord to the point where the camera looks at

    // Use this for initialization
    void Start () {
        point = target.transform.position;//get target's coords
        transform.LookAt(point);//makes the camera look to it
    }
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), 20 * Time.deltaTime * speedMod);
    }
}
