using Core;

namespace Repository
{
    public interface IUserItemRepository : IGeneralRepository<UserItem, Guid>
    {
        IQueryable<UserItem> UserItemQuery { get; }

    }
}
