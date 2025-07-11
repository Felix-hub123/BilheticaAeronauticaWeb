using BilheticaAeronauticaWeb.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IVooService
    {
        Task CriarVooAsync(Voo voo, decimal precoBaseLugar);

        Task EditarVooAsync(Voo voo);

        Task EliminarVooAsync(int vooId);

        Task<IEnumerable<Voo>> ObterVoosDisponiveisAsync();

        Task<IEnumerable<Voo>> ObterHistoricoVoosAsync(int utilizadorId);

        Task<Voo> ObterVooPorIdAsync(int id);

        Task<List<Voo>> PesquisarVoosAsync(DateTime? data, int? origemAeroportoId, int? destinoAeroportoId);
    }
}
