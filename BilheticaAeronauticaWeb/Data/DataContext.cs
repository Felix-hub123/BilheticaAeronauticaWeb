using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            // 1. Chamar a base primeiro para o Identity funcionar
            base.OnModelCreating(modelBuilder);

            #region Configurações de Tipos Decimais (Monetários)
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
            #endregion

            #region Filtros Globais para Soft Delete
           
            modelBuilder.Entity<Aeroporto>().HasQueryFilter(a => !a.WasDeleted);
            modelBuilder.Entity<Aviao>().HasQueryFilter(a => !a.WasDeleted);
            modelBuilder.Entity<Lugar>().HasQueryFilter(l => !l.WasDeleted);
            modelBuilder.Entity<Voo>().HasQueryFilter(v => !v.WasDeleted);
            modelBuilder.Entity<Passageiro>().HasQueryFilter(p => !p.WasDeleted);
            modelBuilder.Entity<Bilhete>().HasQueryFilter(b => !b.WasDeleted);
            modelBuilder.Entity<BilheteDetail>().HasQueryFilter(b => !b.WasDeleted);
            modelBuilder.Entity<BilheteTemp>().HasQueryFilter(b => !b.WasDeleted);
            #endregion

            #region Configuração de Relacionamentos e Restrições (DeleteBehavior)

            // 1. Voo - Aeroporto de Origem
            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Origem)
                .WithMany()
                .HasForeignKey(v => v.OrigemId)
                .OnDelete(DeleteBehavior.Restrict); // Impede que apagar um Aeroporto apague os Voos

            // 2. Voo - Aeroporto de Destino
            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Destino)
                .WithMany()
                .HasForeignKey(v => v.DestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Voo - Aviao
            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Aviao)
                .WithMany()
                .HasForeignKey(v => v.AviaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Lugar - Relação com Avião (Os lugares físicos de fábrica do Aparelho)
            modelBuilder.Entity<Lugar>()
                .HasOne(l => l.Aviao)
                .WithMany(a => a.Lugares) // Mapeia com a lista real que tens em Aviao
                .HasForeignKey(l => l.AviaoId)
                .OnDelete(DeleteBehavior.Cascade); // Se o avião for excluído fisicamente, os assentos vão com ele

            // 5. Lugar - Relação com Voo (Bloqueio dinâmico do assento no Voo)
            // CORREÇÃO: Informar ao EF que a lista inversa é v.Lugares resolve o aviso de "VooId1"
            modelBuilder.Entity<Lugar>()
                .HasOne(l => l.Voo)
                .WithMany(v => v.Lugares)
                .HasForeignKey(l => l.VooId)
                .IsRequired(false) // Permite nulo, já que int? VooId é opcional
                .OnDelete(DeleteBehavior.Restrict); // Crucial para não gerar caminhos cíclicos com o Voo

            // 6. Bilhete - Passageiro
            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Passageiro)
                .WithMany()
                .HasForeignKey(b => b.PassageiroId)
                .OnDelete(DeleteBehavior.Restrict);

            // 7. Bilhete - Voo
            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Voo)
                .WithMany()
                .HasForeignKey(b => b.VooId)
                .OnDelete(DeleteBehavior.Restrict);

            // 8. Bilhete - Lugar
            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Lugar)
                .WithMany()
                .HasForeignKey(b => b.LugarId)
                .OnDelete(DeleteBehavior.Restrict);

            // 9. Bilhete - User (Mapeia a lista virtual de Bilhetes que tens na classe User)
            modelBuilder.Entity<Bilhete>()
                .HasOne<User>()
                .WithMany(u => u.Bilhetes)
                .HasForeignKey(b => b.CriadoPorUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // 10. BilheteTemp - Validações de Integridade do Carrinho
            modelBuilder.Entity<BilheteTemp>()
                .HasOne(b => b.Passageiro)
                .WithMany()
                .HasForeignKey(b => b.PassageiroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BilheteTemp>()
                .HasOne(b => b.Voo)
                .WithMany()
                .HasForeignKey(b => b.VooId)
                .OnDelete(DeleteBehavior.Restrict); // Alterado para Restrict para evitar choque com a cascata do Lugar

            modelBuilder.Entity<BilheteTemp>()
                .HasOne(b => b.Lugar)
                .WithMany()
                .HasForeignKey(b => b.LugarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BilheteTemp>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.CriadoPorUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }

        #region Interceção Automática do Soft Delete
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Captura as entidades marcadas para eliminação
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
               
                var wasDeletedProp = entry.Entity.GetType().GetProperty("WasDeleted");

                if (wasDeletedProp != null)
                {
                    // Altera o estado de 'Deleted' (DELETE) para 'Modified' (UPDATE)
                    entry.State = EntityState.Modified;

                    // Define o valor da propriedade WasDeleted para true
                    wasDeletedProp.SetValue(entry.Entity, true);
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion

    }
}
