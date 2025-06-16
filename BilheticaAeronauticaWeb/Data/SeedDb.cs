using System;
using System.Linq;
using System.Threading.Tasks;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.AspNetCore.Identity;


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

            // 1. Garante que as roles existem
            await EnsureRoleAsync("Admin");
            await EnsureRoleAsync("Funcionario");
            await EnsureRoleAsync("Cliente");

            // 2. Cria utilizadores e associa roles
            var adminUser = await EnsureUserAsync("admin@aero.com", "Admin123!", "Admin");
            var funcUser = await EnsureUserAsync("func@aero.com", "Funcionario123!", "Funcionario");
            var clienteUser = await EnsureUserAsync("cliente@aero.com", "Cliente123!", "Cliente");

            // 3. Cria entidades de domínio associadas aos utilizadores
            if (!_context.Aeroportos.Any())
                AddAeroportos();

            if (!_context.Passageiros.Any())
                AddPassageiros(clienteUser);

            if (!_context.Lugares.Any())
                AddLugares();

           

            await _context.SaveChangesAsync();
        }

        private async Task EnsureRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));
        }


        private async Task<User> EnsureUserAsync(string email, string password, string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await _userHelper.AddUserAsync(user, password);
                if (result.Succeeded)
                    await _userHelper.AddUserToRoleAsync(user, role);
            }
            else
            {
                if (!await _userHelper.IsUserInRoleAsync(user, role))
                    await _userHelper.AddUserToRoleAsync(user, role);
            }
            return user;
        }

        //private void AddBilhetes()
        //{
        //    var voo = _context.Voos.FirstOrDefault();
        //    var lugar = _context.Lugares.FirstOrDefault();
        //    var passageiro = _context.Passageiros.FirstOrDefault();
        //    if (voo != null && lugar != null && passageiro != null)
        //    {
        //        _context.Bilhetes.Add(new Bilhete
        //        {
        //            Tarifa = 100,
        //            Classe = "Económica",
        //            PodeAlterar = false,
        //            DataCompra = DateTime.Now,
        //            //VooId = voo.Id,
        //            //LugarId = lugar.Id,
        //            //PassageiroId = passageiro.Id,
        //            //UserId = null 
        //        });
        //    }
        //}


        private void AddAeroportos()
        {
            _context.Aeroportos.AddRange(
                new Aeroporto { Nome = "Lisboa", Cidade = "Lisboa", Pais = "Portugal", IATA = "LIS" },
                new Aeroporto { Nome = "Porto", Cidade = "Porto", Pais = "Portugal", IATA = "OPO" },
                new Aeroporto { Nome = "Faro", Cidade = "Faro", Pais = "Portugal", IATA = "FAO" }
            );
        }

        private void AddPassageiros(User clienteUser)
        {
            _context.Passageiros.Add(new Passageiro
            {
                Nome = "Ana",
                Apelido = "Silva",
                DocumentoIdentificacao = "CC",
                NumeroDocumento = "12345678",
                DataRegisto = DateTime.UtcNow,
                UserId = clienteUser.Id 
            });
        }

        private void AddLugares()
        {
            for (int fila = 1; fila <= 2; fila++)
            {
                for (int numero = 1; numero <= 5; numero++)
                {
                    _context.Lugares.Add(new Lugar
                    {
                        Fila = fila.ToString(),
                        Numero = numero,
                        Classe = "Económica",
                        Disponivel = true,
                        AeronaveId = 1 
                    });
                }
            }
        }

              
    }
}
