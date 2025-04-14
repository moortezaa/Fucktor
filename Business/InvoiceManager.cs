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
    public class InvoiceManager(IInvoiceRepository invoiceRepository, IInvoiceItemRepository invoiceItemRepository)
    {
        private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
        private readonly IInvoiceItemRepository _invoiceItemRepository = invoiceItemRepository;
        public readonly IQueryable<Invoice> InvoiceQuery = invoiceRepository.InvoiceQuery;
        public readonly IQueryable<InvoiceItem> InvoiceItemQuery = invoiceItemRepository.InvoiceItemQuery;

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

        public async Task<Invoice?> GetInvoiceById(Guid id)
        {
            return await _invoiceRepository.GetByIdAsync(id);
        }

        public async Task<InvoiceItem?> GetInvoiceItemIncludeItemById(Guid id)
        {
            return await _invoiceItemRepository.GetByIdIncludeItemAsync(id);
        }

        public async Task<BusinessResult> UpdateInvoiceItem(InvoiceItem invoiceItem)
        {
            _invoiceItemRepository.Update(invoiceItem);
            var rows = await _invoiceItemRepository.SaveChangesAsync();
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
                Errors = ["no rows affected."]
            };
        }
    }
}
