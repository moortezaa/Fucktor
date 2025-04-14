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

        public void AddUserItem(UserItem userItem)
        {
            _context.UserItems.Add(userItem);
        }

        public async Task<Item?> GetItemByIdIncludeSeller(Guid itemId, Guid sellerId)
        {
            var item = await _context.Items.SingleOrDefaultAsync(i => i.Id == itemId);
            var seller = await _context.UserItems.Where(ui => ui.ItemId == itemId && ui.UserId == sellerId).SingleOrDefaultAsync();
            if (item != null && seller != null)
            {
                item.Sellers = [seller];
            }
            return item;
        }

        public async Task<Item?> GetItemByIdIncludeSellers(Guid id)
        {
            return await _context.Items.Include(i => i.Sellers).Where(i => i.Id == id).SingleOrDefaultAsync();
        }

        public async Task<Item?> GetItemByName(string name)
        {
            return await _context.Items.Where(i => i.Name == name).SingleOrDefaultAsync();
        }

        public async Task<UserItem?> GetItemSeller(Guid itemId, Guid userId)
        {
            return await _context.UserItems.Where(ui => ui.ItemId == itemId && ui.UserId == userId).SingleOrDefaultAsync();
        }

        public async Task<List<UserItem>> GetItemSellers(Guid id)
        {
            return await _context.UserItems.Where(ui => ui.ItemId == id).ToListAsync();
        }

        public async Task<UserItem?> GetUserItem(Guid userId, Guid itemId)
        {
            return await _context.UserItems.Where(ui => ui.UserId == userId && ui.ItemId == itemId).SingleOrDefaultAsync();
        }

        public async Task<List<Item>> GetUserItems(Guid userId)
        {
            return await _context.Items.Where(i => i.Sellers.Any(s => s.UserId == userId)).ToListAsync();
        }
    }
}