using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarSearch : MonoBehaviour
{
    private AStarTile[,] m_worldRepresentation;
    private static AStarSearch ms_instance;
    private int m_worldHeight;
    private int m_worldWidth;

    private const float mc_timeslice = 1.0f;
    
    public class AStarTile
    {
        Tile m_worldTile;
        private AStarTile m_cameFrom;
        private float m_costFromStart;
        private float m_estimatedCostToGoal;

        private int m_xPosition;
        private int m_yPosition;

        public bool IsPassable()
        {
            return m_worldTile is Passable;
        }

        public void Color()
        {
            m_worldTile.ChangeColor();
        }

        public AStarTile(Tile worldTile, int x, int y)
        {
            m_estimatedCostToGoal = Mathf.Infinity;
            m_costFromStart = Mathf.Infinity;
            m_worldTile = worldTile;

            this.m_xPosition = x;
            this.m_yPosition = y;
        }

        public Tile GetTile()
        {
            return m_worldTile;
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

    public void CreateWorldRepresentation()
    {
        m_worldWidth = AssembleMap.m_mapTiles.GetLength(0);
        m_worldHeight = AssembleMap.m_mapTiles.GetLength(1);

        m_worldRepresentation = new AStarTile[m_worldWidth, m_worldHeight];

        for (int y = 0; y < m_worldHeight; y++)
        {
            for (int x = 0; x < m_worldWidth; x++)
            {
                m_worldRepresentation[x, y] = new AStarTile(AssembleMap.m_mapTiles[x, y], x, y);
            }
        }
    }

    public AStarTile GetAStarTileByWorldTile(Tile m_tile)
    {
        for (int y = 0; y < m_worldHeight; y++)
        {
            for (int x = 0; x < m_worldWidth; x++)
            {
                if (m_worldRepresentation[x, y].GetTile() == m_tile)
                {
                    return m_worldRepresentation[x, y];
                }
            }
        }

        return null;

    }

    public static void PerformSearch(Tile startingPoint, Tile endingPoint)
    {
        //Assemble the tile representation of the wordl
        ms_instance.CreateWorldRepresentation();

        ms_instance.StartCoroutine(ms_instance.PerformAStarSearchCoroutine(
            ms_instance.GetAStarTileByWorldTile(startingPoint),
            ms_instance.GetAStarTileByWorldTile(endingPoint)));
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

    public int HeuristicCalculation(AStarTile current, AStarTile goal)
    {
        return Mathf.Abs(current.X - goal.X) + Mathf.Abs(current.Y - goal.Y);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
