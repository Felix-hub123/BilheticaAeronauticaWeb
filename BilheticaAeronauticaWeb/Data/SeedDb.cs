using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Responsável pela criação dos dados iniciais da aplicação.
    /// </summary>
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userHelper = userHelper;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // =========================================================
            // BASE DE DADOS
            // =========================================================

            if (_context.Database.IsNpgsql())
            {
                await EnsurePostgreSqlTablesAsync();
            }
            else
            {
                await _context.Database.EnsureCreatedAsync();
            }

            // =========================================================
            // ROLES
            // =========================================================

            await EnsureRoleAsync("Admin");
            await EnsureRoleAsync("Funcionario");
            await EnsureRoleAsync("Passageiro");

            // =========================================================
            // UTILIZADORES
            // =========================================================

            await EnsureUserWithRoleAsync(
                "admin1@yopmail.com",
                "Admin123!",
                "Admin",
                "Administrador",
                "Completo",
                false);

            await EnsureUserWithRoleAsync(
                "funcionario@yopmail.com",
                "Funcionario123!",
                "Funcionario",
                "Dário",
                "Funcionario",
                false);

            await EnsureUserWithRoleAsync(
                "passageiro@aero.com",
                "Passageiro123!",
                "Passageiro",
                "Paulo",
                "Henrique",
                true);

            /*
             * IMPORTANTE:
             *
             * Guardamos já o Passageiro.
             *
             * Assim, mesmo que exista algum problema posteriormente
             * nos aeroportos, aviões ou lugares, o passageiro não fica
             * por criar.
             */
            await _context.SaveChangesAsync();

            // =========================================================
            // AEROPORTOS
            // =========================================================

            if (!await _context.Aeroportos.AnyAsync())
            {
                _context.Aeroportos.AddRange(
                    new Aeroporto
                    {
                        Nome = "Lisboa",
                        Cidade = "Lisboa",
                        Pais = "Portugal",
                        IATA = "LIS"
                    },
                    new Aeroporto
                    {
                        Nome = "Porto",
                        Cidade = "Porto",
                        Pais = "Portugal",
                        IATA = "OPO"
                    },
                    new Aeroporto
                    {
                        Nome = "Faro",
                        Cidade = "Faro",
                        Pais = "Portugal",
                        IATA = "FAO"
                    }
                );

                await _context.SaveChangesAsync();
            }

            // =========================================================
            // AVIÃO DE TESTE
            // =========================================================

            var aviao =
                await _context.Avioes
                    .FirstOrDefaultAsync();

            if (aviao == null)
            {
                aviao = new Aviao
                {
                    Marca = "Airbus",
                    Modelo = "A320-200",

                    LugaresEconomica = 6,
                    LugaresExecutiva = 0,

                    Disponivel = true,
                    WasDeleted = false,

                    ImageId = Guid.Empty
                };

                _context.Avioes.Add(aviao);

                /*
                 * Precisamos guardar o avião antes de criar lugares,
                 * porque os lugares precisam do AviaoId.
                 */
                await _context.SaveChangesAsync();
            }

            // =========================================================
            // LUGARES
            // =========================================================

            /*
             * Antes:
             *
             * Lugar
             *   AviaoId = 0
             *
             * PostgreSQL:
             * FK_Lugares_Avioes_AviaoId ❌
             *
             *
             * Agora:
             *
             * Lugar
             *   AviaoId = aviao.Id
             *
             * PostgreSQL:
             * FK válida ✅
             */

            var existemLugaresDoAviao =
                await _context.Lugares
                    .AnyAsync(
                        l => l.AviaoId == aviao.Id);

            if (!existemLugaresDoAviao)
            {
                for (int i = 1; i <= 6; i++)
                {
                    _context.Lugares.Add(
                        new Lugar
                        {
                            Codigo = $"A{i}",

                            AviaoId = aviao.Id,

                            Disponivel = true,

                            WasDeleted = false
                        });
                }

                await _context.SaveChangesAsync();
            }
        }

        // =========================================================
        // POSTGRESQL
        // =========================================================

        /// <summary>
        /// No Supabase existem tabelas internas.
        ///
        /// Por isso, EnsureCreated pode entender incorretamente que
        /// a base de dados já foi criada.
        ///
        /// Verificamos especificamente uma tabela da aplicação.
        /// </summary>
        private async Task EnsurePostgreSqlTablesAsync()
        {
            var connection =
                _context.Database.GetDbConnection();

            if (connection.State !=
                System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command =
                connection.CreateCommand();

            command.CommandText = @"
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                    AND table_name = 'Voos'
                );
            ";

            var result =
                await command.ExecuteScalarAsync();

            var voosTableExists =
                result != null &&
                result != DBNull.Value &&
                Convert.ToBoolean(result);

            if (!voosTableExists)
            {
                var createScript =
                    _context.Database
                        .GenerateCreateScript();

                await _context.Database
                    .ExecuteSqlRawAsync(
                        createScript);
            }
        }

        // =========================================================
        // UTILIZADORES
        // =========================================================

        /// <summary>
        /// Garante que o utilizador existe,
        /// possui a role correta e,
        /// quando necessário,
        /// possui também uma entidade Passageiro.
        /// </summary>
        private async Task<User> EnsureUserWithRoleAsync(
            string email,
            string password,
            string role,
            string nome,
            string apelido,
            bool criarPassageiro)
        {
            var user =
                await _userHelper
                    .GetUserByEmailAsync(email);

            // =====================================================
            // CRIAR USER
            // =====================================================

            if (user == null)
            {
                user = new User
                {
                    UserName = email,

                    Email = email,

                    EmailConfirmed = true,

                    Nome = nome,

                    Apelido = apelido
                };

                var result =
                    await _userHelper
                        .AddUserAsync(
                            user,
                            password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Erro ao criar utilizador {email}: " +
                        string.Join(
                            ", ",
                            result.Errors
                                .Select(
                                    e =>
                                        e.Description)));
                }
            }

            // =====================================================
            // GARANTIR ROLE
            // =====================================================

            if (!await _userHelper
                .IsUserInRoleAsync(
                    user,
                    role))
            {
                await _userHelper
                    .AddUserToRoleAsync(
                        user,
                        role);
            }

            // =====================================================
            // GARANTIR PASSAGEIRO
            // =====================================================

            if (criarPassageiro)
            {
                var passageiroExiste =
                    await _context.Passageiros
                        .AnyAsync(
                            p =>
                                p.UserId ==
                                user.Id);

                if (!passageiroExiste)
                {
                    var passageiro =
                        new Passageiro
                        {
                            Nome = nome,

                            Apelido = apelido,

                            UserId = user.Id,

                            DataRegisto =
                                DateTime.UtcNow,

                            WasDeleted = false,

                            ImageId = Guid.Empty
                        };

                    _context.Passageiros.Add(
                        passageiro);
                }
            }

            return user;
        }

        // =========================================================
        // ROLES
        // =========================================================

        /// <summary>
        /// Garante que uma role existe.
        /// </summary>
        private async Task EnsureRoleAsync(
            string roleName)
        {
            if (!await _roleManager
                .RoleExistsAsync(
                    roleName))
            {
                var result =
                    await _roleManager
                        .CreateAsync(
                            new IdentityRole(
                                roleName));

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Erro ao criar role {roleName}: " +
                        string.Join(
                            ", ",
                            result.Errors
                                .Select(
                                    e =>
                                        e.Description)));
                }
            }
        }
    }
}