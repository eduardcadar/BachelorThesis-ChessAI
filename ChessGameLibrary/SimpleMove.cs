using ChessGameLibrary.Enums;

namespace ChessGameLibrary
{
    public class SimpleMove
    {
        public SquareCoords From { get; set; }
        public SquareCoords To { get; set; }
        public PieceType PromotedTo { get; set; }
        public PieceType PieceCaptured { get; set; }

        public SimpleMove(SquareCoords from, SquareCoords to, PieceType promotedTo = PieceType.NONE,
            PieceType pieceCaptured = PieceType.NONE)
        {
            From = from; To = to; PromotedTo = promotedTo; PieceCaptured = pieceCaptured;
        }

        public override string ToString()
        {
            return From.ToString() + To.ToString() +
                (PromotedTo == PieceType.NONE ? "" : PromotedTo.GetLetterNameLower().ToString());
        }
    }
}
