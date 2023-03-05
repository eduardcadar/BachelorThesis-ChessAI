using System;

namespace ChessGameLibrary
{
    public class Program
    {
        static void Main()
        {
            Game game = new Game();
            game.SetPositionFromFEN(Utils.STARTING_FEN);
            game.Move(new SquareCoords(2, 1), new SquareCoords(2, 3));
            game.Move(new SquareCoords(7, 6), new SquareCoords(7, 5));
            game.Move(new SquareCoords(2, 3), new SquareCoords(2, 4));
            game.Move(new SquareCoords(7, 5), new SquareCoords(7, 4));
            game.Move(new SquareCoords(2, 4), new SquareCoords(2, 5));
            game.Move(new SquareCoords(7, 4), new SquareCoords(7, 3));
            game.Move(new SquareCoords(2, 5), new SquareCoords(1, 6));
            game.Move(new SquareCoords(7, 3), new SquareCoords(7, 2));
            game.Move(new SquareCoords(1, 6), new SquareCoords(0, 7), promotedTo: Enums.PieceType.QUEEN);
            Console.WriteLine(game.Board.ToString());

            foreach (var move in game.LegalMoves)
                Console.WriteLine(move.ToString());
        }
    }
}
