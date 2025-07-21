using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace BilheticaAeronauticaWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private Random _random;

        public SeedDb(DataContext context,IUserHelper userHelper, RoleManager<IdentityRole> roleManager  )
        {
            _context = context;
            _userHelper = userHelper;
            _roleManager = roleManager;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

          
            await EnsureRoleAsync("Admin");
            await EnsureRoleAsync("Funcionario");
            await EnsureRoleAsync("Passageiro");




            var adminUser = await EnsureUserWithRoleAsync("admin@aero.com", "Admin123!", "Admin", "Administrador Completo", criarPassageiro: false);
            var funcUser = await EnsureUserWithRoleAsync("func@aero.com", "Funcionario123!", "Funcionario", "Funcionário-Teste", criarPassageiro: false);
            var passageiroUser = await EnsureUserWithRoleAsync("passageiro@aero.com", "Passageiro123!", "Passageiro", "Paulo Henrique", criarPassageiro: true);

            // 3. Outros seeds (apenas seed se não houver)
            if (!_context.Aeroportos.Any())
                AddAeroportos();

            if (!_context.Lugares.Any())
                AddLugares();

        

            await _context.SaveChangesAsync();
        }




        /// <summary>
        /// Cria user, atribui role e, se for passageiro, cria registo na tabela Passageiros.
        /// </summary>
        private async Task<User> EnsureUserWithRoleAsync(
            string email,
            string password,
            string role,
            string nome,
            bool criarPassageiro)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Nome = nome
                };
                var result = await _userHelper.AddUserAsync(user, password);
                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(user, role);
                }
            }
            else
            {
                if (!await _userHelper.IsUserInRoleAsync(user, role))
                    await _userHelper.AddUserToRoleAsync(user, role);
            }

            // Só para a role Passageiro: cria também na tabela Passageiros (ligação obrigatória)
            if (criarPassageiro)
            {
                if (!_context.Passageiros.Any(p => p.UserId == user.Id))
                {
                    _context.Passageiros.Add(new Passageiro
                    {
                        Nome = "Paulo",
                        Apelido = "Henrique",
                        DataRegisto = DateTime.UtcNow,
                        UserId = user.Id
                    });
                }
            }

            return user;
        }


        /// <summary>
        /// Garante/cria role na tabela AspNetRoles
        /// </summary>
        private async Task EnsureRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));
        }




        private void AddAeroportos()
        {
            _context.Aeroportos.AddRange(
                new Aeroporto { Nome = "Lisboa", Cidade = "Lisboa", Pais = "Portugal", IATA = "LIS" },
                new Aeroporto { Nome = "Porto", Cidade = "Porto", Pais = "Portugal", IATA = "OPO" },
                new Aeroporto { Nome = "Faro", Cidade = "Faro", Pais = "Portugal", IATA = "FAO" }
            );
        }

       





        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            
            string[] roles = { "Admin", "Funcionario", "Passageiro" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

         
            var adminEmail = "admin@email.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nome = "Administrador",
          
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }



        private void AddLugares()
        {
            for (int i = 1; i <= 5; i++)
            {
                _context.Lugares.Add(new Lugar
                {
                    Codigo = $"E{i}",
                    Disponivel = true,
                    WasDeleted = false
                });
            }
        }




    }
}
