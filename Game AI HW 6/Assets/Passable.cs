using UnityEngine;
using System.Collections;

public class Passable : Tile
{
    void OnMouseDown()
    {

        if (ms_startingPosition == null)
        {
            ms_startingPosition = AStarSearch.GetAStarTileByWorldTile(this);
            ms_startingPosition.StartLerping();
        }
        else if (ms_endingPosition == null)
        {
            ms_endingPosition = AStarSearch.GetAStarTileByWorldTile(this);
            ms_endingPosition.StartLerping();
        }

        if (ms_startingPosition != null && ms_endingPosition != null)
        {
            ms_startingPosition.StopLerping();
            ms_endingPosition.StopLerping();

            AStarSearch.PerformSearch(ms_startingPosition, ms_endingPosition);

            ms_startingPosition = null;
            ms_endingPosition = null;
        }
    }
    // Update is called once per frame
    void Update () {
	
	}
}
