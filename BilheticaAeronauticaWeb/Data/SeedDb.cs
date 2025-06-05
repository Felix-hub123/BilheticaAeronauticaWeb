using BilheticaAeronauticaWeb.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            if (!_context.Aeroportos.Any())
            {
                AddAeroportos();
            }
            if (!_context.Voos.Any())
            {
                AddVoos();
            }
            if (!_context.Passageiros.Any())
            {
                AddPassageiros();
            }
            if (!_context.Lugares.Any())
            {
                AddLugares();
            }
            if (!_context.Bilhetes.Any())
            {
                AddBilhetes();
            }
            await _context.SaveChangesAsync();
        }

        private void AddBilhetes()
        {
            var voo = _context.Voos.FirstOrDefault();
            var lugar = _context.Lugares.FirstOrDefault();
            var passageiro = _context.Passageiros.FirstOrDefault();
            if (voo != null && lugar != null && passageiro != null)
            {
                _context.Bilhetes.Add(new Bilhete
                {
                    Tarifa = 100,
                    Classe = "Económica",
                    PodeAlterar = false,
                    DataCompra = DateTime.Now,
                    VooId = voo.Id,
                    LugarId = lugar.Id,
                    PassageiroId = passageiro.Id,
                    UserId = null 
                });
            }
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

        private void AddPassageiros()
        {
            _context.Passageiros.AddRange(
                 new Passageiro { Nome = "Ana", Apelido = "Silva", DocumentoIdentificacao = "CC", NumeroDocumento = "12345678" },
                 new Passageiro { Nome = "João", Apelido = "Santos", DocumentoIdentificacao = "CC", NumeroDocumento = "87654321" }
            );
        }

        private void AddVoos()
        {
            var aeroportoLisboa = _context.Aeroportos.FirstOrDefault(a => a.IATA == "LIS");
            var aeroportoPorto = _context.Aeroportos.FirstOrDefault(a => a.IATA == "OPO");
            if (aeroportoLisboa != null && aeroportoPorto != null)
            {
                _context.Voos.Add(
                    new Voo
                    {
                        NumeroVoo = "TP123",
                        DataHora = DateTime.Now.AddDays(1),
                        OrigemId = aeroportoLisboa.Id,
                        DestinoId = aeroportoPorto.Id,
                        Ativo = true,
                        AeronaveId = 1 
                    }
                );
            }
        }

        private void AddAeroportos()
        {
            _context.Aeroportos.AddRange(
                new Aeroporto { Nome = "Lisboa", Cidade = "Lisboa", Pais = "Portugal", IATA = "LIS" },
                new Aeroporto { Nome = "Porto", Cidade = "Porto", Pais = "Portugal", IATA = "OPO" },
                new Aeroporto { Nome = "Faro", Cidade = "Faro", Pais = "Portugal", IATA = "FAO" }
            );
        }
    }
}
