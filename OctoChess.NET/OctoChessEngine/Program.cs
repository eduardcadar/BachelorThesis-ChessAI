using ChessGameLibrary;
using OctoChessEngine;
using System;

//string fen = "rnbqkbnr/P3pppp/2pp4/8/8/8/PP1PPPPP/RNBQKBNR w KQkq - 0 5";
//string fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
//string fen = "r2qkb1r/pp2nppp/3p4/2pNN1B1/2BnP3/3P4/PPP2PPP/R2bK2R w KQkq - 1 0"; // mate in 2
//string fen = "r2qkb1r/pp2np1p/3p1p2/2p1N1B1/2BnP3/3P4/PPP2PPP/R2bK2R w KQkq - 0 2"; // mate in 1

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
