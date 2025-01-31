using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class UserManagerResult
    {
        public bool Succeeded { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();
        public bool UserExist { get; set; }
        public Guid ExistingUserId { get; set; }

        public UserManagerResult()
        {
        }

        public UserManagerResult(IdentityResult result)
        {
            Succeeded = result.Succeeded;
            Errors = result.Errors.Select(e => e.Description).ToList();
        }
    }
}
