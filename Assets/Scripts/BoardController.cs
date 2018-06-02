using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {

    // The length and width of the board, assuming we always want them to be the same.
    [SerializeField]
    int boardsize = 3;

    // A collection of all of the tiles in the board.
    private BoardTile[,] boardTiles;

    // A collection of all the moves that have been made throughout the games played.
    private List<GameMove> gameMoves = new List<GameMove>();

    // BoardTile prefab to be used in initialization of the board
    [SerializeField]
    GameObject boardTilePrefab;

    // BoardTile prefab to be used in the Canvas implementation of the board
    [SerializeField]
    GameObject boardTileUIPrefab;

    // GameCanvas where we spawn the board tiles
    [SerializeField]
    GameObject gameCanvas;

    bool gameOver = false;

    #region Unity Methods
    void Awake()
    {
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Although we initialize the BoardTiles as children of this object, we want to make sure that
    // just in case one is re-parented for whatever reason, that it does not maintain a reference
    // to the OnTilePressed method of this script in its OnTilePressed event.
    void OnDisable()
    {
        try { boardTiles[0,0].OnTilePressed -= OnTilePressed; }
        catch { return; }
        foreach(BoardTile tile in boardTiles)
        {
            try
            {
                tile.OnTilePressed -= OnTilePressed;
            }
            catch
            {
                Debug.Log("Tile not found. Tile removed before board disabled as intended.");
            }
        }
    }

    #endregion

    #region Board Methods
    // Reset the TileState array to the prescribed size, and set all instances in the array to TileState.Empty
    public void SetupBoard(int size)
    {
        // Make sure first that we do, in fact, have the boardTilePrefab set.
        if (!boardTilePrefab)
        {
            Debug.LogError("BoardTile prefab not set.");
            return;
        }

        // Set gameOver to false, in case we are restarting a game instead of going back to the main menu
        gameOver = false;

        // Set the size of the board
        boardsize = size;

        // Initialize the boardTiles array with the appropriate dimensions.
        boardTiles = new BoardTile[size, size];

        // The size and position of the boardTiles depends on the number that we are spawning and the size of the board.
        // First, let's check the size of our board. Since our board will always be square, let's see how the canvas scaled
        // and pick the smaller of the two dimensions.
        float boardWidth = 0f;
        float boardHeight = 0f;

        // We check the size of the canvas since CanvasScaler scales for screen size.
        RectTransform rectTransform = gameCanvas.GetComponent<RectTransform>();
        boardWidth = rectTransform.rect.width;
        boardHeight = rectTransform.rect.height;

        // Now that we know the size of our board, we can calculate the size of the tiles. 
        float tileSize = boardHeight < boardWidth ? boardHeight / size : boardWidth / size;

        // We'll need to also calculate the position of the tiles.
        // Center the tiles on the X axis as well.
        float xPos = 0f;
        float yPos = 0f;

        // Now we instantiate and place the tiles in the correct position.
        for(int row = 0; row < size; row++)
        {
            xPos = (boardWidth / 2) - (tileSize * (boardsize / 2));
            for(int column = 0; column < size; column++)
            {
                // Instantiate a copy of the boardTilePrefab at the appropriate position.
                GameObject tile;

                tile = Instantiate(boardTileUIPrefab, gameCanvas.transform);

                BoardTile boardTile = tile.GetComponent<BoardTile>();
                boardTile.InitializeTile(column, row, tileSize, xPos, yPos);
                boardTile.OnTilePressed += OnTilePressed;

                // After initializing the BoardTile's GameObject, add the boardTile to the boardTiles array.
                boardTiles[column, row] = boardTile;
               
                //Debug.Log("boardTile[" + column + "," + row + "] initialized.");
                //Debug.Log("Tile rect properties: (" + xPos + "," + yPos + "," + tileSize + "," + tileSize + ")");
                xPos += tileSize;
            }
            yPos += tileSize;
        }
    }

    public void ClearBoard()
    {
        foreach(BoardTile tile in boardTiles)
        {
            Destroy(tile.gameObject);
        }
    }

    public void ResetBoard()
    {
        gameOver = false;
        foreach(BoardTile tile in boardTiles)
        {
            tile.ResetTile();
        }
    }

    #endregion

    #region Checking Wins
    // Check for any win condition
    // NOTE: Early exits exist for all of these functions, so runtime should be relatively low.
    bool CheckWins(BoardTile tile)
    {
        int column = tile.column;
        int row = tile.row;
        int playerID = tile.playerID;
        return (CheckHorizontalWin(row, playerID) | CheckVerticalWin(column, playerID) |
            CheckDiagonalWin(column, row, playerID)) == true;
    }

    // Check for a horizontal win in the selected row
    bool CheckHorizontalWin(int row, int playerID)
    {
        // Check all of the tiles in the appropriate row to make sure they are all the same state
        // as the provided tile. I.E. That they are all the same piece.
        for(int column = 0; column < boardsize; column++)
        {
            // If either the tile is "Empty" or the tile has a different playerID than the provided, return false.
            // There is no victory on this turn.
            if(boardTiles[column, row].tileState.Equals(TileState.Empty) || !boardTiles[column, row].playerID.Equals(playerID))
            {
                return false;
            }
        }

        // Otherwise, if we make it through the whole row without returning false, we know that they all match!
        // Therefore, return true!
        return true;
    }

    // Check for a horizontal win in the selected row
    bool CheckVerticalWin(int column, int playerID)
    {
        // Check all of the tiles in the appropriate column to make sure they are all the same state
        // as the provided tile. I.E. That they are all the same piece.
        for (int row = 0; row < boardsize; row++)
        {
            // If either the tile is "Empty" or the tile has a different state than the provided, return false.
            // There is no victory on this turn.
            if (boardTiles[column, row].tileState.Equals(TileState.Empty) || !boardTiles[column, row].playerID.Equals(playerID))
            {
                return false;
            }
        }

        // Otherwise, if we make it through the whole row without returning false, we know that they all match!
        // Therefore, return true!
        return true;
    }

    // Check for a diagonal win.
    bool CheckDiagonalWin(int column, int row, int playerID)
    {
        /* If the column number != the row number, then we are not in the first diagonal
         * path to victory. Otherwise, if the column number + row number is not equal to 
         * boardsize - 1, we are also not in the secondary diagonal path, and should return false.
         *
         *  Short Explanation:
         *  
         *  |0,0| |1,0| |2,0| -- (0,0), (1,1), and (2,2) are all the primary diagonal
         *  |1,0| |1,1| |1,2| -- (2,0), (1,1), and (2,0) are all the secondary. 
         *  |2,0| |2,1| |2,2|
         * 
         *  All of our primary diagonals have the same row and column number, which excludes the rest of the board.
         *  All of our secondary diagonals can have their coordinates added together to equal 2, which is 1 less
         *  than the boardsize. This excludes the primary diagonal (except for |1,1|) and all other tiles.
         */

        if(column != row && column + row != boardsize - 1)
        {
            return false;
        }

        // If the boardsize is an odd number, there will be an overlap point on the diagonals, therefore we should
        // check to make sure we aren't dealing with the centerpoint.
        if(boardsize % 2 != 0)
        {
            if (column == row && column + row == boardsize - 1)
            {
                return (CheckPrimaryDiagonalWin(playerID) | CheckSecondaryDiagonalWin(playerID)) == true;
            } 
        }

        if (column == row) 
        {
            return CheckPrimaryDiagonalWin(playerID);
        } else if (column + row == boardsize - 1)
        {
            return CheckSecondaryDiagonalWin(playerID);
        }

        return false;
    }

    bool CheckPrimaryDiagonalWin(int playerID)
    {
        for (int i = 0; i < boardsize; i++)
        {
            // If either the tile is "Empty" or the tile has a different state than the provided, return false.
            // There is no victory on the primary diagonal this turn.
            if (boardTiles[i, i].tileState.Equals(TileState.Empty) || !boardTiles[i, i].playerID.Equals(playerID))
            {
                return false;
            }
        }

        return true;
    }

    bool CheckSecondaryDiagonalWin(int playerID)
    {
        int row = 0;
        for(int column = 0; column < boardsize; column++)
        {
            // As we already determined, each of these coordinates should be (row + column = boardsize - 1). 
            // rearranged a bit, we get (row = boardsize - 1 - column).
            row = boardsize - 1 - column;
            if(boardTiles[column, row].tileState.Equals(TileState.Empty) || !boardTiles[column, row].playerID.Equals(playerID))
            {
                return false;
            }
        }

        // If we made it through all of those tiles without return false, then we have a victory on the secondary diagonal!
        return true;
    }

    bool CheckTie()
    {
        foreach(BoardTile tile in boardTiles)
        {
            if(tile.IsEmpty())
            {
                return false;
            }
        }

        return true;
    }
    #endregion

    #region Private Functions

    // This is the function that we will use to subscribe to the "OnTilePressed" event from the BoardTiles
    void OnTilePressed(BoardTile tile)
    {
        if (gameOver)
        {
            return;
        }

        // Update the tile to show the sprite of the player.
        tile.UpdatePlayerSprite(GameController.GetCurrentPlayerSprite());

        // Append this move to the gameMoves list
        gameMoves.Add(new GameMove(tile.row, tile.column, GameController.playerTurn));

        if (CheckWins(tile))
        {
            GameController.UpdateGameState(GameState.PlayerWon);
            gameOver = true;
        } else if (CheckTie()) {
            GameController.UpdateGameState(GameState.Draw);
            gameOver = true;
        } else
        {
            GameController.NextTurn();
        }
    }
    #endregion

}

public enum TileState
{
    Empty,
    Filled
}
