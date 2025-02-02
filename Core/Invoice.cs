using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public DateTime DateTime { get; set; }
        public InvoiceStatus Status { get; set; }
        public InvoiceType Type { get; set; }

        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public Guid MainInvoiceId { get; set; }
        public Invoice? MainInvoice { get; set; }

        public Guid SellerId { get; set; }
        public AppUser? Seller { get; set; }

        public Guid BuyerId { get; set; }
        public AppUser? Buyer { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } = [];
    }
}
