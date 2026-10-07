using BilheticaAeronauticaWeb.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Interface para o repositório específico da entidade <see cref="Voo"/>,
    /// estendendo o repositório genérico com métodos específicos para operações complexas
    /// e consultas detalhadas no domínio de voos.
    /// </summary>
    public interface IVooRepository : IGenericRepository<Voo>
    {
        /// <summary>
        /// Obtém a lista completa de voos ativos.
        /// </summary>
        /// <returns>Lista de todos os voos.</returns>
        Task<List<Voo>> GetAllVoosAsync();

        /// <summary>
        /// Obtém a lista completa de voos incluindo entidades relacionadas para uso detalhado.
        /// </summary>
        /// <returns>Lista de voos com dados completos associados.</returns>
        Task<List<Voo>> GetAllVoosWithIncludesAsync();

        /// <summary>
        /// Obtém um voo pelo seu identificador, incluindo dados relacionados como origem, destino e aparelho.
        /// </summary>
        /// <param name="id">ID do voo.</param>
        /// <returns>Instância do voo com dados incluídos, ou null se não encontrado.</returns>
        Task<Voo> GetVooWithIncludesAsync(int id);

        /// <summary>
        /// Obtém a coleção de voos associados a um utilizador identificado pelo seu ID.
        /// </summary>
        /// <param name="utilizadorId">ID do utilizador.</param>
        /// <returns>Enumerável de voos do utilizador.</returns>
        Task<IEnumerable<Voo>> GetVoosByUtilizadorIdAsync(int utilizadorId);


        /// <summary>
        /// Obtém a lista dos voos cujo horário de partida é futuro (após o momento atual).
        /// </summary>
        /// <returns>Lista de voos futuros.</returns>
        Task<List<Voo>> GetVoosFuturosAsync();


        /// <summary>
        /// Obtém a lista dos voos cujo horário de partida já passou (anterior ao momento atual).
        /// </summary>
        /// <returns>Lista de voos passados.</returns>
        Task<List<Voo>> GetVoosPassadosAsync();

        /// <summary>
        /// Gera um número único para um novo voo, seguindo as regras de negócio definidas.
        /// </summary>
        /// <returns>String contendo o número do voo gerado.</returns>
        Task<string> GerarNumeroVooAsync();

        /// <summary>
        /// Obtém o último voo criado ou registado na base de dados, útil para sequência de números.
        /// </summary>
        /// <returns>Instância do último voo registrado, ou null se não existir.</returns>
        Task<Voo> ObterUltimoVooAsync();

        Task<IEnumerable<Lugar>> GetLugaresByVooIdAsync(int vooId);

        Task SaveAsync();

        Task<IEnumerable<Bilhete>> GetBilhetesByVooIdAsync(int vooId);

     



    }
}
