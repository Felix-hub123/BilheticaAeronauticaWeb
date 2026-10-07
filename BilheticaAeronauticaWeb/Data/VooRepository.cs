using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
   
    public class VooRepository : GenericRepository<Voo>, IVooRepository
    {
        private new readonly DataContext _context;

        public VooRepository(DataContext context) : base(context)
        {
            _context = context;


        }

      

        public new async Task DeleteAsync(Voo voo)
        {
            voo.WasDeleted = true;
            _context.Voos.Update(voo);
            await _context.SaveChangesAsync();
        }



        public async Task<List<Voo>> GetAllVoosAsync()
        {
            return await _context.Voos
                 .Where(v => !v.WasDeleted)
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


        public async Task<List<Voo>> GetVoosFuturosAsync()
        {
            return await _context.Voos
                .Include(v => v.Origem)
                .Include(v => v.Destino)
                .Include(v => v.Aviao)
                .Where(v => v.DataHoraPartida >= DateTime.UtcNow)
                .OrderBy(v => v.DataHoraPartida)
                .ToListAsync();
        }

        public async Task<List<Voo>> GetVoosPassadosAsync()
        {
            return await _context.Voos
                .Include(v => v.Origem)
                .Include(v => v.Destino)
                .Include(v => v.Aviao)
                .Where(v => v.DataHoraPartida < DateTime.Now)
                .ToListAsync();
        }

        private async Task<string> GerarNumeroVooAsync()
        {
            var ultimoVoo = await _context.Voos
             .OrderByDescending(v => v.Id)
             .FirstOrDefaultAsync();

            int novoNumero = 1;
            if (ultimoVoo != null)
            {
                var numStr = new string(ultimoVoo.Numero.SkipWhile(c => !char.IsDigit(c)).ToArray());
                if (int.TryParse(numStr, out int lastNum))
                    novoNumero = lastNum + 1;
            }
            return $"TP{novoNumero:D3}";
        }

        Task<string> IVooRepository.GerarNumeroVooAsync()
        {
            return GerarNumeroVooAsync();
        }

        public async Task<Voo> ObterUltimoVooAsync()
        {
            return await _context.Voos
                .OrderByDescending(v => v.Numero)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Lugar>> GetLugaresByVooIdAsync(int vooId)
        {
            return await _context.Lugares
                .Where(l => l.VooId == vooId)
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Bilhete>> GetBilhetesByVooIdAsync(int vooId)
        {
            return await _context.Bilhetes.Where(b => b.VooId == vooId).ToListAsync();
        }


    }
}
