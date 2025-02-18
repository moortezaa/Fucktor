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

        public async Task<IQueryable<Invoice>> GetInvoiceQuery()
        {
            return _invoiceRepository.InvoiceQuery;
        }
    }
}
