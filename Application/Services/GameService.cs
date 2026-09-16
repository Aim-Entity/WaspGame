using Application.Abstracts;
using Application.Abstracts.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task CreateGameAsync()
        {
            var Game = new Game();
            await _gameRepository.AddAsync(Game);
        }

        public Task<IEnumerable<Game>> GetGamesAsync()
        {
            return _gameRepository.GetAllAsync();
        }

        public Task RecoverWaspsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
