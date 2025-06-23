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
                //UserId = clienteUser.Id 
            });
        }


        private void AddVoos()
        {
            var aviao = _context.Avioes.FirstOrDefault(); 
            var origem = _context.Aeroportos.FirstOrDefault();
            var destino = _context.Aeroportos.Skip(1).FirstOrDefault();

            if (aviao != null && origem != null && destino != null)
            {
                _context.Voos.Add(new Voo
                {
                    AviaoId = aviao.Id,
                    OrigemId = origem.Id,
                    DestinoId = destino.Id,
                    DataHoraPartida = DateTime.UtcNow.AddDays(1),
                    DataHoraChegada = DateTime.UtcNow.AddDays(1).AddHours(2)
                });
            }
        }

        private void AddBilhetes(User clienteUser)
        {
            var passageiro = _context.Passageiros.FirstOrDefault(p => p.UserId == clienteUser.Id);
            var voo = _context.Voos.FirstOrDefault();
            var lugar = _context.Lugares.FirstOrDefault();

            if (passageiro != null && voo != null && lugar != null)
            {
                _context.Bilhetes.Add(new Bilhete
                {
                    LugarId = lugar.Id,
                    PassageiroId = passageiro.Id,
                    DataCompra = DateTime.UtcNow,
                    Preco = 100
                });
                lugar.Disponivel = false;
            }
        }


        private void AddLugares()
        {
            // Gera lugares para cada avião existente
            foreach (var aviao in _context.Avioes)
            {
                // Lugares de classe económica
                for (int i = 1; i <= aviao.LugaresEconomica; i++)
                {
                    _context.Lugares.Add(new Lugar
                    {
                        AviaoId = aviao.Id,
                        Codigo = $"E{i}",
                        Disponivel = true,
                        WasDeleted = false
                    });
                }

                // Lugares de classe executiva
                for (int i = 1; i <= aviao.LugaresExecutiva; i++)
                {
                    _context.Lugares.Add(new Lugar
                    {
                        AviaoId = aviao.Id,
                        Codigo = $"X{i}",
                        Disponivel = true,
                        WasDeleted = false
                    });
                }
            }
        }

    }
}
