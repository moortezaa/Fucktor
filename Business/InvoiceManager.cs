using Business.DTO;
using Core;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class InvoiceManager(IInvoiceRepository invoiceRepository, IItemRepository itemRepository)
    {
        private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
        private readonly IItemRepository _itemRepository = itemRepository;
        public readonly IQueryable<Invoice> InvoiceQuery = invoiceRepository.InvoiceQuery;
        public readonly IQueryable<InvoiceItem> InvoiceItemQuery = invoiceRepository.InvoiceItemQuery;

        public async Task<Invoice?> GetInvoiceDetails(Guid id)
        {
            return await _invoiceRepository.GetByIdAsync(id);
        }

        public async Task<BusinessResult> UpdateInvoice(Invoice model)
        {
            _invoiceRepository.Update(model);
            var rows = await _invoiceRepository.SaveChangesAsync();
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
                Errors = ["no rows updated."]
            };
        }

        public async Task<BusinessResult> AddItemToInvoice(Guid id, Guid itemId, Guid sellerId)
        {
            var invoice = await _invoiceRepository.GetByIdIncludeDetailsAsync(id);
            if (invoice == null)
            {
                return new BusinessResult(false, "invoice not found.");
            }

            var item = await _itemRepository.GetItemByIdIncludeSeller(itemId, sellerId);
            if (item == null)
            {
                return new BusinessResult(false, "item not found.");
            }

            if (item.Sellers.Count == 0)
            {
                return new BusinessResult(false, "seller not found.");
            }

            var seller = item.Sellers.Single();

            if (invoice.InvoiceItems.Any(ii => ii.ItemId == itemId))
            {
                return new BusinessResult(false, "Item already exist in invoice.");
            }

            invoice.InvoiceItems.Add(new InvoiceItem()
            {
                ItemId = itemId,
                UnitPrice = seller.DefaultUnitPrice,
                Amount = 1
            });

            _invoiceRepository.Update(invoice);
            var rows = await _invoiceRepository.SaveChangesAsync();
            if (rows > 0)
            {
                return new BusinessResult(true);
            }
            return new BusinessResult(false, "unknown error.");
        }

        public async Task<Invoice?> GetInvoiceById(Guid id)
        {
            return await _invoiceRepository.GetByIdAsync(id);
        }

        public async Task<InvoiceItem> GetInvoiceItemIncludeItemById(Guid invoiceItemId)
        {
            return await _invoiceRepository.GetInvoiceItemIncludeItemById(invoiceItemId);
        }

        public async Task<BusinessResult> UpdateInvoiceItem(InvoiceItem invoiceItem)
        {
            _invoiceRepository.UpdateItem(invoiceItem);
            var rows = await _invoiceRepository.SaveChangesAsync();
            if (rows>0)
            {
                return new BusinessResult(true);
            }
            return new BusinessResult(false, "no rows affected.");
        }

        public async Task<BusinessResult> DeleteInvoiceItem(Guid invoiceItemId)
        {
            _invoiceRepository.DeleteItem(invoiceItemId);
            var rows = await _invoiceRepository.SaveChangesAsync();
            if (rows > 0)
            {
                return new BusinessResult(true);
            }
            return new BusinessResult(false, "no rows affected.");
        }
    }
}
