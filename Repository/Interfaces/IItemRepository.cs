using Core;

namespace Repository
{
    public interface IItemRepository : IGeneralRepository<Item, Guid>
    {
        IQueryable<Item> ItemQuery { get; }

        Task<Item?> GetItemByIdIncludeSellers(Guid id);
        Task<List<UserItem>> GetItemSellers(Guid id);
        Task<List<Item>> GetUserItems(Guid userId);
    }
}
