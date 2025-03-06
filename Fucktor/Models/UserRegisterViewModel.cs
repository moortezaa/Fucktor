using Core;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Fucktor.Models
{
    public class UserRegisterViewModel : AppUser
    {
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }

        [Required]
        [PasswordPropertyText]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
