using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardTile : MonoBehaviour, IPointerClickHandler {

    // Using private setters will keep our variables protected from modification by other scripts, but
    // will still allow those scripts to read these values. If we need to update these values, methods
    // should be created to verify and update those values.


    public TileState tileState { get; private set; }

    public int column { get; private set; }

    public int row { get; private set; }

    // Which player controls this tile? Players indexed at 0 (i.e. "Player 1" is _playerID 0).
    // Value is null until a player takes over the tile.
    public int playerID { get; private set; }

    // If the tile has already been initialized, we don't want to be able to initialize it again
    // This will prevent the column and row from being changed after the first initilization
    bool initialized;

    // We store a reference to the SpriteRenderer that will control the symbol for whichever player
    // has claimed this tile so that we can easily update the sprite later.
    [SerializeField]
    SpriteRenderer playerSymbolSpriteRenderer;

    // For the UI Canvas implementation version, we need a UI Image instead of a SpriteRenderer
    [SerializeField]
    Image playerSymbolImage;
    

    // This delegate will allow the TicTacToeBoard to subscribe for updates when the BoardTile is updated.
    public delegate void TilePressed(BoardTile tile);
    public event TilePressed OnTilePressed;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializeTile(int col, int ro, float tileSize, float xPos, float yPos)
    {
        if (initialized)
        {
            return;
        }

        column = col;
        row = ro;

        tileState = TileState.Empty;

        if(GameController.Instance.UICanvasImplementation)
        {
            // Let's set its position and size now.
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, -yPos);

            playerSymbolImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
        } else
        {
            float newSizeScalar = tileSize / transform.GetChild(1).GetComponent<SpriteRenderer>().bounds.size.x;
            transform.position = new Vector3(xPos, yPos);
            transform.localScale = new Vector3(newSizeScalar, newSizeScalar);
        }
        

        initialized = true;
    }

    // Detect if the tile has been selected by a player. 
    // OnPointerClick will function just as well on mobile as on standalone, without any other configuration.
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log(name + " Game Object Clicked!");
        // If the tile is not Empty, then it is not a valid place to press.
        // TODO: Should probably have some sort of user feedback to let the user know
        // why their press has not been recorded. 
        if (!tileState.Equals(TileState.Empty))
        {
            return;
        }
        else
        {
            // Update the tileState and the playerID with the new information
            tileState = TileState.Filled;
            playerID = GameController.playerTurn;

            // Trigger the OnTilePressed event for listeners
            OnTilePressed(this);
        }
    }

    // Used to set the sprite of the playerSymbol, which shows who has selected this tile.
    public void UpdatePlayerSprite(Sprite sprite)
    {
        if(GameController.Instance.UICanvasImplementation)
        {
            playerSymbolImage.sprite = sprite;
            playerSymbolImage.color = Color.white;
        } else
        {
            playerSymbolSpriteRenderer.sprite = sprite;
        }
    }

    public void ResetTile()
    {
        if (GameController.Instance.UICanvasImplementation)
        {
            playerSymbolImage.color = new Color(0,0,0,0);
        }

        tileState = TileState.Empty;
    }

    public bool IsEmpty()
    {
        return (tileState == TileState.Empty);
    }
}
