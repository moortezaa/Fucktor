using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePayment.Model
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string Payload { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? Description { get; set; }
        public string? RefNumber { get; set; }
    }
}
