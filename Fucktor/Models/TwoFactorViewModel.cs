namespace Fucktor.Models
{
    public class TwoFactorViewModel
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public bool IsPersistent { get; set; }
        public string RedirectUrl { get; set; }
    }
}
