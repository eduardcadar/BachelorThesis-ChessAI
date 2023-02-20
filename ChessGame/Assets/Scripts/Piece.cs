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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override string ToString()
    {
        return LibraryPiece.ToString();
    }
}
