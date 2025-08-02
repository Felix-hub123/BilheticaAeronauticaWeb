using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Services
{
    /// <summary>
    /// Serviço responsável pela gestão dos voos, incluindo criação, edição,
    /// eliminação, pesquisa e obtenção de voos, interagindo com os repositórios correspondentes.
    /// </summary>
    public class VooService : IVooService
    {
        private readonly IVooRepository _vooRepository;
        private readonly ILugarRepository _lugarRepository;
        private readonly IAviaoRepository _aviaoRepository;


        /// <summary>
        /// Construtor que recebe os repositórios necessários para a gestão dos voos.
        /// </summary>
        /// <param name="vooRepository">Repositório para operações com voos.</param>
        /// <param name="lugarRepository">Repositório para operações com lugares.</param>
        /// <param name="aviaoRepository">Repositório para obter dados dos aviões.</param>
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

                if (string.IsNullOrWhiteSpace(voo.Numero))
                {
                    voo.Numero = GerarNumeroVoo();
                }
                   
                // 1. Cria o voo  
                await _vooRepository.CreateAsync(voo);

                // 2. Obtém o avião associado (verifica se existe)
                var aviao = await _aviaoRepository.GetByIdAsync(voo.AviaoId);
                if (aviao == null)
                {
                    throw new Exception("Avião não encontrado.");
                }

                // 3. Verifica se a capacidade está correta
                if (aviao.Capacidade <= 0)
                {
                    throw new Exception("Avião sem capacidade definida.");
                }

                // 4. Cria os lugares automaticamente
                var listaLugares = new List<Lugar>();

                for (int i = 1; i <= aviao.Capacidade; i++)
                {
                    var lugar = new Lugar
                    {
                        Codigo = $"L{i:D3}", 
                        PrecoBase = precoBaseLugar,
                        Disponivel = true,
                        AviaoId = aviao.Id,
                        VooId = voo.Id,
                        WasDeleted = false
                    };
                    listaLugares.Add(lugar);
                }

               
                if (listaLugares.Any())
                    await _lugarRepository.CreateRangeAsync(listaLugares); 

            }
            catch (Exception ex)
            {
               
                throw new Exception($"Erro ao criar voo: {ex.Message}", ex);
            }
        }

        private string GerarNumeroVoo()
        {
            var random = new Random();
            int numero = random.Next(100, 999);
            return $"TP{numero}";
        }


        public async Task EditarVooAsync(Voo voo)
        {
            await _vooRepository.UpdateAsync(voo);
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

        public async Task EliminarVooAsync(int vooId)
        {
            var voo = await _vooRepository.GetByIdAsync(vooId);
            if (voo == null)
            {
                throw new Exception("Voo não encontrado.");
            }

            var lugares = await _vooRepository.GetLugaresByVooIdAsync(vooId);
            if (lugares != null && lugares.Any())
            {
               
                throw new Exception("Não é possível eliminar o voo porque existem bilhetes vendidos associados.");

            }

            await _vooRepository.DeleteAsync(voo);
        }

        public async Task<bool> ExisteLugaresOuBilhetesVendidosAsync(int vooId)
        {
            var lugares = await _vooRepository.GetLugaresByVooIdAsync(vooId);
            return lugares != null && lugares.Any();
        }


        public async Task<(bool sucesso, string mensagem)> AtualizarVooComRegrasAsync(Voo voo, VooViewModel model)
        {
            // Verificar se voo já partiu
            bool vooJaPartiu = voo.DataHoraPartida <= DateTime.UtcNow;

            // Verificar se há bilhetes vendidos
            bool temBilhetes = await ExisteLugaresOuBilhetesVendidosAsync(voo.Id);

            if (vooJaPartiu)
            {
                return (false, "Este voo já partiu e não pode ser editado.");
            }

            if (temBilhetes)
            {
                bool alterouOrigem = voo.OrigemId != model.OrigemId;
                bool alterouDestino = voo.DestinoId != model.DestinoId;
                bool alterouAviao = voo.AviaoId != model.AviaoId;
                bool alterouDatas = voo.DataHoraPartida != model.DataHoraPartida || voo.DataHoraChegada != model.DataHoraChegada;

                if (alterouOrigem || alterouDestino || alterouAviao || alterouDatas)
                {
                    return (false, "Não é possível alterar origem, destino, avião ou datas de um voo que já possui bilhetes vendidos.");
                }

                // Permite alterar somente preço ou outros campos não críticos
                voo.PrecoBase = model.PrecoBase;
                await EditarVooAsync(voo);

                return (true, "Preço do voo atualizado com sucesso! Alterações em dados principais não permitidas pois já existem bilhetes vendidos.");
            }

            // Se pode editar tudo
            voo.OrigemId = model.OrigemId;
            voo.DestinoId = model.DestinoId;
            voo.AviaoId = model.AviaoId;
            voo.DataHoraPartida = model.DataHoraPartida;
            voo.DataHoraChegada = model.DataHoraChegada;
            voo.PrecoBase = model.PrecoBase;

            await EditarVooAsync(voo);

            return (true, "Voo atualizado com sucesso!");
        }


    }
}
