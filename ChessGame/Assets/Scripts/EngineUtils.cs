using ChessGameLibrary;
using OctoChessEngine;
using OctoChessEngine.Domain;
using OctoChessEngine.Enums;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class EngineUtils
    {
        public static int Depth { get; set; } = 3;
        public static bool UseAlphaBetaPruning { get; set; } = true;
        public static EvaluationType EvalType { get; set; } = EvaluationType.MATERIAL;
        public static bool UseIterativeDeepening { get; set; } = true;
        public static int TimeLimit { get; set; } = 10;
        public static bool UseQuiescenceSearch { get; set; } = true;
        public static int MaxQuiescenceDepth { get; set; } = 4;

        private static readonly OctoChess _octoChess = new OctoChess();

        public static async Task<SimpleMove> GetEngineBestMove(string fen)
        {
            await Task.Run(() => _octoChess.SetFenPosition(fen));
            MoveEval moveEval = await Task.Run(
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
            return new SimpleMove(moveEval.From, moveEval.To, promotedTo: moveEval.PromotedTo);
        }
    }
}
