using System.Linq;
using System.Threading.Tasks;
using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;


namespace BilheticaAeronauticaWeb.Data
{
    /// <summary>
    /// Repositório genérico para operações básicas CRUD em entidades que implementam IEntity.
    /// Utiliza Entity Framework Core para acesso assíncrono e tracking otimizado.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade (classe) que implementa IEntity.</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        protected readonly DataContext _context;

        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAllAsync();
            return entity; 
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await SaveAllAsync();
        }

        public async Task DeleteAsync(T entity)
        {
           _context.Set<T>().Remove(entity);
            await SaveAllAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id);
        }

        private async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<bool> AddItemToBilheteAsync(BilheteTemp bilheteTemp, string userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
