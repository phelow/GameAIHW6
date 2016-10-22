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
    public int m_tileWidth = 2;
    
    public class AStarTile
    {
        private Tile [,] m_worldTiles;
        private AStarTile m_cameFrom;
        private float m_costFromStart;
        private float m_estimatedCostToGoal;

        private int m_xPosition;
        private int m_yPosition;

        public bool IsPassable()
        {
            for(int x = 0; x < AStarSearch.ms_instance.m_tileWidth; x++)
            {
                for(int y = 0; y < AStarSearch.ms_instance.m_tileWidth; y++)
                {
                    if(!(m_worldTiles[x,y] is Passable))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool ContainsTile(Tile t)
        {

            for (int x = 0; x < AStarSearch.ms_instance.m_tileWidth; x++)
            {
                for (int y = 0; y < AStarSearch.ms_instance.m_tileWidth; y++)
                {
                    Tile tile = m_worldTiles[x, y];
                    if (tile == t)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Color()
        {
            for (int x = 0; x < AStarSearch.ms_instance.m_tileWidth; x++)
            {
                for (int y = 0; y < AStarSearch.ms_instance.m_tileWidth; y++)
                {
                    m_worldTiles[x,y].ChangeColor();
                }
            }
        }

        public void StartLerping()
        {
            foreach(Tile worldTile in this.m_worldTiles)
            {
                worldTile.StartLerping();
            }
        }

        public void StopLerping()
        {
            foreach (Tile worldTile in this.m_worldTiles)
            {
                worldTile.StopLerping();
            }

        }

        public AStarTile(Tile [,] worldTile, int x, int y)
        {
            m_estimatedCostToGoal = Mathf.Infinity;
            m_costFromStart = Mathf.Infinity;
            m_worldTiles = worldTile;

            this.m_xPosition = x;
            this.m_yPosition = y;
        }

        public Tile[,] GetTiles()
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
    void Start()
    {
        ms_instance = this;
    }

    public static void CreateWorldRepresentation()
    {
        ms_instance.m_worldWidth = AssembleMap.m_mapTiles.GetLength(0)/ ms_instance.m_tileWidth;
        ms_instance.m_worldHeight = AssembleMap.m_mapTiles.GetLength(1)/ ms_instance.m_tileWidth;

        ms_instance.m_worldRepresentation = new AStarTile[ms_instance.m_worldWidth, ms_instance.m_worldHeight];
        int xWorld = 0;
        int yWorld = 0;
        for (int y = 0; y + 1 < AssembleMap.m_mapTiles.GetLength(1); y += ms_instance.m_tileWidth)
        {
            xWorld = 0;
            for (int x = 0; x + 1 < AssembleMap.m_mapTiles.GetLength(0); x += ms_instance.m_tileWidth)
            {
            
                Tile[,] assembleMaps = new Tile[ms_instance.m_tileWidth, ms_instance.m_tileWidth];
                
                for(int i = 0; i < ms_instance.m_tileWidth; i++)
                {
                    for (int j = 0; j < ms_instance.m_tileWidth; j++)
                    {
                            assembleMaps[i, j] = AssembleMap.m_mapTiles[x + i, y + j];
                    }
                }
                if(xWorld < ms_instance.m_worldWidth && yWorld < ms_instance.m_worldHeight) { 
                    ms_instance.m_worldRepresentation[xWorld, yWorld] = new AStarTile(assembleMaps, xWorld, yWorld);
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
                if (ms_instance.m_worldRepresentation[x, y].ContainsTile(m_tile))
                {
                    return ms_instance.m_worldRepresentation[x, y];
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
            if(t > mc_timeslice)
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
                if (!neighbor.IsPassable())
                {
                    continue;
                }

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
    }


    public void StartUsingManhattanDistance()
    {
        m_useManhattanDistance = true;
    }

    public int EuclidianDistance(AStarTile current, AStarTile goal)
    {
        return (int)Mathf.Sqrt(Mathf.Pow((Mathf.Abs((float)current.X) - goal.X),2.0f) + Mathf.Pow(Mathf.Abs(((float)current.Y) - goal.Y),2.0f) );
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

    }
}
