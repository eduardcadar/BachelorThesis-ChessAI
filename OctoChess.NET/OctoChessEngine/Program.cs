using ChessGameLibrary;
using OctoChessEngine;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

//string fen = "rnbqkbnr/P3pppp/2pp4/8/8/8/PP1PPPPP/RNBQKBNR w KQkq - 0 5";
//string fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
//string fen = "r2qkb1r/pp2nppp/3p4/2pNN1B1/2BnP3/3P4/PPP2PPP/R2bK2R w KQkq - 1 0"; // mate in 2
//string fen = "r2qkb1r/pp2np1p/3p1p2/2p1N1B1/2BnP3/3P4/PPP2PPP/R2bK2R w KQkq - 0 2"; // mate in 1

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("OctoChess");
        OctoChess engine = new OctoChess();
        Stopwatch sw = new Stopwatch();
        Game game = new Game();

        string fen = Utils.STARTING_FEN;

        //string fen = "r2q1rk1/ppp1bpp1/3p1n1p/4p3/2BnP1bB/2NP1N2/PPP2PPP/R2QR1K1 w - - 2 10";
        game.SetPositionFromFEN(fen);

        while (!game.IsOver)
        {
            sw.Start();

            Console.WriteLine(game.GetBoardPrintFormat());
            engine.SetFenPosition(game.GetBoardFEN());
            var bestMove = await engine.BestMove(
                maxDepth: 1,
                useAlphaBetaPruning: true,
                evaluationType: OctoChessEngine.Enums.EvaluationType.TRAINED_MODEL,
                useIterativeDeepening: true,
                timeLimit: 30,
                useQuiescenceSearch: true,
                maxQuiescenceDepth: 3
            );
            Console.WriteLine(bestMove);
            game.Move(bestMove.From, bestMove.To, bestMove.PromotedTo);

            sw.Stop();
            Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}");

            Console.Write("Write move: ");
            string move = Console.ReadLine();
            game.Move(move);
        }
    }
}
