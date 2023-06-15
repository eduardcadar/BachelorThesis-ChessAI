using ChessGameLibrary;
using ChessGameLibrary.Enums;
using OctoChessEngine.Domain;
using OctoChessEngine.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OctoChessEngine
{
    public class OctoChess
    {
        private readonly Game _game;

        //private MachineLearningModel _model;
        private readonly Dictionary<string, double> _previousEvals;
        private int _prunesCount = 0;
        private Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _maxTime;

        public List<MoveEval> MoveEvals { get; set; }

        public OctoChess()
        {
            _game = new Game();
            MoveEvals = new List<MoveEval>();
            //_model = new MachineLearningModel();
            //_model.LoadModel();
            _previousEvals = new Dictionary<string, double>();
        }

        public void ClearPreviousEvals()
        {
            _previousEvals.Clear();
        }

        public void OrderMoveEvals()
        {
            if (_game.PlayerToMove == PieceColor.WHITE)
                MoveEvals = MoveEvals.OrderByDescending(m => m.Evaluation).ToList();
            else
                MoveEvals = MoveEvals.OrderBy(m => m.Evaluation).ToList();
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
                        noMoves += NumOfPositionsRecursive(noPositions, 2, depth);
                    if (depth == 1)
                        noMoves += _game.LegalMoves.Count;
                    Console.WriteLine($"{move}: {noMoves}");
                    _game.UndoMove();
                }
            }

            return noPositions;
        }

        public int NumOfPositionsRecursive(int[] noPositions, int currentDepth, int depth)
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
                    noMoves += NumOfPositionsRecursive(noPositions, currentDepth + 1, depth);
                _game.UndoMove();
            }
            return noMoves;
        }

        public MoveEval BestMove(
            int maxDepth = 3,
            bool useAlphaBetaPruning = true,
            EvaluationType evaluationType = EvaluationType.MATERIAL,
            bool useIterativeDeepening = true,
            int timeLimit = 60,
            bool useQuiescenceSearch = true,
            int maxQuiescenceDepth = 5
        )
        {
            if (_game.IsOver)
                throw new NotImplementedException("Game is over");
            if (!_game.IsInitialized)
                throw new NotImplementedException("Board was not initialized");
            if (maxDepth <= 0)
                throw new NotImplementedException("Depth should be >0");

            _stopwatch.Reset();
            if (useIterativeDeepening)
                return IterativeDeepening(
                    maxDepth: maxDepth,
                    useAlphaBetaPruning: useAlphaBetaPruning,
                    evaluationType: evaluationType,
                    timeLimit: timeLimit,
                    useQuiescenceSearch: useQuiescenceSearch,
                    maxQuiescenceDepth: maxQuiescenceDepth
                );

            return GetBestMove(
                depth: maxDepth,
                useAlphaBetaPruning: useAlphaBetaPruning,
                evaluationType: evaluationType,
                useQuiescenceSearch: useQuiescenceSearch,
                maxQuiescenceDepth: maxQuiescenceDepth
            );
        }

        private bool IsQuiescentMove(SimpleMove move)
        {
            if (move.PromotedTo != PieceType.NONE)
                return false;
            if (move.PieceCaptured != PieceType.NONE && move.PieceCaptured != PieceType.PAWN)
                return false;
            return true;
        }

        private bool IsQuiescentPosition()
        {
            foreach (SimpleMove move in _game.LegalMoves)
                if (!IsQuiescentMove(move))
                    return false;
            return true;
        }

        public MoveEval IterativeDeepening(
            int maxDepth,
            bool useAlphaBetaPruning,
            EvaluationType evaluationType,
            int timeLimit,
            bool useQuiescenceSearch,
            int maxQuiescenceDepth
        )
        {
            SimpleMove firstMove = _game.LegalMoves[0];
            MoveEval bestMove = new MoveEval(
                firstMove.From,
                firstMove.To,
                0,
                _game.MovesCount,
                firstMove.PromotedTo
            );
            _maxTime = TimeSpan.FromSeconds(timeLimit);
            for (int currentDepth = 1; currentDepth <= maxDepth; currentDepth++)
            {
                _stopwatch.Start();
                try
                {
                    bestMove = GetBestMove(
                        depth: currentDepth,
                        useAlphaBetaPruning: useAlphaBetaPruning,
                        evaluationType: evaluationType,
                        useQuiescenceSearch: useQuiescenceSearch,
                        maxQuiescenceDepth: maxQuiescenceDepth
                    );
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Time elapsed");
                }

                _stopwatch.Stop();
                if (_stopwatch.Elapsed > _maxTime)
                    break;
            }
            return bestMove;
        }

        private MoveEval GetBestMove(
            int depth,
            bool useAlphaBetaPruning,
            EvaluationType evaluationType,
            bool useQuiescenceSearch,
            int maxQuiescenceDepth
        )
        {
            _game.SetPositionFromFEN(_game.GetBoardFEN());
            MoveEvals.Clear();
            SimpleMove[] moves = _game.LegalMoves
                .OrderByDescending(a => a.PieceCaptured)
                .ThenBy(a => _game.Board.GetSquare(a.From).Piece.Type)
                .ToArray();
            int i = 1,
                l = moves.Length;
            int initialDepth = depth;
            foreach (SimpleMove move in moves)
            {
                _game.Move(move.From, move.To, promotedTo: move.PromotedTo);
                MoveEvals.Add(
                    new MoveEval(
                        move.From,
                        move.To,
                        MiniMax(
                            maximizingPlayer: _game.PlayerToMove,
                            useAlphaBetaPruning: useAlphaBetaPruning,
                            useQuiescenceSearch: useQuiescenceSearch,
                            maxQuiescenceDepth: maxQuiescenceDepth,
                            evaluationType: evaluationType,
                            initialDepth: initialDepth,
                            depth: depth - 1
                        ),
                        moveNumber: _game.MovesCount,
                        promotedTo: move.PromotedTo
                    )
                );

                _game.UndoMove();
                Console.WriteLine($"Move {i}/{l}");
                i++;
            }
            foreach (MoveEval m in MoveEvals)
                Console.WriteLine(m);
            //Console.WriteLine("Prunes: " + _prunesCount);
            return _game.PlayerToMove == PieceColor.WHITE ? MoveEvals.Max()! : MoveEvals.Min()!;
        }

        private double StaticEval(
            int initialDepth,
            int depth,
            GamePhase gamePhase,
            EvaluationType evaluationType
        )
        {
            string positionFen = _game.GetPositionFENString();
            if (_previousEvals.TryGetValue(positionFen, out double value))
                return value;
            double eval = EvaluatePosition(initialDepth, depth, gamePhase, evaluationType);
            _previousEvals[positionFen] = eval;
            return eval;
        }

        private double Quiesce(
            int initialDepth,
            int depth,
            EvaluationType evaluationType,
            int maxQuiescenceDepth,
            double alpha,
            double beta
        )
        {
            if (_stopwatch.Elapsed > _maxTime)
                throw new OperationCanceledException();
            GamePhase gamePhase = EngineUtils.EvalGamePhase(_game.Board, _game.MovesCount);
            double stand_pat = StaticEval(initialDepth, depth, gamePhase, evaluationType);

            if (maxQuiescenceDepth <= 0 || _game.IsOver)
                return stand_pat;

            if (_game.PlayerToMove == PieceColor.WHITE)
            {
                if (stand_pat >= beta)
                    return beta;
                if (alpha < stand_pat)
                    alpha = stand_pat;
            }
            else
            {
                if (stand_pat <= alpha)
                    return alpha;
                if (beta > stand_pat)
                    beta = stand_pat;
            }

            List<SimpleMove> moves = new List<SimpleMove>(
                _game.LegalMoves
                    .Where(m => !IsQuiescentMove(m))
                    .OrderByDescending(a => a.PieceCaptured)
                    .ThenBy(a => _game.Board.GetSquare(a.From).Piece.Type)
            );

            foreach (SimpleMove move in moves)
            {
                _game.Move(move.From, move.To, move.PromotedTo);
                double eval = Quiesce(
                    initialDepth,
                    depth,
                    evaluationType,
                    maxQuiescenceDepth - 1,
                    alpha,
                    beta
                );
                _game.UndoMove();

                if (_game.PlayerToMove == PieceColor.WHITE)
                {
                    if (eval >= beta)
                        return beta;
                    if (eval > alpha)
                        alpha = eval;
                }
                else
                {
                    if (eval <= alpha)
                        return alpha;
                    if (eval < beta)
                        beta = eval;
                }
            }
            if (_game.PlayerToMove == PieceColor.WHITE)
                return alpha;
            else
                return beta;
        }

        private double MiniMax(
            PieceColor maximizingPlayer,
            bool useAlphaBetaPruning,
            bool useQuiescenceSearch,
            int maxQuiescenceDepth,
            EvaluationType evaluationType,
            double alpha = double.MinValue,
            double beta = double.MaxValue,
            int initialDepth = 3,
            int depth = 3
        )
        {
            if (_stopwatch.Elapsed > _maxTime)
                throw new OperationCanceledException();
            GamePhase gamePhase = EngineUtils.EvalGamePhase(_game.Board, _game.MovesCount);
            if (_game.IsOver)
                return StaticEval(initialDepth, depth, gamePhase, evaluationType);
            if (depth <= 0)
            {
                double staticEval = StaticEval(initialDepth, depth, gamePhase, evaluationType);
                if (!useQuiescenceSearch)
                    return staticEval;
                else
                {
                    if (!IsQuiescentPosition())
                        return Quiesce(
                            initialDepth,
                            depth,
                            evaluationType,
                            maxQuiescenceDepth,
                            alpha,
                            beta
                        );
                    else
                        return staticEval;
                }
            }
            List<SimpleMove> moves = new List<SimpleMove>(_game.LegalMoves);
            moves = moves
                .OrderByDescending(a => a.PieceCaptured)
                .ThenBy(a => _game.Board.GetSquare(a.From).Piece.Type)
                .ToList();
            switch (maximizingPlayer)
            {
                case PieceColor.WHITE:
                    double maxEval = double.MinValue;
                    foreach (SimpleMove move in moves)
                    {
                        _game.Move(move.From, move.To, move.PromotedTo);
                        double eval = MiniMax(
                            maximizingPlayer: PieceColor.BLACK,
                            useAlphaBetaPruning: useAlphaBetaPruning,
                            useQuiescenceSearch: useQuiescenceSearch,
                            maxQuiescenceDepth: maxQuiescenceDepth,
                            evaluationType: evaluationType,
                            alpha: alpha,
                            beta: beta,
                            initialDepth: initialDepth,
                            depth: depth - 1
                        );
                        maxEval = Math.Max(maxEval, eval);
                        _game.UndoMove();
                        if (useAlphaBetaPruning)
                        {
                            alpha = Math.Max(alpha, eval);
                            if (beta <= alpha)
                            {
                                _prunesCount++;
                                break;
                            }
                        }
                    }
                    return maxEval;
                case PieceColor.BLACK:
                    double minEval = double.MaxValue;
                    foreach (SimpleMove move in moves)
                    {
                        _game.Move(move.From, move.To, move.PromotedTo);
                        double eval = MiniMax(
                            maximizingPlayer: PieceColor.WHITE,
                            useAlphaBetaPruning: useAlphaBetaPruning,
                            useQuiescenceSearch: useQuiescenceSearch,
                            maxQuiescenceDepth: maxQuiescenceDepth,
                            evaluationType: evaluationType,
                            alpha: alpha,
                            beta: beta,
                            initialDepth: initialDepth,
                            depth: depth - 1
                        );
                        minEval = Math.Min(minEval, eval);
                        _game.UndoMove();
                        if (useAlphaBetaPruning)
                        {
                            beta = Math.Min(beta, eval);
                            if (beta <= alpha)
                            {
                                _prunesCount++;
                                break;
                            }
                        }
                    }
                    return minEval;
                default:
                    return -100;
            }
        }

        public double EvaluatePosition(
            int initialDepth,
            int depth,
            GamePhase gamePhase,
            EvaluationType type = EvaluationType.MATERIAL
        )
        {
            if (_game.IsOver)
            {
                if (_game.IsDraw)
                    return 0;
                int evaluationFactor = _game.PlayerToMove == PieceColor.WHITE ? -1 : 1;
                int eval = EngineUtils.CHECKMATE_VALUE - (initialDepth - depth);
                return evaluationFactor * eval;
            }
            return type switch
            {
                EvaluationType.MATERIAL => EvaluatePieces(_game.Board, gamePhase),
                EvaluationType.TRAINED_MODEL => EvaluateByTrainedModel(gamePhase),
                _ => 0
            };
        }

        private double EvaluateByTrainedModel(GamePhase gamePhase)
        {
            //double materialEval = EvaluatePieces(_game.Board, gamePhase);
            //float[,] positions = DataUtils.GamePositionToFloatPositions(_game);
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < 70; i++)
            //    sb.Append(positions[0, i].ToString()).Append(',');
            //string pos = sb.ToString()[..^1];
            //float modelEval = _model.Predict(positions)[0];

            //return materialEval + modelEval * 100;
            return 0;
        }

        private static double EvaluatePieces(Board board, GamePhase gamePhase)
        {
            double eval = 0;
            foreach (Square sq in board.Squares)
            {
                Piece p = sq.Piece;
                if (p != null)
                {
                    int pieceValue = EngineUtils.PieceValue(
                        p,
                        sq.SquareCoords.File,
                        sq.SquareCoords.Rank,
                        gamePhase
                    );
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
