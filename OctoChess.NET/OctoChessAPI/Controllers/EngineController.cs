using Microsoft.AspNetCore.Mvc;
using OctoChessAPI.DTO;
using OctoChessEngine;
using OctoChessEngine.Domain;
using OctoChessEngine.Enums;
using System.Text.Json;

namespace OctoChessAPI.Controllers
{
    [Route("api/engine")]
    [ApiController]
    public class EngineController : ControllerBase
    {
        private readonly OctoChess _octoChess;

        public EngineController(OctoChess octoChess)
        {
            _octoChess = octoChess;
        }

        [Route("prepare")]
        [HttpGet]
        public async Task<ActionResult> InitializeEngine()
        {
            await Task.Run(() => Console.WriteLine("Prepared engine..."));
            return Ok();
        }

        [Route("bestMove")]
        [HttpGet]
        public async Task<ActionResult<MoveEvalDTO>> GetBestMove(
            string fen,
            int depth = 3,
            bool useAlphaBetaPruning = true,
            EvaluationType evaluationType = EvaluationType.MATERIAL,
            bool useIterativeDeepening = true,
            int timeLimit = 30,
            bool useQuiescenceSearch = true,
            int maxQuiescenceDepth = 4,
            CancellationToken cancellationToken = default
        )
        {
            string decodedFen = System.Web.HttpUtility.UrlDecode(fen);
            _octoChess.SetFenPosition(decodedFen);
            MoveEval bestMove = await _octoChess.BestMove(
                maxDepth: depth,
                useAlphaBetaPruning: useAlphaBetaPruning,
                evaluationType: evaluationType,
                useIterativeDeepening: useIterativeDeepening,
                timeLimit: timeLimit,
                useQuiescenceSearch: useQuiescenceSearch,
                maxQuiescenceDepth: maxQuiescenceDepth,
                cancellationToken: cancellationToken
            );
            MoveEvalDTO moveEvalDTO = new(bestMove);
            string jsonMove = JsonSerializer.Serialize(moveEvalDTO);
            Console.WriteLine(
                $"Best move: {bestMove.From}-{bestMove.To}{(bestMove.PromotedTo != ChessGameLibrary.Enums.PieceType.NONE ? bestMove.PromotedTo : "")}"
            );

            return Ok(jsonMove);
        }
    }
}
