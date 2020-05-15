using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    #region Properties

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
    /// The radius of the board in world space
    /// </summary>
    public float BoundingRadius
    {
        get => boundingRadius;
        set => boundingRadius = value;
    }

    public struct Tile
    {
        // identifier for the tile
        public string id;

        // relative position of the tile
        public Vector3 position;

        // array of connections for this tile
        public string[] connections;

        // true if the tile is occupied
        public bool occupied;
    }

    private Dictionary<string, Tile> tiles;

    #endregion Properties

    #region MonoBehaviour Methods

    void Start()
    {
        tiles = new Dictionary<string, Tile>(25);
        char getBase12Char(int x)
        {
            while (x < 0)
            {
                x += 12;
            }
            while (x >= 12)
            {
                x -= 12;
            }
            return Convert.ToString(x, 16).Last();
        };

        // Centre orbit
        Tile centre = new Tile
        {
            id = "00",
            position = Vector3.zero,
            connections = Enumerable.Range(0, 12).Select(x => String.Format("1{0}", getBase12Char(x))).ToArray(),
            occupied = false
        };
        tiles.Add(centre.id, centre);

        // Outer orbits
        float angle = (Mathf.PI * 2) / 12;
        for (int i = 0; i < 12; i++)
        {
            Vector3 dir = new Vector3(Mathf.Cos(angle * i), 0, Mathf.Sin(angle * i));
            // Middle orbit
            Tile middle = new Tile
            {
                id = String.Format("1{0}", getBase12Char(i)),
                position = dir * innerRowRadius,
                connections = new string[]
                {
                    String.Format("1{0}", getBase12Char(i + 1)),
                    String.Format("1{0}", getBase12Char(i - 1)),
                    String.Format("2{0}", getBase12Char(i)),
                    "00"
                },
                occupied = false
            };
            tiles.Add(middle.id, middle);

            // Outermost orbit
            Tile outer = new Tile
            {
                id = String.Format("2{0}", getBase12Char(i)),
                position = dir * outerRowRadius,
                connections = new string[]
                {
                    String.Format("2{0}", getBase12Char(i + 1)),
                    String.Format("2{0}", getBase12Char(i - 1)),
                    String.Format("1{0}", getBase12Char(i))
                },
                occupied = false
            };
            tiles.Add(outer.id, outer);
        }
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

    public HashSet<Tile> GetMovableTiles(string currentId, int movement)
    {
        HashSet<Tile> movableTiles = new HashSet<Tile>();

        void depthFirstSearch(string id, int remainingMovement, List<string> path)
        {
            path.Add(id);

            if (remainingMovement <= 0)
            {
                movableTiles.Add(tiles[id]);
                return;
            }

            foreach (string possibleId in tiles[id].connections)
            {
                if (!tiles[possibleId].occupied && !path.Contains(possibleId))
                {
                    depthFirstSearch(possibleId, remainingMovement - 1, new List<string>(path));
                }
            }
        }

        depthFirstSearch(currentId, movement, new List<string>());

        return movableTiles;
    }

    public void SetTileOccupied(string id, bool occupied)
    {
        if (tiles.ContainsKey(id))
        {
            Tile tile = tiles[id];
            tile.occupied = occupied;
            tiles[id] = tile;
        }
    }

    #endregion Public Methods
}
