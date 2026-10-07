using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Repositório específico para operações relacionadas
    /// com a entidade Passageiro.
    /// </summary>
    public class PassageiroRepository
        : GenericRepository<Passageiro>, IPassageiroRepository
    {
        private new readonly DataContext _context;

        /// <summary>
        /// Inicializa uma nova instância do PassageiroRepository.
        /// </summary>
        /// <param name="context">
        /// Contexto de dados da aplicação.
        /// </param>
        public PassageiroRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém um passageiro através do identificador
        /// do utilizador associado.
        /// </summary>
        public async Task<Passageiro> GetByUserIdAsync(string userId)
        {
            return await _context.Passageiros
                .FirstOrDefaultAsync(
                    p => p.UserId == userId);
        }

        /// <summary>
        /// Obtém o identificador do passageiro associado
        /// a determinado utilizador.
        /// </summary>
        public async Task<int> ObterIdPorUserIdAsync(string userId)
        {
            var passageiro = await _context.Passageiros
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.UserId == userId);

            return passageiro?.Id ?? 0;
        }

        /// <summary>
        /// Adiciona um novo passageiro à base de dados,
        /// garantindo compatibilidade das datas com PostgreSQL.
        /// </summary>
        public async Task AddAsync(Passageiro passageiro)
        {
            if (passageiro == null)
            {
                throw new ArgumentNullException(nameof(passageiro));
            }

            // =========================================================
            // DATA DE REGISTO
            // =========================================================

            if (passageiro.DataRegisto == default)
            {
                passageiro.DataRegisto = DateTime.UtcNow;
            }
            else
            {
                passageiro.DataRegisto =
                    GarantirUtc(passageiro.DataRegisto);
            }

            // =========================================================
            // DATA DE NASCIMENTO
            // =========================================================
            //
            // O input HTML do tipo "date" normalmente produz
            // DateTime com Kind = Unspecified.
            //
            // Como a base PostgreSQL atual possui esta coluna
            // como timestamp with time zone, o Npgsql exige UTC.
            //
            // Não fazemos ToUniversalTime(), porque uma data de
            // nascimento é uma data civil e não queremos alterar
            // dia/mês devido ao fuso horário.
            // =========================================================

            if (passageiro.DataNascimento.HasValue)
            {
                passageiro.DataNascimento =
                    DateTime.SpecifyKind(
                        passageiro.DataNascimento.Value,
                        DateTimeKind.Utc);
            }

            _context.Passageiros.Add(passageiro);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Normaliza um DateTime para UTC.
        /// </summary>
        private static DateTime GarantirUtc(DateTime data)
        {
            return data.Kind switch
            {
                DateTimeKind.Utc =>
                    data,

                DateTimeKind.Local =>
                    data.ToUniversalTime(),

                DateTimeKind.Unspecified =>
                    DateTime.SpecifyKind(
                        data,
                        DateTimeKind.Utc),

                _ =>
                    data
            };
        }
    }
}
