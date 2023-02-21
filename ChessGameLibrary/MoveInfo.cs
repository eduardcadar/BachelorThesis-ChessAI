using ChessGameLibrary.Enums;

namespace ChessGameLibrary
{
    public class MoveInfo
    {
        public MoveType MoveType { get; set; }
        public Piece Piece { get; set; }
        public PieceType PromotedTo { get; set; }
        public Piece CapturedPiece { get; set; }
        public SquareCoords From { get; set; }
        public SquareCoords To { get; set; }
        public SquareCoords PrevEnPassantSquare { get; set; }
        public int PrevHalfmoveClock { get; set; }
        public bool PrevWhiteCanCastleKing { get; set; }
        public bool PrevWhiteCanCastleQueen { get; set; }
        public bool PrevBlackCanCastleKing { get; set; }
        public bool PrevBlackCanCastleQueen { get; set; }

        public MoveInfo(MoveType moveType, Piece piece, SquareCoords from, SquareCoords to,
            bool prevWhiteCanCastleKing, bool prevWhiteCanCastleQueen, bool prevBlackCanCastleKing, bool prevBlackCanCastleQueen)
        {
            MoveType = moveType;
            Piece = piece;
            From = from;
            To = to;
            PrevWhiteCanCastleKing = prevWhiteCanCastleKing;
            PrevWhiteCanCastleQueen = prevWhiteCanCastleQueen;
            PrevBlackCanCastleKing = prevBlackCanCastleKing;
            PrevBlackCanCastleQueen = prevBlackCanCastleQueen;
            PromotedTo = PieceType.NONE;
            PrevEnPassantSquare = null;
            CapturedPiece = null;
        }

        //public void SetCastleRights(bool K, bool Q, bool k, bool q)
        //{
        //    PrevWhiteCanCastleKing = K;
        //    PrevWhiteCanCastleQueen = Q;
        //    PrevBlackCanCastleKing = k;
        //    PrevBlackCanCastleQueen = q;
        //}
    }
}
