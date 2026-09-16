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

        public Task<Game> CreateGameAsync()
        {
            return _gameRepository.AddAsync(Game.Create());
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
