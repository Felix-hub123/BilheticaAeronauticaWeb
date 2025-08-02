using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Classe responsável por popular a base de dados com dados iniciais,
    /// incluindo roles, utilizadores padrão, aeroportos e lugares.
    /// </summary>
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly RoleManager<IdentityRole> _roleManager;


        /// <summary>
        /// Construtor que recebe as dependências para manipulação do contexto,
        /// gestão de utilizadores e roles.
        /// </summary>
        /// <param name="context">Contexto da base dados.</param>
        /// <param name="userHelper">Helper para operação com utilizadores.</param>
        /// <param name="roleManager">Gestor das roles do Identity.</param>
        public SeedDb(DataContext context,IUserHelper userHelper, RoleManager<IdentityRole> roleManager  )
        {
            _context = context;
            _userHelper = userHelper;
            _roleManager = roleManager;

        }


        /// <summary>
        /// Método principal para executar o seed da base de dados.
        /// Garante que a base existe, cria roles, utilizadores padrão, aeroportos e lugares.
        /// </summary>
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();


            await EnsureRoleAsync("Admin");
            await EnsureRoleAsync("Funcionario");
            await EnsureRoleAsync("Passageiro");




            var adminUser = await EnsureUserWithRoleAsync("admin1@yopmail.com", "Admin123!", "Admin", "Administrador", "Completo", false);
            var funcUser = await EnsureUserWithRoleAsync("funcionario@yopmail.com", "Funcionario123!", "Funcionario", "Dário", "Funcionario", false);
            var passageiroUser = await EnsureUserWithRoleAsync("passageiro@aero.com", "Passageiro123!", "Passageiro", "Paulo", "Henrique", true);


            if (!_context.Aeroportos.Any())
            {
                _context.Aeroportos.AddRange(
                    new Aeroporto { Nome = "Lisboa", Cidade = "Lisboa", Pais = "Portugal", IATA = "LIS" },
                    new Aeroporto { Nome = "Porto", Cidade = "Porto", Pais = "Portugal", IATA = "OPO" },
                    new Aeroporto { Nome = "Faro", Cidade = "Faro", Pais = "Portugal", IATA = "FAO" }
                );
            }

           

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
        /// Cria user, atribui role e, se for passageiro, cria registo na tabela Passageiros.
        /// </summary>
        private async Task<User> EnsureUserWithRoleAsync(string email, string password, string role, string nome, string apelido, bool criarPassageiro)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

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

                var result = await _userHelper.AddUserAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new Exception($"Erro ao criar utilizador {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                await _userHelper.AddUserToRoleAsync(user, role);
            }
            else if (!await _userHelper.IsUserInRoleAsync(user, role))
            {
                await _userHelper.AddUserToRoleAsync(user, role);
            }

            // Adiciona registo de Passageiro se pedido
            if (criarPassageiro)
            {
                if (!_context.Passageiros.Any(p => p.UserId == user.Id))
                {
                    _context.Passageiros.Add(new Passageiro
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
        /// Garante a existência de uma role, criando-a se necessário.
        /// </summary>
        /// <param name="roleName">Nome da role a criar.</param>
        private async Task EnsureRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

         

    }
}
