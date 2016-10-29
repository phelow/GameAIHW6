using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {

	private List<Waypoint> neighbors;
	private ArrayList lines;

    [SerializeField]
    private Material m_renderOverEverything;


    public int x = 7721;


	// Use this for initialization

	public void addNeighbor (Waypoint neighbor){
		neighbors.Add (neighbor);
	}

    public void Initialize()
    {
        neighbors = new List<Waypoint>();
        lines = new ArrayList();

    }

	public void addLine(Vector3 targetPos){
		GameObject curLine = new GameObject();
		curLine.transform.SetParent (this.transform);
		curLine.AddComponent<LineRenderer>();
		LineRenderer lr = curLine.GetComponent<LineRenderer> ();
		lr.SetPosition (0, transform.position);
		lr.SetPosition (1, targetPos);
        lr.material = m_renderOverEverything;
	}

	public Vector3 getPos(){
		return gameObject.transform.position;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
