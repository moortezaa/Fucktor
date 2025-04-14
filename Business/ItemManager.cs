using Business.DTO;
using Core;
using Microsoft.Extensions.Localization;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ItemManager(IItemRepository itemRepository, IStringLocalizer<ItemManager> localizer)
    {
        private readonly IItemRepository _itemRepository = itemRepository;
        private readonly IStringLocalizer<ItemManager> localizer = localizer;

        public readonly IQueryable<Item> ItemQuery = itemRepository.ItemQuery;

        public async Task<ItemResult> CreateOrUpdateItem(Item item, AppUser? seller = null)
        {
            //check if the name is duplicate
            //if the name is duplicate we will use the userItem to customize the item for this user and do not update the item itself
            var duplicateItem = await _itemRepository.GetItemByName(item.Name);
            if (duplicateItem != null && duplicateItem.Id != item.Id)
            {
                //the user is trying to add an item which already exist, so we use the userItem to customize it
                if (seller == null)
                {
                    throw new Exception("seller is required to customize an Item");
                }
                duplicateItem.Sellers.Add(new UserItem()
                {
                    UserId = seller.Id,
                    DefaultUnitPrice = item.Sellers.FirstOrDefault()?.DefaultUnitPrice ?? 0,
                    DisplayName = item.Sellers.FirstOrDefault()?.DisplayName ?? item.Name,
                    StorageAmount = item.Sellers.FirstOrDefault()?.StorageAmount ?? 0,
                });
                _itemRepository.Update(duplicateItem);
                var rows = await _itemRepository.SaveChangesAsync();
                if (rows > 0)
                {
                    return new ItemResult()
                    {
                        Succeeded = true,
                        Item = duplicateItem
                    };
                }
            }
            else
            {
                //sellers should not be modified in this context and should be handled by the seller argument.
                item.Sellers = [];

                if (seller != null)
                {
                    var itemSeller = await _itemRepository.GetItemSeller(item.Id, seller.Id);
                    if (itemSeller == null)
                    {
                        item.Sellers.Add(new UserItem()
                        {
                            User = seller,
                            DisplayName = item.Name,
                        });
                    }
                }
                _itemRepository.Update(item);
                var rows = await _itemRepository.SaveChangesAsync();
                if (rows > 0)
                {
                    var itemSeller = await _itemRepository.GetItemSeller(item.Id, seller.Id);
                    if (itemSeller != null)
                    {
                        item.Sellers.Add(itemSeller);
                    }
                    return new ItemResult()
                    {
                        Succeeded = true,
                        Item = item
                    };
                }
            }
            return new ItemResult()
            {
                Succeeded = false,
                Errors = [localizer["Error updating item."]]
            };
        }

        public async Task<List<Item>> GetUserItems(Guid userId)
        {
            return await _itemRepository.GetUserItems(userId);
        }

        public async Task<BusinessResult> RemoveItem(Guid itemId)
        {
            var item = await _itemRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                return new BusinessResult()
                {
                    Succeeded = false,
                    Errors = [localizer["Item not found."]]
                };
            }
            _itemRepository.Remove(item);
            var rows = await _itemRepository.SaveChangesAsync();

            if (rows > 0)
            {
                return new BusinessResult()
                {
                    Succeeded = true,
                };
            }
            return new BusinessResult()
            {
                Succeeded = false,
                Errors = [localizer["Error removing item."]]
            };
        }

        public async Task<Item?> GetItemById(Guid id)
        {
            return await _itemRepository.GetByIdAsync(id);
        }

        public async Task<Item?> GetItemByIdIncludeSellers(Guid id)
        {
            return await _itemRepository.GetItemByIdIncludeSellers(id);
        }

        public async Task<List<UserItem>> GetItemSellers(Guid id)
        {
            return await _itemRepository.GetItemSellers(id);
        }

        public async Task<UserItem?> GetUserItem(Guid userId, Guid itemId)
        {
            return await _itemRepository.GetUserItem(userId, itemId);
        }

        public async Task<BusinessResult> CreateUserItem(UserItem userItem)
        {
            _itemRepository.AddUserItem(userItem);
            var rows = await _itemRepository.SaveChangesAsync();
            if (rows > 0)
            {
                return new BusinessResult()
                {
                    Succeeded = true,
                };
            }
            return new BusinessResult()
            {
                Succeeded = false,
                Errors = ["now rows affected."]
            };
        }

        public async Task<Item?> GetItemByName(string name)
        {
            return await _itemRepository.GetItemByName(name);
        }
    }
}
