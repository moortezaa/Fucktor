using Business.DTO;
using Core;
using Repository;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace Business
{
    public class AppUserManager
    {
        private IAppUserRepository _appUserRepository;
        private IAppRoleRepository _appRoleRepository;
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        public IQueryable<AppUser> AppUserQuery;
        private IStringLocalizer<AppUserManager> _localizer;

        public AppUserManager(IAppUserRepository appUserRepository, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAppRoleRepository appRoleRepository, IStringLocalizer<AppUserManager> localizer)
        {
            _appUserRepository = appUserRepository;
            AppUserQuery = _appUserRepository.AppUserQuery;
            _userManager = userManager;
            _userManager.RegisterTokenProvider("Phone", new PhoneNumberTokenProvider<AppUser>());
            _signInManager = signInManager;
            _appRoleRepository = appRoleRepository;
            _localizer = localizer;
        }

        public async Task<AuthenticationResult> AddUser(string userName, string password)
        {
            var errors = new List<string>();

            //validating Username
            if (userName.Length < 3)
            {
                errors.Add(_localizer["Username must be more than 2 characters."].Value);
            }
            else if (userName.Length > 20)
            {
                errors.Add(_localizer["Username must be less than 21 characters."].Value);
            }

            var user = new AppUser()
            {
                UserName = userName,
            };
            var result = await _userManager.CreateAsync(user, password);


            if (errors.Count > 0)
            {
                if (result.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                }
                var errorResult = new AuthenticationResult(result)
                {
                    Succeeded = false
                };
                errorResult.Errors.AddRange(errors);
                return errorResult;
            }

            return new AuthenticationResult(result);
        }

        private string? ValidateEmail(string email)
        {
            if (email == null)
                return _localizer["Email Is required"].Value;

            if (!email.Contains('@'))
            {
                return _localizer["Email must have an '@'"].Value;
            }

            if (!email.Contains('.'))
            {
                return _localizer["Email must have a period '.'"].Value;
            }

            var validDomains = new string[] { "gmail.com", "yahoo.com", "outlook.com" };
            var domain = email.Split('@')[1];
            if (!validDomains.Contains(domain))
            {
                return _localizer["Email must be from google, yahoo or outlook"].Value;
            }


            var validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.-_";

            foreach (var c in email.Split('@')[0])
            {
                if (!validCharacters.Contains(c))
                {
                    return _localizer["Email may not contain '{0}'", c].Value;
                }
            }

            return null;
        }

        public async Task<AuthenticationResult> ChangeUserName(Guid id, string newUserName)
        {
            var user = await _appUserRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User Not Found");

            if ((await _appUserRepository.GetByUserNameAsync(newUserName)) != null)
            {
                return new AuthenticationResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { _localizer["Username '{0}' is already taken.", newUserName].Value }
                };
            }
            if (newUserName.Length < 3)
            {
                return new AuthenticationResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { _localizer["Username must be more than 2 characters."].Value }
                };
            }
            if (newUserName.Length > 20)
            {
                return new AuthenticationResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { _localizer["Username must be less than 21 characters."].Value }
                };
            }

            user.UserName = newUserName;
            _appUserRepository.Update(user);

            var rowsAffected = await _appUserRepository.SaveChangesAsync();
            if (rowsAffected > 0)
            {
                return new AuthenticationResult()
                {
                    Succeeded = true,
                };
            }
            return new AuthenticationResult()
            {
                Succeeded = false,
                Errors = new List<string>() { _localizer["No rows affected"].Value }
            };
        }

        public async Task<AppUser?> GetUserByUserName(string userName)
        {
            return await _appUserRepository.GetByUserNameAsync(userName);
        }

        public async Task<List<AppUser>> GetUsers()
        {
            return await _appUserRepository.GetAllAsync();
        }

        public async Task<LoginResult> Login(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            return new LoginResult(result);
        }

        public async Task<LoginResult> TwoFactorLogin(Guid userId, string code, bool isPersistent)
        {
            var user = await _appUserRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User Not Found");
            if (await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", code))
            {
                await _signInManager.SignInAsync(user, isPersistent, "TwoFactorLogin");
                return new LoginResult(SignInResult.Success);
            }
            return new LoginResult(SignInResult.Failed);
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AuthenticationResult> ChangePassword(Guid id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("User Not Found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return new AuthenticationResult(result);
        }

        private List<string> ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return new List<string>() { _localizer["Phone number is required"].Value };
            }
            if (phoneNumber.Length < 12)
            {
                return new List<string>() { _localizer["Phone number must be longer than 11 digits"].Value };
            }

            string countryCode = phoneNumber.Substring(0, phoneNumber.Length - 10);

            if (!ValidationConstantLists.validPhoneNumberCountryCodes.Any(v => "+" + v.Item2 == countryCode))
            {
                return new List<string>() { _localizer["Invalid Country Code '{}'.", countryCode].Value };
            }

            var validCharacters = "+0123456789";
            foreach (var c in phoneNumber)
            {
                if (!validCharacters.Contains(c))
                {
                    return new List<string>() { _localizer["Invalid Character '{0}'.", c].Value };
                }
            }

            return new List<string>();
        }

        public async Task<AuthenticationResult> EnableTwoFactor(Guid id)
        {
            var user = await _appUserRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User Not Found");

            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);

            return new AuthenticationResult(result);
        }

        public async Task<AuthenticationResult> DisableTwoFactor(Guid id)
        {
            var user = await _appUserRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User Not Found");

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            return new AuthenticationResult(result);
        }

        //just in case
        private static bool IsValidIranianNationalCode(string input)
        {
            if (!Regex.IsMatch(input, @"^\d{10}$"))
                return false;
            var check = Convert.ToInt32(input.Substring(9, 1));
            var sum = Enumerable.Range(0, 9)
                .Select(x => Convert.ToInt32(input.Substring(x, 1)) * (10 - x))
                .Sum() % 11;
            return sum < 2 ? check == sum : check + sum == 11;
        }

        public async Task<UserManagerResult> DeleteUser(Guid id)
        {
            var user = await _appUserRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User Not Found");

            _appUserRepository.Remove(user);
            var rows = await _appUserRepository.SaveChangesAsync();
            if (rows == 1)
            {
                return new UserManagerResult()
                {
                    Succeeded = true
                };
            }
            return new UserManagerResult()
            {
                Succeeded = false,
                Errors = new List<string>() { _localizer["No rows affected"].Value }
            };
        }

        public async Task<UserManagerResult> AddUserToRole(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new KeyNotFoundException("User Not Found");
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return new UserManagerResult(result);
        }

        public async Task<List<AppRole>> GetUserRoles(Guid id)
        {
            var user = await _appUserRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User Not Found");
            return await _appUserRepository.GetUserRoles(id);
        }

        public async Task<UserManagerResult> RemoveUserFromRole(Guid id, string roleName)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException("User Not Found");
            var role = await _userManager.GetRolesAsync(user);
            if (!role.Contains(roleName))
            {
                return new UserManagerResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { _localizer["User is not in this role."].Value }
                };
            }
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return new UserManagerResult(result);
        }

        public async Task<bool> HasPermission(Guid id, string permissionName)
        {
            var user = await _appUserRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User Not Found");

            var permissions = await _appUserRepository.GetUserPermissions(id);
            return permissions.Any(p => p.Title == permissionName);
        }

        public async Task<AppUser?> GetUserById(Guid id)
        {
            return await _appUserRepository.GetByIdAsync(id);
        }

        public async Task<UserManagerResult> AddUserByPhoneNumber(string phoneNumber)
        {
            var existingUser = await _appUserRepository.GetByUserNameAsync(phoneNumber);
            if (existingUser != null)
            {
                return new UserManagerResult()
                {
                    Succeeded = false,
                    Errors = new List<string>() { _localizer["User already exists"].Value },
                    UserExist = true,
                    ExistingUserId = existingUser.Id
                };
            }
            var result = await _userManager.CreateAsync(new AppUser()
            {
                PhoneNumber = phoneNumber,
                UserName = phoneNumber,
                TwoFactorEnabled = true,
            }, phoneNumber);
            return new UserManagerResult(result);
        }

        public async Task<UserManagerResult> UpdateUserInfo(string userName, string? name = null, string? lastName = null, string? nationalCode = null,
            Gender gender = Gender.NotSet, string? phoneNumber = null, string? email = null, string? displayName = null)
        {
            var user = await GetUserByUserName(userName) ?? throw new KeyNotFoundException("User Not Found");


            if (phoneNumber != null)
            {
                //if user is not admin then set the user name same as the phone number
                if (user.UserName != "admin")
                {
                    var changeUserNameResult = await _userManager.SetUserNameAsync(user, phoneNumber);
                    if (!changeUserNameResult.Succeeded)
                    {
                        return new UserManagerResult
                        {
                            Succeeded = false,
                            Errors = changeUserNameResult.Errors.Select(e => e.Description).ToList()
                        };
                    }
                }
                user.PhoneNumber = phoneNumber;
            }
            if (name != null)
            {
                user.Name = name;
            }
            if (lastName != null)
            {
                user.LastName = lastName;
            }
            if (nationalCode != null)
            {
                user.NationalCode = nationalCode;
            }
            if (gender != Gender.NotSet)
            {
                user.Gender = gender;
            }
            if (email != null)
            {
                user.Email = email;
            }
            if (displayName != null)
            {
                user.DisplayName = displayName;
            }

            _appUserRepository.Update(user);
            var rows = await _appUserRepository.SaveChangesAsync();
            if (rows == 1)
            {
                return new UserManagerResult()
                {
                    Succeeded = true
                };
            }
            return new UserManagerResult()
            {
                Succeeded = false,
                Errors = new List<string>() { _localizer["No rows affected"].Value }
            };
        }

        public async Task<string> GetPhoneNumberVerificationCode(Guid userId, string phoneNumber)
        {
            var user = await GetUserById(userId) ?? throw new KeyNotFoundException("User Not Found");
            return await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        public async Task<UserManagerResult> VerifyPhoneNumber(Guid userId, string phoneNumber, string code)
        {
            var user = await GetUserById(userId) ?? throw new KeyNotFoundException("User Not Found");
            var result = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, code);
            return new UserManagerResult(result);
        }

        public async Task<AppUser> GetUserByIdIncludeDetails(Guid id)
        {
            return await _appUserRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User Not Found");
        }

        public async Task<bool> IsInRole(Guid userId, string RoleName)
        {
            var user = await GetUserById(userId) ?? throw new KeyNotFoundException("User Not Found");
            return await _userManager.IsInRoleAsync(user, RoleName);
        }

        public async Task<string> GetTwoFactorCode(AppUser user)
        {
            return await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
        }

        public async Task ForceLogin(string phoneNumber)
        {
            var user = await _appUserRepository.GetByUserNameAsync(phoneNumber) ?? throw new KeyNotFoundException("User Not Found");
            await _signInManager.SignInAsync(user, false);
        }

        public async Task SendTwoFactorCodeUsingSMSProvider(AppUser user)
        {
            if (user.PhoneNumber == null)
            {
                throw new Exception("شماره موبایل کاربر ثبت نشده");
            }
            var code = await GetTwoFactorCode(user);
            //TODO: change SMS message
            //TODO: move the send part to a sms manager where we can control the account witch sends the sms
            //new FaraPayamakAPI("nobatexir", "erx@123").SendSMSAsync("1", code, user.PhoneNumber);
        }

        public async Task SendPhoneNumberVerificationCodeUsingSMSProvider(AppUser user)
        {
            if (user.PhoneNumber == null)
            {
                throw new Exception("شماره موبایل کاربر ثبت نشده");
            }
            var code = await GetPhoneNumberVerificationCode(user.Id, user.PhoneNumber);
            //TODO: change SMS message
            //TODO: move the send part to a sms manager where we can control the account witch sends the sms
            //new FaraPayamakAPI("nobatexir", "erx@123").SendSMSAsync("1", code, user.PhoneNumber);
        }

        public async Task<AppUser?> GetUserByPhoneNumberAndNationalCode(string doctorPhone, string doctorNationalCode)
        {
            return await _appUserRepository.GetUserByPhoneNumberAndNationalCode(doctorPhone, doctorNationalCode);
        }
    }
}
