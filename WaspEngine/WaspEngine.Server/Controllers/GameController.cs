using Application.Abstracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WaspEngine.Server.Controllers
{
    [ApiController]
    [Route("api/V1/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("Game", Name = "GetGames")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            var games = await _gameService.GetGamesAsync();

            return Ok(games);
        }

        [HttpPost("Create", Name = "CreateGame")]
        public async Task<ActionResult<Game>> CreateGame()
        {
            var game = await _gameService.CreateGameAsync();

            return Ok(game);
        }
    }
}
