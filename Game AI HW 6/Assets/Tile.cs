using UnityEngine;
using System.Collections;
using System.Linq;

public class Tile : MonoBehaviour {
    protected static Passable ms_startingPosition = null;
    protected static Passable ms_endingPosition = null;
    private Color m_originalColor;

    [SerializeField]
    private MeshRenderer m_meshRenderer;

    private bool m_shouldLerp = true;

    // Use this for initialization
    void Awake () {
        m_originalColor = m_meshRenderer.material.GetColor("_Color");
    }

    // Update is called once per frame
    void Update () {
	    
	}

    public void ChangeColor()
    {
        m_meshRenderer.material.SetColor("_Color", Color.black);
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

    protected void StopLerping()
    {
        m_shouldLerp = false;
        StopCoroutine(BlinkColor());
    }

    protected void StartLerping()
    {
        m_shouldLerp = true;
        StartCoroutine(BlinkColor());
    }
}
