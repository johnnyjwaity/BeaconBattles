using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MainMenu : MonoBehaviour {
    public GameObject options;
    public void OpenOptions()
    {
        options.SetActive(true);
        //FindObjectOfType<OptionsManager>().open();
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
	
}
