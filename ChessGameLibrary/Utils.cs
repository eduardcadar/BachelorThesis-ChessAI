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
    }
}
