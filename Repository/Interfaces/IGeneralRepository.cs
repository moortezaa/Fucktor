namespace Repository
{
    public interface IGeneralRepository<TModel, TKey> where TModel : class
    {
        TModel Add(TModel entity);
        TModel Update(TModel entity);
        void UpdateRange(List<TModel> entities);
        TModel Remove(TModel entity);
        void RemoveRange(List<TModel> entities);
        Task<TModel?> GetByIdAsync(TKey id);
        Task<List<TModel>> GetAllAsync();
        Task<int> SaveChangesAsync();
    }
}