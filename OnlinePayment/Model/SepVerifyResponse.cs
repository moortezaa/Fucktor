using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePayment.Model
{
    internal class SepVerifyResponse
    {
        public SepVerifyInfo TransactionDetail { get; set; }
        public int ResultCode { get; set; }
        public string ResultDescription { get; set; }
        public bool Success { get; set; }
    }
}
