using ChessGameLibrary.Enums;

namespace ChessGameLibrary
{
    public class Square
    {
        public SquareCoords SquareCoords;
        public Piece Piece;
        public bool IsAttackedByWhite;
        public bool IsAttackedByBlack;

        public Square(SquareCoords squareCoords, Piece piece)
        {
            SquareCoords = squareCoords;
            Piece = piece;
            IsAttackedByWhite = false;
            IsAttackedByBlack = false;
        }

        public bool IsAttackedBy(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.NONE)
                return false;
            return pieceColor == PieceColor.WHITE ? IsAttackedByWhite : IsAttackedByBlack;
        }

        public override string ToString()
        {
            return Piece == null ? "-" : Piece.ToString();
        }
    }
}
