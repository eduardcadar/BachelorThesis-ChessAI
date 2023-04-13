using ChessGameLibrary;
using ChessGameLibrary.Enums;
using OctoChessEngine.Domain;
using OctoChessEngine.Enums;

namespace OctoChessEngine.Engines
{
    public class OctoChess
    {
        private readonly Game _game;
        private readonly List<MoveEval> _moveEvals;
        private GamePhase _gamePhase;
        public List<SimpleMove> LegalMoves { get { return _game.LegalMoves; } }

        public OctoChess()
        {
            _game = new();
            _moveEvals = new();
        }

        public void SetFenPosition(string fen)
        {
            _game.SetPositionFromFEN(fen);
        }

        public int[] Perft(int depth = 0)
        {
            int len = depth + 1;
            int[] noPositions = new int[len];
            for (int i = 0; i < len; i++)
                noPositions[i] = 0;
            noPositions[0] = _game.LegalMoves.Count;
            if (depth > 0)
            {
                SimpleMove[] moves = _game.LegalMoves.ToArray();
                foreach (SimpleMove move in moves)
                {
                    _game.Move(move.From, move.To, move.PromotedTo);
                    noPositions[1] += _game.LegalMoves.Count;
                    int noMoves = 0;
                    if (depth > 1)
                        noMoves += NoPositionsRecursive(noPositions, 2, depth);
                    if (depth == 1)
                        noMoves += _game.LegalMoves.Count;
                    Console.WriteLine($"{move}: {noMoves}");
                    _game.UndoMove();
                }
            }

            return noPositions;
        }

        public int NoPositionsRecursive(int[] noPositions, int currentDepth, int depth)
        {
            int noMoves = 0;
            SimpleMove[] moves = _game.LegalMoves.ToArray();
            foreach (SimpleMove move in moves)
            {
                _game.Move(move.From, move.To, move.PromotedTo);
                noPositions[currentDepth] += _game.LegalMoves.Count;
                if (currentDepth == depth)
                    noMoves += _game.LegalMoves.Count;
                if (currentDepth < depth)
                    noMoves += NoPositionsRecursive(noPositions, currentDepth + 1, depth);
                _game.UndoMove();
            }
            return noMoves;
        }

        public async Task<MoveEval> BestMove(int depth = 3, bool alphaBetaPruning = true,
            CancellationToken cancellationToken = default)
        {
            if (_game.IsOver)
                throw new NotImplementedException("Game is over");
            if (!_game.IsInitialized)
                throw new NotImplementedException("Board was not initialized");
            if (depth <= 0)
                throw new NotImplementedException("Depth should be >0");
            Game game = new();
            game.SetPositionFromFEN(_game.GetBoardFEN());
            _moveEvals.Clear();
            SimpleMove[] moves = _game.LegalMoves
                .OrderByDescending(a => a.PieceCaptured)
                .ToArray();
            foreach (SimpleMove move in moves)
            {
                game.Move(move.From, move.To, promotedTo: move.PromotedTo);
                await Task.Run(() => _moveEvals.Add(new MoveEval(move.From, move.To,
                    MiniMax(game, game.PlayerToMove, depth - 1, alphaBetaPruning),
                    moveNumber: game.NoMoves, promotedTo: move.PromotedTo)), cancellationToken);
                game.UndoMove();
            }
            foreach (MoveEval m in _moveEvals)
                Console.WriteLine(m);
            Console.WriteLine();
            return _game.PlayerToMove == PieceColor.WHITE ? _moveEvals.Max() : _moveEvals.Min();
        }

        private double MiniMax(Game game, PieceColor maximizingPlayer, int depth = 3,
             bool alphaBetaPruning = true, double alpha = double.MinValue, double beta = double.MaxValue)
        {
            GamePhase gamePhase = EngineUtils.EvalGamePhase(game.Board, game.NoMoves);
            if (depth <= 0 || game.IsOver)
                return EvaluatePosition(game, gamePhase);
            game.RefreshLegalMoves();
            List<SimpleMove> moves = new(game.LegalMoves);
            moves = moves
                .OrderByDescending(a => a.PieceCaptured)
                .ToList();
            switch (maximizingPlayer)
            {
                case PieceColor.WHITE:
                    double maxEval = double.MinValue;
                    foreach (SimpleMove move in moves)
                    {
                        game.Move(move.From, move.To, move.PromotedTo);
                        double eval = MiniMax(game, PieceColor.BLACK, depth: depth - 1, alphaBetaPruning, alpha, beta);
                        maxEval = double.Max(maxEval, eval);
                        game.UndoMove();
                        if (alphaBetaPruning)
                        {
                            alpha = double.Max(alpha, eval);
                            if (beta <= alpha)
                                break;
                        }
                    }
                    return maxEval;
                case PieceColor.BLACK:
                    double minEval = double.MaxValue;
                    foreach (SimpleMove move in moves)
                    {
                        game.Move(move.From, move.To, move.PromotedTo);
                        double eval = MiniMax(game, PieceColor.WHITE, depth: depth - 1, alphaBetaPruning, alpha, beta);
                        minEval = double.Min(minEval, eval);
                        game.UndoMove();
                        if (alphaBetaPruning)
                        {
                            beta = double.Min(beta, eval);
                            if (beta <= alpha)
                                break;
                        }
                    }
                    return minEval;
                default:
                    throw new NotImplementedException("MiniMax - invalid piece color");
            }
        }

        public static double EvaluatePosition(Game game, GamePhase gamePhase, EvaluationType type = EvaluationType.MATERIAL)
        {
            if (game.IsOver)
            {
                if (game.IsDraw)
                    return 0;
                int evaluationFactor = game.PlayerToMove == PieceColor.WHITE ? -1 : 1;
                return evaluationFactor * EngineUtils.CHECKMATE_VALUE;
            }
            return type switch
            {
                EvaluationType.MATERIAL => EvaluatePieces(game.Board, gamePhase),
                _ => 0
            };
        }

        private static double EvaluatePieces(Board board, GamePhase gamePhase)
        {
            double eval = 0;
            foreach (Square sq in board.Squares)
            {
                Piece p = sq.Piece;
                if (p != null)
                {
                    int pieceValue = EngineUtils.PieceValue(p, sq.SquareCoords.File, sq.SquareCoords.Rank, gamePhase);
                    if (p.Color == PieceColor.WHITE)
                        eval += pieceValue;
                    else
                        eval -= pieceValue;
                }
            }
            return eval;
        }
    }
}
