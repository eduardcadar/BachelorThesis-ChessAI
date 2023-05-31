using ChessGameLibrary;
using ChessGameLibrary.Enums;
using OctoChessEngine;
using OctoChessEngine.Domain;
using OctoChessEngine.Enums;
using System;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class MoveEvalDTO
    {
        public SquareCoords From { get; set; }
        public SquareCoords To { get; set; }
        public PieceType PromotedTo { get; set; }
        public double Evaluation { get; set; }

        public override string ToString()
        {
            return From
                + "-"
                + To
                + (PromotedTo != PieceType.NONE ? PromotedTo.ToString()[0] : "")
                + " "
                + Evaluation;
        }
    }

    public static class EngineUtils
    {
        public enum PlayerType
        {
            HUMAN,
            ENGINE
        }

        public static int Depth { get; set; } = 3;
        public static bool UseAlphaBetaPruning { get; set; } = true;
        public static EvaluationType EvalType { get; set; } = EvaluationType.MATERIAL;
        public static bool UseIterativeDeepening { get; set; } = false;
        public static int TimeLimit { get; set; } = 50;
        public static bool UseQuiescenceSearch { get; set; } = false;
        public static int MaxQuiescenceDepth { get; set; } = 4;

        private static readonly OctoChess _octoChess = new OctoChess();

        public static async Task<SimpleMove> GetEngineBestMove(string fen)
        {
            MoveEval moveEval = new MoveEval(new SquareCoords(0, 0), new SquareCoords(1, 1), 0, 1);
            try
            {
                await Task.Run(() => _octoChess.SetFenPosition(fen));
                moveEval = await Task.Run(
                    () =>
                        _octoChess.BestMove(
                            maxDepth: Depth,
                            useAlphaBetaPruning: UseAlphaBetaPruning,
                            evaluationType: EvalType,
                            useIterativeDeepening: UseIterativeDeepening,
                            timeLimit: TimeLimit,
                            useQuiescenceSearch: UseQuiescenceSearch,
                            maxQuiescenceDepth: MaxQuiescenceDepth
                        )
                );
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.Message);
            }

            return new SimpleMove(moveEval.From, moveEval.To, promotedTo: moveEval.PromotedTo);
        }
    }
}
