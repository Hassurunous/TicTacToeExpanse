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

    // List of sprites selected by the players. Instantiated empty, to be appended to during player selection.
    Dictionary<int, Sprite> chosenSprites = new Dictionary<int, Sprite>();

    // This is a sprite used to show when other sprites have not loaded or been referenced correctly.
    [SerializeField]
    Sprite errorSprite;

    // Instancing so that static methods can be used with non-static properties
    public static GameController Instance { get; private set; }

    [SerializeField]
    GameObject menuCanvas, playerSelectCanvas, gameCanvas, gameOverCanvas;

    [SerializeField]
    PlayerSelectItemsManager playerSelectItemsManager;

    [SerializeField]
    Text victoryText;

    int boardSize;

    // This event will be used to show when a "round" has completed (i.e. every player has gone once).
    // This event could be used to play animations, move the state of the game forward...etc. when this
    // occurs. The first usage foreseen for this is to transition from the Player Select screen when all
    // players have chosen a sprite to use.
    public delegate void RoundComplete();
    public event RoundComplete OnRoundComplete;

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
        //if (GameController.gameState == GameState.Menu || GameController.gameState == GameState.PlayerWon || GameController.gameState == GameState.Draw)
        //{
        //    return;
        //}

        playerTurn += 1;

        if(playerTurn >= numOfPlayers)
        {
            playerTurn = 0;
            if(gameState == GameState.Menu)
            {
                GameController.Instance.PlayGame();
            }
        }

        Debug.Log("Player " + (playerTurn + 1) + "'s turn!");
    }

    public static void UpdateGameState(GameState newState, bool playerWon=false)
    {
        GameController.gameState = newState;
        if (GameController.gameState == GameState.GameOver)
        {
            GameController.Instance.GameOver(playerWon);
            Debug.Log("Game Over! Player " + (GameController.playerTurn + 1) + " wins!");
        } else if (GameController.gameState == GameState.Menu)
        {
            GameController.Instance.MainMenu();
        }
    }

    // This method will be used on the player selection screen to set the player sprites to be used in the GetCurrentPlayerSprite
    // method so that the board can place the tiles correctly.
    public void SetCurrentPlayerSprite(Sprite sprite)
    {
        chosenSprites[GameController.playerTurn] = sprite;
        NextTurn();
    }

    // Return the sprite that should be used for the current player. 
    public static Sprite GetCurrentPlayerSprite()
    {
        // We'll set the sprite to default as the errorSprite, then change it if we are able to return a playerSprite
        Sprite sprite = GameController.Instance.errorSprite;

        // Using the "try;catch" here will make sure that we don't crash the program if there are not enough
        // sprites set for the number of players, or if there is an error in the playerSprites array.
        try
        {
            sprite = GameController.Instance.chosenSprites[GameController.playerTurn];
        }
        catch
        {
            Debug.LogError("Error in locating player sprite for player " + (playerTurn + 1));
        }

        return sprite;

    }

    // Set the board size to be used in the game before moving to the playerSelect screen.
    public void SetBoardSize(int size)
    { 
        boardSize = size;
    }

    public void PlayGame()
    {
        // Hide the menu and/or game over screen once we start the game.
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        playerSelectCanvas.SetActive(false);

        // Set up the board.
        gameObject.GetComponent<BoardController>().SetupBoard(boardSize);

        // Set the gameState
        gameState = GameState.Playing;

        // Reset the player select buttons for the next time they are needed.
        //playerSelectItemsManager.ResetButtons();
    }

    public void MainMenu()
    {
        // Hide the game, game over, and playerSelect screen when we return to the main menu.
        menuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        playerSelectCanvas.SetActive(false);

        // Set the gameState
        gameState = GameState.Menu;
    }

    public void PlayerSelectScreen()
    {
        // Hide all other screens and display the Player Select Screen.
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        playerSelectCanvas.SetActive(true);
    }

    public void RetryGame()
    {
        // Hide all other screens and display the game screen.
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        playerSelectCanvas.SetActive(false);

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
        playerSelectCanvas.SetActive(false);

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
    GameOver
}
