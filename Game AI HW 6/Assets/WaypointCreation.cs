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

	// Use this for initialization
	void Start () {
		waypoints = new List<Waypoint> ();
		lines = new ArrayList ();
		uic = canvas.GetComponent<UIControl> ();
	}

	public void makeWaypoints(){
		//parse waypointFile
		string[] lines = waypointFile.text.Split("\n"[0]);

		foreach (string line in lines) {
			if (line == "")
				continue;
			string[] coordinates = line.Split (","[0]);
			float xpos = float.Parse (coordinates [0]);
			float ypos = float.Parse (coordinates [1]);
			Waypoint curPoint = (GameObject.Instantiate (waypoint, new Vector3 (xpos, ypos, mapOffset), transform.rotation, this.transform) as GameObject).GetComponent<Waypoint> ();
			waypoints.Add (curPoint);
		}

		// ERROR IN addNeighbor function
		//for each waypoint, see if there is line of sight between this and every other waypoint
		//if so, add each other as neighbors and draw a line between them.
		for (int i = 0; i < waypoints.Count; i++) {
			for (int j = i + 1; j < waypoints.Count; j++) {
				Waypoint curPoint = waypoints [i];
				Waypoint otherPoint = waypoints [j];
				Vector3 curPos = curPoint.getPos ();
				Vector3 otherPos = otherPoint.getPos ();
				if (lineOfSight (curPos, otherPos)) {
					curPoint.addNeighbor (otherPoint);
					otherPoint.addNeighbor (curPoint);
					curPoint.addLine (otherPos);
				}
			}
		}
		uic.activateButtons ();
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

		foreach (RaycastHit hit in hits) {
			if (hit.collider != null) {
				Passable passScript = hit.transform.gameObject.GetComponent<Passable> ();
				if (passScript == null) {
					Debug.Log ("does not have component");
					return false;
				}
			}
		}
		Debug.Log ("raycast passed");
		return true;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
