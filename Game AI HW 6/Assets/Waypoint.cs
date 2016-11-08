using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : Tile {

	private HashSet<Waypoint> neighbors;
    private ArrayList lines;

    [SerializeField]
    private Material m_renderOverEverything;

	public bool collisionDetected = false;

	public bool stopGrowth = false;

	private float tileSize;
	private float boxUnit;

	private float height;
	private float width;

	//private Tile m_worldTiles;
	private Waypoint m_cameFrom;
	private float m_costFromStart;
	private float m_estimatedCostToGoal;

	//private int m_xPosition;
	//private int m_yPosition;


    // Use this for initialization
	public void Initialize()
	{
		neighbors = new HashSet<Waypoint>();
		lines = new ArrayList();

		tileSize = Mathf.Pow(transform.parent.GetComponent<AStarSearch> ().m_tileWidth * 1.0f, 1.9f);
		float tileUnit = Mathf.Pow(transform.parent.GetComponent<AStarSearch> ().m_tileWidth * 1.0f, 2.0f);
		boxUnit = tileUnit / tileSize;
		transform.localScale *= tileSize;
		gameObject.GetComponent<BoxCollider> ().size *= boxUnit;
		height = boxUnit;
		width = boxUnit;

		m_costFromStart = Mathf.Infinity;
		m_estimatedCostToGoal = Mathf.Infinity;
	}


    public void addNeighbor(Waypoint neighbor) {
        neighbors.Add(neighbor);
    }
		
    public bool HasSameNeighbors(Waypoint other)
    {
		HashSet<Waypoint> otherNeighbors = other.GetNeighbors();

        foreach(Waypoint wp in otherNeighbors)
        {
            if (!neighbors.Contains(wp))
            {
                return false;
            }
        }
        return true;
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

	public float getHeight(){
		return height;
	}

	public float getWidth(){
		return width;
	}

	public void setHeight(float newHeight){
		height = newHeight;
	}

	public void setWidth(float newWidth){
		width = newWidth;
	}


	public void ColorPath()
	{
		this.ColorNodeAsPath();
	}

	public void Color()
	{
		this.ChangeColor();
	}

	public void StartLerping()
	{
		this.StartLerping();
	}

	public void StopLerping()
	{
		this.StopLerping();

	}

	public float CostFromStart
	{
		get
		{
			return this.m_costFromStart;
		}
		set
		{
			this.m_costFromStart = value;
		}
	}

	public float EstimatedCostToGoal
	{
		get
		{
			return this.m_estimatedCostToGoal;
		}
		set
		{
			this.m_estimatedCostToGoal = value;
		}
	}

	public float X
	{
		get
		{
			return this.transform.position.x;
		}
	}

	public float Y
	{
		get
		{
			return this.transform.position.y;
		}
	}

	public Waypoint CameFrom
	{
		get
		{
			return this.m_cameFrom;
		}
		set
		{
			this.m_cameFrom = value;
		}
	}

	public HashSet<Waypoint> GetNeighbors() {
		return neighbors;
	}

}
