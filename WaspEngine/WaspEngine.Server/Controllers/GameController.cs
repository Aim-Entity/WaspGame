using Application.Abstracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WaspEngine.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet(Name = "GetCurrentGame")]
        public async Task<ActionResult<Game>> GetCurrentGame(CancellationToken cancellationToken)
        {
            var game = await _gameService.GetOrCreateCurrentGameAsync(cancellationToken);

            return Ok(game);
        }

        [HttpPost("reset", Name = "ResetGame")]
        public async Task<ActionResult<Game>> ResetGame(CancellationToken cancellationToken)
        {
            var game = await _gameService.ResetGameAsync(cancellationToken);

            return Ok(game);
        }

        [HttpPost("zap", Name = "ZapCurrentGame")]
        public async Task<ActionResult<ZapResult>> Zap(CancellationToken cancellationToken)
        {
            var result = await _gameService.ZapAsync(cancellationToken);

            return Ok(result);
        }
    }
}
