using Business;
using Business.Attributes;
using Core;
using DSTemplate_UI.Interfaces;
using DSTemplate_UI.ViewModels;
using Fucktor.Attributes;
using Fucktor.Models;
using Fucktor.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Fucktor.Controllers
{
    public class UserController : BaseController, IDSTableController
    {
        private readonly AppUserManager _appUserManager;
        private readonly AppRoleManager _appRoleManager;
        private readonly IDSTableManager _dsTableManager;
        private readonly IStringLocalizer<UserController> _localizer;

        public UserController(IServiceProvider serviceProvider, AppUserManager appUserManager, IDSTableManager dsTableManager, AppRoleManager appRoleManager, IStringLocalizer<UserController> localizer) : base(serviceProvider)
        {
            _appUserManager = appUserManager;
            _dsTableManager = dsTableManager;
            _appRoleManager = appRoleManager;
            _localizer = localizer;
        }

        [Permission("GetUserTables", true)]
        public async Task<JsonResult> DSGetTableData(string tableName, string sortPropertyName, bool? sortDesending, string filters, int page = 1, int rowsPerPage = 10, string routeValues = null)
        {
            if (tableName == "index")
            {
                var users = _appUserManager.AppUserQuery;

                if (!string.IsNullOrEmpty(filters))
                {
                    var filtersObjs = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
                    if (filtersObjs != null && filtersObjs.Any(f => f.PropertyName == nameof(AppUser.RoleNames)))
                    {
                        var roleFilter = filtersObjs.First(f => f.PropertyName == nameof(AppUser.RoleNames));
                        var usersList = await users.ToListAsync();
                        foreach (var user in usersList)
                        {
                            user.RoleNames = (await _appUserManager.GetUserRoles(user.Id)).Select(r => r.Name!).ToList();
                        }

                        users = usersList.Where(u => u.RoleNames.Any(r => r.Contains(roleFilter.FilterTerm, StringComparison.OrdinalIgnoreCase))).AsQueryable();
                    }
                }

                users = await _dsTableManager.DoSFP(users, sortPropertyName, sortDesending, filters, page, rowsPerPage);

                var rows = new List<string>();
                var row = 0;
                foreach (var user in users)
                {
                    if (!user.RoleNames.Any())
                    {
                        user.RoleNames = (await _appUserManager.GetUserRoles(user.Id)).Select(r => r.Name!).ToList();
                    }
                    row++;
                    ViewData["Row"] = row;
                    rows.Add(await _dsTableManager.RenderRow(user, ViewData, customRowView: "User/IndexRow"));
                }
                return await _dsTableManager.Json(rows, tableName);
            }
            else if (tableName == "doctors")
            {
                var users = _appUserManager.AppUserQuery;

                var usersList = await users.ToListAsync();
                foreach (var user in usersList)
                {
                    user.RoleNames = (await _appUserManager.GetUserRoles(user.Id)).Select(r => r.Name!).ToList();
                }

                users = usersList.Where(u => u.RoleNames.Any(r => r == "Doctor")).AsQueryable();

                if (!string.IsNullOrEmpty(filters))
                {
                    var filtersObjs = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
                    if (filtersObjs != null && filtersObjs.Any(f => f.PropertyName == nameof(AppUser.RoleNames)))
                    {
                        var roleFilter = filtersObjs.First(f => f.PropertyName == nameof(AppUser.RoleNames));

                        users = usersList.Where(u => u.RoleNames.Any(r => r.Contains(roleFilter.FilterTerm, StringComparison.OrdinalIgnoreCase))).AsQueryable();
                    }
                }

                users = await _dsTableManager.DoSFP(users, sortPropertyName, sortDesending, filters, page, rowsPerPage);

                var rows = new List<string>();
                var row = 0;
                foreach (var user in users)
                {
                    if (!user.RoleNames.Any())
                    {
                        user.RoleNames = (await _appUserManager.GetUserRoles(user.Id)).Select(r => r.Name!).ToList();
                    }
                    row++;
                    ViewData["Row"] = row;
                    rows.Add(await _dsTableManager.RenderRow(user, ViewData, customRowView: "User/IndexRow"));
                }
                return await _dsTableManager.Json(rows, tableName);
            }
            return Json("invalid table name");
        }

        [Permission("GetUserTables", true)]
        public async Task<JsonResult> DSGetTableDataCount(string tableName, string filters, string routeValues = null)
        {
            if (tableName == "index")
            {
                var users = _appUserManager.AppUserQuery;
                if (!string.IsNullOrEmpty(filters))
                {
                    var filtersObjs = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
                    if (filtersObjs != null && filtersObjs.Any(f => f.PropertyName == nameof(AppUser.RoleNames)))
                    {
                        var roleFilter = filtersObjs.First(f => f.PropertyName == nameof(AppUser.RoleNames));
                        var usersList = await users.ToListAsync();
                        foreach (var user in usersList)
                        {
                            user.RoleNames = (await _appUserManager.GetUserRoles(user.Id)).Select(r => r.Name!).ToList();
                        }

                        users = usersList.Where(u => u.RoleNames.Any(r => r.Contains(roleFilter.FilterTerm, StringComparison.OrdinalIgnoreCase))).AsQueryable();
                    }
                }
                var count = await _dsTableManager.CountData(users, filters);
                return await _dsTableManager.Json(count, tableName);
            }
            else if (tableName == "doctors")
            {
                var users = _appUserManager.AppUserQuery;

                var usersList = await users.ToListAsync();
                foreach (var user in usersList)
                {
                    user.RoleNames = (await _appUserManager.GetUserRoles(user.Id)).Select(r => r.Name!).ToList();
                }

                users = usersList.Where(u => u.RoleNames.Any(r => r == "Doctor")).AsQueryable();

                if (!string.IsNullOrEmpty(filters))
                {
                    var filtersObjs = JsonConvert.DeserializeObject<List<DSFilterViewModel>>(filters);
                    if (filtersObjs != null && filtersObjs.Any(f => f.PropertyName == nameof(AppUser.RoleNames)))
                    {
                        var roleFilter = filtersObjs.First(f => f.PropertyName == nameof(AppUser.RoleNames));

                        users = usersList.Where(u => u.RoleNames.Any(r => r.Contains(roleFilter.FilterTerm, StringComparison.OrdinalIgnoreCase))).AsQueryable();
                    }
                }
                var count = await _dsTableManager.CountData(users, filters);
                return await _dsTableManager.Json(count, tableName);
            }
            return await _dsTableManager.Json(0, tableName);
        }

        [HttpGet]
        [Dashboard("users", Order = 0)]
        [Permission("ViewUsers", true)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Permission("ViewDoctorsList", true)]
        [Dashboard("user-doctor", Order = 2)]
        public async Task<IActionResult> DoctorsList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SignIn(string returnUrl)
        {
            var model = new SignInViewModel()
            {
                RedirectUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _appUserManager.Login(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                if (model.RedirectUrl != null)
                {
                    return Redirect(model.RedirectUrl);
                }
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).RemoveController());
            }
            else if (result.RequiresTwoFactor)
            {
                var user = await _appUserManager.GetUserByUserName(model.UserName);
                try
                {
                    await _appUserManager.SendTwoFactorCodeUsingSMSProvider(user);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("UserName", _localizer[e.Message].Value);
                    return View(model);
                }
                return RedirectToAction(nameof(TwoFactorLoginForm), nameof(UserController).RemoveController(), new TwoFactorViewModel
                {
                    UserId = user.Id,
                    Code = "",
                    IsPersistent = model.RememberMe,
                    RedirectUrl = model.RedirectUrl
                });
            }
            ModelState.AddModelError("UserName", string.Join("\n", result.Errors));
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorLoginForm(TwoFactorViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorLogin(TwoFactorViewModel model)
        {
            var result = await _appUserManager.TwoFactorLogin(model.UserId, model.Code, model.IsPersistent);
            if (result.Succeeded)
            {
                if (model.RedirectUrl != null)
                {
                    return Redirect(model.RedirectUrl);
                }
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).RemoveController());
            }
            ModelState.AddModelError("Code", _localizer["Invalid Code"].Value);
            return View(nameof(TwoFactorLoginForm), model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _appUserManager.Logout();
            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        [Permission("EditUser", true)]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!await _appUsermanager.IsInRole(CurrentUser.Id, "Admin") && id != CurrentUser.Id)
            {
                return RedirectToAction(nameof(AccessDenied));
            }
            var user = await _appUserManager.GetUserById(id);
            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        [Permission("EditUser", true)]
        public async Task<IActionResult> Edit(AppUser model)
        {
            if (!await _appUsermanager.IsInRole(CurrentUser.Id, "Admin") && model.Id != CurrentUser.Id)
            {
                return AccessDenied();
            }
            var user = await _appUserManager.GetUserById(model.Id);
            if (user != null)
            {
                var updateResult = await _appUserManager.UpdateUserInfo(model.UserName, model.Name, model.LastName, model.NationalCode,
                    model.Gender, model.PhoneNumber, model.Email, model.DisplayName);
                if (updateResult.Succeeded)
                {
                    return RedirectToAction(nameof(Detail), new { model.Id });
                }
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(user);
            }
            return RedirectToAction(nameof(Detail), new { model.Id });
        }

        [HttpGet]
        [Permission("ViewUserDetail", true)]
        public async Task<IActionResult> Detail(Guid id)
        {
            var user = await _appUserManager.GetUserByIdIncludeDetails(id);
            if (user != null)
            {
                var roles = await _appUserManager.GetUserRoles(user.Id);
                ViewData["Roles"] = roles;
                var availableRoles = await _appRoleManager.GetRoles();
                availableRoles.RemoveAll(r => roles.Any(ur => ur.Id == r.Id));
                ViewData["AvailableRoles"] = availableRoles;
                return View(user);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Permission("AddUserRole", true)]
        public async Task<IActionResult> AddUserToRole(Guid userId, Guid roleId)
        {
            var user = await _appUserManager.GetUserById(userId);
            if (user != null)
            {
                var role = await _appRoleManager.GetRoleById(roleId);
                if (role != null)
                {
                    var result = await _appUserManager.AddUserToRole(userId, role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Detail), new { id = userId });
                    }
                }
            }
            return RedirectToAction(nameof(Detail), new { id = userId });
        }

        [HttpPost]
        [Permission("RemoveUserRole", true)]
        public async Task<IActionResult> RemoveUserFromRole(Guid userId, Guid roleId)
        {
            var user = await _appUserManager.GetUserById(userId);
            if (user != null)
            {
                var role = await _appRoleManager.GetRoleById(roleId);
                if (role != null)
                {
                    var result = await _appUserManager.RemoveUserFromRole(userId, role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Detail), new { id = userId });
                    }
                }
            }
            return RedirectToAction(nameof(Detail), new { id = userId });
        }

        [HttpPost]
        [Permission("VerifyPhoneNumber", true)]
        public async Task<IActionResult> SendPhoneNumberVerificationCode(Guid userId, string phoneNumber)
        {
            var user = await _appUserManager.GetUserById(userId);
            try
            {
                await _appUserManager.SendPhoneNumberVerificationCodeUsingSMSProvider(user);
            }
            catch (Exception e)
            {
                Json(new { success = false, message = _localizer[e.Message].Value });
            }
            return Json(new { success = true });
        }

        [HttpGet]
        [Permission("VerifyPhoneNumber", true)]
        public async Task<IActionResult> VerifyPhoneNumber(Guid id)
        {
            var user = await _appUserManager.GetUserById(id);
            if (user != null)
            {
                //TODO: send the code to the user
                try
                {
                    await _appUserManager.SendPhoneNumberVerificationCodeUsingSMSProvider(user);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("code", _localizer[e.Message].Value);
                    return View(user);
                }
                var code = await _appUserManager.GetPhoneNumberVerificationCode(user.Id, user.PhoneNumber);
                ViewData["Code"] = code;
                return View(user);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Permission("VerifyPhoneNumber", true)]
        public async Task<IActionResult> VerifyPhoneNumber(Guid userId, string phoneNumber, string code)
        {
            var result = await _appUserManager.VerifyPhoneNumber(userId, phoneNumber, code);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = string.Join("\n", result.Errors) });
        }

        [HttpGet]
        [Permission("ChangePassword", true)]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            var user = await _appUserManager.GetUserById(id);
            if (await _appUsermanager.IsInRole(CurrentUser.Id, "Admin") || id == CurrentUser.Id)
            {
                if (user != null)
                {
                    return View(new ChangePasswordViewModel()
                    {
                        Id = user.Id,
                    });
                }
            }
            return AccessDenied();
        }

        [HttpPost]
        [Permission("ChangePassword", true)]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _appUserManager.GetUserById(model.Id);
            if (user != null)
            {
                if (await _appUsermanager.IsInRole(CurrentUser.Id, "Admin") || model.Id == CurrentUser.Id)
                {
                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        ModelState.AddModelError(nameof(model.NewPassword), _localizer["NewPassword doesn't match Confirm passwword"].Value);
                        return View(model);
                    }
                    var result = await _appUserManager.ChangePassword(user.Id, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Detail), new { id = model.Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Join("\n", result.Errors));
                        return View(model);
                    }
                }
            }
            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [Permission("GetUser", true)]
        public async Task<IActionResult> GetUser(string phoneNumber)
        {
            var user = await _appUserManager.GetUserByUserName(phoneNumber);
            if (user != null)
            {
                return Json(new { success = true, user = new { user.Id, user.Name, user.LastName, user.NationalCode } });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        [Permission("EnableTwoFactor", true)]
        public async Task<JsonResult> EnableTwoFactorAuthentication(Guid userId)
        {
            var user = await _appUserManager.GetUserById(userId);
            if (user != null && !user.PhoneNumberConfirmed)
            {
                return Json(new
                {
                    success = false,
                    message = _localizer["you should confirm your phone number first."].Value
                });
            }
            var result = await _appUserManager.EnableTwoFactor(userId);
            return Json(new
            {
                success = result.Succeeded,
                message = _localizer["two factor login enabled successfuly"].Value
            });
        }

        [HttpPost]
        [Permission("EnableTwoFactor", true)]
        public async Task<JsonResult> DisableTwoFactorAuthentication(Guid userId)
        {
            var user = await _appUserManager.GetUserById(userId);
            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = _localizer["user not found."].Value
                });
            }
            var result = await _appUserManager.DisableTwoFactor(userId);
            return Json(new
            {
                success = result.Succeeded,
                message = _localizer["two factor login disabled successfuly"].Value
            });
        }

        [HttpPost]
        public async Task<JsonResult> NeedPassword(SignInViewModel model)
        {
            var user = await _appUserManager.GetUserByUserName(model.UserName);
            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = _localizer["UserName is incorrect"].Value,
                });
            }

            if (await _appUserManager.IsInRole(user.Id, "Doctor") || await _appUserManager.IsInRole(user.Id, "Admin"))
            {
                return Json(new
                {
                    success = true,
                    needPassword = true
                });
            }
            return Json(new
            {
                success = true,
                needPassword = false
            });
        }
    }
}
