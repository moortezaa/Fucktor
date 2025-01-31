using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ChangePhoneNumberResult : AuthenticationResult
    {
        public string? ConfirmationCode { get; set; }
    }
}
