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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Game>> GetCurrentGame()
        {
            var game = await _gameService.GetCurrentGameAsync();

            return Ok(game);
        }

        [HttpPost("reset", Name = "ResetGame")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Game>> ResetGame()
        {
            var game = await _gameService.ResetGameAsync();

            return Ok(game);
        }

        [HttpPost("zap", Name = "ZapCurrentGame")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ZapResult>> Zap()
        {
            var result = await _gameService.ZapAsync();

            return Ok(result);
        }
    }
}
