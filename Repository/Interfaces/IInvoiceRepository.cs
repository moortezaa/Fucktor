using Core;

namespace Repository
{
    public interface IInvoiceRepository : IGeneralRepository<Invoice, Guid>
    {
        IQueryable<Invoice> InvoiceQuery { get; }
        IQueryable<InvoiceItem> InvoiceItemQuery { get; }

        Task DeleteItem(Guid invoiceItemId);
        Task<Invoice?> GetByIdIncludeDetailsAsync(Guid id);
        Task<InvoiceItem?> GetInvoiceItemIncludeItemById(Guid invoiceItemId);
        Task<List<Invoice>> GetUserInvoices(Guid userId);
        void UpdateItem(InvoiceItem invoiceItem);
    }
}
