using UnityEngine;
using System.Collections;
using System.Linq;

public class Tile : MonoBehaviour {
    private static Tile ms_startingPosition = null;
    private static Tile ms_endingPosition = null;
    private Color m_originalColor;

    [SerializeField]
    private MeshRenderer m_meshRenderer;

    private bool m_shouldLerp = true;

    // Use this for initialization
    void Start () {
        m_originalColor = m_meshRenderer.material.color;
    }

    // Update is called once per frame
    void Update () {
	    
	}

    void OnMouseDown()
    {
        if(ms_startingPosition != null && ms_endingPosition != null)
        {
            ms_startingPosition.StopLerping();
            ms_endingPosition.StopLerping();

            ms_startingPosition = null;
            ms_endingPosition = null;
        }

        if(ms_startingPosition == null)
        {
            ms_startingPosition = this;
            this.StartLerping();
        } else if (ms_endingPosition == null)
        {
            ms_endingPosition = this;
            this.StartLerping();
        }
    }

    private IEnumerator BlinkColor()
    {
        while (m_shouldLerp)
        {
            m_meshRenderer.enabled = false;

            yield return new WaitForSeconds(.1f);
            m_meshRenderer.enabled = true;
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
