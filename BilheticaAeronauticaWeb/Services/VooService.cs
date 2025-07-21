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
            try
            {
               
                await _vooRepository.CreateAsync(voo);

                
                var aviao = await _aviaoRepository.GetByIdAsync(voo.AviaoId);
                if (aviao == null)
                {
                    throw new Exception("Avião não encontrado.");
                }
                   

                if (aviao.Capacidade <= 0)
                {
                    throw new Exception("Avião sem capacidade definida.");
                }
                  

                // Gerar automaticamente os lugares (ex: L1 a L100)
                for (int i = 1; i <= aviao.Capacidade; i++)
                {
                    var lugar = new Lugar
                    {
                        Codigo = $"L{i:D3}", // ex: L001, L002, ...
                        PrecoBase = precoBaseLugar,
                        Disponivel = true,
                        AviaoId = aviao.Id,
                        VooId = voo.Id,
                        WasDeleted = false
                    };
                    await _lugarRepository.CreateAsync(lugar);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar voo: {ex.Message}", ex);
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

      

        public async Task<Voo> ObterVooPorIdAsync(int id)
        {
            
            return await _vooRepository.GetVooWithIncludesAsync(id);
        }

        public async Task<List<Voo>> PesquisarVoosAsync(DateTime? data, int? origemAeroportoId, int? destinoAeroportoId)
        {
            var voos = await _vooRepository.GetAllVoosWithIncludesAsync();

            if (data.HasValue)
                voos = voos.Where(v => v.DataHoraPartida.Date == data.Value.Date).ToList();
            if (origemAeroportoId.HasValue)
                voos = voos.Where(v => v.OrigemId == origemAeroportoId.Value).ToList();
            if (destinoAeroportoId.HasValue)
                voos = voos.Where(v => v.DestinoId == destinoAeroportoId.Value).ToList();

            return voos;
        }

       

    }
}
