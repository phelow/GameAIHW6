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
	private List<Waypoint> removedWP;

	public GameObject canvas;
	private UIControl uic;
	private float tileSize;

    public static WaypointCreation ms_instance;

    // Use this for initialization
    void Start()
    {
        ms_instance = this;
        waypoints = new List<Waypoint>();
		removedWP = new List<Waypoint> ();
        uic = canvas.GetComponent<UIControl>();
		tileSize = Mathf.Pow(gameObject.GetComponent<AStarSearch> ().m_tileWidth * 1.0f, 2.0f);
		mapOffset = tileSize * 100;
    }

    private IEnumerator WaypointCoroutine()
	{
        float t = 0.0f;
        foreach (AStarSearch.AStarTile tile in AStarSearch.ms_instance.m_worldRepresentation)
        {
            t += .1f;
            if(t > 100.0f)
            {
                t = 0.0f;
                yield return new WaitForEndOfFrame();
            }

			if(tile == null)
			{
				Debug.Log("Tile is weird");
				continue;
			}
				
			Passable pass = tile.GetTiles () as Passable;
			if(pass == null)
            {
                continue;
            }
				
			Waypoint curPoint = (GameObject.Instantiate(waypoint, new Vector3(tile.GetTiles().transform.position.x, tile.GetTiles().transform.position.y, mapOffset), transform.rotation, this.transform) as GameObject).GetComponent<Waypoint>();
            waypoints.Add(curPoint);
            curPoint.GetComponent<Waypoint>().Initialize();
        }

		yield return new WaitForEndOfFrame();

		//t = 0.0f;
		int i = 5;
		while (i > 0) {
			foreach (Waypoint wp in waypoints) {
				/*t += .1f;
				if(t > 100.0f)
				{
					t = 0.0f;
					yield return new WaitForEndOfFrame();
				}*/

				if(removedWP.Contains(wp)){
					continue;
				}
				//1. expand box to collide with neighbors
				Debug.Log("Grow Waypoint: " + wp.transform.position);
				yield return new WaitForEndOfFrame();

				//1. raycast in four directions
				RaycastHit leftHit;
				Debug.DrawRay (wp.transform.position, new Vector3 (-1* wp.getWidth() * tileSize, 0, 0),Color.black,1.0f);
				if(Physics.Raycast(wp.transform.position, new Vector3(-1, 0, 0), out leftHit, wp.getWidth() * tileSize)){
					Debug.Log ("hit left");
					if (leftHit.transform.position.y == transform.position.y) {
						Waypoint otherWP = leftHit.transform.gameObject.GetComponent<Waypoint> ();
						if (!removedWP.Contains(otherWP) && otherWP.getHeight () == wp.getHeight()) {
							Debug.Log ("left match");
							Merge(wp, otherWP, true);
						}
					}
				}
				RaycastHit rightHit;
				Debug.DrawRay (wp.transform.position, new Vector3(1* wp.getWidth()* tileSize, 0, 0),Color.black,1.0f);
				if(Physics.Raycast(wp.transform.position, new Vector3(1, 0, 0), out rightHit, wp.getWidth() * tileSize)){
					Debug.Log ("hit right");
					if (rightHit.transform.position.y == transform.position.y) {
						Debug.Log ("horizontal match");
						Waypoint otherWP = rightHit.transform.gameObject.GetComponent<Waypoint> ();
						if (!removedWP.Contains(otherWP) && otherWP.getHeight () == wp.getHeight()) {
							Debug.Log ("right match");
							Merge (wp, otherWP, true);
						}
					}
				}
				RaycastHit topHit;
				Debug.DrawRay (wp.transform.position, new Vector3 (0, 1* wp.getWidth()*tileSize, 0),Color.black,1.0f);
				if (Physics.Raycast (wp.transform.position, new Vector3 (0, 1, 0), out topHit, wp.getHeight() * tileSize)){
					Debug.Log ("hit top");
					if (topHit.transform.position.x == transform.position.x) {
						Waypoint otherWP = topHit.transform.gameObject.GetComponent<Waypoint> ();
						if (!removedWP.Contains(otherWP) && otherWP.getWidth () == wp.getWidth()) {
							Debug.Log ("top match");
							Merge (wp, otherWP, false);
						}
					}
				}
				RaycastHit bottomHit;
				Debug.DrawRay (wp.transform.position, new Vector3(0, -1* wp.getWidth()*tileSize, 0),Color.black,1.0f);
				if(Physics.Raycast(wp.transform.position, new Vector3(0, -1, 0), out bottomHit, wp.getHeight() * tileSize)){
					Debug.Log ("hit bottom");
					if (bottomHit.transform.position.x == transform.position.x) {
						Waypoint otherWP = bottomHit.transform.gameObject.GetComponent<Waypoint> ();
						if (!removedWP.Contains(otherWP) && otherWP.getWidth () == wp.getWidth()) {
							Debug.Log ("bottom match");
							Merge (wp, otherWP, false);
						}
					}
				}
			}
			for(int j = 0; i < removedWP.Count; i++){
				Waypoint wp = removedWP [j];
				waypoints.Remove (wp);
				removedWP.Remove (wp);
				Destroy (wp);
			}
			i--;
		}

        /*t = 0.0f;
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
        }*/
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

	void Merge(Waypoint curPoint, Waypoint otherPoint, bool isHorizontal){
		Debug.Log("merging");
		Debug.Break ();
		Vector3 newCenter = curPoint.transform.position + ((otherPoint.transform.position - curPoint.transform.position)/2);
		if (isHorizontal) {
			float newWidth = otherPoint.getWidth () + curPoint.getWidth ();
			Debug.Log ("new Width: " + newWidth);
			removedWP.Add (otherPoint);
			curPoint.transform.position = newCenter;
			BoxCollider box = curPoint.gameObject.GetComponent<BoxCollider> ();
			box.size = new Vector3(newWidth, box.size.y, box.size.z);
			curPoint.setWidth (newWidth);
			Debug.Log ("Box size: " + box.size);
		} else {
			float newHeight = otherPoint.getHeight () + curPoint.getHeight ();
			Debug.Log ("new Height: " + newHeight);
			removedWP.Add(otherPoint);
			curPoint.transform.position = newCenter;
			BoxCollider box = curPoint.gameObject.GetComponent<BoxCollider> ();
			box.size = new Vector3(box.size.x, newHeight, box.size.z);
			curPoint.setHeight (newHeight);
			Debug.Log ("Box size: " + box.size);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
