using System;

namespace ChessGameLibrary
{
    public class Program
    {
        static void Main()
        {
            Game game = new Game();
            game.SetPositionFromFEN(Utils.STARTING_FEN);
            game.Move(new SquareCoords(6, 1), new SquareCoords(6, 2), Enums.PieceType.NONE);
            game.Move(new SquareCoords(3, 6), new SquareCoords(3, 4), Enums.PieceType.NONE);
            game.Move(new SquareCoords(2, 3), new SquareCoords(3, 4), Enums.PieceType.NONE);
            game.Move(new SquareCoords(2, 6), new SquareCoords(2, 4), Enums.PieceType.NONE);
            game.Move(new SquareCoords(3, 4), new SquareCoords(2, 5), Enums.PieceType.NONE);
            Console.WriteLine(game.Board.ToString());

            var moves = game.MoveSquares(new SquareCoords(4, 7));
            foreach (var move in moves)
                Console.WriteLine(move.ToString());
        }
    }
}
