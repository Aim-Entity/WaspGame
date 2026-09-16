using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstracts
{
    public interface IGameService
    {
        public Task<IEnumerable<Game>> GetGamesAsync();

        public Task<Game> CreateGameAsync();

        public Task RecoverWaspsAsync();
    }
}
