using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal abstract class GeneralRepository<TDB,TModel, TKey> : IGeneralRepository<TModel, TKey> where TDB : DbContext where TModel:class
    {
        public readonly TDB _context;

        public GeneralRepository(TDB sqlDbContext)
        {
            _context = sqlDbContext;
        }

        public TModel Add(TModel entity)
        {
            _context.Add(entity);
            return entity;
        }

        public async Task<List<TModel>> GetAllAsync()
        {
            return await _context.Set<TModel>().ToListAsync();
        }

        public async Task<TModel?> GetByIdAsync(TKey id)
        {
            return await _context.Set<TModel>().FindAsync(id);
        }

        public TModel Remove(TModel entity)
        {
            return _context.Set<TModel>().Remove(entity).Entity;
        }

        public void RemoveRange(List<TModel> entities)
        {
            _context.Set<TModel>().RemoveRange(entities);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public TModel Update(TModel entity)
        {
            return _context.Set<TModel>().Update(entity).Entity;
        }

        public void UpdateRange(List<TModel> entities)
        {
            _context.Set<TModel>().UpdateRange(entities);
        }
    }
}
