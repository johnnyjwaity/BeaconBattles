using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour {

    public Button[] tabs;
    public Image[] panels;

	// Use this for initialization
	void Start () {
		foreach(Button tab in tabs)
        {
            var c = tab.colors;
            c.normalColor = new Color(180, 180, 180);
            tab.colors = c;
        }
        var colors = tabs[0].colors;
        colors.normalColor = Color.white;
        tabs[0].colors = colors;
        foreach(Image panel in panels)
        {
            panel.gameObject.SetActive(false);
        }
        panels[0].gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void tabClicked(int index)
    {
        foreach (Button tab in tabs)
        {
            var c = tab.colors;
            c.normalColor = new Color(180, 180, 180);
            tab.colors = c;
        }
        var colors = tabs[index].colors;
        colors.normalColor = Color.white;
        tabs[index].colors = colors;
        foreach (Image panel in panels)
        {
            panel.gameObject.SetActive(false);
        }
        panels[index].gameObject.SetActive(true);
    }
    public void open()
    {
        gameObject.SetActive(true);
    }
    public void close()
    {
        gameObject.SetActive(false);
    }
}
