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

        [HttpGet("all", Name = "GetGames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            var games = await _gameService.GetGamesAsync();

            return Ok(games);
        }

        [HttpGet("{id:long}", Name = "GetGameById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Game>> GetGameById(long id)
        {
            var game = await _gameService.GetGameAsync(id);

            return game is null ? NotFound() : Ok(game);
        }

        [HttpPost(Name = "CreateGame")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Game>> CreateGame()
        {
            var game = await _gameService.CreateGameAsync();

            return CreatedAtRoute("GetGameById", new { id = game.Id }, game);
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

        [HttpPost("{id:long}/zap", Name = "ZapGame")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ZapResult>> Zap(long id)
        {
            var result = await _gameService.ZapAsync(id);

            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("recover", Name = "RecoverGameWasps")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Game>> RecoverWasps()
        {
            await _gameService.RecoverWaspsAsync();

            var game = await _gameService.GetCurrentGameAsync();

            return Ok(game);
        }

        [HttpDelete("{id:long}", Name = "DeleteGame")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGame(long id)
        {
            var deleted = await _gameService.DeleteGameAsync(id);

            return deleted ? NoContent() : NotFound();
        }
    }
}
