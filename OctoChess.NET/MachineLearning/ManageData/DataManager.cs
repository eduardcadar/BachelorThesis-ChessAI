using ChessGameLibrary;
using MachineLearning.PGN;
using System.Text;
using static MachineLearning.ManageData.DataUtils;

namespace MachineLearning.ManageData
{
    public class DataManager
    {
        public static FilePositions GetTrainPositionsFromFolder(string folder)
        {
            FilePositions fp = new();
            foreach (string file in Directory.GetFiles(folder))
                fp.Add(GetPositionsFromFile(file));
            return fp;
        }

        public static FilePositions GetPositionsFromFile(string fileName)
        {
            List<float[]> positions = new();
            List<float> results = new();
            string[] lines = File.ReadAllLines(fileName)[1..];
            foreach (string line in lines)
            {
                var f = line.Split(',')
                    .Select(nr => float.Parse(nr))
                    .ToArray();
                results.Add(f[^1]);
                positions.Add(f[..^1]);
            }
            return new FilePositions(
                CreateRectangularArray(positions), results.ToArray());
        }

        public void WritePGNGameToFile(string fileName, PGNGame pgnGame)
        {
            string toWrite = GetPositionsStringFromPGNGame(pgnGame);
            if (!File.Exists(fileName))
                File.AppendAllText(fileName, DataUtils.HEADER + Environment.NewLine);
            File.AppendAllText(fileName, toWrite);
        }

        public string GetPositionsStringFromPGNGame(PGNGame pgnGame)
        {
            StringBuilder sb = new();
            Game game = new();
            game.SetPositionFromFEN(Utils.STARTING_FEN);

            foreach (PGNMove move in pgnGame.Moves)
            {
                string positionEncoding = GamePositionToString(game);
                string result = "0.5";
                if (pgnGame.GameResult == GameResult.WHITE)
                    result = "1";
                else if (pgnGame.GameResult == GameResult.BLACK)
                    result = "0";
                sb.Append(positionEncoding)
                    .Append(',')
                    .AppendLine(result);

                game.Move(move.From, move.To, move.PromotedTo, fiftyMovesRule: false);
            }

            return sb.ToString();
        }

        public string GamePositionToString(Game game)
        {
            StringBuilder sb = new();

            foreach (Square square in game.Board.Squares)
            {
                sb.Append(DataUtils.EncodePiece(square.Piece))
                    .Append(',');
            }

            if (game.WhiteCanCastleKing)
                sb.Append("1,");
            else
                sb.Append("0,");
            if (game.WhiteCanCastleQueen)
                sb.Append("1,");
            else
                sb.Append("0,");
            if (game.BlackCanCastleKing)
                sb.Append("1,");
            else
                sb.Append("0,");
            if (game.BlackCanCastleQueen)
                sb.Append("1,");
            else
                sb.Append("0,");

            if (game.EnPassantSquare == null)
                sb.Append("-1,");
            else
                sb.Append(game.EnPassantSquare.File)
                    .Append(',');

            if (game.PlayerToMove == ChessGameLibrary.Enums.PieceColor.WHITE)
                sb.Append('0');
            else
                sb.Append('1');

            return sb.ToString();
        }
    }
}
