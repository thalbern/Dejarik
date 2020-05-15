using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    #region Properties

    [SerializeField]
    [Tooltip("The radius of the board in world space")]
    private float radius = 10;

    /// <summary>
    /// The radius of the board in world space
    /// </summary>
    public float Radius
    {
        get => radius;
        set => radius = value;
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
                position = dir * (radius / 2),
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
                id = String.Format("2{0}",getBase12Char(i)),
                position = dir * radius,
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

    // Update is called once per frame
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

#endregion MonoBehaviour Methods

#region Public Methods

    public List<Tile> GetMovableTiles(string currentId, int movement)
    {
        List<Tile> movableTiles = new List<Tile>();

        //TBI

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
