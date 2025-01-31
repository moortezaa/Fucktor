using Business.DTO;
using Core;
using Repository;
using System.Text.RegularExpressions;

namespace Business
{
    public class PermissionManager
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionManager(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<PermissionManagerResult> AddPermission(string permissionName, string? group = null)
        {
            // Check if permission already exists
            var permission = await _permissionRepository.GetPermissionByTitle(permissionName);
            if (permission != null)
            {
                return new PermissionManagerResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { "Permission already exists" }
                };
            }

            // Validate permission name
            List<string> errors = ValidatePermissionName(permissionName);

            if (errors.Any())
            {
                return new PermissionManagerResult()
                {
                    Succeeded = false,
                    Errors = errors
                };
            }

            _permissionRepository.Add(new Permission()
            {
                Title = permissionName,
                Group = group
            });
            var rows = await _permissionRepository.SaveChangesAsync();
            if (rows != 1)
            {
                return new PermissionManagerResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { "Error adding permission" }
                };
            }
            return new PermissionManagerResult()
            {
                Succeeded = true
            };
        }

        public async Task<List<Permission>> GetPermissions()
        {
            return await _permissionRepository.GetAllAsync();
        }

        private static List<string> ValidatePermissionName(string permissionName)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(permissionName))
            {
                errors.Add("Permission Name cannot be empty.");
            }
            else if (permissionName.Length <= 3)
            {
                errors.Add("Permission name must be more than 3 characters long");
            }
            else if (permissionName.Length >= 41)
            {
                errors.Add("Permission name must be less than 41 characters long");
            }
            else if (Regex.IsMatch(permissionName, @"\d"))
            {
                errors.Add("Permission name must not contain numbers");
            }
            else if (Regex.IsMatch(permissionName, @"\W"))
            {
                errors.Add("Permission name must not contain symbols");
            }

            return errors;
        }

        public async Task<PermissionManagerResult> DeletePermission(string permissionName)
        {
            var permission = await _permissionRepository.GetPermissionByTitle(permissionName) ?? throw new Exception("Permission not found");
            _permissionRepository.Remove(permission);
            var rows = await _permissionRepository.SaveChangesAsync();
            if (rows != 1)
            {
                return new PermissionManagerResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { "Error deleting permission" }
                };
            }
            return new PermissionManagerResult()
            {
                Succeeded = true
            };
        }
    }
}
