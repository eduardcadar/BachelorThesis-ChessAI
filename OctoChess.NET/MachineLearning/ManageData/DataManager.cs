using ChessGameLibrary;
using MachineLearning.PGN;
using System.Text;
using static MachineLearning.ManageData.DataUtils;

namespace MachineLearning.ManageData
{
    public static class DataManager
    {
        private static readonly int CHECKMATE_POINTS = 318;

        public static FilePositions GetMeanOfPositionResults(string folder)
        {
            int positionsCount = 0;
            Dictionary<string, List<float>> positionsResults = new();
            foreach (string file in Directory.GetFiles(folder))
            {
                Console.WriteLine(file);
                string[] lines = File.ReadAllLines(file)[1..];
                foreach (string line in lines)
                {
                    string[] split = line.Split(',');
                    float result = float.Parse(split[^1]);
                    string position = line[..^(split[^1].Length + 1)];
                    if (!positionsResults.ContainsKey(position))
                        positionsResults.Add(position, new());
                    positionsResults[position].Add(result);
                    positionsCount++;
                }
            }
            List<float[]> positions = new();
            List<float> values = new();
            int i = 0;
            int a = positionsResults.Keys.Count / 10;
            foreach (string key in positionsResults.Keys)
            {
                if (i % a == 0)
                    Console.Write($"{i / a * 10}% ");
                i++;
                positions.Add(EncodedPositionStringToFloatArray(key));
                values.Add(positionsResults[key].Average());
            }
            Console.WriteLine();
            Console.WriteLine($"Number of positions: {positionsCount}");
            Console.WriteLine($"Number of distinct positions: {positionsResults.Keys.Count}");
            return new(CreateRectangularArray(positions), values.ToArray());
        }

        public static PositionsEvals GetPositionsEvalsFromFolder(
            string folder,
            PositionsFileType type = PositionsFileType.CHESSDATA
        )
        {
            PositionsEvals pe = new();
            string fileContains = "chessData";
            switch (type)
            {
                case PositionsFileType.CHESSDATA:
                    fileContains = "chessData";
                    break;
                case PositionsFileType.RANDOM_EVALS:
                    fileContains = "random_evals";
                    break;
                case PositionsFileType.TACTIC_EVALS:
                    fileContains = "tactic_evals";
                    break;
            }
            foreach (string file in Directory.GetFiles(folder).Where(f => f.Contains(fileContains)))
                pe.Add(
                    GetPositionsEvalsFromFile(
                        file,
                        containsBestMove: type == PositionsFileType.TACTIC_EVALS
                    )
                );
            return pe;
        }

        public static PositionsEvals GetPositionsEvalsFromFile(
            string fileName,
            bool containsBestMove = false
        )
        {
            Console.WriteLine("file: " + fileName);
            Game game = new();
            List<float[]> positions = new();
            List<float> evals = new();
            string[] lines = File.ReadAllLines(fileName)[1..];
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                string fen = values[0];
                string eval = values[1];

                game.SetPositionFromFEN(fen);
                string repr = GamePositionToDataString(game);
                float[] position = EncodedPositionStringToFloatArray(repr);

                float evalPoints;
                if (eval.Contains('#'))
                    evalPoints = CHECKMATE_POINTS * (eval.Contains('-') ? -1 : 1);
                else
                    evalPoints = int.Parse(eval);

                positions.Add(position);
                evals.Add(evalPoints / 100);

                if (evals.Count > 1000) // REMOVE AFTER TESTING //////////////////////////////////////////
                    break;
            }

            return new PositionsEvals(CreateRectangularArray(positions), evals.ToArray());
        }

        public static float[] EncodedPositionStringToFloatArray(string position) =>
            position.Split(',').Select(float.Parse).ToArray();

        public static FilePositions GetTrainPositionsFromFolder(string folder)
        {
            FilePositions fp = new();
            foreach (string file in Directory.GetFiles(folder))
                fp.Add(GetPositionsFromFile(file));
            return fp;
        }

        public static FilePositions GetPositionsFromFile(string fileName)
        {
            Console.WriteLine("positions from: " + fileName);
            List<float[]> positions = new();
            List<float> results = new();
            string[] lines = File.ReadAllLines(fileName)[1..];
            foreach (string line in lines)
            {
                var f = EncodedPositionStringToFloatArray(line);
                results.Add(f[^1]);
                positions.Add(f[..^1]);
            }
            return new FilePositions(CreateRectangularArray(positions), results.ToArray());
        }

        //
        public static void WritePGNGameToFile(string fileName, PGNGame pgnGame)
        {
            string toWrite = GetPositionsStringFromPGNGame(pgnGame);
            if (!File.Exists(fileName))
                File.AppendAllText(fileName, HEADER + Environment.NewLine);
            File.AppendAllText(fileName, toWrite);
        }

        //
        public static string GetPositionsStringFromPGNGame(PGNGame pgnGame)
        {
            StringBuilder sb = new();
            Game game = new();
            game.SetPositionFromFEN(Utils.STARTING_FEN);

            foreach (PGNMove move in pgnGame.Moves)
            {
                string positionEncoding = GamePositionToDataString(game);
                string result = "0";
                if (pgnGame.GameResult == GameResult.WHITE)
                    result = "1";
                else if (pgnGame.GameResult == GameResult.BLACK)
                    result = "-1";
                sb.Append(positionEncoding).Append(',').AppendLine(result);

                game.Move(move.From, move.To, move.PromotedTo, fiftyMovesRule: false);
            }

            return sb.ToString();
        }

        // Takes position from game and turns it into encoded string
        public static string GamePositionToDataString(Game game)
        {
            StringBuilder sb = new();

            foreach (Square square in game.Board.Squares)
            {
                sb.Append(EncodePiece(square.Piece)).Append(',');
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
                sb.Append(game.EnPassantSquare.File).Append(',');

            if (game.PlayerToMove == ChessGameLibrary.Enums.PieceColor.WHITE)
                sb.Append('0');
            else
                sb.Append('1');

            return sb.ToString();
        }
    }
}
