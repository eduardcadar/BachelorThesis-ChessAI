using ChessGameLibrary;
using ChessGameLibrary.Enums;
using OctoChessEngine.Domain;

namespace OctoChessAPI.DTO
{
    public record MoveEvalDTO
    {
        public SquareCoords From { get; set; }
        public SquareCoords To { get; set; }
        public PieceType PromotedTo { get; set; }
        public double Evaluation { get; set; }

        public MoveEvalDTO(MoveEval moveEval)
        {
            From = moveEval.From;
            To = moveEval.To;
            PromotedTo = moveEval.PromotedTo;
            Evaluation = moveEval.Evaluation;
        }
    }
}
