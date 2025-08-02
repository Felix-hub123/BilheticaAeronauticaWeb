using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public class AeroportoRepository : GenericRepository<Aeroporto>, IAeroportoRepository
    {


        /// <summary>
        /// Repositório para operações de dados relativas a aeroportos,
        /// baseado no repositório genérico, podendo ser extendido com consultas específicas.
        /// </summary>
        public AeroportoRepository(DataContext context) : base(context) 
        { 


        }

        public async Task<bool> TemVoosAssociadosAsync(int aeroportoId)
        {
            return await _context.Voos.AnyAsync(v =>
         (v.OrigemId == aeroportoId || v.DestinoId == aeroportoId)
         && !v.WasDeleted);
        }


        public async Task DeleteAeroportoComValidacaoAsync(Aeroporto aeroporto)
        {
            bool temVoos = await _context.Voos.AnyAsync(v =>
                v.OrigemId == aeroporto.Id || v.DestinoId == aeroporto.Id);

            if (await TemVoosAssociadosAsync(aeroporto.Id))
            {
                throw new InvalidOperationException("Não é possível apagar este aeroporto porque ele está associado a voos ativos.");
            }
            _context.Aeroportos.Remove(aeroporto);
            await _context.SaveChangesAsync();
        }




    }
}
