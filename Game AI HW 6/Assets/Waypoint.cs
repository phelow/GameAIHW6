using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {

	private List<Waypoint> neighbors;
	private ArrayList lines;

	public int x = 7721;


	// Use this for initialization
	void Start () {
		neighbors = new List<Waypoint> ();
		lines = new ArrayList ();
	}

	public void addNeighbor (Waypoint neighbor){
		neighbors.Add (neighbor);
	}

	public void addLine(Vector3 targetPos){
		GameObject curLine = new GameObject();
		curLine.transform.SetParent (this.transform);
		curLine.AddComponent<LineRenderer>();
		LineRenderer lr = curLine.GetComponent<LineRenderer> ();
		lr.SetPosition (0, transform.position);
		lr.SetPosition (1, targetPos);
	}

	public Vector3 getPos(){
		return gameObject.transform.position;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
