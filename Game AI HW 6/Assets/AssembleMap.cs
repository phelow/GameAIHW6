using UnityEngine;
using System.Collections;
using System.Linq;

public class AssembleMap : MonoBehaviour {
    [SerializeField]
    public GameObject mp_lava;
    [SerializeField]
    public GameObject mp_water;
    [SerializeField]
    public GameObject mp_ground;

    [SerializeField]
    private TextAsset m_map;

    [SerializeField]
    private Camera m_camera;

    public static AssembleMap ms_instance;

    private const float mc_timeslice = .1f;

    public static TileType[,] m_mapTiles;

    public enum TileType
    {
        Passable,
        OutOFBounds,
        Tree
    }


    public const float mc_cameraOffset = 32.5f;

    // Use this for initialization
    void Start () {
        ms_instance = this;
        StartCoroutine(MakeMap());
    }

    private IEnumerator MakeMap()
    {
        string[] lines = m_map.text.Split("\n"[0]);
        int y = 0;
        m_camera.orthographicSize = lines.Length * AStarSearch.ms_instance.m_tileWidth / 2;
        m_camera.transform.position = new Vector3(m_camera.transform.position.x + m_camera.orthographicSize + mc_cameraOffset, m_camera.transform.position.y + m_camera.orthographicSize, m_camera.transform.position.z);


        string[] mapInput = lines.Skip(4).ToArray();
        
        int width = mapInput[0].Length;
        int height = mapInput.Length;

        m_mapTiles = new TileType[width, height];
        float t = 0.0f;
        int x = 0;
        foreach (string line in mapInput)
        {
            t += Time.deltaTime;
            if(t > mc_timeslice)
            {
                t = 0;
                yield return new WaitForEndOfFrame();
            }
            x = 0;
            foreach (char c in line)
            {

                TileType curTile = TileType.OutOFBounds;
                switch (c)
                {
                    case '.':
                        curTile = TileType.Passable;
                        break;
                    case '@':
                        curTile = TileType.OutOFBounds;
                        break;
                    case 'T':
                        curTile = TileType.Tree ;
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


        AStarSearch.CreateWorldRepresentation();

    }
}
