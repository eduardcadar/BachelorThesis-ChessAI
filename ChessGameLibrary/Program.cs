using System;

namespace ChessGameLibrary
{
    public class Program
    {
        static void Main()
        {
            Game game = new Game();
            game.SetPositionFromFEN(Utils.STARTING_FEN);
            game.Move(new SquareCoords(6, 1), new SquareCoords(6, 2));
            Console.WriteLine(game.WhiteCanCastleKing);
            game.Move(new SquareCoords(1, 6), new SquareCoords(1, 5));
            Console.WriteLine(game.WhiteCanCastleKing);
            game.Move(new SquareCoords(5, 0), new SquareCoords(6, 1));
            Console.WriteLine(game.WhiteCanCastleKing);
            game.Move(new SquareCoords(2, 7), new SquareCoords(1, 6));
            Console.WriteLine(game.WhiteCanCastleKing);
            game.Move(new SquareCoords(6, 0), new SquareCoords(7, 2));
            Console.WriteLine(game.WhiteCanCastleKing);
            game.Move(new SquareCoords(1, 6), new SquareCoords(6, 1));
            Console.WriteLine(game.WhiteCanCastleKing);
            game.Move(new SquareCoords(4, 1), new SquareCoords(4, 2));
            Console.WriteLine(game.WhiteCanCastleKing);
            game.Move(new SquareCoords(6, 1), new SquareCoords(7, 0));
            Console.WriteLine(game.WhiteCanCastleKing);
            Console.WriteLine(game.Board.ToString());

            //foreach (var move in game.LegalMoves)
            //    Console.WriteLine(move.ToString());
        }
    }
}
