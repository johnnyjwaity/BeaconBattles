using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NetData {
    public NetData(string type, string value)
    {
        this.type = type;
        this.value = value;
    }
    public string type;
    public string key;
    public string value;
    public int id;
    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
