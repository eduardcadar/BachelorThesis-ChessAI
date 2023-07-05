using ChessGameLibrary;
using OctoChessEngine;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        //Evaluation evaluation = new Evaluation();
        //evaluation.Evaluate();
        //evaluation.ShowEvaluationResultsFromFiles();
        //return;

        Console.WriteLine("OctoChess");
        OctoChess engine = new OctoChess();
        Game game = new Game();

        string fen = Utils.STARTING_FEN;

        //string fen = "r1b3k1/pppp1rpp/1q2p3/4bp2/8/3BPQP1/PPPPRPKP/R1B5 w - - 11 20";
        game.SetPositionFromFEN(fen);

        //Stopwatch sw = new Stopwatch();
        //while (!game.IsOver)
        //{
        //sw.Start();
        // only material eval move
        Console.WriteLine(game.GetBoardPrintFormat());
        engine.ClearPreviousEvals();
        engine.SetFenPosition(game.GetBoardFEN());
        var bestMoveMinimax = engine.BestMove(
            maxDepth: 3,
            useAlphaBetaPruning: true,
            evaluationType: OctoChessEngine.Enums.EvaluationType.MATERIAL,
            useIterativeDeepening: true,
            timeLimit: 5,
            useQuiescenceSearch: true,
            maxQuiescenceDepth: 3
        );
        Console.WriteLine("Best move: " + bestMoveMinimax);
        game.Move(bestMoveMinimax.From, bestMoveMinimax.To, bestMoveMinimax.PromotedTo);

        //sw.Stop();
        //Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}");

        //if (game.IsOver)
        //    break;
        // nn eval move
        engine.ClearPreviousEvals();
        engine.SetFenPosition(game.GetBoardFEN());
        var bestMoveNn = engine.BestMove(
            maxDepth: 2,
            useAlphaBetaPruning: true,
            evaluationType: OctoChessEngine.Enums.EvaluationType.MATERIAL,
            useIterativeDeepening: false,
            timeLimit: 30,
            useQuiescenceSearch: true,
            maxQuiescenceDepth: 3
        );
        Console.WriteLine("Best move: " + bestMoveNn);
        game.Move(bestMoveNn.From, bestMoveNn.To, bestMoveNn.PromotedTo);

        //Console.Write("Write move: ");
        //string move = Console.ReadLine();
        //game.Move(move);
        //}
    }
}
