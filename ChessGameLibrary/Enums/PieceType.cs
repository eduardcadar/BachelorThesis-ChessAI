namespace ChessGameLibrary.Enums
{
    public enum PieceType
    {
        NONE = 0,
        PAWN = 1,
        KNIGHT = 2,
        BISHOP = 3,
        ROOK = 4,
        QUEEN = 5,
        KING = 6
    }

    public static class PieceTypeExtensions
    {
        public static char GetLetterName(this PieceType pieceType)
        {
            return pieceType switch
            {
                PieceType.PAWN => 'P',
                PieceType.KNIGHT => 'N',
                PieceType.BISHOP => 'B',
                PieceType.ROOK => 'R',
                PieceType.QUEEN => 'Q',
                PieceType.KING => 'K',
                _ => '0'
            };
        }

        public static char GetLetterNameLower(this PieceType pieceType)
        {
            return pieceType switch
            {
                PieceType.PAWN => 'p',
                PieceType.KNIGHT => 'n',
                PieceType.BISHOP => 'b',
                PieceType.ROOK => 'r',
                PieceType.QUEEN => 'q',
                PieceType.KING => 'k',
                _ => '0'
            };
        }
    }
}
