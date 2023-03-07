using UnityEngine;
using LibraryPiece = ChessGameLibrary.Piece;
using ChessGameLibrary.Enums;

public class Piece : MonoBehaviour
{
    public GameObject PieceGameObject;
    public LibraryPiece LibraryPiece;

    public void Initialize(GameObject pieceGameObject, PieceType type, PieceColor color)
    {
        PieceGameObject = pieceGameObject;
        LibraryPiece = new LibraryPiece(type, color);
    }

    private void OnMouseDown()
    {
        Debug.Log($"click on {LibraryPiece}");
    }

    public override string ToString()
    {
        return LibraryPiece.ToString();
    }
}
