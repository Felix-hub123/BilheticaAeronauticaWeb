using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Repositório específico para operações relacionadas à entidade <see cref="Passageiro"/>.
    /// </summary>
    public class PassageiroRepository : GenericRepository<Passageiro>, IPassageiroRepository
    {
        private new readonly DataContext _context;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="PassageiroRepository"/> com o contexto de dados fornecido.
        /// </summary>
        /// <param name="context">Contexto de dados da aplicação.</param>
        public PassageiroRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém um passageiro pelo seu identificador de utilizador (UserId).
        /// </summary>
        /// <param name="userId">Identificador do utilizador associado ao passageiro.</param>
        /// <returns>Instância do <see cref="Passageiro"/> ou null se não encontrado.</returns>
        public async Task<Passageiro> GetByUserIdAsync(string userId)
        {
            return await _context.Passageiros.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<int> ObterIdPorUserIdAsync(string userId)
        {
            var passageiro = await _context.Passageiros
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return passageiro?.Id ?? 0;
        }

        public async Task AddAsync(Passageiro passageiro)
        {
            _context.Passageiros.Add(passageiro);
            await _context.SaveChangesAsync();
        }


    }
}
