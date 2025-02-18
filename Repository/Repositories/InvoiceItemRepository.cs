using Core;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal class InvoiceItemRepository : GeneralRepository<SqlDbContext, InvoiceItem, Guid>, IInvoiceItemRepository
    {
        public InvoiceItemRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public IQueryable<InvoiceItem> InvoiceItemQuery { get => _context.InvoiceItems; }
    }
}