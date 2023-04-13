using ChessGameLibrary;
using OctoChessEngine.Domain;
using OctoChessEngine.Enums;
using Stockfish.NET;

namespace OctoChessEngine.Engines
{
    public class StockfishEngine
    {
        private readonly IStockfish _stockfish;
        private const string STOCKFISH_PATH = @"C:\facultate\BachelorThesis-ChessAI\OctoChess.NET\stockfish\stockfish-windows-2022-x86-64-avx2.exe";

        public StockfishEngine()
        {
            _stockfish = new Stockfish.NET.Stockfish(STOCKFISH_PATH);
        }

        public void SetFenPosition(string fen)
            => _stockfish.SetFenPosition(fen);

        public double StaticEvaluatePosition(Board? board = null, EvaluationType type = EvaluationType.MATERIAL)
        {
            throw new NotImplementedException();
        }

        public MoveEval BestMove(int depth = 3)
        {
            //string bestMove = _stockfish.GetBestMove();
            //SquareCoords from = new(bestMove[..2]);
            //SquareCoords to = new(bestMove[2..]);
            //return new MoveEval(from, to);
            throw new NotImplementedException();
        }
    }
}
