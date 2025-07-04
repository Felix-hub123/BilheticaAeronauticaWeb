using BilheticaAeronauticaWeb.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IVooRepository : IGenericRepository<Voo>
    {
        Task<List<Voo>> GetAllVoosAsync();

        Task<List<Voo>> GetAllVoosWithIncludesAsync();

        Task<Voo> GetVooWithIncludesAsync(int id);

        Task<IEnumerable<Voo>> GetVoosByUtilizadorIdAsync(int utilizadorId);

    }
}
