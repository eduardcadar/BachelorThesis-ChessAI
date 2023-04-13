using ChessGameLibrary;
using ChessGameLibrary.Enums;

namespace MachineLearning.PGN
{
    public class PGNMove
    {
        public PieceType PieceType { get; set; }
        public SquareCoords? From { get; set; }
        public SquareCoords? To { get; set; }
        public bool IsCastle { get; set; }
        public bool IsCapture { get; set; }
        public bool IsPromotion { get; set; }
        public PieceType PromotedTo { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckMate { get; set; }

        public PGNMove()
        {
            PieceType = PromotedTo = PieceType.NONE;
            IsCastle = false;
        }

        public SimpleMove ToSimpleMove() => new(From, To, PromotedTo);

        public override string ToString()
        {
            return PieceType.GetLetterName() + From.ToString() + To.ToString();
        }
    }
}
