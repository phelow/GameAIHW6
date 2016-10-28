using UnityEngine;
using System.Collections;
using System.Linq;

public class Tile : MonoBehaviour {
    protected static AStarSearch.AStarTile ms_startingPosition = null;
    protected static AStarSearch.AStarTile ms_endingPosition = null;
    private float m_clickTime = 1.0f;
    private Color m_originalColor;

    [SerializeField]
    private MeshRenderer m_meshRenderer;

    private bool m_shouldLerp = true;

    // Use this for initialization
    void Awake () {
        m_originalColor = m_meshRenderer.material.GetColor("_Color");
    }

    public void ChangeColor()
    {
        m_meshRenderer.material.SetColor("_Color", Color.black);
    }
    
    public void ColorNodeAsPath()
    {
        m_meshRenderer.material.SetColor("_Color", Color.cyan);
    }

    void OnMouseDown()
    {
        //if it's a right click, cycle between passable, out of bounds and tree
        if (AStarSearch.ms_rmbDown)
        {
            Debug.Log("MouseButtonOneIsDown");
            AStarSearch.ReplaceTile(this);
            return;
        }


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

    private IEnumerator BlinkColor()
    {
        while (m_shouldLerp)
        {
            m_meshRenderer.material.SetColor("_Color", Color.black);
            yield return new WaitForSeconds(.1f);
            m_meshRenderer.material.SetColor("_Color", m_originalColor);
            yield return new WaitForSeconds(.1f);

        }
    }

    public void StopLerping()
    {
        m_shouldLerp = false;
        StopCoroutine(BlinkColor());
    }

    public void StartLerping()
    {
        m_shouldLerp = true;
        StartCoroutine(BlinkColor());
    }
}
