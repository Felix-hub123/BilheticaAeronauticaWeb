using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;

namespace BilheticaAeronauticaWeb.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Aviao> Avioes { get; set; }
        public DbSet<Aeroporto> Aeroportos { get; set; }
        public DbSet<Lugar> Lugares { get; set; }
        public DbSet<Passageiro> Passageiros { get; set; }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

       
    }
}
