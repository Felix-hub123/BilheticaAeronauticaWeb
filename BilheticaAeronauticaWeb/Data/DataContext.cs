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
            // Chame base uma única vez e logo no início
            base.OnModelCreating(modelBuilder);

            // Configurações de tipos decimais para colunas monetárias
            modelBuilder.Entity<Aeroporto>()
                .Property(p => p.TaxaAeroportoPadrao)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Voo>()
                .Property(v => v.PrecoBase)
                .HasColumnType("decimal(18,2)");

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

            // Configuração dos relacionamentos e comportamento DeleteBehavior

            // Voo - Origem (Aeroporto) - restrito para evitar exclusão se existir voo
            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Origem)
                .WithMany()
                .HasForeignKey(v => v.OrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voo>()
              .HasOne(v => v.Origem)
              .WithMany()
              .HasForeignKey(v => v.OrigemId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Destino)
                .WithMany()
                .HasForeignKey(v => v.DestinoId)
                .OnDelete(DeleteBehavior.NoAction);


            // Bilhete - Passageiro (usuário)
            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Passageiro)
                .WithMany()
                .HasForeignKey(b => b.PassageiroId)
                .IsRequired(false) // Passageiro pode ser null?
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Voo)
                .WithMany()
                .HasForeignKey(b => b.VooId)
                .OnDelete(DeleteBehavior.Restrict);


            // Mesmas configurações para BilheteTemp
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

            // Filtros globais para soft delete
            modelBuilder.Entity<Voo>()
                .HasQueryFilter(v => !v.WasDeleted);

            modelBuilder.Entity<Aeroporto>()
            .Property(a => a.WasDeleted)
            .HasDefaultValue(false)
            .IsRequired();


            modelBuilder.Entity<Bilhete>().HasQueryFilter(b => !b.WasDeleted);
            modelBuilder.Entity<BilheteTemp>().HasQueryFilter(b => !b.WasDeleted);
            modelBuilder.Entity<Lugar>().HasQueryFilter(l => !l.WasDeleted);
            modelBuilder.Entity<Aeroporto>().HasQueryFilter(a => !a.WasDeleted);
            modelBuilder.Entity<Voo>().HasQueryFilter(v => !v.WasDeleted);

        }

    }
}
