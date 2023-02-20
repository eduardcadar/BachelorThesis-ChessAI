using System;

namespace ChessGameLibrary
{
    public class Program
    {
        static void Main()
        {
            Game game = new Game();
            game.SetPositionFromFEN(Utils.STARTING_FEN);
            game.Move(new SquareCoords(5, 1), new SquareCoords(5, 3));
            Console.WriteLine(game.State);
            game.Move(new SquareCoords(4, 6), new SquareCoords(4, 4));
            Console.WriteLine(game.State);
            game.Move(new SquareCoords(6, 1), new SquareCoords(6, 3));
            Console.WriteLine(game.State);
            game.Move(new SquareCoords(3, 7), new SquareCoords(7, 3));
            Console.WriteLine(game.State);
            Console.WriteLine(game.Board.ToString());

            foreach (var move in game.LegalMoves)
                Console.WriteLine(move.ToString());
        }
    }
}
