using Microsoft.AspNetCore.Identity;

namespace Business.DTO
{
    public class PermissionManagerResult
    {
        public bool Succeeded { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();

        public PermissionManagerResult()
        {
        }

        public PermissionManagerResult(IdentityResult result)
        {
            Succeeded = result.Succeeded;
            Errors = result.Errors.Select(e => e.Description).ToList();
        }
    }
}