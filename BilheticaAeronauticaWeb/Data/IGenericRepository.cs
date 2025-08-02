using BilheticaAeronauticaWeb.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Interface genérica para repositório CRUD básico de entidades.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade que implementa IEntity.</typeparam>
    public interface IGenericRepository<T> where T : class
    { 
        
        /// <summary>
      /// Obtém uma entidade pelo seu identificador.
      /// </summary>
      /// <param name="id">ID da entidade.</param>
      /// <returns>Entidade correspondente ou null.</returns>
        IQueryable<T> GetAll();


        /// <summary>
        /// Obtém uma entidade pelo seu identificador.
        /// </summary>
        /// <param name="id">ID da entidade.</param>
        /// <returns>Entidade correspondente ou null.</returns>
        Task<T> GetByIdAsync(int id);


        /// <summary>
        /// Cria uma nova entidade na base de dados.
        /// </summary>
        /// <param name="entity">Entidade para criação.</param>
        /// <returns>Entidade criada com ID atualizado.</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Atualiza uma entidade existente.
        /// </summary>
        /// <param name="entity">Entidade a atualizar.</param>
        /// <returns>Task assíncrona.</returns>

        Task UpdateAsync(T entity);

        /// <summary>
        /// Remove uma entidade da base de dados.
        /// </summary>
        /// <param name="entity">Entidade a remover.</param>
        /// <returns>Task assíncrona.</returns>
        Task DeleteAsync(T entity);


        /// <summary>
        /// Verifica se uma entidade com o dado ID existe na base de dados.
        /// </summary>
        /// <param name="id">ID da entidade a verificar.</param>
        /// <returns>True se existir, false caso contrário.</returns>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Adiciona um item do tipo BilheteTemp associado a um utilizador.
        /// Funcionalidade específica para gestão de bilhetes temporários.
        /// </summary>
        /// <param name="bilheteTemp">Instância do bilhete temporário a adicionar.</param>
        /// <param name="userId">ID do utilizador associado ao bilhete.</param>
        /// <returns>True se a operação for bem sucedida.</returns>
        Task<bool> AddItemToBilheteAsync(BilheteTemp bilheteTemp, string userId);
    }
    
    
}
