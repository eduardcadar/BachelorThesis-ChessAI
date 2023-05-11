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
    }

    public static class EngineUtils
    {
        private static HttpClient _httpClient =
            new() { BaseAddress = new Uri("https://localhost:7091/api/engine/bestMove") };

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

        public static async Task<SimpleMove> GetEngineBestMove(
            string fen,
            int depth,
            bool useAlphaBetaPruning,
            bool useIterativeDeepening,
            int timeLimit,
            bool useQuiescenceSearch,
            int maxQuiescenceDepth,
            EvaluationType evaluationType
        )
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["fen"] = fen;
            query["depth"] = depth.ToString();
            query["useAlphaBetaPruning"] = useAlphaBetaPruning.ToString();
            query["evaluationType"] = evaluationType.ToString();
            query["useIterativeDeepening"] = useIterativeDeepening.ToString();
            query["timeLimit"] = timeLimit.ToString();
            query["useQuiescenceSearch"] = useQuiescenceSearch.ToString();
            query["maxQuiescenceDepth"] = maxQuiescenceDepth.ToString();
            string queryString = "?" + query.ToString();
            var response = await _httpClient.GetAsync(queryString);
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
