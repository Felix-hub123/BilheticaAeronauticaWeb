using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Repositório para operações específicas da entidade Lugar (assento),
    /// incluindo métodos para obter lugares disponíveis por voo e criar múltiplos lugares.
    /// </summary>
    public class LugarRepository : GenericRepository<Lugar>, ILugarRepository
    {
        private new readonly DataContext _context;

        public LugarRepository(DataContext context) : base(context)
        {
            _context = context;
        }


        /// <summary>
        /// Obtém os lugares disponíveis para um determinado voo,
        /// filtrando apenas os marcados como disponíveis e não excluídos logicamente,
        /// ordenados pelo código do lugar.
        /// </summary>
        /// <param name="vooId">ID do voo para filtragem dos lugares.</param>
        /// <returns>Lista ordenada de lugares disponíveis para o voo.</returns>
        public async Task<IEnumerable<Lugar>> GetLugaresDisponiveisByVooIdAsync(int vooId)
        {

            var voo = await _context.Voos.FirstOrDefaultAsync(v => v.Id == vooId);
            if (voo == null)
                return new List<Lugar>(); 

            return await _context.Lugares
                .Where(l => l.VooId == vooId && l.Disponivel == true && !l.WasDeleted)
                .OrderBy(l => l.Codigo)
                .ToListAsync();
        }


        /// <summary>
        /// Obtém todos os lugares associados a um avião específico,
        /// excluindo os marcados como excluídos logicamente.
        /// </summary>
        /// <param name="aviaoId">ID do avião para buscar os lugares.</param>
        /// <returns>Lista de lugares pertencentes ao avião.</returns>
        public async Task<List<Lugar>> GetLugaresByAviaoIdAsync(int aviaoId)
        {
            return await _context.Lugares.Where(l => l.AviaoId == aviaoId).ToListAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<Lugar> lugares)
        {
            await _context.Lugares.AddRangeAsync(lugares);
            await _context.SaveChangesAsync();
        }


    }
}
