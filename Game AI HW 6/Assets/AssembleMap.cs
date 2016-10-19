using UnityEngine;
using System.Collections;

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
	// Use this for initialization
	void Start () {
        string[] lines = m_map.text.Split("\n"[0]);
        int y = 0;
        m_camera.orthographicSize = lines.Length/2;
        m_camera.transform.position = new Vector3(m_camera.transform.position.x + m_camera.orthographicSize, m_camera.transform.position.y + m_camera.orthographicSize, m_camera.transform.position.z);
        foreach (string line in lines)
        {
            Debug.Log(line);
            y++;
            int x = 0;
            foreach (char c in line)
            {
                x++;
                switch (c)
                {
                    case '.':
                        GameObject.Instantiate(mp_ground, new Vector3(transform.position.x + x, transform.position.y + y), transform.rotation, null);
                        break;
                    case '@':
                        GameObject.Instantiate(mp_lava, new Vector3(transform.position.x + x, transform.position.y + y), transform.rotation, null);
                        break;
                    case 'T':
                        GameObject.Instantiate(mp_water, new Vector3(transform.position.x + x, transform.position.y + y), transform.rotation, null);
                        break;
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
