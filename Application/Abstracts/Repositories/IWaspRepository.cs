using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstracts.Repositories
{
    public interface IWaspRepository
    {
        public Task<IEnumerable<Wasp>> GetAllAsync();
    }
}
