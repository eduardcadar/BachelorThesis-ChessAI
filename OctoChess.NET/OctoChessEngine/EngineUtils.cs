using ChessGameLibrary;
using ChessGameLibrary.Enums;
using OctoChessEngine.Enums;

namespace OctoChessEngine
{
    public static class EngineUtils
    {
        public static int CHECKMATE_VALUE { get; } = int.MaxValue / 2;
        private static int DEFAULT_PAWN_VALUE { get; } = 100;
        private static int DEFAULT_KNIGHT_VALUE { get; } = 320;
        private static int DEFAULT_BISHOP_VALUE { get; } = 330;
        private static int DEFAULT_ROOK_VALUE { get; } = 500;
        private static int DEFAULT_QUEEN_VALUE { get; } = 900;
        private static int DEFAULT_KING_VALUE { get; } = 20000;

        public static int PieceValue(Piece piece, int file, int rank, GamePhase gamePhase)
        {
            if (piece.Color == PieceColor.WHITE)
                return piece.Type switch
                {
                    PieceType.NONE => throw new NotImplementedException(),
                    PieceType.PAWN => DEFAULT_PAWN_VALUE + PieceSquareTable.PawnTable[rank][file],
                    PieceType.KNIGHT => DEFAULT_KNIGHT_VALUE + PieceSquareTable.KnightTable[rank][file],
                    PieceType.BISHOP => DEFAULT_BISHOP_VALUE + PieceSquareTable.BishopTable[rank][file],
                    PieceType.ROOK => DEFAULT_ROOK_VALUE + PieceSquareTable.RookTable[rank][file],
                    PieceType.QUEEN => DEFAULT_QUEEN_VALUE + PieceSquareTable.QueenTable[rank][file],
                    PieceType.KING => DEFAULT_KING_VALUE + (gamePhase == GamePhase.ENDGAME
                        ? PieceSquareTable.KingTableEndgame[rank][file] : PieceSquareTable.KingTable[rank][file]),
                    _ => 0
                };
            return piece.Type switch
            {
                PieceType.NONE => throw new NotImplementedException(),
                PieceType.PAWN => DEFAULT_PAWN_VALUE + PieceSquareTable.PawnTableBlack[rank][file],
                PieceType.KNIGHT => DEFAULT_KNIGHT_VALUE + PieceSquareTable.KnightTableBlack[rank][file],
                PieceType.BISHOP => DEFAULT_BISHOP_VALUE + PieceSquareTable.BishopTableBlack[rank][file],
                PieceType.ROOK => DEFAULT_ROOK_VALUE + PieceSquareTable.RookTableBlack[rank][file],
                PieceType.QUEEN => DEFAULT_QUEEN_VALUE + PieceSquareTable.QueenTableBlack[rank][file],
                PieceType.KING => DEFAULT_KING_VALUE + (gamePhase == GamePhase.ENDGAME
                    ? PieceSquareTable.KingTableBlackEndgame[rank][file] : PieceSquareTable.KingTableBlack[rank][file]),
                _ => 0
            };

        }

        public static GamePhase EvalGamePhase(Board board, int moveNumber)
        {
            if (moveNumber < 13)
                return GamePhase.OPENING;
            var pieces = board.Squares.Cast<Square>()
                .Where(sq => sq.Piece != null)
                .Select(sq => sq.Piece);
            if (!pieces.Any(p => p.Type == PieceType.QUEEN))
                return GamePhase.ENDGAME;
            if (pieces.Count(p => p.Type != PieceType.KING && p.Type != PieceType.PAWN && p.Type != PieceType.QUEEN) < 3)
                return GamePhase.ENDGAME;
            return GamePhase.MIDDLEGAME;
        }

        private static class PieceSquareTable
        {
            public static int[][] PawnTable { get; } =
            {
                new int[] {  0,  0,  0,  0,  0,  0,  0,  0 },
                new int[] { 50, 50, 50, 50, 50, 50, 50, 50 },
                new int[] { 10, 10, 20, 30, 30, 20, 10, 10 },
                new int[] {  5,  5, 10, 25, 25, 10,  5,  5 },
                new int[] {  0,  0,  0, 20, 20,  0,  0,  0 },
                new int[] {  5, -5,-10,  0,  0,-10, -5,  5 },
                new int[] {  5, 10, 10,-20,-20, 10, 10,  5 },
                new int[] {  0,  0,  0,  0,  0,  0,  0,  0 },
            };
            public static int[][] PawnTableBlack { get; } =
            {
                new int[] {  0,  0,  0,  0,  0,  0,  0,  0 },
                new int[] {  5, 10, 10,-20,-20, 10, 10,  5 },
                new int[] {  5, -5,-10,  0,  0,-10, -5,  5 },
                new int[] {  0,  0,  0, 20, 20,  0,  0,  0 },
                new int[] {  5,  5, 10, 25, 25, 10,  5,  5 },
                new int[] { 10, 10, 20, 30, 30, 20, 10, 10 },
                new int[] { 50, 50, 50, 50, 50, 50, 50, 50 },
                new int[] {  0,  0,  0,  0,  0,  0,  0,  0 },
            };
            public static int[][] KnightTable { get; } =
            {
                new int[] { -50,-40,-30,-30,-30,-30,-40,-50 },
                new int[] { -40,-20,  0,  0,  0,  0,-20,-40 },
                new int[] { -30,  0, 10, 15, 15, 10,  0,-30 },
                new int[] { -30,  5, 15, 20, 20, 15,  5,-30 },
                new int[] { -30,  0, 15, 20, 20, 15,  0,-30 },
                new int[] { -30,  5, 10, 15, 15, 10,  5,-30 },
                new int[] { -40,-20,  0,  5,  5,  0,-20,-40 },
                new int[] { -50,-40,-30,-30,-30,-30,-40,-50 },
            };
            public static int[][] KnightTableBlack { get; } =
            {
                new int[] { -50,-40,-30,-30,-30,-30,-40,-50 },
                new int[] { -40,-20,  0,  5,  5,  0,-20,-40 },
                new int[] { -30,  5, 10, 15, 15, 10,  5,-30 },
                new int[] { -30,  0, 15, 20, 20, 15,  0,-30 },
                new int[] { -30,  5, 15, 20, 20, 15,  5,-30 },
                new int[] { -30,  0, 10, 15, 15, 10,  0,-30 },
                new int[] { -40,-20,  0,  0,  0,  0,-20,-40 },
                new int[] { -50,-40,-30,-30,-30,-30,-40,-50 },
            };
            public static int[][] BishopTable { get; } =
            {
                new int[] { -20,-10,-10,-10,-10,-10,-10,-20 },
                new int[] { -10,  0,  0,  0,  0,  0,  0,-10 },
                new int[] { -10,  0,  5, 10, 10,  5,  0,-10 },
                new int[] { -10,  5,  5, 10, 10,  5,  5,-10 },
                new int[] { -10,  0, 10, 10, 10, 10,  0,-10 },
                new int[] { -10, 10, 10, 10, 10, 10, 10,-10 },
                new int[] { -10,  5,  0,  0,  0,  0,  5,-10 },
                new int[] { -20,-10,-10,-10,-10,-10,-10,-20 },
            };
            public static int[][] BishopTableBlack { get; } =
            {
                new int[] { -20,-10,-10,-10,-10,-10,-10,-20 },
                new int[] { -10,  5,  0,  0,  0,  0,  5,-10 },
                new int[] { -10, 10, 10, 10, 10, 10, 10,-10 },
                new int[] { -10,  0, 10, 10, 10, 10,  0,-10 },
                new int[] { -10,  5,  5, 10, 10,  5,  5,-10 },
                new int[] { -10,  0,  5, 10, 10,  5,  0,-10 },
                new int[] { -10,  0,  0,  0,  0,  0,  0,-10 },
                new int[] { -20,-10,-10,-10,-10,-10,-10,-20 },
            };
            public static int[][] RookTable { get; } =
            {
                new int[] {  0,  0,  0,  0,  0,  0,  0,  0 },
                new int[] {  5, 10, 10, 10, 10, 10, 10,  5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] {  0,  0,  0,  5,  5,  0,  0,  0 },
            };
            public static int[][] RookTableBlack { get; } =
            {
                new int[] {  0,  0,  0,  5,  5,  0,  0,  0 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] { -5,  0,  0,  0,  0,  0,  0, -5 },
                new int[] {  5, 10, 10, 10, 10, 10, 10,  5 },
                new int[] {  0,  0,  0,  0,  0,  0,  0,  0 },
            };
            public static int[][] QueenTable { get; } =
            {
                new int[] { -20,-10,-10, -5, -5,-10,-10,-20 },
                new int[] { -10,  0,  0,  0,  0,  0,  0,-10 },
                new int[] { -10,  0,  5,  5,  5,  5,  0,-10 },
                new int[] {  -5,  0,  5,  5,  5,  5,  0, -5 },
                new int[] {   0,  0,  5,  5,  5,  5,  0, -5 },
                new int[] { -10,  5,  5,  5,  5,  5,  0,-10 },
                new int[] { -10,  0,  5,  0,  0,  0,  0,-10 },
                new int[] { -20,-10,-10, -5, -5,-10,-10,-20 },
            };
            public static int[][] QueenTableBlack { get; } =
            {
                new int[] { -20,-10,-10, -5, -5,-10,-10,-20 },
                new int[] { -10,  0,  5,  0,  0,  0,  0,-10 },
                new int[] { -10,  5,  5,  5,  5,  5,  0,-10 },
                new int[] {   0,  0,  5,  5,  5,  5,  0, -5 },
                new int[] {  -5,  0,  5,  5,  5,  5,  0, -5 },
                new int[] { -10,  0,  5,  5,  5,  5,  0,-10 },
                new int[] { -10,  0,  0,  0,  0,  0,  0,-10 },
                new int[] { -20,-10,-10, -5, -5,-10,-10,-20 },
            };
            public static int[][] KingTable { get; } =
            {
                new int[] { -30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] { -30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] { -30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] { -30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] { -20,-30,-30,-40,-40,-30,-30,-20 },
                new int[] { -10,-20,-20,-20,-20,-20,-20,-10 },
                new int[] {  20, 20,  0,  0,  0,  0, 20, 20 },
                new int[] {  20, 30, 10,  0,  0, 10, 30, 20 },
            };
            public static int[][] KingTableBlack { get; } =
            {
                new int[] {  20, 30, 10,  0,  0, 10, 30, 20 },
                new int[] {  20, 20,  0,  0,  0,  0, 20, 20 },
                new int[] { -10,-20,-20,-20,-20,-20,-20,-10 },
                new int[] { -20,-30,-30,-40,-40,-30,-30,-20 },
                new int[] { -30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] { -30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] { -30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] { -30,-40,-40,-50,-50,-40,-40,-30 },
            };
            public static int[][] KingTableEndgame { get; } =
            {
                new int[] { -50,-40,-30,-20,-20,-30,-40,-50 },
                new int[] { -30,-20,-10,  0,  0,-10,-20,-30 },
                new int[] { -30,-10, 20, 30, 30, 20,-10,-30 },
                new int[] { -30,-10, 30, 40, 40, 30,-10,-30 },
                new int[] { -30,-10, 30, 40, 40, 30,-10,-30 },
                new int[] { -30,-10, 20, 30, 30, 20,-10,-30 },
                new int[] { -30,-30,  0,  0,  0,  0,-30,-30 },
                new int[] { -50,-30,-30,-30,-30,-30,-30,-50 },
            };
            public static int[][] KingTableBlackEndgame { get; } =
            {
                new int[] { -50,-30,-30,-30,-30,-30,-30,-50 },
                new int[] { -30,-30,  0,  0,  0,  0,-30,-30 },
                new int[] { -30,-10, 20, 30, 30, 20,-10,-30 },
                new int[] { -30,-10, 30, 40, 40, 30,-10,-30 },
                new int[] { -30,-10, 30, 40, 40, 30,-10,-30 },
                new int[] { -30,-10, 20, 30, 30, 20,-10,-30 },
                new int[] { -30,-20,-10,  0,  0,-10,-20,-30 },
                new int[] { -50,-40,-30,-20,-20,-30,-40,-50 },
            };
        }
    }
}
