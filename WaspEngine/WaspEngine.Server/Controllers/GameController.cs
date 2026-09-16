using Application.Abstracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WaspEngine.Server.Controllers
{
    [ApiController]
    [Route("Api/V1/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService ggameService)
        {
            _gameService = ggameService;
        }

        [HttpGet(Name = "Game")]
        public async Task<IEnumerable<Game>> Game()
        {
            return await _gameService.GetGamesAsync();
        }

        [HttpPost(Name = "Create")]
        public async Task Create()
        {
            await _gameService.CreateGameAsync();
        }
    }
}
