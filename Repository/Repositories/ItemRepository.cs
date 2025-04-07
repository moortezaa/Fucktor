using Core;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class ItemRepository : GeneralRepository<SqlDbContext, Item, Guid>, IItemRepository
    {
        public ItemRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public IQueryable<Item> ItemQuery { get => _context.Items; }

        public async Task<Item?> GetItemByIdIncludeSellers(Guid id)
        {
            return await _context.Items.Include(i=>i.Sellers).Where(i => i.Id == id).SingleOrDefaultAsync();
        }

        public async Task<List<UserItem>> GetItemSellers(Guid id)
        {
            return await _context.UserItems.Where(ui=>ui.ItemId == id).ToListAsync();
        }

        public async Task<List<Item>> GetUserItems(Guid userId)
        {
            return await _context.Items.Where(i => i.Sellers.Any(s => s.UserId == userId)).ToListAsync();
        }
    }
}