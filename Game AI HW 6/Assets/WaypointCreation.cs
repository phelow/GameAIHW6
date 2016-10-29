using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WaypointCreation : MonoBehaviour {
	public GameObject waypoint;

	public TextAsset waypointFile;
	public TextAsset map;

	public float mapOffset;

	private List<Waypoint> waypoints;
	private ArrayList lines;

	public GameObject canvas;
	private UIControl uic;

    public static WaypointCreation ms_instance;

    // Use this for initialization
    void Start()
    {
        ms_instance = this;
        waypoints = new List<Waypoint>();
        lines = new ArrayList();
        uic = canvas.GetComponent<UIControl>();
    }

    private IEnumerator WaypointCoroutine()
    {

        //parse waypointFile
        string[] lines = waypointFile.text.Split("\n"[0]);

        float t = 0.0f;
        foreach (AStarSearch.AStarTile tile in AStarSearch.ms_instance.m_worldRepresentation)
        {
            t += .1f;
            if(t > 100.0f)
            {
                t = 0.0f;
                yield return new WaitForEndOfFrame();
            }
            if(!tile.GetTiles() is Passable)
            {
                continue;
            }

            if(tile == null)
            {
                Debug.Log("Tile is weird");
                continue;
            }
            Waypoint curPoint = (GameObject.Instantiate(waypoint, new Vector3(tile.GetTiles().transform.position.x, tile.GetTiles().transform.position.y, tile.GetTiles().transform.position.z), transform.rotation, this.transform) as GameObject).GetComponent<Waypoint>();
            waypoints.Add(curPoint);
            curPoint.GetComponent<Waypoint>().Initialize();
        }

        // ERROR IN addNeighbor function
        //for each waypoint, see if there is line of sight between this and every other waypoint
        //if so, add each other as neighbors and draw a line between them.

        t = 0.0f;
        for (int i = 0; i < waypoints.Count; i++)
        {
            for (int j = i + 1; j < waypoints.Count; j++)
            {

                t += .1f;
                if (t > 100.0f)
                {
                    t = 0.0f;
                    yield return new WaitForEndOfFrame();
                }

                Waypoint curPoint = waypoints[i];
                Waypoint otherPoint = waypoints[j];
                Vector3 curPos = curPoint.getPos();
                Vector3 otherPos = otherPoint.getPos();
                if (lineOfSight(curPos, otherPos))
                {
                    yield return new WaitForEndOfFrame();
                    curPoint.addNeighbor(otherPoint);
                    otherPoint.addNeighbor(curPoint);

                    if (curPoint.HasSameNeighbors(otherPoint))
                    {
                        Destroy(otherPoint);
                        continue;
                    }
                    curPoint.addLine(otherPos);
                }
            }
        }
        uic.activateButtons();

    }

	public void makeWaypoints(){

        StartCoroutine(WaypointCoroutine());
	}

	private bool lineOfSight(Vector3 curPos, Vector3 otherPos){
		//determine if there are no obstacles between curPoint and otherPoint
		curPos = new Vector3(curPos.x, curPos.y, 0f);
		otherPos = new Vector3 (otherPos.x, otherPos.y, 0f);
		Vector3 difference = otherPos - curPos;
		float distance = difference.magnitude;
		Vector3 direction = difference.normalized;
		RaycastHit[] hits;
		hits = Physics.RaycastAll (curPos, direction, distance);
		foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null) {
                Waypoint wp = hit.transform.gameObject.GetComponent<Waypoint>();
                if(wp != null)
                {
                    continue;
                }


				Passable passScript = hit.transform.gameObject.GetComponent<Passable> ();
				if (passScript == null) {
					//Debug.Log ("does not have component");
					return false;
				}
			}
		}
		//Debug.Log ("raycast passed");
		return true;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
