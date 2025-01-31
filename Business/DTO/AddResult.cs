using Microsoft.AspNetCore.Identity;

namespace Business.DTO
{
    public class AddResult : AuthenticationResult
    {
        public AddResult(IdentityResult result) : base(result)
        {
        }
    }
}