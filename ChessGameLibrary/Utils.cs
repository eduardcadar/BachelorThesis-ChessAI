using ChessGameLibrary.Enums;

namespace ChessGameLibrary
{
    public static class Utils
    {
        public static int NO_FILES { get; } = 8;
        public static int NO_RANKS { get; } = 8;
        public static string STARTING_FEN { get; } = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static int[] KnightMoveCoordsX { get; } = new int[]
        {
            1, 2, 2, 1, -1, -2, -2, -1
        };

        public static int[] KnightMoveCoordsY { get; } = new int[]
        {
            2, 1, -1, -2, 2, 1, -1, -2
        };

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
            if (from.Equals(4, firstRank) &&
                (to.Equals(6, firstRank) || to.Equals(2, firstRank)))
                return true;
            return false;
        }
    }
}
