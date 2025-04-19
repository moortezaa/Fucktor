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

        public new Invoice Remove(Invoice invoice)
        {
            var items = _context.InvoiceItems.Where(ii=>ii.InvoiceId == invoice.Id);
            _context.InvoiceItems.RemoveRange(items);
            return _context.Invoices.Remove(invoice).Entity;
        }

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
                    .ThenInclude(ii => ii.Item)
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<InvoiceItem?> GetInvoiceItemIncludeItemById(Guid invoiceItemId)
        {
            return await _context.InvoiceItems.Include(ii => ii.Item).Where(ii => ii.Id == invoiceItemId).SingleOrDefaultAsync();
        }

        public async Task<long?> GetLastUserInvoiceNumber(Guid userId)
        {
            return await _context.Invoices.Where(i=>i.SellerId == userId).OrderBy(i=>i.Number).Select(i=>i.Number).LastOrDefaultAsync();
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