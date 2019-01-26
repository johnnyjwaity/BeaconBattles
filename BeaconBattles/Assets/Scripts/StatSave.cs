using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
public class StatSave {
    public int deathCount;
    public int winCount;
    public int gamesCount;
    public StatSave(int deaths, int wins, int games)
    {
        deathCount = deaths;
        winCount = wins;
        gamesCount = games;
    }
    public static StatSave Add(StatSave s)
    {
        StatSave old = null;
        if (File.Exists("stats.json"))
        {
            StreamReader reader = File.OpenText("stats.json");
            string data = reader.ReadLine();
            reader.Close();
            old = JsonConvert.DeserializeObject<StatSave>(data);
        }
        else
        {
            old = new StatSave(0, 0, 0);
        }
        old.deathCount += s.deathCount;
        old.winCount += s.winCount;
        old.gamesCount += s.gamesCount;
        string text = JsonConvert.SerializeObject(old);
        Debug.Log(text);
        StreamWriter stream = File.CreateText("stats.json");
        stream.WriteLine(text);
        stream.Close();
        return old;
    }
}
