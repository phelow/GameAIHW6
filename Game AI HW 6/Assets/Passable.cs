using UnityEngine;
using System.Collections;

public class Passable : Tile
{
    void OnMouseDown()
    {

        if (ms_startingPosition == null)
        {
            ms_startingPosition = this;
            this.StartLerping();
        }
        else if (ms_endingPosition == null)
        {
            ms_endingPosition = this;
            this.StartLerping();
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
