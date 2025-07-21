using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IBilheteService
    {

        Task<Lugar> GetLugarByIdAsync(int lugarId);
        Task<Voo> GetVooByIdAsync(int vooId);
        Task<IEnumerable<SelectListItem>> GetVoosSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetLugaresSelectListAsync(int vooId);
        decimal CalcularPrecoBilhete(Lugar lugar, Voo voo, bool bagagemExtra, bool refeicao);
        Task<bool> LugarDisponivelAsync(int vooId, int lugarId);
        Task<Bilhete> CriarBilheteAsync(int vooId, int lugarId, int passageiroId, bool bagagemExtra, bool refeicao);






    }

}
