using ChessGameLibrary.Enums;
using UnityEngine;

public class WhitePromotionPieceScript : MonoBehaviour
{
    [SerializeField] private Chessboard chessboard;
    public PieceType PieceType { get; set; }

    public void QueenClicked() { PieceType = PieceType.QUEEN; }

    public void RookClicked() { PieceType = PieceType.ROOK; }

    public void BishopClicked() { PieceType = PieceType.BISHOP; }

    public void KnightClicked() { PieceType = PieceType.KNIGHT; }
}
