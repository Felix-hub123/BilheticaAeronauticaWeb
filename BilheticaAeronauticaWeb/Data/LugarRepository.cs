using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public class LugarRepository : GenericRepository<Lugar>, ILugarRepository
    {
        private readonly DataContext _context;

        public LugarRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lugar>> GetLugaresDisponiveisByVooIdAsync(int vooId)
        {
     
            var voo = await _context.Voos.FirstOrDefaultAsync(v => v.Id == vooId);
            if (voo == null)
                return Enumerable.Empty<Lugar>();

            return await _context.Lugares
                .Where(l => l.AviaoId == voo.AviaoId) 
                .OrderBy(l => l.Codigo)
                .ToListAsync();
        }

        public async Task<List<Lugar>> GetLugaresByAviaoIdAsync(int aviaoId)
        {
            return await _context.Lugares.Where(l => l.AviaoId == aviaoId).ToListAsync();
        }

    }
}
