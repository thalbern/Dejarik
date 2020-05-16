using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    #region Properties

    [Header("Sizes")]
    [SerializeField]
    [Tooltip("Radius of the inner row of pieces")]
    private float innerRowRadius = 1;

    [SerializeField]
    [Tooltip("Radius of the outer row of pieces")]
    private float outerRowRadius = 2;

    [SerializeField]
    [Tooltip("The radius of the board")]
    private float boundingRadius = 3;

    /// <summary>
    /// The radius of the circle going through the center of the inner row
    /// of tiles, where the pieces are positioned.
    /// </summary>
    public float InnerRowRadius => innerRowRadius;

    /// <summary>
    /// The radius of the circle going through the center of the outer row
    /// of tiles, where the pieces are positioned.
    /// </summary>
    public float OuterRowRadius => outerRowRadius;

    /// <summary>
    /// The bounding radius of the board at the outer side of the outer row.
    /// </summary>
    public float BoundingRadius => boundingRadius;

    public class Tile
    {
        // board position of the tile
        public TilePos position;

        // array of connections for this tile
        public Tile[] connections;

        // true if the tile is occupied
        public bool occupied;
    }

    private Dictionary<TilePos, Tile> tiles;

    #endregion Properties

    #region MonoBehaviour Methods

    private void Start()
    {
        tiles = new Dictionary<TilePos, Tile>(25);

        Tile centre = new Tile
        {
            position = TilePos.Zero,
            connections = null,
            occupied = false
        };
        tiles.Add(centre.position, centre);

        Tile[] inners = new Tile[12];
        Tile[] outers = new Tile[12];
        for (int i = 0; i < 12; ++i)
        {
            // Inner row
            Tile inner = new Tile
            {
                position = new TilePos(1, i),
                connections = null,
                occupied = false
            };
            tiles.Add(inner.position, inner);
            inners[i] = inner;

            // Outer row
            Tile outer = new Tile
            {
                position = new TilePos(2, i),
                connections = null,
                occupied = false
            };
            tiles.Add(outer.position, outer);
            outers[i] = outer;
        }

        centre.connections = new Tile[12];
        for (int i = 0; i < 12; ++i)
        {
            centre.connections[i] = inners[i];
        }
        for (int i = 0; i < 12; ++i)
        {
            inners[i].connections = new Tile[4];
            inners[i].connections[0] = centre;
            inners[i].connections[1] = inners[(i + 11) % 12];
            inners[i].connections[2] = inners[(i + 1) % 12];
            inners[i].connections[3] = outers[i];

            outers[i].connections = new Tile[3];
            outers[i].connections[0] = outers[(i + 11) % 12];
            outers[i].connections[1] = outers[(i + 1) % 12];
            outers[i].connections[2] = inners[i];
        }

        //// TEMP - Spawn some pieces for debugging
        //GameObject piece1 = GameObject.Instantiate(CharacterManager.instance.ghhhk, transform);
        //piece1.transform.position = GetPosition(0, 0);
        //GameObject piece2 = GameObject.Instantiate(CharacterManager.instance.kLorSlug, transform);
        //piece2.transform.position = GetPosition(1, 0);
        //GameObject piece3 = GameObject.Instantiate(CharacterManager.instance.monnok, transform);
        //piece3.transform.position = GetPosition(2, 1);
    }

    void Update()
    {
#if false
        foreach(var tile in tiles.Values)
        {
            foreach (var connection in tile.connections)
            {
                Debug.DrawLine(transform.TransformVector(tile.position), transform.TransformVector(tiles[connection].position), Color.cyan);
            }
        }
#endif
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, boundingRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, innerRowRadius);
        Gizmos.DrawWireSphere(transform.position, outerRowRadius);
        float dth = 360f / 12f;
        float th = dth / 2f;
        for (int i = 0; i < 12; ++i)
        {
            Vector3 dir = Quaternion.AngleAxis(th, Vector3.up) * Vector3.right * boundingRadius;
            Gizmos.DrawLine(transform.position, transform.position + dir);
            th += dth;
        }
    }
#endif

    #endregion MonoBehaviour Methods

    #region Public Methods

    /// <summary>
    /// Get the world-space position of a piece for the given tile position on the board.
    /// </summary>
    /// <param name="position">The tile position on the board.</param>
    /// <returns>The world-space position of the tile center where a piece would stand.</returns>
    public Vector3 GetPosition(TilePos position)
    {
        const float deltaAngle = (Mathf.PI * 2) / 12;

        if (position.row == 0)
        {
            return transform.position;
        }

        float angle = deltaAngle * (position.sector + 0.5f);
        Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (position.row == 1)
        {
            return transform.position + dir * innerRowRadius;
        }
        if (position.row == 2)
        {
            return transform.position + dir * outerRowRadius;
        }

        throw new ArgumentException();
    }

    /// <summary>
    /// Get the set of tiles reachable from the given starting position with the exact move count,
    /// and which are not already occupied. Tiles reachable with less moves are not returned.
    /// </summary>
    /// <param name="startPos">Tile where the move starts.</param>
    /// <param name="moveCount">Number of moves from the starting position.</param>
    /// <returns>The set of non-occupied tiles reachable from the starting position.</returns>
    public HashSet<Tile> GetMovableTiles(TilePos startPos, int moveCount)
    {
        HashSet<Tile> movableTiles = new HashSet<Tile>();

        void depthFirstSearch(TilePos curPos, int remainingMovement, List<TilePos> path)
        {
            path.Add(curPos);

            if (remainingMovement <= 0)
            {
                movableTiles.Add(tiles[curPos]);
                return;
            }

            foreach (Tile tile in tiles[curPos].connections)
            {
                if (!tile.occupied && !path.Contains(tile.position))
                {
                    depthFirstSearch(tile.position, remainingMovement - 1, new List<TilePos>(path));
                }
            }
        }

        depthFirstSearch(startPos, moveCount, new List<TilePos>());

        return movableTiles;
    }

    /// <summary>
    /// Retrieve a tile by position and set its occupied state.
    /// </summary>
    /// <param name="position">The tile position.</param>
    /// <param name="occupied">The occupied state to set.</param>
    public void SetTileOccupied(TilePos position, bool occupied)
    {
        if (tiles.TryGetValue(position, out Tile tile))
        {
            tile.occupied = occupied;
        }
    }

    /// <summary>
    /// Lookup a tile by position.
    /// </summary>
    /// <param name="position">Board position of the tile to find.</param>
    /// <returns>The tile corresponding to the given position.</returns>
    public Tile GetTile(TilePos position)
    {
        if (tiles.TryGetValue(position, out Tile tile))
        {
            return tile;
        }
        throw new ArgumentException();
    }

    public enum HighlightType
    {
        Selection,
        MoveTarget
    }

    public void HighlightTile(TilePos position, HighlightType highlightType)
    {

    }

    #endregion Public Methods
}
