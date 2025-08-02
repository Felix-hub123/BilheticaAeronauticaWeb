using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{/// <summary>
 /// Interface para o repositório específico de gestão de bilhetes e bilhetes temporários,
 /// definindo operações CRUD e métodos de negócio particulares ao domínio.
 /// </summary>
    public interface IBilheteRepository : IGenericRepository<Bilhete>
    {
        /// <summary>
        /// Obtém a lista de bilhetes definitivos associados a um utilizador identificado pelo seu userId.
        /// </summary>
        /// <param name="userId">Identificador do utilizador.</param>
        /// <returns>Lista de bilhetes do utilizador.</returns>
        Task<List<Bilhete>> GetBilhetesByUserAsync(string userId);

        /// <summary>
        /// Obtém um bilhete definitivo pelo seu identificador.
        /// </summary>
        /// <param name="id">ID do bilhete.</param>
        /// <returns>Bilhete correspondente ou null se não encontrado.</returns>
        Task<Bilhete> GetBilheteAsync(int id);


        /// <summary>
        /// Obtém os bilhetes associados a um voo específico.
        /// </summary>
        /// <param name="vooId">ID do voo.</param>
        /// <returns>Lista de bilhetes pertencentes ao voo.</returns>
        Task<List<Bilhete>> GetBilhetesByVooAsync(int vooId);


        /// <summary>
        /// Obtém os bilhetes futuros (voos a partir da data atual) de um utilizador identificado por userId.
        /// </summary>
        /// <param name="userId">Identificador do utilizador.</param>
        /// <returns>Lista de bilhetes futuros ativos do utilizador.</returns>
        Task<List<Bilhete>> GetBilhetesFuturosByUserAsync(string userId);

        /// <summary>
        /// Obtém os bilhetes temporários (pré-reservas) associados a um utilizador pelo seu userId.
        /// </summary>
        /// <param name="userId">Identificador do utilizador.</param>
        /// <returns>Lista de bilhetes temporários ativos.</returns>
        Task<List<BilheteTemp>> GetBilheteTempsByUserAsync(string userId);

        /// <summary>
        /// Adiciona um bilhete temporário para um determinado utilizador.
        /// </summary>
        /// <param name="bilheteTemp">Dados do bilhete temporário a adicionar.</param>
        /// <param name="userId">ID do utilizador que está a criar a reserva temporária.</param>
        /// <returns>True se adicionado com sucesso; False se já existir conflito.</returns>
        Task<bool> AddBilheteTempAsync(BilheteTemp bilheteTemp, string userId);

        /// <summary>
        /// Elimina permanentemente um bilhete temporário pelo seu ID.
        /// </summary>
        /// <param name="id">ID do bilhete temporário a eliminar.</param>
        Task DeleteBilheteTempAsync(int id);

        /// <summary>
        /// Confirma todos os bilhetes temporários de um utilizador, convertendo-os em bilhetes definitivos.
        /// </summary>
        /// <param name="userId">ID do utilizador dono dos bilhetes temporários.</param>
        /// <returns>True se a confirmação foi bem sucedida.</returns>
        Task<bool> ConfirmBilheteAsync(string userId);


        /// <summary>
        /// Aplica soft delete (eliminação lógica) a um bilhete existente pelo seu ID.
        /// </summary>
        /// <param name="id">ID do bilhete a eliminar logicamente.</param>
        /// <returns>True se realizado com sucesso; False se bilhete não encontrado.</returns>
        Task<bool> SoftDeleteBilheteAsync(int id);

        /// <summary>
        /// Obtém um bilhete específico pelo par voo e lugar.
        /// </summary>
        /// <param name="vooId">ID do voo.</param>
        /// <param name="lugarId">ID do lugar.</param>
        /// <returns>Bilhete correspondente ou null se não existente.</returns>
        Task<Bilhete> GetByVooAndLugarAsync(int vooId, int lugarId);



        /// <summary>
        /// Confirma um bilhete temporário específico para o utilizador, criando o bilhete definitivo.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <param name="idBilheteTemp">ID do bilhete temporário a confirmar.</param>
        /// <returns>True se confirmado com sucesso; False se o bilhete temporário não existe.</returns>
        Task<bool> ConfirmBilheteTempAsync(string userId, int idBilheteTemp);


        /// <summary>
        /// Obtém todos os bilhetes definitivos, incluindo dados completos das entidades relacionadas.
        /// </summary>
        /// <returns>Enumerável com todos os bilhetes registrados.</returns>
        Task<IEnumerable<Bilhete>> GetAllBilhetesAsync();


        /// <summary>
        /// Atualiza um bilhete existente no sistema.
        /// </summary>
        /// <param name="bilhete">Objeto bilhete com dados atualizados.</param>
        Task UpdateBilheteAsync(Bilhete bilhete);


        /// <summary>
        /// Marca um bilhete como pago e atualiza o seu estado e data de emissão.
        /// </summary>
        /// <param name="bilheteId">ID do bilhete a confirmar pagamento.</param>
        /// <returns>True se atualização bem sucedida; False caso contrário.</returns>
        Task<bool> ConfirmarPagamentoEBilheteAsync(int bilheteId);


        /// <summary>
        /// Obtém todos os bilhetes associados a um voo, sem dados adicionais.
        /// </summary>
        /// <param name="vooId">ID do voo.</param>
        /// <returns>Lista de bilhetes para o voo.</returns>
        Task<List<Bilhete>> GetByVooIdAsync(int vooId);


        Task<BilheteTemp> GetBilheteTempByIdAsync(int id);


        Task CriarBilheteDefinitivoAsync(PagamentoMbwayViewModel model, int passageiroId);

        Task RemoverBilheteTempAsync(int vooId, int lugarId, int passageiroId);



    }

}
