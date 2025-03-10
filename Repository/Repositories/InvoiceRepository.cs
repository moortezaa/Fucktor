﻿using Core;
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

        public async Task<Invoice?> GetByIdIncludeDetailsAsync(Guid id)
        {
            return await _context.Invoices
                .Include(i=>i.Seller)
                .Include(i=>i.Buyer)
                .Include(i=>i.InvoiceItems)
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<List<Invoice>> GetUserInvoices(Guid userId)
        {
            return await _context.Invoices.Where(i=>i.BuyerId == userId || i.SellerId == userId).ToListAsync();
        }
    }
}