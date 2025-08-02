using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Services
{
    /// <summary>
    /// Interface que define os serviços relacionados à gestão de voos,
    /// incluindo operações de criação, edição, eliminação e consulta de voos.
    /// </summary>
    public interface IVooService
    {
        Task CriarVooAsync(Voo voo, decimal precoBaseLugar);

        Task EditarVooAsync(Voo voo);

        Task EliminarVooAsync(int vooId);

        Task<IEnumerable<Voo>> ObterVoosDisponiveisAsync();

        Task<IEnumerable<Voo>> ObterHistoricoVoosAsync(int utilizadorId);

        Task<Voo> ObterVooPorIdAsync(int id);

        Task<List<Voo>> PesquisarVoosAsync(DateTime? data, int? origemAeroportoId, int? destinoAeroportoId);

        Task<bool> ExisteLugaresOuBilhetesVendidosAsync(int vooId);

        Task<(bool sucesso, string mensagem)> AtualizarVooComRegrasAsync(Voo voo, VooViewModel model);

    }
}
