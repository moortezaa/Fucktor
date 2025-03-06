using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Fucktor.Utils;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Business;
using Core;
using Business.Attributes;

namespace Fucktor.Controllers
{
    public class BaseController : Controller
    {
        internal readonly AppUserManager _appUserManager;
        public CultureInfo Culture { get; set; }

        public BaseController(IServiceProvider serviceProvider)
        {
            _appUserManager = serviceProvider.GetRequiredService<AppUserManager>();
        }

        public AppUser? CurrentUser { get; set; }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //geting the user culture
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            Culture = requestCulture?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;
            ViewData["Culture"] = Culture;

            var currentUserId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != null)
            {
                CurrentUser = await _appUserManager.GetUserById(Guid.Parse(currentUserId));
                ViewData["CurrentUser"] = CurrentUser;
            }

            //get the controller name and the action name
            var controllerName = context.RouteData.Values["controller"].ToString();
            var actionName = context.RouteData.Values["action"].ToString();

            //get request method
            var requestMethod = context.HttpContext.Request.Method;

            //check if the user is authorized
            //get the permission attribute of the action
            var permissionAttribute = context.ActionDescriptor.EndpointMetadata.OfType<PermissionAttribute>().FirstOrDefault();
            //if permission attribute is not null then check the user permission
            if (permissionAttribute != null)
            {
                //if the user is not authenticated then redirect to login page
                if (CurrentUser == null)
                {
                    context.Result = RedirectToAction(nameof(UserController.SignIn), nameof(UserController).RemoveController(), new { returnUrl = context.HttpContext.Request.Path });
                    return;
                }

                //if the user does not have the required permission then redirect to access denied page
                if (!await _appUserManager.HasPermission(CurrentUser.Id, permissionAttribute.Permission))
                {
                    context.Result = RedirectToAction(nameof(UserController.AccessDenied),nameof(UserController).RemoveController());
                    return;
                }
            }

            //else execute the action
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                //log the exception in a file with the current date and time and the date as the file name
                var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                //ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                using (StreamWriter writer = new StreamWriter(logPath, true))
                {
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + ex.Message);
                }
            }
            return;
        }
    }
}
