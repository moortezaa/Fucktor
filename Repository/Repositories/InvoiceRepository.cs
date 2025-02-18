using Core;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class InvoiceRepository : GeneralRepository<SqlDbContext, Invoice, Guid>, IInvoiceRepository
    {
        public InvoiceRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public IQueryable<Invoice> InvoiceQuery { get => _context.Invoices; }

        public async Task<List<Invoice>> GetUserInvoices(Guid userId)
        {
            return await _context.Invoices.Where(i=>i.BuyerId == userId || i.SellerId == userId).ToListAsync();
        }
    }
}