using System;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

/// <summary>
/// Polar coordinates of a tile on the board.
/// </summary>
public struct TilePos : IEquatable<TilePos>
{
    /// <summary>
    /// Origin of the coordinates, which coincides with the center tile of the board.
    /// </summary>
    public static readonly TilePos Zero = new TilePos(0, 0);

    /// <summary>
    /// Tile row in [0:2], where 0 is the center tile.
    /// </summary>
    public int row;

    /// <summary>
    /// Tile sector in [0:11]. The center tile has always sector zero.
    /// </summary>
    public int sector;

    public TilePos(int row, int sector)
    {
        this.row = Mathf.Clamp(row, 0, 2);
        this.sector = Mathf.Clamp(sector, 0, 11);
        if (row == 0)
        {
            this.sector = 0;
        }
    }

    public bool Equals(TilePos other)
    {
        return (other.row == row) && (other.sector == sector);
    }

    public static bool operator==(TilePos lhs, TilePos rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator!=(TilePos lhs, TilePos rhs)
    {
        return !lhs.Equals(rhs);
    }

    public override bool Equals(object other)
    {
        if (other == null)
        {
            return false;
        }
        if (other is TilePos tilePos)
        {
            return Equals(tilePos);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"r={row} s={sector}";
    }
}
