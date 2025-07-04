using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Services
{
    public class VooService : IVooService
    {
        private readonly IVooRepository _vooRepository;
        private readonly ILugarRepository _lugarRepository;
        private readonly IAviaoRepository _aviaoRepository;

        public VooService(IVooRepository vooRepository,
            ILugarRepository lugarRepository,
            IAviaoRepository aviaoRepository)
        {
            _vooRepository = vooRepository;
            _lugarRepository = lugarRepository;
            _aviaoRepository = aviaoRepository;
        }

        public async Task CriarVooAsync(Voo voo, decimal precoBaseLugar)
        {
            await _vooRepository.CreateAsync(voo);
            var lugaresModelo = await _lugarRepository.GetAll()
                  .Where(l => l.AviaoId == voo.AviaoId && l.VooId == null)
                  .ToListAsync();

            foreach (var lugar in lugaresModelo)
            {
                var novoLugar = new Lugar
                {
                    Codigo = lugar.Codigo,
                    PrecoBase = precoBaseLugar,
                    Disponivel = true,
                    AviaoId = lugar.AviaoId,
                    VooId = voo.Id, 
                    WasDeleted = false
                };
                await _lugarRepository.CreateAsync(novoLugar);
            }
        }

        public async Task EditarVooAsync(Voo voo)
        {
            await _vooRepository.UpdateAsync(voo);
        }

        public async Task EliminarVooAsync(int id)
        {
            var voo = await _vooRepository.GetByIdAsync(id);
            if (voo != null)
            {
                await _vooRepository.DeleteAsync(voo);
            }
        }

        public async Task<IEnumerable<Voo>> ObterHistoricoVoosAsync(int user)
        {
            return await _vooRepository.GetVoosByUtilizadorIdAsync(user);
        }

        public async Task<IEnumerable<Voo>> ObterVoosDisponiveisAsync()
        {
            return await _vooRepository.GetAllVoosAsync();
        }

        private List<Lugar> GerarLugaresParaVoo(Voo voo, Aviao aviao)
        {
            var lugares = new List<Lugar>();
            int lugaresPorFila = 6; // Exemplo: 6 lugares por fila (A-F)
            char[] letras = { 'A', 'B', 'C', 'D', 'E', 'F' };
            int totalLugares = aviao.Capacidade;
            int totalFilas = (int)Math.Ceiling((double)totalLugares / lugaresPorFila);

            int lugarAtual = 0;
            for (int fila = 1; fila <= totalFilas; fila++)
            {
                for (int l = 0; l < lugaresPorFila && lugarAtual < totalLugares; l++)
                {
                    lugares.Add(new Lugar
                    {
                        Codigo = $"{fila}{letras[l]}",
                        VooId = voo.Id,
                        Disponivel = false
                    });
                    lugarAtual++;
                }
            }
            return lugares;
        }

        public async Task<Voo> ObterVooPorIdAsync(int id)
        {
            
            return await _vooRepository.GetVooWithIncludesAsync(id);
        }

       
    }
}
