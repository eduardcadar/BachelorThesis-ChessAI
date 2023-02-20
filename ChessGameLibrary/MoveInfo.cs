using ChessGameLibrary.Enums;

namespace ChessGameLibrary
{
    public class MoveInfo
    {
        public MoveType MoveType { get; set; }
        public Piece Piece { get; set; }
        public PieceType PromotedTo { get; set; }
        public Piece CapturedPiece { get; set; }
        public SquareCoords From { get; set; }
        public SquareCoords To { get; set; }
        public SquareCoords PrevEnPassantSquare { get; set; }

        public MoveInfo(MoveType moveType, Piece piece, SquareCoords from, SquareCoords to)
        {
            MoveType = moveType;
            Piece = piece;
            From = from;
            To = to;
            PromotedTo = PieceType.NONE;
            CapturedPiece = null;
            PrevEnPassantSquare = null;
        }
    }
}
