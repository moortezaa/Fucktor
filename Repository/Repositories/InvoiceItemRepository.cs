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

        public async Task<InvoiceItem?> GetByIdIncludeItemAsync(Guid id)
        {
            return await _context.InvoiceItems
                .Include(ii=>ii.Item)
                .Where(ii=>ii.Id == id).SingleOrDefaultAsync();
        }
    }
}