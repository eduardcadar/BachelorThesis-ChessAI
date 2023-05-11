using ChessGameLibrary;
using ChessGameLibrary.Enums;
using System.Text;
using System.Text.RegularExpressions;

namespace MachineLearning.PGN
{
    public static partial class PGNParser
    {
        private static Game _game = new();

        //private static readonly string _lineSeparator = "\n";

        public static IEnumerable<PGNGame> ParsePGNFile(string filePath)
        {
            string fileContent = File.ReadAllText(filePath, Encoding.UTF8).Trim();
            string[] gameStrings = Regex.Split(fileContent, "\r\n\r\n\r\n");
            if (gameStrings.Length == 1) // if line terminator is not windows style, but linux
                gameStrings = Regex.Split(fileContent, "\n\n\n");
            foreach (string gameString in gameStrings)
            {
                string[] gameTagsAndMoves = Regex.Split(gameString, "\r\n\r\n");
                if (gameTagsAndMoves.Length == 1)
                    gameTagsAndMoves = Regex.Split(gameString, "\n\n");
                if (gameTagsAndMoves.Length != 2)
                    throw new ParserException("gameDataAndMoves length should be 2!");

                PGNTag[] gameTags = GetTagsFromTagsText(gameTagsAndMoves[0]);

                PGNMove[] moves = GetPGNMovesFromMovesText(
                    RemoveResultFromMoves(gameTagsAndMoves[1])
                );

                GameResult result = GetResultFromTag(
                    gameTags.Single(t => t.Key.Equals("Result")).Value
                );

                yield return new(gameTags, moves, result);
            }
        }

        private static GameResult GetResultFromTag(string resultTag)
        {
            return resultTag switch
            {
                "1-0" => GameResult.WHITE,
                "0-1" => GameResult.BLACK,
                "1/2-1/2" => GameResult.DRAW,
                _ => throw new ParserException("Wrong result string")
            };
        }

        private static string RemoveResultFromMoves(string movesText) =>
            string.Join(' ', movesText.Split(' ')[..^1]);

        private static PGNMove[] GetPGNMovesFromMovesText(string movesText)
        {
            _game = new();
            _game.SetPositionFromFEN(Utils.STARTING_FEN);
            string movesString = Regex.Replace(movesText, "(\r\n)|(\n)", " ");
            movesString = Regex.Replace(movesString, @"[0-9]+\.", " ").Trim();
            movesString = Regex.Replace(movesString, @"[ ]{2,}", " ");
            return movesString
                .Split(' ', StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries)
                .Select(GetPGNMoveFromMoveText)
                .ToArray();
        }

        private static PGNMove GetPGNMoveFromMoveText(string moveText)
        {
            PGNMove pgnMove =
                new()
                {
                    IsCheck = moveText[^1] == '+' || moveText[^1] == '#',
                    IsCheckMate = moveText[^1] == '#'
                };
            if (pgnMove.IsCheck)
                moveText = moveText[..^1];
            if (IsCastleMove(moveText))
                HandleCastleData(moveText, pgnMove);
            else
            {
                if (IsPromotionMove(moveText))
                {
                    pgnMove.IsPromotion = true;
                    pgnMove.PromotedTo = Utils.GetPieceTypeFromPieceLetter(moveText[^1]);
                    moveText = moveText[..^2];
                }
                if (char.IsUpper(moveText[0]))
                {
                    pgnMove.PieceType = Utils.GetPieceTypeFromPieceLetter(moveText[0]);
                    moveText = moveText[1..];
                }
                else
                    pgnMove.PieceType = PieceType.PAWN;

                if (IsCaptureMove(moveText))
                {
                    pgnMove.IsCapture = true;
                    moveText = moveText.Replace("x", string.Empty);
                }
                pgnMove.To = new(moveText[^2..]);
                moveText = moveText[..^2];
                if (moveText.Length == 2)
                {
                    pgnMove.From = new SquareCoords(moveText);
                }
                else
                {
                    SimpleMove[] moves = _game.LegalMoves.ToArray();
                    if (moveText.Length == 1)
                    {
                        if (!char.IsNumber(moveText[0]))
                        {
                            int file = moveText[0] - 'a';
                            moves = moves.Where(m => m.From.File == file).ToArray();
                        }
                        else
                        {
                            int rank = moveText[0] - '1';
                            moves = moves.Where(m => m.From.Rank == rank).ToArray();
                        }
                    }
                    pgnMove.From = moves
                        .Single(
                            m =>
                                m.To.Equals(pgnMove.To)
                                && _game.Board
                                    .GetSquare(m.From)
                                    .Piece.Type.Equals(pgnMove.PieceType)
                                && m.PromotedTo == pgnMove.PromotedTo
                        )
                        .From;
                }
            }
            SimpleMove simpleMove = GetSimpleMoveFromPGNMove(pgnMove);
            _game.Move(
                simpleMove.From,
                simpleMove.To,
                simpleMove.PromotedTo,
                fiftyMovesRule: false
            );
            return pgnMove;
        }

        private static SimpleMove GetSimpleMoveFromPGNMove(PGNMove pgnMove) =>
            new(pgnMove.From, pgnMove.To, pgnMove.PromotedTo);

        private static bool IsCaptureMove(string moveText) => moveText.Contains('x');

        private static bool IsPromotionMove(string moveText) => moveText[^2] == '=';

        private static bool IsCastleMove(string moveText) => moveText.Contains("O-O");

        private static void HandleCastleData(string moveText, PGNMove pgnMove)
        {
            pgnMove.PieceType = PieceType.KING;
            const int fromFile = 4;
            int rank = 0,
                toFile = 6;
            pgnMove.IsCastle = true;
            if (_game.PlayerToMove == PieceColor.BLACK)
                rank = 7;
            if (moveText.Contains("O-O-O"))
                toFile = 2;
            pgnMove.From = new SquareCoords(fromFile, rank);
            pgnMove.To = new SquareCoords(toFile, rank);
            pgnMove.IsCapture = false;
            pgnMove.IsPromotion = false;
        }

        private static PGNTag[] GetTagsFromTagsText(string tagsText)
        {
            string[] gameTagsStrings = Regex.Split(tagsText, "\r\n");
            if (gameTagsStrings.Length == 1)
                gameTagsStrings = Regex.Split(tagsText, "\n");
            return gameTagsStrings.Select(GetTagFromLine).ToArray();
        }

        private static PGNTag GetTagFromLine(string line)
        {
            string tagPattern = @"\[(?<key>.*) ""(?<value>.*)""\]";
            Regex expression = new(tagPattern);
            Match m = expression.Match(line);
            if (!m.Success)
                throw new ParserException("String is not in correct tag format");

            string key = m.Groups["key"].Value;
            string value = m.Groups["value"].Value;
            return new PGNTag(key, value);
        }
    }
}
