using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We will use the Serializable class BoardTileMove to keep track of all of the moves that take place in a given game
// Making it a serializable class means that we can easily export the moves to another program through something like JSON
// to be reconstructed later.
[System.Serializable]
public class GameMove
{
    // Using private setters will keep our variables protected from modification by other scripts, but
    // will still allow those scripts to read these values. If we need to update these values, methods
    // should be created to verify and update those values.

    public int row { get; private set; }

    public int column { get; private set; }

    public int playerID { get; private set; }

    public GameMove(int ro, int col, int playerID)
    {
        this.row = ro;
        this.column = col;
        this.playerID = playerID;
    }
}
