using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Alert : MonoBehaviour {

    private AlertDelegate alertDelegate;
    public TMP_InputField input;
    public TextMeshProUGUI inputText;
    public TextMeshProUGUI question;
    

	// Use this for initialization
	void Start () {
        //gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
     * Function Changes the Message of the alert
     */
    public void ConfigureQuestion(string question)
    {
        this.question.text = question;
    }
    /*
     * Determines if Alert Should have an input
     */
    public void ShowInput(bool enabled)
    {
        input.gameObject.SetActive(enabled);
    }
    public void Display(AlertDelegate alertDelegate)
    {
        this.alertDelegate = alertDelegate;
        Debug.Log("setting active");
    }
    /*
     * What to do when Alert Ok button is clicked
     */
    public void EnterButtonClicked()
    {
        if(alertDelegate != null)
        {
            alertDelegate.OnConfiramtion(this);
        }
        else
        {
            Remove();
        }
        
    }
    public void Remove()
    {
        Destroy(gameObject);
    }
}
