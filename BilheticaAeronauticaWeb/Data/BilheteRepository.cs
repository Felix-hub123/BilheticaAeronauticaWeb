using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BilheticaAeronauticaWeb.Data
{
    public class BilheteRepository : GenericRepository<Bilhete>, IBilheteRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public BilheteRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<bool> AddBilheteTempAsync(BilheteTemp bilheteTemp, string userId)
        {
            bool existeTemp = await _context.BilhetesTemp.AnyAsync(b =>
                b.VooId == bilheteTemp.VooId &&
                b.LugarId == bilheteTemp.LugarId &&
                b.PassageiroId == bilheteTemp.PassageiroId &&
                !b.WasDeleted);

           
            bool existeDefinitivo = await _context.Bilhetes.AnyAsync(b =>
                b.VooId == bilheteTemp.VooId &&
                b.LugarId == bilheteTemp.LugarId &&
                b.PassageiroId == bilheteTemp.PassageiroId &&
                !b.WasDeleted);

            if (existeTemp || existeDefinitivo)
                return false; // Já existe, não adiciona

            bilheteTemp.CriadoPorUserId = userId;
            _context.BilhetesTemp.Add(bilheteTemp);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ConfirmBilheteAsync(string userId)
        {
            var temps = await _context.BilhetesTemp
             .Include(t => t.Passageiro)
             .Include(t => t.Voo)
             .Include(t => t.Lugar)
             .Where(t => t.CriadoPorUserId == userId && !t.WasDeleted)
             .ToListAsync();

            if (!temps.Any())
                return false;

            foreach (var temp in temps)
            {
                var bilhete = new Bilhete
                {
                    DataCompra = DateTime.UtcNow,
                    Passageiro = temp.Passageiro,
                    Voo = temp.Voo,
                    Lugar = temp.Lugar,
                    Valor = temp.Preco,
                    CriadoPorUserId = temp.CriadoPorUserId,
                    WasDeleted = false
                };
                _context.Bilhetes.Add(bilhete);
                _context.BilhetesTemp.Remove(temp);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task DeleteBilheteTempAsync(int id)
        {
            var bilheteTemp = await _context.BilhetesTemp.FindAsync(id);
            if (bilheteTemp != null)
            {
                _context.BilhetesTemp.Remove(bilheteTemp);
                await _context.SaveChangesAsync();
            }
        }

      

        public async Task<Bilhete> GetBilheteAsync(int id)
        {
            return await _context.Bilhetes
            .Include(b => b.Passageiro)
            .Include(b => b.Voo)
            .Include(b => b.Lugar)
            .FirstOrDefaultAsync(b => b.Id == id && !b.WasDeleted);
        }

      

        public async Task<List<Bilhete>> GetBilhetesByUserAsync(string userId)
        {
            return await _context.Bilhetes
            .Include(b => b.Passageiro)
            .Include(b => b.Voo)
            .Include(b => b.Lugar)
            .Where(b => b.CriadoPorUserId == userId && !b.WasDeleted)
            .ToListAsync();


        }

        public async Task<List<Bilhete>> GetBilhetesByVooAsync(int vooId)
        {
            return await _context.Bilhetes
             .Include(b => b.Passageiro)
             .Include(b => b.Lugar)
             .Where(b => b.Voo.Id == vooId && !b.WasDeleted)
             .ToListAsync();
        }

        public async Task<List<BilheteTemp>> GetBilheteTempsByUserAsync(string userId)
        {
            return await _context.BilhetesTemp
             .Include(b => b.Passageiro)
             .Include(b => b.Voo)
             .Include(b => b.Lugar)
             .Where(b => b.CriadoPorUserId == userId && !b.WasDeleted)
             .ToListAsync();
        }

       

        public async Task<bool> SoftDeleteBilheteAsync(int id)
        {
            var bilhete = await _context.Bilhetes.FindAsync(id);
            if (bilhete == null)
                return false;
            bilhete.WasDeleted = true;
            _context.Bilhetes.Update(bilhete);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateBilheteAsync(Bilhete bilhete)
        {
            _context.Bilhetes.Update(bilhete);
            await _context.SaveChangesAsync();
        }


        public async Task<Bilhete> GetByVooAndLugarAsync(int vooId, int lugarId)
        { 
            return await _context.Bilhetes
            .FirstOrDefaultAsync(b => b.VooId == vooId && b.LugarId == lugarId);
        }

        public async Task<bool> ConfirmBilheteTempAsync(string userId, int idBilheteTemp)
        {
           
            var bilheteTemp = await _context.BilhetesTemp
                .FirstOrDefaultAsync(b => b.Id == idBilheteTemp && b.CriadoPorUserId == userId);

            if (bilheteTemp == null)
                return false;

            
            var bilhete = new Bilhete
            {
                VooId = bilheteTemp.VooId,
                PassageiroId = bilheteTemp.PassageiroId,
                LugarId = bilheteTemp.LugarId,
                Valor = bilheteTemp.Preco,
                BagagemExtra = bilheteTemp.BagagemExtra,
                Refeicao = bilheteTemp.Refeicao,
                DataCompra = DateTime.UtcNow
               
            };

     
            _context.Bilhetes.Add(bilhete);

           
            _context.BilhetesTemp.Remove(bilheteTemp);

        
            await _context.SaveChangesAsync();

            return true;
        }


    }

}
