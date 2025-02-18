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
    public class InvoiceManager(IInvoiceRepository invoiceRepository, IStringLocalizer<InvoiceManager> localizer)
    {
        private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
        private readonly IStringLocalizer<InvoiceManager> localizer;

        public async Task<IQueryable<Invoice>> GetInvoiceQuery()
        {
            return _invoiceRepository.InvoiceQuery;
        }

        public async Task<BusinessResult> CreateOrUpdateInvoice(Invoice invoice)
        {
            invoice = _invoiceRepository.Update(invoice);
            var rows = await _invoiceRepository.SaveChangesAsync();
            if (rows > 0)
            {
                return new BusinessResult()
                {
                    Succeeded = true
                };
            }
            return new BusinessResult()
            {
                Succeeded = false,
                Errors = { localizer["Failed to update invoice."].Value }
            };
        }

        public async Task<List<Invoice>> GetUserInvoices(Guid userId)
        {
            return await _invoiceRepository.GetUserInvoices(userId);
        }

        public async Task<BusinessResult> RemoveInvoice(Guid invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return new BusinessResult()
                {
                    Succeeded = false,
                    Errors = { localizer["Invoice not found."].Value }
                };
            }
            _invoiceRepository.Remove(invoice);
            var rows = await _invoiceRepository.SaveChangesAsync();
            if (rows > 0)
            {
                return new BusinessResult()
                {
                    Succeeded = true
                };
            }
            return new BusinessResult()
            {
                Succeeded = false,
                Errors = { localizer["Failed to remove invoice."].Value }
            };
        }

        public async Task<Invoice?> GetInvoiceWithDetails(Guid id)
        {
            return await _invoiceRepository.GetByIdIncludeDetailsAsync(id);
        }

        public async Task<Invoice?> GetInvoiceById(Guid id)
        {
            return await _invoiceRepository.GetByIdAsync(id);
        }
    }
}
