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
    public class InvoiceManager(IInvoiceRepository invoiceRepository)
    {
        private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
        public readonly IQueryable<Invoice> InvoiceQuery = invoiceRepository.InvoiceQuery;

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
    }
}
