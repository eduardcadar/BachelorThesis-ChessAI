using ChessGameLibrary;
using ChessGameLibrary.Enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Assets.Scripts.EngineUtils;

public class Chessboard : MonoBehaviour
{
    [Header("Style")]
    [SerializeField]
    private Material tileMaterial;

    [SerializeField]
    private Material selectedMaterial;
    public GameObject SelectedSquarePrefab;
    public GameObject ValidMoveSquarePrefab;
    public GameObject ThreatMapSquarePrefab;
    public WhitePromotionPieceScript SelectPromotionPieceW;

    [SerializeField]
    private GameObject PawnW;

    [SerializeField]
    private GameObject KnightW;

    [SerializeField]
    private GameObject PawnB;

    [SerializeField]
    private GameObject KnightB;

    [SerializeField]
    private GameObject BishopB;

    [SerializeField]
    private GameObject BishopW;

    [SerializeField]
    private GameObject RookB;

    [SerializeField]
    private GameObject RookW;

    [SerializeField]
    private GameObject QueenB;

    [SerializeField]
    private GameObject QueenW;

    [SerializeField]
    private GameObject KingB;

    [SerializeField]
    private GameObject KingW;

    public GameObject PlayerToMoveText;

    public Game Game;
    public bool IsPlayerWhiteHuman { get; set; } = true;
    public bool IsPlayerBlackHuman { get; set; } = true;
    public GameObject SelectedSquare { get; set; } = null;
    public List<GameObject> ValidMoves = new();
    public List<GameObject> ThreatMap = new();
    public bool ShowWhiteThreatMap = false;
    public bool ShowBlackThreatMap = false;

    public void SetPlayerTypes(PlayerType white, PlayerType black)
    {
        IsPlayerWhiteHuman = (white == PlayerType.HUMAN);
        IsPlayerBlackHuman = (black == PlayerType.HUMAN);
    }

    // LOGIC
    private static readonly string startingFEN =
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private float TILE_SIZE;
    public const float TIME_BEFORE_ENGINE_MOVES = 0.5f; // in seconds
    private GameObject[,] tiles;

    private void Awake()
    {
        selectedMaterial.color = Color.red;
        GenerateAllTiles(TILE_COUNT_X, TILE_COUNT_Y);
        //int ok = GenerateClassicPieces();
        //string FEN = "rnbqkbnr/P3pppp/2pp4/8/8/8/PP1PPPPP/RNBQKBNR w KQkq - 0 5";
        InitializeBoard();
    }

    public void InitializeBoard()
    {
        ClearBoardOfPieces();
        string FEN = Utils.STARTING_FEN;
        Game = new();
        //Game.SetPlayerTypes(PlayerType.HUMAN, PlayerType.STOCKFISH);
        Game.SetPositionFromFEN(FEN);
        PlayerToMoveText.GetComponent<TextMeshProUGUI>().text =
            Game.PlayerToMove.ToString() + " to move";
        int ok = GeneratePiecesFromFEN(FEN);
        if (ok < 0)
        {
            //error
        }
    }

    public void RemoveThreatMap()
    {
        ShowWhiteThreatMap = ShowBlackThreatMap = false;
        foreach (GameObject g in ThreatMap)
            Destroy(g);
        ThreatMap.Clear();
    }

    public void RefreshThreatMap()
    {
        foreach (GameObject g in ThreatMap)
            Destroy(g);
        ThreatMap.Clear();
        PieceColor color = PieceColor.NONE;
        if (ShowWhiteThreatMap)
            color = PieceColor.WHITE;
        if (ShowBlackThreatMap)
            color = PieceColor.BLACK;
        if (color == PieceColor.NONE)
            return;
        foreach (Square sq in Game.Board.Squares)
        {
            if (color == PieceColor.WHITE ? sq.IsAttackedByWhite : sq.IsAttackedByBlack)
            {
                int x = sq.SquareCoords.File,
                    y = sq.SquareCoords.Rank;
                ThreatMap.Add(Instantiate(ThreatMapSquarePrefab, tiles[x, y].transform));
            }
        }
    }

    public GameObject GetTile(int file, int rank)
    {
        return tiles[file, rank];
    }

    public GameObject GetTile(SquareCoords sq)
    {
        return tiles[sq.File, sq.Rank];
    }

    private void ClearBoardOfPieces()
    {
        foreach (GameObject tile in tiles)
        {
            Piece piece = tile.GetComponentInChildren<Piece>();
            if (piece != null)
                Destroy(piece.PieceGameObject);
        }
    }

    private int GeneratePiecesFromFEN(string FEN)
    {
        string[] fields = FEN.Split(' ');
        int rank = 7,
            file = 0;
        foreach (char letter in fields[0])
        {
            switch (letter)
            {
                case 'p':
                    InstantiatePiece(PieceType.PAWN, PieceColor.BLACK, file, rank);
                    break;
                case 'n':
                    InstantiatePiece(PieceType.KNIGHT, PieceColor.BLACK, file, rank);
                    break;
                case 'b':
                    InstantiatePiece(PieceType.BISHOP, PieceColor.BLACK, file, rank);
                    break;
                case 'r':
                    InstantiatePiece(PieceType.ROOK, PieceColor.BLACK, file, rank);
                    break;
                case 'q':
                    InstantiatePiece(PieceType.QUEEN, PieceColor.BLACK, file, rank);
                    break;
                case 'k':
                    InstantiatePiece(PieceType.KING, PieceColor.BLACK, file, rank);
                    break;
                case 'P':
                    InstantiatePiece(PieceType.PAWN, PieceColor.WHITE, file, rank);
                    break;
                case 'N':
                    InstantiatePiece(PieceType.KNIGHT, PieceColor.WHITE, file, rank);
                    break;
                case 'B':
                    InstantiatePiece(PieceType.BISHOP, PieceColor.WHITE, file, rank);
                    break;
                case 'R':
                    InstantiatePiece(PieceType.ROOK, PieceColor.WHITE, file, rank);
                    break;
                case 'Q':
                    InstantiatePiece(PieceType.QUEEN, PieceColor.WHITE, file, rank);
                    break;
                case 'K':
                    InstantiatePiece(PieceType.KING, PieceColor.WHITE, file, rank);
                    break;
                case '/':
                    rank--;
                    file = -1;
                    break;
                case '1':
                    break;
                case '2':
                    file += 1;
                    break;
                case '3':
                    file += 2;
                    break;
                case '4':
                    file += 3;
                    break;
                case '5':
                    file += 4;
                    break;
                case '6':
                    file += 5;
                    break;
                case '7':
                    file += 6;
                    break;
                case '8':
                    file += 7;
                    break;
                default:
                    return -1;
            }
            file++;
        }
        return 0;
    }

    private int GenerateClassicPieces()
    {
        return GeneratePiecesFromFEN(startingFEN);
    }

    public void InstantiatePiece(PieceType pieceType, PieceColor color, int x, int y)
    {
        if (x < 0 || x >= TILE_COUNT_X || y < 0 || y >= TILE_COUNT_Y)
            return;
        GameObject tileObject = tiles[x, y];
        GameObject pieceGameObject;
        if (color == PieceColor.WHITE)
        {
            pieceGameObject = pieceType switch
            {
                PieceType.PAWN => Instantiate(PawnW, tileObject.transform),
                PieceType.KNIGHT => Instantiate(KnightW, tileObject.transform),
                PieceType.BISHOP => Instantiate(BishopW, tileObject.transform),
                PieceType.ROOK => Instantiate(RookW, tileObject.transform),
                PieceType.QUEEN => Instantiate(QueenW, tileObject.transform),
                PieceType.KING => Instantiate(KingW, tileObject.transform),
                _ => null,
            };
        }
        else
        {
            pieceGameObject = pieceType switch
            {
                PieceType.PAWN => Instantiate(PawnB, tileObject.transform),
                PieceType.KNIGHT => Instantiate(KnightB, tileObject.transform),
                PieceType.BISHOP => Instantiate(BishopB, tileObject.transform),
                PieceType.ROOK => Instantiate(RookB, tileObject.transform),
                PieceType.QUEEN => Instantiate(QueenB, tileObject.transform),
                PieceType.KING => Instantiate(KingB, tileObject.transform),
                _ => null,
            };
        }
        if (pieceGameObject)
        {
            Piece piece = pieceGameObject.AddComponent<Piece>();
            pieceGameObject.AddComponent<SpriteRenderer>().sortingLayerName = "Pieces";
            piece.Initialize(pieceGameObject, pieceType, color);
            pieceGameObject.transform.parent = tileObject.transform;
            //pieceGameObject.transform.localScale = new Vector3(TILE_SIZE, TILE_SIZE);
        }
    }

    private void OnMouseDown() { }

    private void Update() { }

    // Generate the board
    private void GenerateAllTiles(int tileCountX, int tileCountY)
    {
        TILE_SIZE = 0.0623f;
        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(TILE_SIZE, x, y);
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileGameObject = new($"File:{x}, Rank:{y}");
        tileGameObject.transform.localScale = new Vector3(tileSize, tileSize);
        BoardTile tileObject = tileGameObject.AddComponent<BoardTile>();

        tileObject.InitializeTile(this, x, y);
        tileObject.transform.position = new Vector3(
            -4 * tileSize + x * tileSize + tileSize / 2f,
            -4 * tileSize + y * tileSize + tileSize / 2f,
            0
        );
        tileObject.transform.parent = transform;

        Mesh mesh = new();

        //tileGameObject.AddComponent<MeshFilter>().mesh = mesh;
        //tileGameObject.AddComponent<MeshRenderer>().material = tileMaterial;
        tileGameObject.AddComponent<SpriteRenderer>();
        SpriteRenderer spR = tileGameObject.GetComponent<SpriteRenderer>();
        if (spR)
        {
            spR.material = tileMaterial;
            spR.sortingLayerName = "Tiles";
        }

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, y * tileSize, 0.1f);
        vertices[1] = new Vector3(x * tileSize, (y + 1) * tileSize, 0.1f);
        vertices[2] = new Vector3((x + 1) * tileSize, y * tileSize, 0.1f);
        vertices[3] = new Vector3((x + 1) * tileSize, (y + 1) * tileSize, 0.1f);

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = tris;

        tileGameObject.layer = LayerMask.NameToLayer("Tile");
        tileGameObject.AddComponent<BoxCollider>();

        return tileGameObject;
    }
}
