using BilheticaAeronauticaWeb.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Interface específica para operações no repositório de lugares (assentos),
    /// baseada no repositório genérico, com métodos para obter lugares disponíveis,
    /// listar por avião e criação múltipla de lugares.
    /// </summary>
    public interface ILugarRepository : IGenericRepository<Lugar>
    {
        /// <summary>
        /// Obtém a lista de todos os lugares que pertencem a um avião específico.
        /// </summary>
        /// <param name="aviaoId">Identificador do avião.</param>
        /// <returns>Lista de lugares pertencentes ao avião.</returns>
        Task<IEnumerable<Lugar>> GetLugaresDisponiveisByVooIdAsync(int vooId);

        /// <summary>
        /// Cria e adiciona uma coleção de lugares ao contexto, persistindo-os no banco de dados.
        /// </summary>
        /// <param name="lugares">Coleção de lugares a adicionar.</param>
        /// <returns>Task assíncrona representando a operação de criação em massa.</returns>
        Task<List<Lugar>> GetLugaresByAviaoIdAsync(int aviaoId);


        /// <summary>
        /// Cria e adiciona uma coleção de lugares (assentos) ao contexto do banco de dados,
        /// persistindo-os na base de dados numa operação única assíncrona.
        /// </summary>
        /// <param name="lugares">Coleção de entidades Lugar a adicionar.</param>
        /// <returns>Task que representa a operação assíncrona de criação.</returns>
        Task CreateRangeAsync(IEnumerable<Lugar> lugares);

    }
}
