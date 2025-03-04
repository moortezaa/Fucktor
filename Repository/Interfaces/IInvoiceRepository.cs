using Core;

namespace Repository
{
    public interface IInvoiceRepository : IGeneralRepository<Invoice, Guid>
    {
        IQueryable<Invoice> InvoiceQuery { get; }

        Task<Invoice?> GetByIdIncludeDetailsAsync(Guid id);
        Task<List<Invoice>> GetUserInvoices(Guid userId);
    }
}
