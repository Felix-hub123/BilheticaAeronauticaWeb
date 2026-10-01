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
    /// Classe responsável por popular a base de dados com dados iniciais.
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
            // CRIAÇÃO DA BASE DE DADOS
            // =========================================================
            if (_context.Database.IsNpgsql())
            {
                await EnsurePostgreSqlTablesAsync();
            }
            else
            {
                // SQL Server local
                await _context.Database.EnsureCreatedAsync();
            }

            // =========================================================
            // ROLES
            // =========================================================
            await EnsureRoleAsync("Admin");
            await EnsureRoleAsync("Funcionario");
            await EnsureRoleAsync("Passageiro");

            // =========================================================
            // UTILIZADORES INICIAIS
            // =========================================================
            var adminUser = await EnsureUserWithRoleAsync(
                "admin1@yopmail.com",
                "Admin123!",
                "Admin",
                "Administrador",
                "Completo",
                false);

            var funcUser = await EnsureUserWithRoleAsync(
                "funcionario@yopmail.com",
                "Funcionario123!",
                "Funcionario",
                "Dário",
                "Funcionario",
                false);

            var passageiroUser = await EnsureUserWithRoleAsync(
                "passageiro@aero.com",
                "Passageiro123!",
                "Passageiro",
                "Paulo",
                "Henrique",
                true);

            // =========================================================
            // AEROPORTOS
            // =========================================================
            if (!_context.Aeroportos.Any())
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
            }

            // =========================================================
            // LUGARES
            // =========================================================
            if (!_context.Lugares.Any())
            {
                for (int i = 1; i <= 6; i++)
                {
                    _context.Lugares.Add(new Lugar
                    {
                        Codigo = $"A{i}",
                        Disponivel = true,
                        WasDeleted = false
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// No Supabase já existem tabelas internas.
        /// Por isso EnsureCreated pode não criar as tabelas da aplicação.
        /// Verificamos especificamente se a tabela Voos existe.
        /// </summary>
        private async Task EnsurePostgreSqlTablesAsync()
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'public'
                    AND table_name = 'Voos'
                );
            ";

            var result = await command.ExecuteScalarAsync();

            var voosTableExists =
                result != null &&
                result != DBNull.Value &&
                Convert.ToBoolean(result);

            if (!voosTableExists)
            {
                var createScript =
                    _context.Database.GenerateCreateScript();

                await _context.Database.ExecuteSqlRawAsync(
                    createScript);
            }
        }

        /// <summary>
        /// Cria utilizador, atribui role e cria passageiro quando necessário.
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
                await _userHelper.GetUserByEmailAsync(email);

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
                    await _userHelper.AddUserAsync(
                        user,
                        password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Erro ao criar utilizador {email}: " +
                        string.Join(
                            ", ",
                            result.Errors.Select(
                                e => e.Description)));
                }

                await _userHelper.AddUserToRoleAsync(
                    user,
                    role);
            }
            else if (
                !await _userHelper.IsUserInRoleAsync(
                    user,
                    role))
            {
                await _userHelper.AddUserToRoleAsync(
                    user,
                    role);
            }

            if (criarPassageiro)
            {
                if (!_context.Passageiros.Any(
                    p => p.UserId == user.Id))
                {
                    _context.Passageiros.Add(
                        new Passageiro
                        {
                            Nome = nome,
                            Apelido = apelido,
                            DataRegisto = DateTime.UtcNow,
                            UserId = user.Id
                        });
                }
            }

            return user;
        }

        /// <summary>
        /// Garante que determinada role existe.
        /// </summary>
        private async Task EnsureRoleAsync(
            string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(
                    roleName))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(roleName));
            }
        }
    }
}
