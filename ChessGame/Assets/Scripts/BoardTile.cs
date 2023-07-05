using Assets.Scripts;
using ChessGameLibrary;
using ChessGameLibrary.Enums;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoardTile : MonoBehaviour
{
    public Chessboard Chessboard;
    public int x;
    public int y;
    private static bool _finishedMove = true;

    private void Update()
    {
        if (
            Chessboard.PlaysAgainstEngine
            && Chessboard.Game.State == GameState.INPROGRESS
            && _finishedMove
        )
        {
            switch (Chessboard.Game.PlayerToMove)
            {
                case PieceColor.WHITE:
                    if (!Chessboard.PlaysAsWhite)
                        EngineMove();
                    break;
                case PieceColor.BLACK:
                    if (Chessboard.PlaysAsWhite)
                        EngineMove();
                    break;
            }
        }
    }

    private void StartedSearching()
    {
        Chessboard.EngineIsSearchingText.SetActive(true);
        Chessboard.ButtonSetEngineOptions.GetComponent<Button>().interactable = false;
        Chessboard.ButtonRestartGame.GetComponent<Button>().interactable = false;
        Chessboard.ButtonSetFen.GetComponent<Button>().interactable = false;
        Chessboard.ButtonPlayAsWhite.GetComponent<Button>().interactable = false;
        Chessboard.ButtonPlayAsBlack.GetComponent<Button>().interactable = false;
        Chessboard.ButtonVsPlayer.GetComponent<Button>().interactable = false;
        Chessboard.ButtonVsEngine.GetComponent<Button>().interactable = false;
    }

    private void FinishedSearching()
    {
        Chessboard.EngineIsSearchingText.SetActive(false);
        Chessboard.ButtonSetEngineOptions.GetComponent<Button>().interactable = true;
        Chessboard.ButtonRestartGame.GetComponent<Button>().interactable = true;
        Chessboard.ButtonSetFen.GetComponent<Button>().interactable = true;
        Chessboard.ButtonPlayAsWhite.GetComponent<Button>().interactable = true;
        Chessboard.ButtonPlayAsBlack.GetComponent<Button>().interactable = true;
        Chessboard.ButtonVsPlayer.GetComponent<Button>().interactable = true;
        Chessboard.ButtonVsEngine.GetComponent<Button>().interactable = true;
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
                Chessboard.UpdateFenInputField();
            }
        }
    }

    private void UpdateLastMoveSquares(SquareCoords from, SquareCoords to)
    {
        if (Chessboard.LastMoveFromSquare != null)
            Destroy(Chessboard.LastMoveFromSquare);
        Chessboard.LastMoveFromSquare = Instantiate(
            Chessboard.LastMoveSquarePrefab,
            Chessboard.GetTile(from).transform
        );
        Chessboard.LastMoveFromSquare.AddComponent<SpriteRenderer>().sortingLayerName = "Tiles";

        if (Chessboard.LastMoveToSquare != null)
            Destroy(Chessboard.LastMoveToSquare);
        Chessboard.LastMoveToSquare = Instantiate(
            Chessboard.LastMoveSquarePrefab,
            Chessboard.GetTile(to).transform
        );
        Chessboard.LastMoveToSquare.AddComponent<SpriteRenderer>().sortingLayerName = "Tiles";
    }

    async void EngineMove()
    {
        StartedSearching();
        _finishedMove = false;
        await Task.Yield();
        //return new WaitForSecondsRealtime(Chessboard.TIME_BEFORE_ENGINE_MOVES);

        Debug.Log("engine searching for move...");
        try
        {
            SimpleMove move = await EngineUtils.GetEngineBestMove(
                fen: Chessboard.Game.GetBoardFEN()
            );

            Debug.Log("ENGINE MOVE: " + move.ToString());
            MoveInfo moveInfo = Chessboard.Game.Move(
                move.From,
                move.To,
                promotedTo: move.PromotedTo
            );
            MoveOnBoard(moveInfo);

            if (Chessboard.Game.State != GameState.INPROGRESS)
                Debug.Log(Chessboard.Game.State);
            _finishedMove = true;
            FinishedSearching();
            UpdateLastMoveSquares(move.From, move.To);
            Chessboard.UpdateFenInputField();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
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
        Chessboard.UpdatePlayerToMoveText();
        Chessboard.UpdateGameStateText();
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
                if (!Chessboard.PlaysAgainstEngine || Chessboard.PlaysAsWhite)
                    HumanMove();
                break;
            case PieceColor.BLACK:
                if (!Chessboard.PlaysAgainstEngine || !Chessboard.PlaysAsWhite)
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

        UpdateLastMoveSquares(moveInfo.From, moveInfo.To);

        if (Chessboard.Game.State != GameState.INPROGRESS)
            Debug.Log(Chessboard.Game.State);
        DestroySelectedAndValidSquares();
        Chessboard.UpdatePlayerToMoveText();
        Chessboard.UpdateGameStateText();
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
