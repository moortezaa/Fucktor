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
    }
}