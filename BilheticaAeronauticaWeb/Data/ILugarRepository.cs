using BilheticaAeronauticaWeb.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public interface ILugarRepository : IGenericRepository<Lugar>
    {
        Task<IEnumerable<Lugar>> GetLugaresDisponiveisByVooIdAsync(int vooId);

        Task<List<Lugar>> GetLugaresByAviaoIdAsync(int aviaoId);


    }
}
