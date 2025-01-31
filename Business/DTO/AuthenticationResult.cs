using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class AuthenticationResult
    {
        public bool Succeeded { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();

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
