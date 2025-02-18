using Core;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class UserItemRepository : GeneralRepository<SqlDbContext, UserItem, Guid>, IUserItemRepository
    {
        public UserItemRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public IQueryable<UserItem> UserItemQuery { get => _context.UserItems; }
    }
}