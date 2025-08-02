using BilheticaAeronauticaWeb.Data.Entities;

namespace BilheticaAeronauticaWeb.Data
{/// <summary>
 /// Obtém todos os aviões disponíveis para utilização (disponível = true).
 /// </summary>
 /// <returns>Lista de aviões disponíveis.</returns>
    public interface IAviaoRepository : IGenericRepository<Aviao>
    {
    }
}
