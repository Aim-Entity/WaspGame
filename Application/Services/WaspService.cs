using Application.Abstracts;
using Application.Abstracts.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class WaspService : IWaspService
    {
        private readonly IWaspRepository _waspRepository;

        public WaspService(IWaspRepository waspRepository)
        {
            _waspRepository = waspRepository;
        }
    }
}
