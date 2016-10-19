using UnityEngine;
using System.Collections;
using System.Linq;

public class AssembleMap : MonoBehaviour {
    [SerializeField]
    private GameObject mp_lava;
    [SerializeField]
    private GameObject mp_water;
    [SerializeField]
    private GameObject mp_ground;

    [SerializeField]
    private TextAsset m_map;

    [SerializeField]
    private Camera m_camera;

    private float mc_timeslice = .1f;

    private Tile[,] m_mapTiles;

    public const float mc_cameraOffset = 32.5f;

    // Use this for initialization
    void Start () {
        StartCoroutine(MakeMap());
    }
	
    private IEnumerator MakeMap()
    {
        string[] lines = m_map.text.Split("\n"[0]);
        int y = 0;
        m_camera.orthographicSize = lines.Length / 2;
        m_camera.transform.position = new Vector3(m_camera.transform.position.x + m_camera.orthographicSize + mc_cameraOffset, m_camera.transform.position.y + m_camera.orthographicSize, m_camera.transform.position.z);


        string[] mapInput = lines.Skip(4).ToArray();
        
        int width = mapInput[0].Length;
        int height = mapInput.Length;

        m_mapTiles = new Tile[width, height];
        float t = 0.0f;
        foreach (string line in mapInput)
        {
            t += Time.deltaTime;
            if(t > mc_timeslice)
            {
                t = 0;
                yield return new WaitForEndOfFrame();
            }
            int x = 0;
            foreach (char c in line)
            {

                Tile curTile = null;
                switch (c)
                {
                    case '.':
                        curTile = (GameObject.Instantiate(mp_ground, new Vector3(transform.position.x + x, transform.position.y + y), transform.rotation, this.transform) as GameObject).GetComponent<Tile>();
                        break;
                    case '@':
                        curTile = (GameObject.Instantiate(mp_lava, new Vector3(transform.position.x + x, transform.position.y + y), transform.rotation, this.transform) as GameObject).GetComponent<Tile>();
                        break;
                    case 'T':
                        curTile = (GameObject.Instantiate(mp_water, new Vector3(transform.position.x + x, transform.position.y + y), transform.rotation, this.transform) as GameObject).GetComponent<Tile>();
                        break;
                }

                if (curTile == null)
                {
                    continue;
                }

                m_mapTiles[x, y] = curTile;

                x++;
            }
            y++;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
