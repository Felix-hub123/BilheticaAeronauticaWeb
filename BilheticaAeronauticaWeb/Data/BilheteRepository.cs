using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BilheticaAeronauticaWeb.Data
{

    /// <summary>
    /// Repositório específico para gestão da entidade Bilhete e bilhetes temporários,
    /// com operações de criação, confirmação, consulta e eliminação lógica, incluindo regras de negócio associadas.
    /// </summary>
    public class BilheteRepository : GenericRepository<Bilhete>, IBilheteRepository
    {
        private new readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public BilheteRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }



        /// <summary>
        /// Tenta adicionar um bilhete temporário para um utilizador, garantindo que não existam conflitos
        /// com bilhetes definitivos ou outros temporários iguais.
        /// </summary>
        /// <param name="bilheteTemp">Bilhete temporário a adicionar.</param>
        /// <param name="userId">ID do utilizador que adiciona o bilhete temporário.</param>
        /// <returns>True se adicionado com sucesso; False se já existir bilhete igual.</returns>
        public async Task<bool> AddBilheteTempAsync(BilheteTemp bilheteTemp, string userId)
        {
            bool existeTemp = await _context.BilhetesTemp.AnyAsync(b =>
                b.VooId == bilheteTemp.VooId &&
                b.LugarId == bilheteTemp.LugarId &&
                b.PassageiroId == bilheteTemp.PassageiroId &&
                !b.WasDeleted);

           
            bool existeDefinitivo = await _context.Bilhetes.AnyAsync(b =>
                b.VooId == bilheteTemp.VooId &&
                b.LugarId == bilheteTemp.LugarId &&
                b.PassageiroId == bilheteTemp.PassageiroId &&
                !b.WasDeleted);

            if (existeTemp || existeDefinitivo)
                return false; // Já existe, não adiciona

            bilheteTemp.CriadoPorUserId = userId;
            _context.BilhetesTemp.Add(bilheteTemp);
            return await _context.SaveChangesAsync() > 0;
        }


        /// <summary>
        /// Converte todos os bilhetes temporários associados a um utilizador em bilhetes definitivos,
        /// removendo os temporários da base de dados.
        /// </summary>
        /// <param name="userId">ID do utilizador dono dos bilhetes temporários.</param>
        /// <returns>True se a confirmação ocorrer com sucesso; caso contrário, False.</returns>
        public async Task<bool> ConfirmBilheteAsync(string userId)
        {
            var temps = await _context.BilhetesTemp
             .Include(t => t.Passageiro)
             .Include(t => t.Voo)
             .Include(t => t.Lugar)
             .Where(t => t.CriadoPorUserId == userId && !t.WasDeleted)
             .ToListAsync();

            if (!temps.Any())
                return false;

            foreach (var temp in temps)
            {
                var bilhete = new Bilhete
                {
                    DataCompra = DateTime.UtcNow,
                    Passageiro = temp.Passageiro,
                    Voo = temp.Voo,
                    Lugar = temp.Lugar,
                    Valor = temp.Preco,
                    CriadoPorUserId = temp.CriadoPorUserId,
                    WasDeleted = false
                };
                _context.Bilhetes.Add(bilhete);
                _context.BilhetesTemp.Remove(temp);
            }
            return await _context.SaveChangesAsync() > 0;
        }


        /// <summary>
        /// Remove permanentemente um bilhete temporário da base de dados pelo seu identificador.
        /// </summary>
        /// <param name="id">ID do bilhete temporário a remover.</param>
        public async Task DeleteBilheteTempAsync(int id)
        {
            var bilheteTemp = await _context.BilhetesTemp.FindAsync(id);
            if (bilheteTemp != null)
            {
                _context.BilhetesTemp.Remove(bilheteTemp);
                await _context.SaveChangesAsync();
            }
        }


        /// <summary>
        /// Obtém um bilhete definitivo pelo seu ID, carregando dados associados
        /// de passageiro, voo e lugar, apenas se não estiver marcado como eliminado.
        /// </summary>
        /// <param name="id">ID do bilhete.</param>
        /// <returns>Instância do bilhete ou null se não encontrado.</returns>
        public async Task<Bilhete> GetBilheteAsync(int id)
        {
            return await _context.Bilhetes
            .Include(b => b.Passageiro)
            .Include(b => b.Voo)
            .Include(b => b.Lugar)
            .FirstOrDefaultAsync(b => b.Id == id && !b.WasDeleted);
        }


        /// <summary>
        /// Obtém todos os bilhetes definitivos associados a um dado utilizador (via Passageiro.UserId).
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <returns>Lista de bilhetes do utilizador.</returns>
        public async Task<List<Bilhete>> GetBilhetesByUserAsync(string userId)
        {
            return await _context.Bilhetes
            .Include(b => b.Passageiro)
            .Include(b => b.Voo)
            .Include(b => b.Lugar)
            .Where(b => b.Passageiro.UserId == userId)
            .ToListAsync();


        }

      

        /// <summary>
        /// Obtém todos os bilhetes associados a um determinado voo.
        /// </summary>
        /// <param name="vooId">ID do voo.</param>
        /// <returns>Lista de bilhetes pertencentes ao voo.</returns>
        public async Task<List<Bilhete>> GetBilhetesByVooAsync(int vooId)
        {
            return await _context.Bilhetes
             .Include(b => b.Passageiro)
             .Include(b => b.Lugar)
             .Where(b => b.Voo.Id == vooId && !b.WasDeleted)
             .ToListAsync();
        }



        /// <summary>
        /// Obtém todos os bilhetes temporários associados a um utilizador pelo seu ID.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <returns>Lista de bilhetes temporários não eliminados.</returns>
        public async Task<List<BilheteTemp>> GetBilheteTempsByUserAsync(string userId)
        {
            return await _context.BilhetesTemp
         .Include(b => b.Passageiro)
         .Include(b => b.Voo)
             .ThenInclude(v => v.Origem)
         .Include(b => b.Voo)
             .ThenInclude(v => v.Destino)
         .Include(b => b.Lugar)
         .Where(b => b.CriadoPorUserId == userId && !b.WasDeleted)
         .ToListAsync();
        }


        /// <summary>
        /// Aplica soft delete (eliminação lógica) a um bilhete existente pelo seu ID.
        /// </summary>
        /// <param name="id">ID do bilhete a eliminar.</param>
        /// <returns>True se a operação for bem sucedida; False se bilhete não for encontrado.</returns>
        public async Task<bool> SoftDeleteBilheteAsync(int id)
        {
            var bilhete = await _context.Bilhetes.FindAsync(id);
            if (bilhete == null)
                return false;
            bilhete.WasDeleted = true;
            _context.Bilhetes.Update(bilhete);
            return await _context.SaveChangesAsync() > 0;
        }


        /// <summary>
        /// Atualiza os dados de um bilhete existente.
        /// </summary>
        /// <param name="bilhete">Bilhete atualizado.</param>
        public async Task UpdateBilheteAsync(Bilhete bilhete)
        {
            _context.Bilhetes.Update(bilhete);
            await _context.SaveChangesAsync();
        }



        /// <summary>
        /// Obtém um bilhete específico pelo voo e lugar indicados.
        /// </summary>
        /// <param name="vooId">ID do voo.</param>
        /// <param name="lugarId">ID do lugar.</param>
        /// <returns>Bilhete encontrado ou null se não existir.</returns>
        public async Task<Bilhete> GetByVooAndLugarAsync(int vooId, int lugarId)
        { 
            return await _context.Bilhetes
            .FirstOrDefaultAsync(b => b.VooId == vooId && b.LugarId == lugarId);
        }


        /// <summary>
        /// Confirma um bilhete temporário específico, criando o bilhete definitivo e removendo o temporário.
        /// </summary>
        /// <param name="userId">ID do utilizador dono do bilhete temporário.</param>
        /// <param name="idBilheteTemp">ID do bilhete temporário a confirmar.</param>
        /// <returns>True se confirmado com sucesso; False se o bilhete temporário não existir.</returns>
        public async Task<bool> ConfirmBilheteTempAsync(string userId, int idBilheteTemp)
        {
           
            var bilheteTemp = await _context.BilhetesTemp
                .FirstOrDefaultAsync(b => b.Id == idBilheteTemp && b.CriadoPorUserId == userId);

            if (bilheteTemp == null)
                return false;

            
            var bilhete = new Bilhete
            {
                VooId = bilheteTemp.VooId,
                PassageiroId = bilheteTemp.PassageiroId,
                LugarId = bilheteTemp.LugarId,
                Valor = bilheteTemp.Preco,
                BagagemExtra = bilheteTemp.BagagemExtra,
                Refeicao = bilheteTemp.Refeicao,
                DataCompra = DateTime.UtcNow
               
            };

     
            _context.Bilhetes.Add(bilhete);

           
            _context.BilhetesTemp.Remove(bilheteTemp);

        
            await _context.SaveChangesAsync();

            return true;
        }


        /// <summary>
        /// Obtém todos os bilhetes definitivos com respectivos dados associados.
        /// </summary>
        /// <returns>Lista completa de bilhetes.</returns>
        public async Task<IEnumerable<Bilhete>> GetAllBilhetesAsync()
        {
            return await _context.Bilhetes
           .Include(b => b.Voo)
           .Include(b => b.Lugar)
           .Include(b => b.Passageiro)
           .ToListAsync();

        }




        /// <summary>
        /// Marca um bilhete como pago, atualizando estado, data de emissão e marcação de pagamento confirmado.
        /// </summary>
        /// <param name="bilheteId">ID do bilhete a confirmar pagamento.</param>
        /// <returns>True se a confirmação ocorrer com sucesso, caso contrário False.</returns>
        public async Task<bool> ConfirmarPagamentoEBilheteAsync(int bilheteId)
        {
           
            var bilhete = await _context.Bilhetes
                .FirstOrDefaultAsync(b => b.Id == bilheteId);

            if (bilhete == null)
                return false;

            
            bilhete.PagamentoConfirmado = true; 
            bilhete.DataEmissao = DateTime.Now;
            bilhete.Estado = "Emitido"; 

          
            _context.Bilhetes.Update(bilhete);
            await _context.SaveChangesAsync();

            return true;
        }


        /// <summary>
        /// Obtém os bilhetes futuros (data de voo maior ou igual à atual) para um utilizador identificado pelo UserId,
        /// com dados completos de voo, destino e origem.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <returns>Lista dos bilhetes futuros ativos.</returns>
        public async Task<List<Bilhete>> GetBilhetesFuturosByUserAsync(string userId)
        {
            var now = DateTime.UtcNow;

            return await _context.Bilhetes
                .Include(b => b.Voo)
                    .ThenInclude(v => v.Origem)
                .Include(b => b.Voo)
                    .ThenInclude(v => v.Destino)
                .Include(b => b.Lugar)
                .Where(b => b.Passageiro.UserId == userId
                            && b.Voo.DataHoraPartida >= now
                            && !b.WasDeleted)
                .ToListAsync();
        }


        /// <summary>
        /// Obtém todos os bilhetes definitivos associados a um determinado voo pelo seu ID.
        /// </summary>
        /// <param name="vooId">ID do voo.</param>
        /// <returns>Lista de bilhetes do voo.</returns>
        public async Task<List<Bilhete>> GetByVooIdAsync(int vooId)
        {
            return await _context.Bilhetes.Where(b => b.VooId == vooId).ToListAsync();
        }

        public async Task<BilheteTemp> GetBilheteTempByIdAsync(int id)
        {
            return await _context.BilhetesTemp
                .Include(b => b.Voo)
                    .ThenInclude(v => v.Origem)
                .Include(b => b.Voo)
                    .ThenInclude(v => v.Destino)
                .Include(b => b.Passageiro)
                .Include(b => b.Lugar)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task CriarBilheteDefinitivoAsync(PagamentoMbwayViewModel model, int passageiroId)
        {
            var bilhete = new Bilhete
            {
                VooId = model.VooId,
                LugarId = model.LugarId,
                Valor = model.Valor,
                PassageiroId = passageiroId,
                DataReserva = DateTime.UtcNow,
                BagagemExtra = model.BagagemExtra, 
                Refeicao = model.Refeicao,         
                WasDeleted = false
            };

            _context.Bilhetes.Add(bilhete);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverBilheteTempAsync(int vooId, int lugarId, int passageiroId)
        {
            var bilheteTemp = await _context.BilhetesTemp
                .FirstOrDefaultAsync(b => b.VooId == vooId && b.LugarId == lugarId && b.PassageiroId == passageiroId && !b.WasDeleted);

            if (bilheteTemp != null)
            {
               
                bilheteTemp.WasDeleted = true;

                // Ou remover fisicamente da base:
                _context.BilhetesTemp.Remove(bilheteTemp);

                await _context.SaveChangesAsync();
            }
        }


    }

}
