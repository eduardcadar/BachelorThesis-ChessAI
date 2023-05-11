using ChessGameLibrary;
using ChessGameLibrary.Enums;
using System.Text;

namespace OctoChessEngine.Domain
{
    public class MoveEval : IComparable<MoveEval>
    {
        public SquareCoords From { get; set; }
        public SquareCoords To { get; set; }
        public PieceType PromotedTo { get; set; }
        public double Evaluation { get; set; }
        public int MoveNumber { get; set; }

        public MoveEval(
            SquareCoords from,
            SquareCoords to,
            double evaluation,
            int moveNumber,
            PieceType promotedTo = PieceType.NONE
        )
        {
            From = from;
            To = to;
            PromotedTo = promotedTo;
            MoveNumber = moveNumber;
            Evaluation = evaluation;
        }

        public int CompareTo(MoveEval? other)
        {
            return Evaluation.CompareTo(other.Evaluation);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("Move ").Append(MoveNumber).Append(' ').Append(From).Append('-').Append(To);
            if (PromotedTo != PieceType.NONE)
                sb.Append(PromotedTo);
            if (Evaluation == EngineUtils.CHECKMATE_VALUE)
                sb.Append(" WHITE FORCED_MATE");
            else if (Evaluation == EngineUtils.CHECKMATE_VALUE * (-1))
                sb.Append(" BLACK FORCED MATE");
            else
                sb.Append(' ').Append(Evaluation / 100);
            return sb.ToString();
        }
    }
}
