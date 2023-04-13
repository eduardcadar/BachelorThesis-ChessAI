using ChessGameLibrary;
using OctoChessEngine.Engines;
using System.Diagnostics;

Console.WriteLine("OctoChess");

OctoChess engine = new();
//string fen = "rnbqkbnr/P3pppp/2pp4/8/8/8/PP1PPPPP/RNBQKBNR w KQkq - 0 5";
//string fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
//string fen = "r2qkb1r/pp2nppp/3p4/2pNN1B1/2BnP3/3P4/PPP2PPP/R2bK2R w KQkq - 1 0"; // mate in 2
//string fen = "r2qkb1r/pp2np1p/3p1p2/2p1N1B1/2BnP3/3P4/PPP2PPP/R2bK2R w KQkq - 0 2"; // mate in 1
string fen = Utils.STARTING_FEN;
engine.SetFenPosition(fen);
Stopwatch sw = new();
sw.Start();
Console.WriteLine(engine.BestMove(depth: 3, alphaBetaPruning: true));
sw.Stop();
Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}");