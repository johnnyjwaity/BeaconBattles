﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectData {
    public int n;
    public VectorData p;
    public VectorData r;
    public GameObjectData[] c;

    public static void updateGameObject(GameObjectData data, GameObject obj)
    {
        if(data == null)
        {
            Debug.Log(data.n);
            return;
        }
        //update position and rotation
        obj.transform.position = data.p.toVector3();
        obj.transform.rotation = data.r.toQuaternion();
        foreach(GameObjectData child in data.c)
        {
            //update children of the gameobject recursively
            updateGameObject(child, obj.transform.GetChild(child.n).gameObject);
        }
    }
}
