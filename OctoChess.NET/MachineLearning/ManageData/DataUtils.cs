using ChessGameLibrary;

namespace MachineLearning.ManageData
{
    public static class DataUtils
    {
        public class FilePositions
        {
            public float[,] Positions { get; set; }
            public float[] Results { get; set; }

            public FilePositions()
            {
                Positions = new float[0, 0];
                Results = Array.Empty<float>();
            }

            public FilePositions(float[,] positions, float[] results)
            {
                if (positions.GetLength(0) != results.Length)
                    throw new Exception("Each position should have its result");
                Positions = positions;
                Results = results;
            }

            public void Add(FilePositions fp)
            {
                if (fp.Positions.GetLength(0) != fp.Results.Length)
                    throw new Exception("Each position should have its result");
                if (Positions.Length == 0)
                    Positions = new float[0, fp.Positions.GetLength(1)];
                Positions = ConcatArrays(Positions, fp.Positions);

                int newLength = Results.Length + fp.Results.Length;
                float[] newResults = new float[newLength];
                Results.CopyTo(newResults, 0);
                fp.Results.CopyTo(newResults, Results.Length);
                Results = newResults;
            }
        }

        public static T[,] ConcatArrays<T>(T[,] a, T[,] b)
        {
            int rowsA = a.GetLength(0);
            int colsA = a.GetLength(1);
            int rowsB = b.GetLength(0);
            int colsB = b.GetLength(1);

            if (colsA != colsB)
                throw new ArgumentException("Arrays must have the same number of columns");

            T[,] result = new T[rowsA + rowsB, colsA];

            for (int i = 0; i < rowsA; i++)
                for (int j = 0; j < colsA; j++)
                    result[i, j] = a[i, j];

            for (int i = 0; i < rowsB; i++)
                for (int j = 0; j < colsB; j++)
                    result[i + rowsA, j] = b[i, j];

            return result;
        }

        public static T[] GetRow<T>(T[,] matrix, int rowNumber)
        {
            return Enumerable
                .Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
        }

        public static T[,] CreateRectangularArray<T>(IList<T[]> arrays)
        {
            // TODO: Validation and special-casing for arrays.Count == 0
            int minorLength = arrays[0].Length;
            T[,] ret = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }

        public static readonly string HEADER =
            "a1,a2,a3,a4,a5,a6,a7,a8,"
            + "b1,b2,b3,b4,b5,b6,b7,b8,"
            + "c1,c2,c3,c4,c5,c6,c7,c8,"
            + "d1,d2,d3,d4,d5,d6,d7,d8,"
            + "e1,e2,e3,e4,e5,e6,e7,e8,"
            + "f1,f2,f3,f4,f5,f6,f7,f8,"
            + "g1,g2,g3,g4,g5,g6,g7,g8,"
            + "h1,h2,h3,h4,h5,h6,h7,h8,"
            + "WhiteCanCastleKing,"
            + "WhiteCanCastleQueen,"
            + "BlackCanCastleKing,"
            + "BlackCanCastleQueen,"
            + "EnPassantFile,"
            + "PlayerToMove,"
            + "Result";

        // NO PIECE = 0
        // WHITE PAWN = 1
        // WHITE KNIGHT = 2
        // WHITE BISHOP = 3
        // WHITE ROOK = 4
        // WHITE QUEEN = 5
        // WHITE KING = 6
        // BLACK PAWN = 7
        // BLACK KNIGHT = 8
        // BLACK BISHOP = 9
        // BLACK ROOK = 10
        // BLACK QUEEN = 11
        // BLACK KING = 12
        public static byte EncodePiece(Piece piece)
        {
            if (piece == null)
                return 0;

            byte encoding = 1;

            if (piece.Color == ChessGameLibrary.Enums.PieceColor.BLACK)
                encoding += 6;

            switch (piece.Type)
            {
                case ChessGameLibrary.Enums.PieceType.KNIGHT:
                    encoding += 1;
                    break;
                case ChessGameLibrary.Enums.PieceType.BISHOP:
                    encoding += 2;
                    break;
                case ChessGameLibrary.Enums.PieceType.ROOK:
                    encoding += 3;
                    break;
                case ChessGameLibrary.Enums.PieceType.QUEEN:
                    encoding += 4;
                    break;
                case ChessGameLibrary.Enums.PieceType.KING:
                    encoding += 5;
                    break;
                default:
                    break;
            }

            return encoding;
        }
    }
}
