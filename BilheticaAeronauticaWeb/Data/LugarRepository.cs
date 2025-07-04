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
            return await _context.Lugares
          .Where(l => l.VooId == vooId && l.Disponivel == true)
          .OrderBy(l => l.Codigo) 
          .ToListAsync();
        }
    }
}
