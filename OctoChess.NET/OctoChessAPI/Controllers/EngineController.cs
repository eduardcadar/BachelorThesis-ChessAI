using Microsoft.AspNetCore.Mvc;
using OctoChessAPI.DTO;
using OctoChessEngine.Domain;
using OctoChessEngine.Engines;

namespace OctoChessAPI.Controllers
{
    [Route("api/engine")]
    [ApiController]
    public class EngineController : ControllerBase
    {
        private readonly OctoChess _octoChess = new();

        [Route("bestMove/{fen}")]
        [HttpGet]
        public async Task<ActionResult<MoveEvalDTO>> GetBestMove(string fen, CancellationToken cancellationToken = default)
        {
            _octoChess.SetFenPosition(fen);
            MoveEval bestMove = await _octoChess.BestMove(depth: 3, alphaBetaPruning: true, cancellationToken: cancellationToken);
            MoveEvalDTO moveEvalDTO = new(bestMove);
            return Ok(moveEvalDTO);
        }
    }
}
