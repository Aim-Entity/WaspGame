using Application.Abstracts;
using Application.Abstracts.Repositories;
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
    }
}
