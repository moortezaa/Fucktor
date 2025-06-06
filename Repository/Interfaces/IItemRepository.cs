﻿using Core;

namespace Repository
{
    public interface IItemRepository : IGeneralRepository<Item, Guid>
    {
        IQueryable<Item> ItemQuery { get; }

        void AddUserItem(UserItem userItem);
        Task<Item?> GetItemByIdIncludeSeller(Guid itemId, Guid sellerId);
        Task<Item?> GetItemByIdIncludeSellers(Guid id);
        Task<Item?> GetItemByName(string name);
        Task<UserItem?> GetItemSeller(Guid itemId, Guid userId);
        Task<List<UserItem>> GetItemSellers(Guid id);
        Task<UserItem?> GetUserItem(Guid userId, Guid itemId);
        Task<List<Item>> GetUserItems(Guid userId);
    }
}
