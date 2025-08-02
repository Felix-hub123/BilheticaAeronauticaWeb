using BilheticaAeronauticaWeb.Data.Entities;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{/// <summary>
 /// Obtém um aeroporto pelo código IATA.
 /// </summary>
 /// <param name="iata">Código IATA (3 caracteres) do aeroporto.</param>
 /// <returns>Instância do aeroporto correspondente ou null.</returns>
    public interface IAeroportoRepository : IGenericRepository<Aeroporto>
    {


        Task<bool> TemVoosAssociadosAsync(int aeroportoId);

        Task DeleteAeroportoComValidacaoAsync(Aeroporto aeroporto);

    }


}
