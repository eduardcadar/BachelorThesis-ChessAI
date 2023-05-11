using ChessGameLibrary.Enums;
using System.Collections.Generic;

namespace ChessGameLibrary
{
    public static class Utils
    {
        public static int NO_FILES { get; } = 8;
        public static int NO_RANKS { get; } = 8;
        public static string STARTING_FEN { get; } =
            "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public static int HALFMOVE_CLOCK_LIMIT { get; } = 50;

        public static PieceType[] PromotionPieceTypes { get; } =
            { PieceType.KNIGHT, PieceType.BISHOP, PieceType.ROOK, PieceType.QUEEN };

        public static List<char> PieceLetters = new List<char>()
        {
            'P',
            'N',
            'B',
            'R',
            'Q',
            'K',
            'p',
            'n',
            'b',
            'r',
            'q',
            'k'
        };

        public static int[] KnightMoveCoordsX { get; } = new int[] { 1, 2, 2, 1, -1, -2, -2, -1 };

        public static int[] KnightMoveCoordsY { get; } = new int[] { 2, 1, -1, -2, 2, 1, -1, -2 };

        public static SimpleMove GetRookCastleMove(SquareCoords kingSquare)
        {
            if (kingSquare.Rank == 0)
            {
                if (kingSquare.File == 6)
                    return new SimpleMove(new SquareCoords(7, 0), new SquareCoords(5, 0));
                else if (kingSquare.File == 2)
                    return new SimpleMove(new SquareCoords(0, 0), new SquareCoords(3, 0));
            }
            else if (kingSquare.Rank == 7)
            {
                if (kingSquare.File == 6)
                    return new SimpleMove(new SquareCoords(7, 7), new SquareCoords(5, 7));
                else if (kingSquare.File == 2)
                    return new SimpleMove(new SquareCoords(0, 7), new SquareCoords(3, 7));
            }
            // error
            return new SimpleMove(new SquareCoords(0, 0), new SquareCoords(0, 0));
        }

        public static bool IsCastleMove(Piece piece, SquareCoords from, SquareCoords to)
        {
            if (piece.Type != PieceType.KING)
                return false;
            int firstRank = piece.Color == PieceColor.WHITE ? 0 : 7;
            if (from.Equals(4, firstRank) && (to.Equals(6, firstRank) || to.Equals(2, firstRank)))
                return true;
            return false;
        }

        public static bool IsPromotionMove(Piece piece, SquareCoords to)
        {
            int lastPawnRank = piece.Color == PieceColor.WHITE ? 7 : 0;
            return (piece.Type == PieceType.PAWN) && (to.Rank == lastPawnRank);
        }

        public static PieceType GetPieceTypeFromPieceLetter(char letter)
        {
            return letter switch
            {
                'p' => PieceType.PAWN,
                'n' => PieceType.KNIGHT,
                'b' => PieceType.BISHOP,
                'r' => PieceType.ROOK,
                'q' => PieceType.QUEEN,
                'k' => PieceType.KING,
                'P' => PieceType.PAWN,
                'N' => PieceType.KNIGHT,
                'B' => PieceType.BISHOP,
                'R' => PieceType.ROOK,
                'Q' => PieceType.QUEEN,
                'K' => PieceType.KING,
                _ => PieceType.NONE
            };
        }

        public static PieceColor GetPieceColorFromPieceLetter(char letter)
        {
            return char.IsLower(letter) ? PieceColor.BLACK : PieceColor.WHITE;
        }
    }
}
