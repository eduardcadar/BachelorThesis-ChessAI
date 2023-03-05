using System;
using System.Collections.Generic;
using System.Linq;
using ChessGameLibrary.Enums;

namespace ChessGameLibrary
{
    public class Game
    {
        public Board Board;
        public PieceColor PlayerToMove;
        public SquareCoords EnPassantSquare;
        public GameState State;
        public List<MoveInfo> MovesHistory = new List<MoveInfo>();
        public List<SimpleMove> LegalMoves = new List<SimpleMove>();
        public bool WhiteCanCastleKing;
        public bool BlackCanCastleKing;
        public bool WhiteCanCastleQueen;
        public bool BlackCanCastleQueen;
        public int HalfmoveClock;
        public int NoMoves;

        public bool SetPositionFromFEN(string fen)
        {
            Board = new Board();
            string[] fields = fen.Split(' ');
            int rank = 7, file = 0;
            foreach (char letter in fields[0])
            {
                switch (letter)
                {
                    case 'p': AddPieceToBoard(PieceType.PAWN, PieceColor.BLACK, file, rank); break;
                    case 'n': AddPieceToBoard(PieceType.KNIGHT, PieceColor.BLACK, file, rank); break;
                    case 'b': AddPieceToBoard(PieceType.BISHOP, PieceColor.BLACK, file, rank); break;
                    case 'r': AddPieceToBoard(PieceType.ROOK, PieceColor.BLACK, file, rank); break;
                    case 'q': AddPieceToBoard(PieceType.QUEEN, PieceColor.BLACK, file, rank); break;
                    case 'k': AddPieceToBoard(PieceType.KING, PieceColor.BLACK, file, rank); break;
                    case 'P': AddPieceToBoard(PieceType.PAWN, PieceColor.WHITE, file, rank); break;
                    case 'N': AddPieceToBoard(PieceType.KNIGHT, PieceColor.WHITE, file, rank); break;
                    case 'B': AddPieceToBoard(PieceType.BISHOP, PieceColor.WHITE, file, rank); break;
                    case 'R': AddPieceToBoard(PieceType.ROOK, PieceColor.WHITE, file, rank); break;
                    case 'Q': AddPieceToBoard(PieceType.QUEEN, PieceColor.WHITE, file, rank); break;
                    case 'K': AddPieceToBoard(PieceType.KING, PieceColor.WHITE, file, rank); break;
                    case '/': rank--; file = -1; break;
                    case '1': break;
                    case '2': file += 1; break;
                    case '3': file += 2; break;
                    case '4': file += 3; break;
                    case '5': file += 4; break;
                    case '6': file += 5; break;
                    case '7': file += 6; break;
                    case '8': file += 7; break;
                    default: return false;
                }
                file++;
            }

            switch (fields[1])
            {
                case "w": PlayerToMove = PieceColor.WHITE; break;
                case "b": PlayerToMove = PieceColor.BLACK; break;
                default: return false;
            }

            WhiteCanCastleKing = fields[2].Contains('K');
            WhiteCanCastleQueen = fields[2].Contains('Q');
            BlackCanCastleKing = fields[2].Contains('k');
            BlackCanCastleQueen = fields[2].Contains('q');

            EnPassantSquare = fields[3] != "-" ? new SquareCoords(fields[3]) : null;

            HalfmoveClock = int.Parse(fields[4]);

            NoMoves = int.Parse(fields[5]);

            State = GameState.INPROGRESS;
            UpdateThreatMap();
            RefreshLegalMoves();
            CheckGameState();
            return true;
        }

        public void RefreshLegalMoves()
        {
            LegalMoves.Clear();
            foreach (Square from in Board.Squares)
            {
                if (from.Piece == null || from.Piece.Color != PlayerToMove)
                    continue;
                LegalMoves.AddRange(MoveSquares(from.SquareCoords)
                    .Select(to => new SimpleMove(from.SquareCoords, to)));
            }
        }

        public void UndoMove()
        {
            MoveInfo moveToUndo = MovesHistory.Last();
            MovesHistory.Remove(moveToUndo);
            SquareCoords from = moveToUndo.From, to = moveToUndo.To;
            Board.Squares[from.File, from.Rank].Piece = moveToUndo.Piece;
            Board.Squares[to.File, to.Rank].Piece = null;
            if (moveToUndo.CapturedPiece != null)
                Board.Squares[to.File, to.Rank].Piece = moveToUndo.CapturedPiece;

            if (moveToUndo.MoveType == MoveType.CASTLE) // if castle move, also move the rook back
            {
                SimpleMove rookMove = Utils.GetRookCastleMove(to);
                Board.GetSquare(rookMove.From).Piece = Board.GetSquare(rookMove.To).Piece;
                Board.GetSquare(rookMove.To).Piece = null;
            }
            PlayerToMove = PlayerToMove == PieceColor.WHITE ? PieceColor.BLACK : PieceColor.WHITE;
            EnPassantSquare = moveToUndo.PrevEnPassantSquare;
            WhiteCanCastleKing = moveToUndo.PrevWhiteCanCastleKing;
            WhiteCanCastleQueen = moveToUndo.PrevWhiteCanCastleQueen;
            BlackCanCastleKing = moveToUndo.PrevBlackCanCastleKing;
            BlackCanCastleQueen = moveToUndo.PrevBlackCanCastleQueen;
            HalfmoveClock = moveToUndo.PrevHalfmoveClock;
            NoMoves--;
            UpdateThreatMap();
        }

        public MoveInfo Move(SquareCoords from, SquareCoords to, PieceType promotedTo = PieceType.NONE, bool justCheck = false)
        {
            Square fromSq = Board.Squares[from.File, from.Rank];
            Piece piece = fromSq.Piece;
            Square toSq = Board.Squares[to.File, to.Rank];
            MoveInfo moveInfo = new MoveInfo(MoveType.NORMAL, piece, from, to,
                WhiteCanCastleKing, WhiteCanCastleQueen, BlackCanCastleKing, BlackCanCastleQueen)
            {
                PrevEnPassantSquare = EnPassantSquare,
                PrevHalfmoveClock = HalfmoveClock
            };
            if (piece == null)
            {
                Console.WriteLine("NO PIECE ON FROM SQUARE");
                return moveInfo;
                //error
            }
            if (piece.Color != PlayerToMove)
            {
                Console.WriteLine("WRONG MOVE, THE OTHER PLAYER HAS TO MOVE");
                return moveInfo;
                //error
            }
            // to disable castling rights after king/rook moves or rook is captured
            if (piece.Type == PieceType.KING) KingMoved(piece.Color);
            RookMovedOrCaptured(from);
            RookMovedOrCaptured(to);
            if (Utils.IsCastleMove(piece, from, to)) // if castle move, also move the rook
            {
                SimpleMove rookMove = Utils.GetRookCastleMove(to);
                Board.GetSquare(rookMove.To).Piece = Board.GetSquare(rookMove.From).Piece;
                Board.GetSquare(rookMove.From).Piece = null;
                moveInfo.MoveType = MoveType.CASTLE;
            }

            int lastPawnRank = piece.Color == PieceColor.WHITE ? 7 : 0;
            int direction = piece.Color == PieceColor.WHITE ? 1 : -1;
            if (piece.Type == PieceType.PAWN && to.Equals(EnPassantSquare))
            {
                moveInfo.MoveType = MoveType.ENPASSANT;
                Board.Squares[to.File, to.Rank - direction].Piece = null;
            }
            EnPassantSquare = null;
            if (piece.Type == PieceType.PAWN && Math.Abs(from.Rank - to.Rank) == 2)
                EnPassantSquare = new SquareCoords(from.File, (from.Rank + to.Rank) / 2);
            if (toSq.Piece != null)
                moveInfo.CapturedPiece = new Piece(toSq.Piece.Type, toSq.Piece.Color);
            toSq.Piece = piece;
            fromSq.Piece = null;
            if ((piece.Type == PieceType.PAWN) && (to.Rank == lastPawnRank))
            {
                moveInfo.MoveType = MoveType.PROMOTION;
                moveInfo.PromotedTo = promotedTo;
                toSq.Piece.Type = promotedTo;
            }
            PlayerToMove = PlayerToMove == PieceColor.WHITE ? PieceColor.BLACK : PieceColor.WHITE;
            MovesHistory.Add(moveInfo);
            if (piece.Type == PieceType.PAWN || moveInfo.CapturedPiece != null)
                HalfmoveClock = 0;
            else
                HalfmoveClock++;
            NoMoves++;
            UpdateThreatMap();
            if (HalfmoveClock == Utils.HALFMOVE_CLOCK_LIMIT)
            {
                State = GameState.DRAW;
                return moveInfo;
            }
            if (!justCheck)
                RefreshLegalMoves();
            CheckGameState();
            return moveInfo;
        }

        private void RookMovedOrCaptured(SquareCoords sq)
        {
            if (sq.Equals(0, 0))
                WhiteCanCastleQueen = false;
            else if (sq.Equals(7, 0))
                WhiteCanCastleKing = false;
            else if (sq.Equals(0, 7))
                BlackCanCastleQueen = false;
            else if (sq.Equals(7, 7))
                BlackCanCastleKing = false;
        }

        private void KingMoved(PieceColor pieceColor)
        {
            switch (pieceColor)
            {
                case PieceColor.WHITE:
                    WhiteCanCastleKing = false;
                    WhiteCanCastleQueen = false;
                    break;
                case PieceColor.BLACK:
                    BlackCanCastleKing = false;
                    BlackCanCastleQueen = false;
                    break;
            }
        }

        private void CheckGameState()
        {
            PieceColor otherPlayer = PlayerToMove == PieceColor.WHITE ? PieceColor.BLACK : PieceColor.WHITE;
            if (LegalMoves.Count == 0)
            {
                Square sq = Board.Squares
                    .Cast<Square>()
                    .Single(s => s.Piece != null && s.Piece.Type == PieceType.KING && s.Piece.Color == PlayerToMove);
                if (sq.IsAttackedBy(otherPlayer))
                    State = GameState.CHECKMATE;
                else
                    State = GameState.STALEMATE;
            }
            else
                State = GameState.INPROGRESS;
        }

        private bool CheckMove(SquareCoords from, SquareCoords to)
        {
            Move(from, to, promotedTo: PieceType.PAWN, justCheck: true);
            bool boardIsValid = BoardIsValid();
            UndoMove();
            return boardIsValid;
        }

        private bool BoardIsValid()
        {
            if (Board == null) return false;
            PieceColor otherPlayer = PlayerToMove == PieceColor.WHITE ? PieceColor.BLACK : PieceColor.WHITE;
            Square sq = Board.Squares
                .Cast<Square>()
                .Single(s => s.Piece != null && s.Piece.Type == PieceType.KING && s.Piece.Color == otherPlayer);
            if (sq.IsAttackedBy(PlayerToMove))
                return false;
            return true;
        }

        public List<SquareCoords> MoveSquares(SquareCoords from)
        {
            Piece piece = Board.Squares[from.File, from.Rank].Piece;
            if (piece == null)
                return new List<SquareCoords>();
            List<SquareCoords> squareCoords = piece.Type switch
            {
                PieceType.PAWN => PawnMoves(piece.Color, from),
                PieceType.KNIGHT => KnightMoves(piece.Color, from),
                PieceType.BISHOP => BishopMoves(piece.Color, from),
                PieceType.ROOK => RookMoves(piece.Color, from),
                PieceType.QUEEN => QueenMoves(piece.Color, from),
                PieceType.KING => KingMoves(piece.Color, from),
                _ => throw new NotImplementedException()
            };
            squareCoords = squareCoords
                .Where(to => CheckMove(from, to))
                .ToList();
            return squareCoords;
        }

        private void UpdateThreatMap()
        {
            foreach (Square s in Board.Squares)
                s.IsAttackedByWhite = s.IsAttackedByBlack = false;
            foreach (Square square in Board.Squares)
            {
                Piece piece = square.Piece;
                if (piece == null) continue;
                List<SquareCoords> pieceThreatMap =
                    piece.Type switch
                    {
                        PieceType.PAWN => PawnThreatMap(piece.Color, square.SquareCoords),
                        PieceType.KNIGHT => KnightThreatMap(square.SquareCoords),
                        PieceType.BISHOP => BishopThreatMap(square.SquareCoords),
                        PieceType.ROOK => RookThreatMap(square.SquareCoords),
                        PieceType.QUEEN => QueenThreatMap(square.SquareCoords),
                        PieceType.KING => KingThreatMap(square.SquareCoords),
                        _ => throw new NotImplementedException(),
                    };
                switch (piece.Color)
                {
                    case PieceColor.WHITE:
                        foreach (SquareCoords sc in pieceThreatMap)
                            Board.Squares[sc.File, sc.Rank].IsAttackedByWhite = true;
                        break;
                    case PieceColor.BLACK:
                        foreach (SquareCoords sc in pieceThreatMap)
                            Board.Squares[sc.File, sc.Rank].IsAttackedByBlack = true;
                        break;
                }
            }
        }

        private List<SquareCoords> PawnThreatMap(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int direction = pieceColor == PieceColor.WHITE ? 1 : -1;
            SquareCoords s1, s2;
            s1 = new SquareCoords(from.File + 1, from.Rank + direction);
            s2 = new SquareCoords(from.File - 1, from.Rank + direction);
            if (IsInsideBoard(s1.File, s1.Rank))
                squares.Add(new SquareCoords(from.File + 1, from.Rank + direction));
            if (IsInsideBoard(s2.File, s2.Rank))
                squares.Add(new SquareCoords(from.File - 1, from.Rank + direction));
            return squares;
        }

        private List<SquareCoords> KnightThreatMap(SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            for (int i = 0; i < Utils.KnightMoveCoordsX.Length; i++)
            {
                int X = Utils.KnightMoveCoordsX[i], Y = Utils.KnightMoveCoordsY[i];
                int newFile = from.File + X, newRank = from.Rank + Y;
                if (IsInsideBoard(newFile, newRank))
                {
                    squares.Add(new SquareCoords(newFile, newRank));
                }
            }
            return squares;
        }

        private List<SquareCoords> BishopThreatMap(SquareCoords from)
        {
            return BishopMoves(PieceColor.NONE, from);
        }

        private List<SquareCoords> RookThreatMap(SquareCoords from)
        {
            return RookMoves(PieceColor.NONE, from);
        }

        private List<SquareCoords> QueenThreatMap(SquareCoords from)
        {
            return QueenMoves(PieceColor.NONE, from);
        }

        private List<SquareCoords> KingThreatMap(SquareCoords from)
        {
            return KingMoves(PieceColor.NONE, from);
        }

        private void AddPieceToBoard(PieceType pieceType, PieceColor pieceColor, int file, int rank)
        {
            if (file < 0 || file >= Utils.NO_FILES || rank < 0 || rank >= Utils.NO_RANKS)
            {
                // error
            }
            Piece piece = new Piece(pieceType, pieceColor);
            Square square = new Square(new SquareCoords(file, rank), piece);
            Board.Squares[file, rank] = square;
        }

        private List<SquareCoords> KingMoves(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            // up
            if (IsSquareEmptyOrEnemyPiece(pieceColor, from.File, from.Rank + 1))
                squares.Add(new SquareCoords(from.File, from.Rank + 1));
            // up and right
            if (IsSquareEmptyOrEnemyPiece(pieceColor, from.File + 1, from.Rank + 1))
                squares.Add(new SquareCoords(from.File + 1, from.Rank + 1));
            // right
            if (IsSquareEmptyOrEnemyPiece(pieceColor, from.File + 1, from.Rank))
                squares.Add(new SquareCoords(from.File + 1, from.Rank));
            // right and down
            if (IsSquareEmptyOrEnemyPiece(pieceColor, from.File + 1, from.Rank - 1))
                squares.Add(new SquareCoords(from.File + 1, from.Rank - 1));
            // down
            if (IsSquareEmptyOrEnemyPiece(pieceColor, from.File, from.Rank - 1))
                squares.Add(new SquareCoords(from.File, from.Rank - 1));
            // down and left
            if (IsSquareEmptyOrEnemyPiece(pieceColor, from.File - 1, from.Rank - 1))
                squares.Add(new SquareCoords(from.File - 1, from.Rank - 1));
            // left
            if (IsSquareEmptyOrEnemyPiece(pieceColor, from.File - 1, from.Rank))
                squares.Add(new SquareCoords(from.File - 1, from.Rank));
            // left and up
            if (IsSquareEmptyOrEnemyPiece(pieceColor, from.File - 1, from.Rank + 1))
                squares.Add(new SquareCoords(from.File - 1, from.Rank + 1));

            // CASTLE
            switch (pieceColor)
            {
                case PieceColor.WHITE:
                    if (WhiteCanCastleKing)
                    {
                        if (!Board.Squares[4, 0].IsAttackedByBlack &&
                            IsSquareEmptyAndNotAttacked(PieceColor.BLACK, 5, 0) &&
                            IsSquareEmptyAndNotAttacked(PieceColor.BLACK, 6, 0))
                            squares.Add(new SquareCoords(6, 0));
                    }
                    if (WhiteCanCastleQueen)
                    {
                        if (!Board.Squares[4, 0].IsAttackedByBlack &&
                            IsSquareEmptyAndNotAttacked(PieceColor.BLACK, 2, 0) &&
                            IsSquareEmptyAndNotAttacked(PieceColor.BLACK, 3, 0))
                            squares.Add(new SquareCoords(2, 0));
                    }
                    break;
                case PieceColor.BLACK:
                    if (BlackCanCastleKing)
                    {
                        if (!Board.Squares[4, 7].IsAttackedByWhite &&
                            IsSquareEmptyAndNotAttacked(PieceColor.WHITE, 5, 7) &&
                            IsSquareEmptyAndNotAttacked(PieceColor.WHITE, 6, 7))
                            squares.Add(new SquareCoords(6, 7));
                    }
                    if (BlackCanCastleQueen)
                    {
                        if (!Board.Squares[4, 7].IsAttackedByWhite &&
                            IsSquareEmptyAndNotAttacked(PieceColor.WHITE, 2, 7) &&
                            IsSquareEmptyAndNotAttacked(PieceColor.WHITE, 3, 7))
                            squares.Add(new SquareCoords(2, 7));
                    }
                    break;
            }
            return squares;
        }

        private List<SquareCoords> QueenMoves(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            squares.AddRange(SquaresUp(pieceColor, from));
            squares.AddRange(SquaresUpAndRight(pieceColor, from));
            squares.AddRange(SquaresRight(pieceColor, from));
            squares.AddRange(SquaresDownAndRight(pieceColor, from));
            squares.AddRange(SquaresDown(pieceColor, from));
            squares.AddRange(SquaresDownAndLeft(pieceColor, from));
            squares.AddRange(SquaresLeft(pieceColor, from));
            squares.AddRange(SquaresUpAndLeft(pieceColor, from));
            return squares;
        }

        private List<SquareCoords> RookMoves(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            squares.AddRange(SquaresUp(pieceColor, from));
            squares.AddRange(SquaresRight(pieceColor, from));
            squares.AddRange(SquaresDown(pieceColor, from));
            squares.AddRange(SquaresLeft(pieceColor, from));
            return squares;
        }

        private List<SquareCoords> BishopMoves(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            squares.AddRange(SquaresUpAndRight(pieceColor, from));
            squares.AddRange(SquaresDownAndRight(pieceColor, from));
            squares.AddRange(SquaresDownAndLeft(pieceColor, from));
            squares.AddRange(SquaresUpAndLeft(pieceColor, from));
            return squares;
        }

        private List<SquareCoords> PawnMoves(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int direction = pieceColor == PieceColor.WHITE ? 1 : -1;
            int firstPawnRank = pieceColor == PieceColor.WHITE ? 1 : 6;
            int nextRank = from.Rank + direction;
            if (IsInsideBoard(nextRank))
            {
                if (Board.Squares[from.File, nextRank].Piece == null)
                {
                    squares.Add(new SquareCoords(from.File, nextRank));
                    if (from.Rank == firstPawnRank && Board.Squares[from.File, nextRank + direction].Piece == null)
                        squares.Add(new SquareCoords(from.File, nextRank + direction));
                }
                if (IsInsideBoard(from.File + 1))
                {
                    if (IsEnemyPieceOnSquare(pieceColor, from.File + 1, nextRank)
                        || IsEnPassantSquare(from.File + 1, nextRank))
                        squares.Add(new SquareCoords(from.File + 1, nextRank));
                }
                if (IsInsideBoard(from.File - 1))
                {
                    if (IsEnemyPieceOnSquare(pieceColor, from.File - 1, nextRank)
                        || IsEnPassantSquare(from.File - 1, nextRank))
                        squares.Add(new SquareCoords(from.File - 1, nextRank));
                }
            }
            return squares;
        }

        private List<SquareCoords> KnightMoves(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            for (int i = 0; i < Utils.KnightMoveCoordsX.Length; i++)
            {
                int X = Utils.KnightMoveCoordsX[i], Y = Utils.KnightMoveCoordsY[i];
                int newFile = from.File + X, newRank = from.Rank + Y;
                if (IsInsideBoard(newFile, newRank))
                {
                    Piece p = Board.Squares[newFile, newRank].Piece;
                    if (p == null || p.Color != pieceColor)
                        squares.Add(new SquareCoords(newFile, newRank));
                }
            }
            return squares;
        }

        private List<SquareCoords> SquaresUp(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int i = 1;
            while (IsSquareEmptyOrEnemyPiece(pieceColor, from.File, from.Rank + i))
            {
                squares.Add(new SquareCoords(from.File, from.Rank + i));
                if (IsEnemyPieceOnSquare(pieceColor, from.File, from.Rank + i))
                    break;
                i++;
            }
            return squares;
        }

        private List<SquareCoords> SquaresUpAndRight(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int i = 1;
            while (IsSquareEmptyOrEnemyPiece(pieceColor, from.File + i, from.Rank + i))
            {
                squares.Add(new SquareCoords(from.File + i, from.Rank + i));
                if (IsEnemyPieceOnSquare(pieceColor, from.File + i, from.Rank + i))
                    break;
                i++;
            }
            return squares;
        }

        private List<SquareCoords> SquaresRight(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int i = 1;
            while (IsSquareEmptyOrEnemyPiece(pieceColor, from.File + i, from.Rank))
            {
                squares.Add(new SquareCoords(from.File + i, from.Rank));
                if (IsEnemyPieceOnSquare(pieceColor, from.File + i, from.Rank))
                    break;
                i++;
            }
            return squares;
        }

        private List<SquareCoords> SquaresDownAndRight(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int i = 1;
            while (IsSquareEmptyOrEnemyPiece(pieceColor, from.File + i, from.Rank - i))
            {
                squares.Add(new SquareCoords(from.File + i, from.Rank - i));
                if (IsEnemyPieceOnSquare(pieceColor, from.File + i, from.Rank - i))
                    break;
                i++;
            }
            return squares;
        }

        private List<SquareCoords> SquaresDown(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int i = 1;
            while (IsSquareEmptyOrEnemyPiece(pieceColor, from.File, from.Rank - i))
            {
                squares.Add(new SquareCoords(from.File, from.Rank - i));
                if (IsEnemyPieceOnSquare(pieceColor, from.File, from.Rank - i))
                    break;
                i++;
            }
            return squares;
        }

        private List<SquareCoords> SquaresDownAndLeft(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int i = 1;
            while (IsSquareEmptyOrEnemyPiece(pieceColor, from.File - i, from.Rank - i))
            {
                squares.Add(new SquareCoords(from.File - i, from.Rank - i));
                if (IsEnemyPieceOnSquare(pieceColor, from.File - i, from.Rank - i))
                    break;
                i++;
            }
            return squares;
        }

        private List<SquareCoords> SquaresLeft(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int i = 1;
            while (IsSquareEmptyOrEnemyPiece(pieceColor, from.File - i, from.Rank))
            {
                squares.Add(new SquareCoords(from.File - i, from.Rank));
                if (IsEnemyPieceOnSquare(pieceColor, from.File - i, from.Rank))
                    break;
                i++;
            }
            return squares;
        }

        private List<SquareCoords> SquaresUpAndLeft(PieceColor pieceColor, SquareCoords from)
        {
            List<SquareCoords> squares = new List<SquareCoords>();
            int i = 1;
            while (IsSquareEmptyOrEnemyPiece(pieceColor, from.File - i, from.Rank + i))
            {
                squares.Add(new SquareCoords(from.File - i, from.Rank + i));
                if (IsEnemyPieceOnSquare(pieceColor, from.File - i, from.Rank + i))
                    break;
                i++;
            }
            return squares;
        }

        private bool IsSquareEmptyAndNotAttacked(PieceColor pieceColor, int file, int rank)
        {
            Square sq = Board.Squares[file, rank];
            if (sq.Piece != null)
                return false;
            return pieceColor == PieceColor.WHITE ? !sq.IsAttackedByWhite : !sq.IsAttackedByBlack;
        }

        private bool IsEmptySquare(int file, int rank)
        {
            return Board.Squares[file, rank].Piece == null;
        }

        private bool IsEnemyPieceOnSquare(PieceColor pieceColor, int file, int rank)
        {
            if (IsEmptySquare(file, rank))
                return false;
            return Board.Squares[file, rank].Piece.Color != pieceColor;
        }

        private bool IsEnPassantSquare(int file, int rank)
        {
            if (EnPassantSquare == null)
                return false;
            return EnPassantSquare.Equals(file, rank);
        }

        private bool IsSquareEmptyOrEnemyPiece(PieceColor pieceColor, int file, int rank)
        {
            if (IsInsideBoard(file, rank))
            {
                Piece p = Board.Squares[file, rank].Piece;
                if (p == null || p.Color != pieceColor)
                    return true;
            }
            return false;
        }

        private bool IsInsideBoard(int x)
        {
            return x >= 0 && x < 8;
        }

        private bool IsInsideBoard(int x, int y)
        {
            return IsInsideBoard(x) && IsInsideBoard(y);
        }
    }
}
