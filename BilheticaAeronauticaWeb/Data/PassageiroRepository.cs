using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public class PassageiroRepository : GenericRepository<Passageiro>, IPassageiroRepository
    {
        private readonly DataContext _context;

        public PassageiroRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Passageiro> GetByUserIdAsync(string userId)
        {
            return await _context.Passageiros.FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
