using Core;

namespace Repository
{
    public interface IInvoiceRepository : IGeneralRepository<Invoice, Guid>
    {
        IQueryable<Invoice> InvoiceQuery { get; }

        Task<List<Invoice>> GetUserInvoices(Guid userId);
    }
}
