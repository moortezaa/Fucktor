using Core;

namespace Repository
{
    public interface IItemRepository : IGeneralRepository<Item, Guid>
    {
        IQueryable<Item> ItemQuery { get; }

        Task<List<Item>> GetUserItems(Guid userId);
    }
}
