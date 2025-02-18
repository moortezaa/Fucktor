using Business.DTO;
using Core;
using Repository;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace Business
{
    public class AppRoleManager
    {
        private readonly IAppRoleRepository _roleRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly RoleManager<AppRole> _roleManager;

        public AppRoleManager(IAppRoleRepository roleRepository, RoleManager<AppRole> roleManager, IPermissionRepository permissionRepository, IAppUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _roleManager = roleManager;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
        }

        public async Task<AuthenticationResult> AddRole(string roleName)
        {
            // Validation
            List<string> validationErrors = ValidateRoleName(roleName);

            if (validationErrors.Any())
            {
                return new AuthenticationResult()
                {
                    Succeeded = false,
                    Errors = validationErrors
                };
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new AppRole() { Name = roleName });
                return new AuthenticationResult(result);
            }

            return new AuthenticationResult()
            {
                Succeeded = false,
                Errors = new List<string>() { "Role already exists" }
            };
        }

        public async Task<List<AppRole>> GetRoles()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<AuthenticationResult> RenameRole(string roleName, string newName)
        {
            var role = await _roleManager.FindByNameAsync(roleName) ?? throw new Exception("Role not found");

            var errors = ValidateRoleName(newName);
            if (errors.Any())
            {
                return new AuthenticationResult()
                {
                    Succeeded = false,
                    Errors = errors
                };
            }

            var result = await _roleManager.SetRoleNameAsync(role, newName);
            if (result.Succeeded)
            {
                var updateResult = await _roleManager.UpdateAsync(role);
                return new AuthenticationResult(updateResult);
            }

            return new AuthenticationResult(result);
        }

        private static List<string> ValidateRoleName(string roleName)
        {
            var validationErrors = new List<string>();
            if (string.IsNullOrEmpty(roleName))
            {
                validationErrors.Add("Role name cannot be empty");
            }

            if (roleName.Length <= 3)
            {
                validationErrors.Add("Role name must be more than 3 characters long");
            }

            if (roleName.Length >= 21)
            {
                validationErrors.Add("Role name must be less than 21 characters long");
            }

            if (Regex.IsMatch(roleName, @"\d"))
            {
                validationErrors.Add("Role name must not contain numbers");
            }

            if (Regex.IsMatch(roleName, @"[^a-zA-Z0-9]"))
            {
                validationErrors.Add("Role name must not contain symbols");
            }

            return validationErrors;
        }

        public async Task<AuthenticationResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName) ?? throw new Exception("Role not found");

            // Check if role is assigned to any user
            if (await _roleRepository.IsAnyUserInRole(role.Id))
            {
                return new AuthenticationResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { "Role is assigned to user" }
                };
            }

            var result = await _roleManager.DeleteAsync(role);
            return new AuthenticationResult(result);
        }

        public async Task<AuthenticationResult> AddPermissionToRole(string roleName, string permissionName)
        {
            var role = await _roleRepository.GetRoleByNameIncludePermissionsAsync(roleName) ?? throw new KeyNotFoundException("Role not found");

            var permission = await _permissionRepository.GetPermissionByTitle(permissionName) ?? throw new InvalidOperationException("Permission not found");

            if (role.Permissions.Any(rp => rp.PermissionId == permission.Id))
            {
                return new AuthenticationResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { "Permission already assigned to role" }
                };
            }

            _roleRepository.AddRolePermission(new RolePermission()
            {
                PermissionId = permission.Id,
                RoleId = role.Id
            });
            var rows = await _roleRepository.SaveChangesAsync();
            if (rows == 1)
            {
                return new AuthenticationResult()
                {
                    Succeeded = true
                };
            }
            return new AuthenticationResult()
            {
                Succeeded = false,
                Errors = new List<string>() { "Error adding permission to role" }
            };
        }

        public async Task<List<Permission>> GetPermissionsForRole(string roleName)
        {
            var role = await _roleRepository.GetRoleByNameIncludePermissionsAsync(roleName) ?? throw new KeyNotFoundException("Role not found");

            return role.Permissions.Select(rp => rp.Permission!).ToList();
        }

        public async Task<AuthenticationResult> RemovePermissionFromRole(string roleName, string permission)
        {
            var role = await _roleRepository.GetRoleByNameIncludePermissionsAsync(roleName) ?? throw new KeyNotFoundException("Role not found");

            var rolePermission = role.Permissions.FirstOrDefault(rp => rp.Permission!.Title == permission);
            if (rolePermission == null)
            {
                throw new InvalidOperationException("Permission not found");
            }

            _roleRepository.RemoveRolePermission(rolePermission);
            var rows = await _roleRepository.SaveChangesAsync();
            if (rows != 1)
            {
                return new AuthenticationResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { "Error removing permission from role" }
                };
            }
            return new AuthenticationResult()
            {
                Succeeded = true
            };
        }

        public async Task<AppRole?> GetRoleByName(string roleName)
        {
            return await _roleRepository.GetRoleByNameAsync(roleName);
        }

        public async Task<AppRole> GetRoleById(Guid roleId)
        {
            return await _roleRepository.GetByIdAsync(roleId) ?? throw new KeyNotFoundException("Role not found");
        }
    }
}
