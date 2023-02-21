using ChessGameLibrary.Enums;

namespace ChessGameLibrary
{
    public class Piece
    {
        public PieceType Type;
        public PieceColor Color;

        public Piece(PieceType type, PieceColor color) { Type = type; Color = color; }

        public override string ToString()
        {
            return (Color == PieceColor.WHITE
                ? Type.GetLetterName()
                : Type.GetLetterNameLower())
                .ToString();
        }
    }
}
