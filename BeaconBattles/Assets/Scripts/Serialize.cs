using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Serialize {

	public static string SerializeGameObject(GameObject obj)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("name", "\"" + obj.name + "\"");
        dic.Add("position", SerializeVector(obj.transform.position));
        dic.Add("rotation", SerializeVector(obj.transform.rotation.eulerAngles));
        string children = "[";
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            string child = SerializeGameObject(obj.transform.GetChild(i).gameObject);
            children += child;
            if (i != obj.transform.childCount - 1)
            {
                children += ",";
            }
        }
        children += "]";
        dic.Add("children", children);
        var json = SerializeDictionary(dic);
        //PlayerController.updateFromJSON(json);
        return json;
    }
    public static string sg(GameObject obj)
    {
        return JsonConvert.SerializeObject(convertToGameObjectData(obj));
    }
    public static GameObjectData convertToGameObjectData(GameObject obj)
    {
        GameObjectData data = new GameObjectData();
        data.n = obj.transform.GetSiblingIndex();
        data.p = new VectorData(obj.transform.position);
        data.r = new VectorData(obj.transform.rotation.eulerAngles);
        data.c = new GameObjectData[obj.transform.childCount];
        for(int i = 0; i < obj.transform.childCount; i++)
        {
            data.c[i] = convertToGameObjectData(obj.transform.GetChild(i).gameObject);
        }
        return data;
    }
    public static string SerializeVector(Vector3 vec)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("x", "" + vec.x);
        dic.Add("y", "" + vec.y);
        dic.Add("z", "" + vec.z);
        return SerializeDictionary(dic);
    }
    public static string SerializeDictionary(Dictionary<string, string> dic)
    {
        string json = "{";
        foreach(string key in dic.Keys)
        {
            json += "\"" + key + "\": " + dic[key] + ",";
            
        }
        if(dic.Keys.Count > 0)
        {
            json = json.Substring(0, json.Length - 1);
        }
        json += "}";
        return json;
    }
}
