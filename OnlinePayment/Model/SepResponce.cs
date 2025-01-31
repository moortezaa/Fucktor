using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePayment.Model
{
    internal class SepResponce
    {
        public int Status { get; set; }
        public string Token { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
    }
}
