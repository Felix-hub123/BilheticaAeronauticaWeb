using BilheticaAeronauticaWeb.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Interface para repositório específico de <see cref="Passageiro"/>,
    /// com métodos adicionais para operações relacionadas a utilizadores do sistema.
    /// </summary>
    public interface IPassageiroRepository : IGenericRepository<Passageiro>
    {
        /// <summary>
        /// Obtém a entidade <see cref="Passageiro"/> associada ao identificador do utilizador (userId).
        /// </summary>
        /// <param name="userId">Identificador do utilizador na aplicação.</param>
        /// <returns>
        /// Uma instância de <see cref="Passageiro"/> correspondente ao utilizador, ou null se não existir.
        /// </returns>
        Task<Passageiro> GetByUserIdAsync(string userId);

        Task<int> ObterIdPorUserIdAsync(string userId);

        Task AddAsync(Passageiro passageiro);

    }
}
