using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bilhete = BilheticaAeronauticaWeb.Data.Entities.Bilhete;


namespace BilheticaAeronauticaWeb.Services
{
    /// <summary>
    /// Serviço que encapsula a lógica de negócio relacionada à manipulação de bilhetes,
    /// incluindo cálculo de preço, verificação de disponibilidade e criação de bilhetes.
    /// </summary>
    public class BilheteService : IBilheteService
    {
        private readonly IVooRepository _vooRepository;
        private readonly ILugarRepository _lugarRepository;
        private readonly IBilheteRepository _bilheteRepository;

        public BilheteService(IVooRepository vooRepository,
            ILugarRepository lugarRepository,
            IBilheteRepository bilheteRepository)
        {
            _vooRepository = vooRepository;
            _lugarRepository = lugarRepository;
            _bilheteRepository = bilheteRepository;
        }

        /// <summary>
        /// Calcula o preço do bilhete considerando preços base, extras e desconto para voos distantes.
        /// </summary>
        public decimal CalcularPrecoBilhete(Lugar lugar, Voo voo, bool bagagemExtra, bool refeicao)
        {
            decimal preco = lugar.PrecoBase;
               if (bagagemExtra)
                preco += 30;
            if (refeicao)
                preco += 20;
            if (voo.DataHoraPartida > DateTime.Now.AddMonths(3))
                preco *= 0.9M;
            return preco;
        }


        /// <summary>
        /// Verifica assíncronamente se o lugar está disponível para o voo informado.
        /// </summary>

        public async Task<bool> LugarDisponivelAsync(int vooId, int lugarId)
        {
            var bilhete = await _bilheteRepository.GetByVooAndLugarAsync(vooId, lugarId);
            return bilhete == null;
        }


        /// <summary>
        /// Cria um bilhete para o voo e lugar indicados, marcando o lugar como ocupado.
        /// </summary>
        public async Task<Bilhete> CriarBilheteAsync(int vooId, int lugarId, int passageiroId, bool bagagemExtra, bool refeicao)
        {
            var voo = await _vooRepository.GetVooWithIncludesAsync(vooId);
            var lugar = await _lugarRepository.GetByIdAsync(lugarId);

            if (voo == null || lugar == null)
            {
                throw new Exception("Voo ou lugar inválido.");
            }
               

            if (!await LugarDisponivelAsync(vooId, lugarId))
            {
                throw new Exception("Lugar não disponível para este voo.");
            }


            var valor = CalcularPrecoBilhete(lugar, voo, bagagemExtra, refeicao);

            var bilhete = new Bilhete
            {
                VooId = vooId,
                LugarId = lugarId,
                PassageiroId = passageiroId,
                Valor = valor,
                BagagemExtra = bagagemExtra,
                Refeicao = refeicao,
                DataReserva = DateTime.Now
            };

            await _bilheteRepository.CreateAsync(bilhete);
            lugar.Disponivel = false;
            await _lugarRepository.UpdateAsync(lugar);
            return bilhete;
        }



        public async Task<bool> ReservarBilheteTempMBWayAsync(int vooId, int lugarId, int passageiroId,
           bool bagagemExtra, bool refeicao, string userId)
        {
            var voo = await _vooRepository.GetByIdAsync(vooId);
            var lugar = await _lugarRepository.GetByIdAsync(lugarId);

            if (voo == null || lugar == null)
            {
                throw new Exception("Voo ou lugar inválido.");
            }

            if (!await LugarDisponivelAsync(vooId, lugarId))
            {
                return false;
            }

            var preco = CalcularPrecoBilhete(lugar, voo, bagagemExtra, refeicao);

            var bilheteTemp = new BilheteTemp
            {
                VooId = vooId,
                LugarId = lugarId,
                PassageiroId = passageiroId,
                BagagemExtra = bagagemExtra,
                Refeicao = refeicao,
                Preco = preco,
                CriadoPorUserId = userId,
                DataReserva = DateTime.UtcNow,
                WasDeleted = false
            };

            // Adiciona bilhete temporário
            return await _bilheteRepository.AddBilheteTempAsync(bilheteTemp, userId);
        }

        /// <summary>
        /// Obtém a lista de voos formatada para uso em dropdown lists.
        /// </summary>
        public async Task<IEnumerable<SelectListItem>> GetVoosSelectListAsync()
        {
            var voos = await _vooRepository.GetAllVoosWithIncludesAsync();
            return voos.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = $"{v.Numero} - {v.Origem.Nome} → {v.Destino.Nome}"
            });
        }


        /// <summary>
        /// Obtém a lista de lugares de um voo com indicação de ocupação, formatada para dropdown lists.
        /// </summary>
        public async Task<IEnumerable<SelectListItem>> GetLugaresSelectListAsync(int vooId)
        {
            var voo = await _vooRepository.GetByIdAsync(vooId);
            if (voo == null)
                return new List<SelectListItem>();

            var lugares = await _lugarRepository.GetLugaresByAviaoIdAsync(voo.AviaoId);
            var ocupados = (await _bilheteRepository.GetByVooIdAsync(vooId)).Select(b => b.LugarId).ToHashSet();

            return lugares.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = ocupados.Contains(l.Id) ? $"{l.Codigo} (ocupado)" : l.Codigo,
                Disabled = ocupados.Contains(l.Id)
            });
        }


        /// <summary>
        /// Confirma o pagamento MB WAY do utilizador, convertendo os bilhetes temporários em definitivos
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True se a confirmação ocorrer com sucesso.</returns>
        public async Task<bool> ConfirmarPagamentoMBWayAsync(string userId, string numeroTelemovel)
        {
            // Confirma bilhetes temporários deste utilizador (remove temporários, cria definitivos)
            var confirmado = await _bilheteRepository.ConfirmBilheteAsync(userId);

            if (!confirmado)
                return false;

            // Após criar bilhetes definitivos, marca lugares como ocupados
            var bilhetesConfirmados = await _bilheteRepository.GetBilhetesByUserAsync(userId);

            foreach (var bilhete in bilhetesConfirmados)
            {
                var lugar = await _lugarRepository.GetByIdAsync(bilhete.LugarId);
                if (lugar != null && lugar.Disponivel)
                {
                    lugar.Disponivel = false;
                    await _lugarRepository.UpdateAsync(lugar);
                }
            }

            return true;
        }

        public async Task<Lugar> GetLugarByIdAsync(int lugarId)
        {
            return await _lugarRepository.GetByIdAsync(lugarId);
        }

        public async Task<Voo> GetVooByIdAsync(int vooId)
        {
            return await _vooRepository.GetByIdAsync(vooId);
        }


     
    }   
}
