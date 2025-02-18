using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class UserManagerResult : AuthenticationResult
    {
        public bool UserExist { get; set; }
        public Guid ExistingUserId { get; set; }

        public UserManagerResult()
        {
        }

        public UserManagerResult(IdentityResult identityResult) : base(identityResult) { }
    }
}
