using ChessGameLibrary;
using ChessGameLibrary.Enums;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public Chessboard Chessboard;
    public int x;
    public int y;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnMouseDown()
    {
        Chessboard chessboard = transform.parent.GetComponent<Chessboard>();
        chessboard.RemoveThreatMap();
        if (chessboard.SelectedSquare != null)
        {
            var clickedSquare = this;
            var validMoveSq = clickedSquare.GetComponentsInChildren<Transform>()
                .SingleOrDefault(c => c.name.Contains("ValidMoveSquare"));
            // if click on validMoveSquare move the selected piece
            if (validMoveSq != null)
            {
                var fromTile = chessboard.SelectedSquare.transform.parent.ConvertTo<BoardTile>();
                MoveInfo moveInfo = chessboard.Game.Move(new SquareCoords(fromTile.x, fromTile.y), new SquareCoords(x, y));
                Piece pieceToCapture = clickedSquare.GetComponentInChildren<Piece>();
                if (pieceToCapture != null)
                    Destroy(pieceToCapture.PieceGameObject);
                else if (moveInfo.MoveType == MoveType.ENPASSANT)
                {
                    int direction = moveInfo.Piece.Color == PieceColor.WHITE ? 1 : -1;
                    Piece capturedPawn = chessboard.GetTile(moveInfo.To.File, moveInfo.To.Rank - direction)
                        .GetComponentInChildren<Piece>();
                    Destroy(capturedPawn.PieceGameObject);
                }

                Piece pieceToMove = chessboard.SelectedSquare.transform.parent.GetComponentInChildren<Piece>();
                pieceToMove.transform.parent = clickedSquare.transform;
                pieceToMove.transform.position = clickedSquare.transform.position;
                if (chessboard.Game.State != GameState.INPROGRESS)
                    Debug.Log(chessboard.Game.State);
            }
            //
            Destroy(chessboard.SelectedSquare);
            chessboard.SelectedSquare = null;
            foreach (var move in chessboard.ValidMoves)
                Destroy(move);
            chessboard.ValidMoves.Clear();
            return;
        }
        Piece piece = GetComponentInChildren<Piece>();
        if (piece == null || piece.LibraryPiece.Color != chessboard.Game.PlayerToMove)
            return;
        chessboard.SelectedSquare = Instantiate(chessboard.SelectedSquarePrefab, transform);
        chessboard.SelectedSquare.AddComponent<SpriteRenderer>().sortingLayerName = "Tiles";

        SquareCoords[] validMoveSquares = chessboard.Game.MoveSquares(new SquareCoords(x, y))
            .ToArray();
        foreach (SquareCoords sq in validMoveSquares)
        {
            GameObject validSquare = Instantiate(chessboard.ValidMoveSquarePrefab, chessboard.GetTile(sq.File, sq.Rank).transform);
            validSquare.AddComponent<SpriteRenderer>().sortingLayerName = "Tiles";
            chessboard.ValidMoves.Add(validSquare);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void InitializeTile(Chessboard chessboard, int x, int y)
    {
        Chessboard = chessboard;
        this.x = x;
        this.y = y;
    }
}
