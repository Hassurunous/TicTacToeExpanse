using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // The number of players. Right now this is hardcoded to 2, but it could possibly be updated
    // later to increase the complexity and capacity of the game!
    [SerializeField]
    private int _numOfPlayers = 2;

    public static int numOfPlayers
    {
        get
        {
            return GameController.Instance._numOfPlayers;
        }
    }

    // Keep track of whose turn it is.
    // By making it static, we will be able to access it even if there is not a GameController object.
    // By giving it a private setter, we ensure that no other scripts can modify it.
    public static int playerTurn {get; private set;}

    // Track the state that the game is in. It will start in the "Menu" state, initially.
    private static GameState gameState = GameState.Menu;

    // The Sprites to be used for each player
    [SerializeField]
    Sprite[] playerSprites;

    // This is a sprite used to show when other sprites have not loaded or been referenced correctly.
    [SerializeField]
    Sprite errorSprite;

    // Instancing so that static methods can be used with non-static properties
    public static GameController Instance { get; private set; }

    [SerializeField]
    GameObject menuCanvas, gameCanvas, gameOverCanvas;

    [SerializeField]
    Text victoryText;

    #region Unity Monobehaviour Overrides
    void Awake()
    {
        //Check if instance already exists
        if (Instance == null)
        {
            //if not, set instance to this
            Instance = this;
        }
        //If instance already exists and it's not this:
        else if (Instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameController.
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    #endregion

    #region Public Functions
    public static void NextTurn()
    {
        if (GameController.gameState == GameState.Menu || GameController.gameState == GameState.PlayerWon || GameController.gameState == GameState.Draw)
        {
            return;
        }

        playerTurn += 1;

        if(playerTurn >= numOfPlayers)
        {
            playerTurn = 0;
        }

        Debug.Log("Player " + (playerTurn + 1) + "'s turn!");
    }

    public static void UpdateGameState(GameState newState)
    {
        GameController.gameState = newState;
        if (GameController.gameState == GameState.PlayerWon)
        {
            GameController.Instance.GameOver(true);
            Debug.Log("Game Over! Player " + (GameController.playerTurn + 1) + " wins!");
        } else if (GameController.gameState == GameState.Draw) {
            GameController.Instance.GameOver(false);
        } else if (GameController.gameState == GameState.Menu)
        {
            GameController.Instance.MainMenu();
        }
    }

    public static Sprite GetCurrentPlayerSprite()
    {
        // We'll set the sprite to default as the errorSprite, then change it if we are able to return a playerSprite
        Sprite sprite = GameController.Instance.errorSprite;

        // Using the "try;catch" here will make sure that we don't crash the program if there are not enough
        // sprites set for the number of players, or if there is an error in the playerSprites array.
        try
        {
            sprite = GameController.Instance.playerSprites[GameController.playerTurn];
        }
        catch
        {
            Debug.LogError("Error in locating player sprite for player " + (playerTurn + 1));
        }

        return sprite;

    }

    public void PlayGame(int boardSize)
    {
        // Hide the menu and/or game over screen once we start the game.
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);

        // Set up the board.
        gameObject.GetComponent<BoardController>().SetupBoard(boardSize);

        // Set the gameState
        gameState = GameState.Playing;
    }

    public void MainMenu()
    {
        // Hide the menu and/or game over screen once we start the game.
        menuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);

        // Set the gameState
        gameState = GameState.Menu;
    }

    public void RetryGame()
    {
        // Hide the menu and/or game over screen once we start the game.
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);

        // Set the gameState
        gameState = GameState.Playing;

        // Reset the player turn.
        playerTurn = 0;
    }

    public void GameOver(bool playerWon)
    {
        // Hide the menu and/or game over screen once we start the game.
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);

        if (victoryText)
        {
            if (playerWon)
            {
                victoryText.text = "Player " + (GameController.playerTurn + 1) + " wins!";
            } else
            {
                victoryText.text = "It's a draw!";
            }
            
        }
    }
    #endregion
}

public enum GameState
{
    Menu,
    Playing,
    PlayerWon,
    Draw
}
