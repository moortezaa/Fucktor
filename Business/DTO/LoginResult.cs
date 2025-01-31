using Microsoft.AspNetCore.Identity;

namespace Business.DTO
{
    public class LoginResult : AuthenticationResult
    {
        public bool RequiresTwoFactor { get; set; } = false;

        public LoginResult(){}

        public LoginResult(SignInResult result)
        {
            Succeeded = result.Succeeded;
            if (result.IsLockedOut)
            {
                Errors.Add("Too many login attempts. you are locked out.");
            }
            else if (result.IsNotAllowed)
            {
                Errors.Add("You are not allowed to login.");
            }
            else if (result.RequiresTwoFactor)
            {
                Errors.Add("You need to authenticate using two-factor-authentication.");
                RequiresTwoFactor = true;
            }
        }
    }
}
