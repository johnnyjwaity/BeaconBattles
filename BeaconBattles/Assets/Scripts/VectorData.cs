using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VectorData {
    public float x;
    public float y;
    public float z;

    public VectorData(Vector3 vec)
    {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }

    public Vector3 toVector3()
    {
        return new Vector3(x, y, z);
    }

    public Quaternion toQuaternion()
    {
        return Quaternion.Euler(toVector3());
    }
}
