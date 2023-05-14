using Assets.Scripts;
using ChessGameLibrary;
using ChessGameLibrary.Enums;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public Chessboard Chessboard;
    public int x;
    public int y;
    private static bool _finishedMove = true;

    private void Update()
    {
        if (Chessboard.Game.State == GameState.INPROGRESS)
        {
            if (_finishedMove)
            {
                switch (Chessboard.Game.PlayerToMove)
                {
                    case PieceColor.WHITE:
                        if (!Chessboard.IsPlayerWhiteHuman)
                            EngineMove();
                        break;
                    case PieceColor.BLACK:
                        if (!Chessboard.IsPlayerBlackHuman)
                            EngineMove();
                        break;
                }
            }
        }
    }

    private void HumanMove()
    {
        if (Chessboard.SelectedSquare == null)
        {
            Piece piece = GetComponentInChildren<Piece>();
            if (piece != null && piece.LibraryPiece.Color == Chessboard.Game.PlayerToMove)
                SelectPiece();
        }
        else
        {
            BoardTile clickedSquare = this;
            var validMoveSq = clickedSquare
                .GetComponentsInChildren<Transform>()
                .SingleOrDefault(c => c.name.Contains("ValidMoveSquare"));
            // if click on validMoveSquare move the selected piece
            if (validMoveSq != null)
            {
                _finishedMove = false;
                Debug.Log("HUMAN MOVE");
                MovePiece(clickedSquare);
            }
            if (_finishedMove)
            {
                DestroySelectedAndValidSquares();
            }
            return;
        }
    }

    async void EngineMove()
    {
        _finishedMove = false;
        await Task.Yield();
        //return new WaitForSecondsRealtime(Chessboard.TIME_BEFORE_ENGINE_MOVES);

        Debug.Log("engine searching for move...");
        SimpleMove move = await EngineUtils.GetEngineBestMove(
            fen: Chessboard.Game.GetBoardFEN(),
            depth: 3,
            useAlphaBetaPruning: true,
            useIterativeDeepening: false,
            timeLimit: 50,
            useQuiescenceSearch: false,
            maxQuiescenceDepth: 4,
            evaluationType: EngineUtils.EvaluationType.TRAINED_MODEL
        );

        //int i = Random.Range(0, Chessboard.Game.LegalMoves.Count);
        //SimpleMove move = Chessboard.Game.LegalMoves[i];
        Debug.Log("ENGINE MOVE: " + move.ToString());
        MoveInfo moveInfo = Chessboard.Game.Move(move.From, move.To, promotedTo: move.PromotedTo);
        MoveOnBoard(moveInfo);

        if (Chessboard.Game.State != GameState.INPROGRESS)
            Debug.Log(Chessboard.Game.State);
        _finishedMove = true;
    }

    private void MoveOnBoard(MoveInfo move)
    {
        Piece pieceToMove = Chessboard.GetTile(move.From).GetComponentInChildren<Piece>();
        Piece pieceToCapture = Chessboard.GetTile(move.To).GetComponentInChildren<Piece>();
        if (pieceToCapture != null)
            Destroy(pieceToCapture.PieceGameObject);
        else if (move.MoveType == MoveType.ENPASSANT)
        {
            int direction = move.Piece.Color == PieceColor.WHITE ? 1 : -1;
            Piece capturedPawn = Chessboard
                .GetTile(move.To.File, move.To.Rank - direction)
                .GetComponentInChildren<Piece>();
            Destroy(capturedPawn.PieceGameObject);
        }
        else if (move.MoveType == MoveType.CASTLE)
        {
            SimpleMove rookMove = Utils.GetRookCastleMove(move.To);
            Piece rookToMove = Chessboard.GetTile(rookMove.From).GetComponentInChildren<Piece>();
            BoardTile tileToMoveTo = Chessboard.GetTile(rookMove.To).GetComponent<BoardTile>();
            rookToMove.transform.parent = tileToMoveTo.transform;
            rookToMove.transform.position = tileToMoveTo.transform.position;
        }

        if (move.MoveType == MoveType.PROMOTION)
        {
            Chessboard.InstantiatePiece(
                move.PromotedTo,
                move.Piece.Color,
                move.To.File,
                move.To.Rank
            );
            Destroy(pieceToMove.PieceGameObject);
        }
        else
        {
            BoardTile boardTileTo = Chessboard.GetTile(move.To).GetComponent<BoardTile>();
            pieceToMove.transform.parent = boardTileTo.transform;
            pieceToMove.transform.position = boardTileTo.transform.position;
        }
    }

    private void OnMouseDown()
    {
        if (Chessboard.SelectPromotionPieceW.gameObject.activeSelf)
            return;
        Chessboard.RemoveThreatMap();
        if (Chessboard.Game.State != GameState.INPROGRESS)
            return;
        switch (Chessboard.Game.PlayerToMove)
        {
            case PieceColor.WHITE:
                if (Chessboard.IsPlayerWhiteHuman)
                    HumanMove();
                break;
            case PieceColor.BLACK:
                if (Chessboard.IsPlayerBlackHuman)
                    HumanMove();
                break;
        }
    }

    private void SelectPiece()
    {
        Chessboard.SelectedSquare = Instantiate(Chessboard.SelectedSquarePrefab, transform);
        Chessboard.SelectedSquare.AddComponent<SpriteRenderer>().sortingLayerName = "Tiles";

        SquareCoords[] validMoveSquares = Chessboard.Game
            .MoveSquares(new SquareCoords(x, y))
            .ToArray();
        foreach (SquareCoords sq in validMoveSquares)
        {
            GameObject validSquare = Instantiate(
                Chessboard.ValidMoveSquarePrefab,
                Chessboard.GetTile(sq.File, sq.Rank).transform
            );
            validSquare.AddComponent<SpriteRenderer>().sortingLayerName = "Tiles";
            Chessboard.ValidMoves.Add(validSquare);
        }
    }

    public void DestroySelectedAndValidSquares()
    {
        if (Chessboard.SelectedSquare == null)
            return;
        Destroy(Chessboard.SelectedSquare);
        Chessboard.SelectedSquare = null;
        foreach (var move in Chessboard.ValidMoves)
            Destroy(move);
        Chessboard.ValidMoves.Clear();
    }

    IEnumerator SelectPromotionPiece(BoardTile clickedSquare, Piece pieceToMove)
    {
        yield return new WaitWhile(
            () => Chessboard.SelectPromotionPieceW.PieceType == PieceType.NONE
        );
        Chessboard.SelectPromotionPieceW.gameObject.SetActive(false);
        Debug.Log("Promotion piece selected: " + Chessboard.SelectPromotionPieceW.PieceType);
        ContinueMove(clickedSquare, pieceToMove);
    }

    private IEnumerator PromotionMove(BoardTile clickedSquare, Piece pieceToMove)
    {
        // pop-up to select promotion piece
        //var prefab = switch color
        //var selectPiecePopup = Instantiate(chessboard.SelectPromotionPieceWPrefab, chessboard.transform);
        Chessboard.SelectPromotionPieceW.PieceType = PieceType.NONE;
        Chessboard.SelectPromotionPieceW.gameObject.SetActive(true);
        yield return StartCoroutine(SelectPromotionPiece(clickedSquare, pieceToMove));
    }

    private void ContinueMove(BoardTile clickedSquare, Piece pieceToMove)
    {
        PieceType promotedTo = Chessboard.SelectPromotionPieceW.PieceType;
        var fromTile = Chessboard.SelectedSquare.transform.parent.ConvertTo<BoardTile>();
        MoveInfo moveInfo = Chessboard.Game.Move(
            new SquareCoords(fromTile.x, fromTile.y),
            new SquareCoords(x, y),
            promotedTo
        );
        Piece pieceToCapture = clickedSquare.GetComponentInChildren<Piece>();
        if (pieceToCapture != null)
            Destroy(pieceToCapture.PieceGameObject);
        else if (moveInfo.MoveType == MoveType.ENPASSANT)
        {
            int direction = moveInfo.Piece.Color == PieceColor.WHITE ? 1 : -1;
            Piece capturedPawn = Chessboard
                .GetTile(moveInfo.To.File, moveInfo.To.Rank - direction)
                .GetComponentInChildren<Piece>();
            Destroy(capturedPawn.PieceGameObject);
        }
        else if (moveInfo.MoveType == MoveType.CASTLE)
        {
            SimpleMove rookMove = Utils.GetRookCastleMove(moveInfo.To);
            Piece rookToMove = Chessboard.GetTile(rookMove.From).GetComponentInChildren<Piece>();
            BoardTile tileToMoveTo = Chessboard.GetTile(rookMove.To).GetComponent<BoardTile>();
            rookToMove.transform.parent = tileToMoveTo.transform;
            rookToMove.transform.position = tileToMoveTo.transform.position;
        }

        if (moveInfo.MoveType == MoveType.PROMOTION)
        {
            Chessboard.InstantiatePiece(moveInfo.PromotedTo, moveInfo.Piece.Color, x, y);
            Destroy(pieceToMove.PieceGameObject);
        }
        else
        {
            pieceToMove.transform.parent = clickedSquare.transform;
            pieceToMove.transform.position = clickedSquare.transform.position;
        }
        _finishedMove = true;

        if (Chessboard.Game.State != GameState.INPROGRESS)
            Debug.Log(Chessboard.Game.State);
        DestroySelectedAndValidSquares();
        //switch (Chessboard.Game.PlayerToMove)
        //{
        //    case PieceColor.WHITE:
        //        if (!Chessboard.IsPlayerWhiteHuman) EngineMove();
        //        break;
        //    case PieceColor.BLACK:
        //        if (!Chessboard.IsPlayerBlackHuman) EngineMove();
        //        break;
        //}
    }

    private void MovePiece(BoardTile clickedSquare)
    {
        Piece pieceToMove =
            Chessboard.SelectedSquare.transform.parent.GetComponentInChildren<Piece>();
        int lastPawnRank = pieceToMove.LibraryPiece.Color == PieceColor.WHITE ? 7 : 0;
        if (pieceToMove.LibraryPiece.Type == PieceType.PAWN && clickedSquare.y == lastPawnRank)
        {
            _finishedMove = false;
            StartCoroutine(PromotionMove(clickedSquare, pieceToMove));
            return;
        }
        ContinueMove(clickedSquare, pieceToMove);
    }

    public void InitializeTile(Chessboard chessboard, int x, int y)
    {
        Chessboard = chessboard;
        this.x = x;
        this.y = y;
    }
}
