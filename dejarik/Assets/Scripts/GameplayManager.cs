
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public enum PieceType
{
    Ghhhk,
    Grimtaash,
    Houjix,
    KLorSlug,
    KintanStrider,
    MantellianSavrip,
    Monnok,
    NgOk
}

public class Player
{
    // TODO - make private
    public int id;
}

public class Piece
{
    // TODO - make private
    public int id;
    public Player owner;
    public PieceType type;
    public int row;
    public int sector;
    public bool IsSelected = false;
}

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance = null; //singleton instance

    // TODO - Make private, assign when instantiated after game start
    public Board board = null;
    public PhotonView photonView = null;

    private int nextFreePieceId = 0;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    private Dictionary<int, Piece> pieces = new Dictionary<int, Piece>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

#if DEBUG
        DebugMenu.instance.AddAction("new Ghhhk(1,0)", () => PlacePiece(new Player { id = 42 }, PieceType.Ghhhk, 1, 0));
#endif
    }

    private void OnEnable()
    {
        Debug.Assert(board != null);
        photonView = PhotonView.Get(this);
        Debug.Assert(photonView != null);
    }

    private Piece PlacePieceImpl(Player player, int pieceId, PieceType type, int row, int sector)
    {
        Board.Tile tile = board.GetTile(row, sector);
        if (tile.occupied)
        {
            return null;
        }
        if (pieceId < 0)
        {
            // Local piece, pick new ID
            // FIXME - This doesn't work; both players will have a different counter
            pieceId = nextFreePieceId++;
        }
        var piece = new Piece { id = pieceId, owner = player, type = type, row = row, sector = sector };
        pieces.Add(pieceId, piece);
        board.SetTileOccupied($"{row}{sector:X}", true);

        // Instantiate game object for piece
        GameObject pieceGO = CharacterManager.instance.Instantiate(type);
        pieceGO.transform.position = board.GetPosition(row, sector);

        return piece;
    }

    private void SelectPieceImpl(Player player, Piece piece)
    {
        Debug.Assert(player == piece.owner);
        piece.IsSelected = true;
        board.HighlightTile(piece.row, piece.sector);
    }

    [PunRPC]
    private void PlacePieceRPC(int playerId, int pieceId, PieceType type, int row, int sector)
    {
        // Other player placed a piece, update the board
        if (!players.TryGetValue(playerId, out Player player))
        {
            Debug.LogError($"Unknown player ID {playerId}.");
            return;
        }
        PlacePieceImpl(player, pieceId, type, row, sector);
    }

    [PunRPC]
    private void SelectPieceRPC(int playerId, int pieceId)
    {
        // Other player selected a piece, update the board
        if (!players.TryGetValue(playerId, out Player player))
        {
            Debug.LogError($"Unknown player ID {playerId}.");
            return;
        }
        if (!pieces.TryGetValue(pieceId, out Piece piece))
        {
            Debug.LogError($"Unknown piece ID {pieceId}.");
            return;
        }
        SelectPieceImpl(player, piece);
    }

    /// <summary>
    /// Place a new piece on the board for the first time.
    /// </summary>
    /// <param name="player">The player owning the piece</param>
    /// <param name="type">The type of piece to place</param>
    /// <param name="row">The row where to place the piece</param>
    /// <param name="sector">The sector where to place the piece</param>
    /// <returns>The newly created piece.</returns>
    public Piece PlacePiece(Player player, PieceType type, int row, int sector)
    {
        Piece piece = PlacePieceImpl(player, -1, type, row, sector);
        if (piece != null)
        {
            // Update remote player
            photonView.RPC("PlacePieceRPC", RpcTarget.Others, player.id, piece.id, piece.type, piece.row, piece.sector);
        }
        return piece;
    }

    /// <summary>
    /// Select a piece to act on it (move or attack).
    /// </summary>
    /// <param name="player">The player selecting the piece</param>
    /// <param name="piece">The piece to select</param>
    public void SelectPiece(Player player, Piece piece)
    {
        SelectPieceImpl(player, piece);

        // Update remote player
        photonView.RPC("SelectPieceRPC", RpcTarget.Others, player.id, piece.id);
    }
}
