using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour {

    public Button[] tabs;
    public Image[] panels;
    public TMP_Dropdown resolution;
    public Toggle fullscreen;
    public Slider musicSlider;
    public Slider soundSlider;
    public Toggle muteToggle;

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

        Resolution[] res = Screen.resolutions;
        List<string> resStr = new List<string>();
        for (int i = 0; i < res.Length; i++)
        {
            resStr.Add(res[i].width + "x" + res[i].height);
        }
        resolution.ClearOptions();
        resolution.AddOptions(resStr);


        if(!PlayerPrefs.HasKey("music_volume"))
        {
            PlayerPrefs.SetFloat("music_volume", 0.5f);
        }
        if (!PlayerPrefs.HasKey("sound_volume"))
        {
            PlayerPrefs.SetFloat("sound_volume", 0.5f);
        }
        if (!PlayerPrefs.HasKey("mute"))
        {
            PlayerPrefs.SetInt("mute", 0);
        }
        musicSlider.value = PlayerPrefs.GetFloat("music_volume");
        soundSlider.value = PlayerPrefs.GetFloat("sound_volume");
        muteToggle.isOn = (PlayerPrefs.GetInt("mute") == 0) ? false : true;
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
    public void ResChanged()
    {
        string res = resolution.itemText.text;
        int width = int.Parse(res.Split('x')[0]);
        int height = int.Parse(res.Split('x')[1]);
        Screen.SetResolution(width, height, Screen.fullScreen);
    }
    public void fullscreenChanged()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen.isOn);
    }
    public void musicChanged()
    {
        PlayerPrefs.SetFloat("music_volume", musicSlider.value);
    }
    public void soundChanged()
    {
        PlayerPrefs.SetFloat("sound_volume", soundSlider.value);
    }
    public void muteChanged()
    {
        PlayerPrefs.SetInt("mute", (muteToggle.isOn) ? 1 : 0);
    }
}
