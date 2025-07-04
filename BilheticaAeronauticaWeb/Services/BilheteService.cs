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

        public decimal CalcularPrecoBilhete(Lugar lugar, Voo voo, bool bagagemExtra, bool refeicao)
        {
            decimal preco = lugar.PrecoBase;
            preco += voo.TaxaAeroporto;
            if (bagagemExtra)
                preco += 30;
            if (refeicao)
                preco += 20;
            if (voo.DataHoraPartida > DateTime.Now.AddMonths(3))
                preco *= 0.9M;
            return preco;
        }

        public async Task<bool> LugarDisponivelAsync(int vooId, int lugarId)
        {
            var bilhete = await _bilheteRepository.GetByVooAndLugarAsync(vooId, lugarId);
            return bilhete == null;
        }

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
            return bilhete;
        }



        public async Task<IEnumerable<SelectListItem>> GetVoosSelectListAsync()
        {
            var voos = await _vooRepository.GetAllVoosWithIncludesAsync();
            return voos.Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = $"{v.Numero} - {v.Origem.Nome} → {v.Destino.Nome}"
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetLugaresSelectListAsync(int vooId)
        {
            var lugares = await _lugarRepository.GetAll().ToListAsync();
            return lugares
                .Where(l => l.Disponivel && l.VooId == vooId)
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Codigo
                });
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
