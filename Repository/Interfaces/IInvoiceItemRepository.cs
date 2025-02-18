﻿using Core;

namespace Repository
{
    public interface IInvoiceItemRepository : IGeneralRepository<InvoiceItem, Guid>
    {
        IQueryable<InvoiceItem> InvoiceItemQuery { get; }

    }
}
