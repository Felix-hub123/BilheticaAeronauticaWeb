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
        public DbSet<Bilhete> Bilhetes { get; set; } 

        public DbSet<Voo> Voos { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Voo>()
              .HasOne(v => v.Origem)
              .WithMany()
             .HasForeignKey(v => v.OrigemId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Destino)
                .WithMany()
                .HasForeignKey(v => v.DestinoId)
                .OnDelete(DeleteBehavior.Restrict);





        }
    }
}
