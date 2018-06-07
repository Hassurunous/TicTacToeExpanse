using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectItemsManager : MonoBehaviour {

    [SerializeField]
    Button[] buttons;

    [SerializeField]
    Text playerTurnText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        ResetButtons();
        SetPlayerTurnText();
    }

    public void ResetButtons()
    {
        //StartCoroutine(ButtonResetOnDelay());
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    // Delay the button reset by a frame to make sure they don't accidentally get deactivated after
    // they are reset. 
    IEnumerator ButtonResetOnDelay()
    {
        yield return new WaitForEndOfFrame();
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
        yield break;
    }

    // Set the text element for which player's turn it is.
    public void SetPlayerTurnText()
    {
        playerTurnText.text = "Player " + (GameController.playerTurn + 1) + ": Select your piece."; 
    }
}
