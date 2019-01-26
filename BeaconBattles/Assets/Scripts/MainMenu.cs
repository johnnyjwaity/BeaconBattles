using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class MainMenu : MonoBehaviour, AlertDelegate {
    public GameObject options;
    public GameObject alertPrefab;
    private int alertType = 0;
    private string playerName;
    public TextMeshProUGUI wins;
    public TextMeshProUGUI deaths;
    public TextMeshProUGUI games;
    
    private void Start()
    {
        StatSave stats = StatSave.Add(new StatSave(0, 0, 0));
        wins.text = "Wins: " + stats.winCount;
        deaths.text = "Deaths: " + stats.deathCount;
        games.text = "Games: " + stats.gamesCount;

        
    }
    //Create game button cliked
    public void CreateGame()
    {
        GameObject alertCanvas = Instantiate(alertPrefab);
        Alert alert = alertCanvas.GetComponent<Alert>();
        alert.ConfigureQuestion("What's Your Name?");
        alert.ShowInput(true);
        alert.Display(this);
        alertType = 1;
    }
    //Join game button clicked
    public void JoinGame()
    {
        GameObject alertCanvas = Instantiate(alertPrefab);
        Alert alert = alertCanvas.GetComponent<Alert>();
        alert.ConfigureQuestion("What's Your Name?");
        alert.ShowInput(true);
        alert.Display(this);
        alertType = 2;
    }
    //Send confiramtion alert
    public void OnConfiramtion(Alert alert)
    {
        if(alertType == 1)
        {
            FindObjectOfType<Multiplayer>().CreateGame(alert.inputText.GetParsedText());
            alert.Remove();
            alertType = 0;
        }else if(alertType == 2)
        {
            playerName = alert.inputText.text;
            alert.Remove();
            GameObject alertCanvas = Instantiate(alertPrefab);
            Alert alert1 = alertCanvas.GetComponent<Alert>();
            alert1.ConfigureQuestion("Enter Lobby ID");
            alert1.ShowInput(true);
            alert1.Display(this);
            alertType = 3;
        }else if(alertType == 3)
        {
            Debug.Log("name: " + playerName);
            FindObjectOfType<Multiplayer>().JoinGame(playerName, int.Parse(alert.inputText.text));
            alert.Remove();
            alertType = 0;
        }
    }

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
