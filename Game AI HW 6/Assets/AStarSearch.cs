using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarSearch : MonoBehaviour
{
    private AStarTile[,] m_worldRepresentation;
    public static AStarSearch ms_instance;
    private int m_worldHeight;
    private int m_worldWidth;
    [SerializeField]
    private bool m_useManhattanDistance = false;

    private const float mc_timeslice = 1.0f;

    [SerializeField]
    private GameObject mp_passable;
    public static bool ms_rmbDown = false;

    public static bool m_shouldLerp = true;
    [SerializeField]
    private GameObject mp_outOfBounds;
    [SerializeField]
    private GameObject mp_tree;
    [SerializeField]
    public int m_tileWidth = 2;

    public static void ReplaceTile(Tile toReplace)
    {
        GetAStarTileByWorldTile(toReplace).ReplaceTile(toReplace);
    }

    public class AStarTile
    {
        private Tile m_worldTiles;
        private AStarTile m_cameFrom;
        private float m_costFromStart;
        private float m_estimatedCostToGoal;

        private int m_xPosition;
        private int m_yPosition;

        public void ReplaceTile(Tile tile)
        {
            if ((m_worldTiles == tile))
            {
                Vector3 pos = tile.transform.position;
                Quaternion rot = tile.transform.rotation;
                Destroy(tile.gameObject);
                if(tile is Passable)
                {
                    m_worldTiles = (GameObject.Instantiate(ms_instance.mp_outOfBounds, pos, rot, null) as GameObject).GetComponent<Tile>();
                            
                }else if (tile is OutOfBounds)
                {
                    m_worldTiles = (GameObject.Instantiate(ms_instance.mp_tree, pos, rot, null) as GameObject).GetComponent<Tile>();

                }
                else if (tile is Tree)
                {
                    m_worldTiles= (GameObject.Instantiate(ms_instance.mp_passable, pos, rot, null) as GameObject).GetComponent<Tile>();

                }
            }

        }

        public bool IsPassable()
        {
            if (!(m_worldTiles is Passable))
            {
                return false;
            }
            return true;
        }

        public void ColorPath()
        {
            m_worldTiles.ColorNodeAsPath();
        }

        public bool ContainsTile(Tile t)
        {
            if (m_worldTiles == t)
            {
                return true;
            }
            return false;
        }

        public void Color()
        {
            m_worldTiles.ChangeColor();
        }

        public void StartLerping()
        {
             m_worldTiles.StartLerping();
        }

        public void StopLerping()
        {
                m_worldTiles.StopLerping();

        }

        public AStarTile(Tile worldTile, int x, int y)
        {
            m_estimatedCostToGoal = Mathf.Infinity;
            m_costFromStart = Mathf.Infinity;
            m_worldTiles = worldTile;

            this.m_xPosition = x;
            this.m_yPosition = y;
        }

        public Tile GetTiles()
        {
            return m_worldTiles;
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

        public int X
        {
            get
            {
                return this.m_xPosition;
            }
        }

        public int Y
        {
            get
            {
                return this.m_yPosition;
            }
        }

        public AStarTile CameFrom
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
    }
    // Use this for initialization
    void Awake()
    {
        ms_instance = this;
    }

    public static void CreateWorldRepresentation()
    {
        ms_instance.m_worldWidth = AssembleMap.m_mapTiles.GetLength(0) / ms_instance.m_tileWidth;
        ms_instance.m_worldHeight = AssembleMap.m_mapTiles.GetLength(1) / ms_instance.m_tileWidth;

        ms_instance.m_worldRepresentation = new AStarTile[ms_instance.m_worldWidth, ms_instance.m_worldHeight];
        int xWorld = 0;
        int yWorld = 0;
        for (int y = 0; y + 1 < AssembleMap.m_mapTiles.GetLength(1); y += ms_instance.m_tileWidth)
        {
            xWorld = 0;
            for (int x = 0; x + 1 < AssembleMap.m_mapTiles.GetLength(0); x += ms_instance.m_tileWidth)
            {

                int passableCount = 0;
                int outOFBoundsCount = 0;
                int waterCount = 0;

                for(int i = 0; i < ms_instance.m_tileWidth; i++)
                {
                    for (int j = 0; j < ms_instance.m_tileWidth; j++)
                    {
                        if(AssembleMap.m_mapTiles[x+i,y+j] == AssembleMap.TileType.Tree)
                        {
                            waterCount++;
                        }
                        if (AssembleMap.m_mapTiles[x+i,y+ j] == AssembleMap.TileType.Passable)
                        {
                            passableCount++;
                        }
                        if (AssembleMap.m_mapTiles[x+i, y+j] == AssembleMap.TileType.OutOFBounds)
                        {
                            outOFBoundsCount++;
                        }
                    }

                }
                
                if(outOFBoundsCount > 0)
                {
                    GameObject block = GameObject.Instantiate(AssembleMap.ms_instance.mp_lava, ms_instance.transform.position + new Vector3(x * ms_instance.m_tileWidth, y * ms_instance.m_tileWidth, 0), ms_instance.transform.rotation, null) as GameObject;
                    block.transform.localScale *= Mathf.Pow(ms_instance.m_tileWidth,1.9f);
                    ms_instance.m_worldRepresentation[xWorld, yWorld] = new AStarTile((block).GetComponent<Tile>(), xWorld, yWorld);
                } else if(waterCount > 0)
                {
                    GameObject block = GameObject.Instantiate(AssembleMap.ms_instance.mp_water, ms_instance.transform.position + new Vector3(x * ms_instance.m_tileWidth, y * ms_instance.m_tileWidth, 0), ms_instance.transform.rotation, null) as GameObject;
                    block.transform.localScale *= Mathf.Pow(ms_instance.m_tileWidth, 1.9f);
                    ms_instance.m_worldRepresentation[xWorld, yWorld] = new AStarTile((block).GetComponent<Tile>(), xWorld, yWorld);

                }
                else
                {
                    GameObject block = GameObject.Instantiate(AssembleMap.ms_instance.mp_ground, ms_instance.transform.position + new Vector3(x * ms_instance.m_tileWidth, y * ms_instance.m_tileWidth, 0), ms_instance.transform.rotation, null) as GameObject;
                    block.transform.localScale *= Mathf.Pow(ms_instance.m_tileWidth, 1.9f);
                    ms_instance.m_worldRepresentation[xWorld, yWorld] = new AStarTile((block).GetComponent<Tile>(), xWorld, yWorld);

                }
                xWorld++;
            }
            yWorld++;
        }
    }

    public static AStarTile GetAStarTileByWorldTile(Tile m_tile)
    {
        for (int y = 0; y < ms_instance.m_worldHeight; y++)
        {
            for (int x = 0; x < ms_instance.m_worldWidth; x++)
            {
                try {
                    if (ms_instance.m_worldRepresentation[x, y].ContainsTile(m_tile))
                    {
                        return ms_instance.m_worldRepresentation[x, y];
                    }
                }
                catch
                {

                }
            }
        }

        return null;

    }

    public static void PerformSearch(AStarTile startingPoint, AStarTile endingPoint)
    {
        //Assemble the tile representation of the wordl
        ms_instance.StartCoroutine(ms_instance.PerformAStarSearchCoroutine(
            startingPoint,
            endingPoint));
    }

    private IEnumerator PerformAStarSearchCoroutine(AStarTile startingTile, AStarTile endingTile)
    {
        yield return new WaitForEndOfFrame();

        HashSet<AStarTile> closedSet = new HashSet<AStarTile>(); //the already evaluated set of nodes

        HashSet<AStarTile> openSet = new HashSet<AStarTile>(); //the set of currently discovered nodes to be evaluated

        openSet.Add(startingTile);

        startingTile.CostFromStart = 0;
        startingTile.EstimatedCostToGoal = HeuristicCalculation(startingTile, endingTile);
        float t = 0.0f;
        while (openSet.Count > 0)
        {

            t += Time.deltaTime;
            if (t > mc_timeslice)
            {
                yield return new WaitForEndOfFrame();
                t = 0;
            }
            AStarTile current = null;
            foreach (AStarTile tile in openSet)
            {
                if (current == null || tile.EstimatedCostToGoal < current.EstimatedCostToGoal)
                {
                    current = tile;
                }
            }


            current.Color();
            openSet.Remove(current);
            closedSet.Add(current);
            
            if(current.IsPassable() == false)
            {
                continue;
            }
            if (current == endingTile)
            {
                break;
            }

            HashSet<AStarTile> neighbors = new HashSet<AStarTile>();

            if (current.X - 1 > 0)
            {
                if (current.Y - 1 > 0)
                {
                    neighbors.Add(m_worldRepresentation[current.X - 1, current.Y - 1]);
                }

                if (current.Y + 1 < this.m_worldHeight)
                {
                    neighbors.Add(m_worldRepresentation[current.X - 1, current.Y + 1]);

                }
                neighbors.Add(m_worldRepresentation[current.X - 1, current.Y]);
            }
            if (current.Y - 1 > 0)
            {
                neighbors.Add(m_worldRepresentation[current.X, current.Y - 1]);
            }

            if (current.Y + 1 < this.m_worldHeight)
            {
                neighbors.Add(m_worldRepresentation[current.X, current.Y + 1]);

            }

            if (current.X + 1 < this.m_worldWidth)
            {
                if (current.Y - 1 > 0)
                {
                    neighbors.Add(m_worldRepresentation[current.X + 1, current.Y - 1]);
                }

                if (current.Y + 1 < this.m_worldHeight)
                {
                    neighbors.Add(m_worldRepresentation[current.X + 1, current.Y + 1]);
                }

                neighbors.Add(m_worldRepresentation[current.X + 1, current.Y]);
            }

            foreach (AStarTile neighbor in neighbors)
            {

                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float tentativeScore = current.CostFromStart + 1; //TODO: 1 is the distance between the two nodes (will always be one for now, change this tater)

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeScore >= neighbor.CostFromStart)
                {
                    continue;
                }
                neighbor.CameFrom = current;
                neighbor.CostFromStart = tentativeScore;
                neighbor.EstimatedCostToGoal = HeuristicCalculation(neighbor, endingTile) + current.CostFromStart;
            }
        }


        AStarTile backTrackCurrent = endingTile;
        while (backTrackCurrent != startingTile)
        {
            backTrackCurrent.ColorPath();
            backTrackCurrent = backTrackCurrent.CameFrom;
            yield return new WaitForEndOfFrame();
        }

    }


    public void StartUsingManhattanDistance()
    {
        m_useManhattanDistance = true;
    }

    public int EuclidianDistance(AStarTile current, AStarTile goal)
    {
        return (int)Mathf.Sqrt(Mathf.Pow((Mathf.Abs((float)current.X) - goal.X), 2.0f) + Mathf.Pow(Mathf.Abs(((float)current.Y) - goal.Y), 2.0f));
    }

    public int ManHattanDistance(AStarTile current, AStarTile goal)
    {
        return Mathf.Abs(current.X - goal.X) + Mathf.Abs(current.Y - goal.Y);
    }

    public int HeuristicCalculation(AStarTile current, AStarTile goal)
    {
        return m_useManhattanDistance ? ManHattanDistance(current, goal) : EuclidianDistance(current, goal);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ms_rmbDown = !ms_rmbDown;
        }
    }
}
