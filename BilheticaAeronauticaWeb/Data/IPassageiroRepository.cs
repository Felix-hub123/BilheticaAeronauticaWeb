using BilheticaAeronauticaWeb.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IPassageiroRepository : IGenericRepository<Passageiro>
    {
        Task<Passageiro> GetByUserIdAsync(string userId);

    }
}
