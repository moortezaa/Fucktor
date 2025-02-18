using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class AuthenticationResult : BusinessResult
    {
        public AuthenticationResult()
        {
        }

        public AuthenticationResult(IdentityResult result)
        {
            Succeeded = result.Succeeded;
            Errors = result.Errors.Select(e => e.Description).ToList();
        }
    }
}
