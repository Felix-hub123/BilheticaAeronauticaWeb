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

        public DbSet<BilheteDetail> BilheteDetails { get; set; }

        public DbSet<BilheteTemp> BilhetesTemp { get; set; }    


        public DbSet<Voo> Voos { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Aeroporto>()
             .Property(p => p.TaxaAeroportoPadrao)
             .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Bilhete>()
              .Property(b => b.Valor)
              .HasColumnType("decimal(18,2)");

          modelBuilder.Entity<BilheteTemp>()
             .Property(b => b.Preco)
             .HasColumnType("decimal(18,2)");

          modelBuilder.Entity<BilheteDetail>()
             .Property(b => b.Preco)
             .HasColumnType("decimal(18,2)");

          modelBuilder.Entity<Lugar>()
             .Property(l => l.PrecoBase)
             .HasColumnType("decimal(18,2)");

         
            modelBuilder.Entity<Voo>()
             .HasOne(v => v.Origem)
             .WithMany()
             .HasForeignKey(v => v.OrigemId)
             .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Destino)
                .WithMany()
                .HasForeignKey(v => v.DestinoId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Lugar)
                .WithMany()
                .HasForeignKey(b => b.LugarId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Passageiro)
                .WithMany()
                .HasForeignKey(b => b.PassageiroId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Voo)
                .WithMany()
                .HasForeignKey(b => b.VooId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BilheteTemp>()
                .HasOne(b => b.Lugar)
                .WithMany()
                .HasForeignKey(b => b.LugarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BilheteTemp>()
                .HasOne(b => b.Passageiro)
                .WithMany()
                .HasForeignKey(b => b.PassageiroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BilheteTemp>()
                .HasOne(b => b.Voo)
                .WithMany()
                .HasForeignKey(b => b.VooId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Voo>()
                .Property(v => v.PrecoBase)
                .HasColumnType("decimal(18,2)");




        }
    }
}
