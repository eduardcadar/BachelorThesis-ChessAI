using System.Text;

namespace ChessGameLibrary
{
    public class Board
    {
        public Square[,] Squares = new Square[Utils.FILES_COUNT, Utils.RANKS_COUNT];

        public Board()
        {
            for (int i = 0; i < Squares.GetLength(0); i++)
                for (int j = 0; j < Squares.GetLength(1); j++)
                    Squares[i, j] = new Square(new SquareCoords(i, j), null);
        }

        public Square GetSquare(SquareCoords squareCoords)
        {
            return Squares[squareCoords.File, squareCoords.Rank];
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int j = Squares.GetLength(1) - 1; j >= 0; j--)
            {
                for (int i = 0; i < Squares.GetLength(0); i++)
                    sb.Append(Squares[i, j].ToString()).Append(' ');
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
