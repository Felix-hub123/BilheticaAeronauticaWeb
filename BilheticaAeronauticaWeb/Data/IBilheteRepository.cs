using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IBilheteRepository : IGenericRepository<Bilhete>
    {
        Task<List<Bilhete>> GetBilhetesByUserAsync(string userId);
        Task<Bilhete> GetBilheteAsync(int id); 
        Task<List<Bilhete>> GetBilhetesByVooAsync(int vooId);

        Task<List<Bilhete>> GetBilhetesFuturosByUserAsync(string userId);


        Task<List<BilheteTemp>> GetBilheteTempsByUserAsync(string userId);
        Task<bool> AddBilheteTempAsync(BilheteTemp bilheteTemp, string userId);
        Task DeleteBilheteTempAsync(int id);
        Task<bool> ConfirmBilheteAsync(string userId);

        Task<bool> SoftDeleteBilheteAsync(int id);
        Task<Bilhete> GetByVooAndLugarAsync(int vooId, int lugarId);

        Task<bool> ConfirmBilheteTempAsync(string userId, int idBilheteTemp);

        Task<IEnumerable<Bilhete>> GetAllBilhetesAsync();

        Task UpdateBilheteAsync(Bilhete bilhete);

        Task<bool> ConfirmarPagamentoEBilheteAsync(int bilheteId);

        Task<List<Bilhete>> GetByVooIdAsync(int vooId);

       
    }
       
}
