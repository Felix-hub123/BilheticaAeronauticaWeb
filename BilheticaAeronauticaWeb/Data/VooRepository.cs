using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
   
    public class VooRepository : GenericRepository<Voo>, IVooRepository
    {
        private readonly DataContext _context;

        public VooRepository(DataContext context) : base(context)
        {
            _context = context;


        }

        public async Task<List<Voo>> GetAllVoosAsync()
        {
            return await _context.Voos
                .Include(v => v.Origem)
                .Include(v => v.Destino)
                .Include(v => v.Aviao)
                .ToListAsync();
        }

        public async Task<List<Voo>> GetAllVoosWithIncludesAsync()
        {
            return await _context.Voos
                .Include(v => v.Origem)
                .Include(v => v.Destino)
                .Include(v => v.Aviao)
                .ToListAsync();
        }


        public async Task<Voo> GetVooWithIncludesAsync(int id)
        {
            return await _context.Voos
                .Include(v => v.Origem)
                .Include(v => v.Destino)
                .Include(v => v.Aviao)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Voo>> GetVoosByUtilizadorIdAsync(int utilizadorId)
        {
            return await _context.Bilhetes
                .Where(b => b.PassageiroId == utilizadorId)
                .Select(b => b.Voo)
                .Distinct()
                .ToListAsync();
        }

       



    }
}
