using ChessGameLibrary;
using ChessGameLibrary.Enums;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Root
    {
        public From From { get; set; }
        public To To { get; set; }
        public int PromotedTo { get; set; }
        public int Evaluation { get; set; }
    }

    public class From
    {
        public int File { get; set; }
        public int Rank { get; set; }
    }

    public class To
    {
        public int File { get; set; }
        public int Rank { get; set; }
    }

    public class MoveEvalDTO
    {
        public SquareCoords From { get; set; }
        public SquareCoords To { get; set; }
        public PieceType PromotedTo { get; set; }
        public double Evaluation { get; set; }

        public override string ToString()
        {
            return From
                + "-"
                + To
                + (PromotedTo != PieceType.NONE ? PromotedTo.ToString()[0] : "")
                + " "
                + Evaluation;
        }
    }

    public static class EngineUtils
    {
        private static readonly HttpClient _httpClient =
            new() { BaseAddress = new Uri("https://localhost:7091/api/engine/") };

        public enum PlayerType
        {
            HUMAN,
            ENGINE
        }

        public enum EvaluationType
        {
            MATERIAL = 0,
            TRAINED_MODEL = 1
        }

        public static int Depth { get; set; } = 3;
        public static bool UseAlphaBetaPruning { get; set; } = true;
        public static EvaluationType EvalType { get; set; } = EvaluationType.MATERIAL;
        public static bool UseIterativeDeepening { get; set; } = false;
        public static int TimeLimit { get; set; } = 50;
        public static bool UseQuiescenceSearch { get; set; } = false;
        public static int MaxQuiescenceDepth { get; set; } = 4;

        public static async Task PrepareEngine()
        {
            await _httpClient.GetAsync("prepare");
            UnityEngine.Debug.Log("Engine prepared...");
        }

        public static async Task<SimpleMove> GetEngineBestMove(string fen)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["fen"] = fen;
            query["depth"] = Depth.ToString();
            query["useAlphaBetaPruning"] = UseAlphaBetaPruning.ToString();
            query["evaluationType"] = ((int)EvalType).ToString();
            query["useIterativeDeepening"] = UseIterativeDeepening.ToString();
            query["timeLimit"] = TimeLimit.ToString();
            query["useQuiescenceSearch"] = UseQuiescenceSearch.ToString();
            query["maxQuiescenceDepth"] = MaxQuiescenceDepth.ToString();
            string queryString = "?" + query.ToString();
            var response = await _httpClient.GetAsync("bestMove" + queryString);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<Root>(jsonResponse);

            var moveEvalDTO = new MoveEvalDTO()
            {
                From = new SquareCoords(root.From.File, root.From.Rank),
                To = new SquareCoords(root.To.File, root.To.Rank),
                PromotedTo = (PieceType)root.PromotedTo,
                Evaluation = root.Evaluation,
            };

            return new SimpleMove(
                moveEvalDTO.From,
                moveEvalDTO.To,
                promotedTo: moveEvalDTO.PromotedTo
            );
        }
    }
}
