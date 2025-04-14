using Core;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Repository
{
    internal class InvoiceRepository : GeneralRepository<SqlDbContext, Invoice, Guid>, IInvoiceRepository
    {
        public InvoiceRepository(SqlDbContext sqlDbContext) : base(sqlDbContext)
        {
        }

        public IQueryable<Invoice> InvoiceQuery { get => _context.Invoices; }
        public IQueryable<InvoiceItem> InvoiceItemQuery { get => _context.InvoiceItems; }

        public async Task DeleteItem(Guid invoiceItemId)
        {
            var item = await _context.InvoiceItems.SingleOrDefaultAsync(ii => ii.Id == invoiceItemId);
            if (item != null)
            {
                _context.InvoiceItems.Remove(item);
            }
        }

        public async Task<Invoice?> GetByIdIncludeDetailsAsync(Guid id)
        {
            return await _context.Invoices
                .Include(i => i.Seller)
                .Include(i => i.Buyer)
                .Include(i => i.InvoiceItems)
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<InvoiceItem?> GetInvoiceItemIncludeItemById(Guid invoiceItemId)
        {
            return await _context.InvoiceItems.Where(ii => ii.Id == invoiceItemId).SingleOrDefaultAsync();
        }

        public async Task<List<Invoice>> GetUserInvoices(Guid userId)
        {
            return await _context.Invoices.Where(i => i.BuyerId == userId || i.SellerId == userId).ToListAsync();
        }

        public void UpdateItem(InvoiceItem invoiceItem)
        {
            _context.InvoiceItems.Update(invoiceItem);
        }
    }
}