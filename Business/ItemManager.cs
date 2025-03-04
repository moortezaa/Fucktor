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

        public readonly IQueryable ItemQuery = itemRepository.ItemQuery;

        public async Task<BusinessResult> CreateOrUpdateItem(Item item)
        {
            _itemRepository.Update(item);
            var rows = await _itemRepository.SaveChangesAsync();
            if (rows>0)
            {
                return new BusinessResult()
                {
                    Succeeded = true,
                };
            }
            return new BusinessResult()
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
    }
}
