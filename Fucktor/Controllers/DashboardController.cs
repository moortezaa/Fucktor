using Business.Attributes;
using Fucktor.Attributes;
using Fucktor.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Fucktor.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IStringLocalizer<DashboardController> _localizer;
        public DashboardController(IServiceProvider serviceProvider, IStringLocalizer<DashboardController> localizer) : base(serviceProvider)
        {
            _localizer = localizer;
        }

        [HttpPost]
        [Permission("UseDashboard", true)]
        public async Task<JsonResult> GetDashboard()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            var Controllers = assembly.GetTypes()
                .Where(t => t.IsClass && t.FullName.Contains($"ExirNobat.Controllers"));

            var dashboardItems = new List<DashboardItem>();
            foreach (var controller in Controllers)
            {
                var dashboardAttributes = controller.GetMethods().Where(a => a.CustomAttributes.Any(c => c.AttributeType == typeof(DashboardAttribute)))
                    .Select(a => new { a,a.Name, Attribute = (DashboardAttribute)a.GetCustomAttribute(typeof(DashboardAttribute))! }).ToList();
                for (int i = 0; i < dashboardAttributes.Count; i++)
                {
                    var permissionAttribute = dashboardAttributes[i].a.GetCustomAttribute<PermissionAttribute>();

                    if (permissionAttribute != null)
                    {
                        if (!await _appUsermanager.HasPermission(CurrentUser.Id,permissionAttribute.Permission))
                        {
                            continue;
                        }
                    }

                    var dashboardAttribute = dashboardAttributes[i];
                    var dashboardItem = new DashboardItem()
                    {
                        Display = _localizer[controller.Name.Replace("Controller", "") + "." + dashboardAttribute.Name].Value,
                        Icon = dashboardAttribute.Attribute.Icon,
                        Link = Url.Action(dashboardAttribute.Name, controller.Name.Replace("Controller", "")) ?? "",
                        Order = dashboardAttribute.Attribute.Order
                    };
                    //add to the parents child if has parent
                    var insertIndex = 0;
                    if (dashboardAttribute.Attribute.ParentAction != null)
                    {
                        var parent = dashboardItems.Where(d => d.Display == controller.Name.Replace("Controller", "") + "." + dashboardAttribute.Attribute.ParentAction).SingleOrDefault();
                        if (parent == null)
                        {
                            var parentAttribute = dashboardAttributes.Where(a => a.Name == dashboardAttribute.Attribute.ParentAction).SingleOrDefault()
                                ?? throw new Exception("The Developer has entered an action that doesn't exist");
                            dashboardAttributes.Remove(parentAttribute);
                            parent = new DashboardItem()
                            {
                                Display = _localizer[controller.Name.Replace("Controller", "") + "." + parentAttribute.Name].Value,
                                Icon = parentAttribute.Attribute.Icon,
                                Link = Url.Action(parentAttribute.Name, controller.Name.Replace("Controller", "")) ?? "",
                                Order = parentAttribute.Attribute.Order,
                            };
                            insertIndex = FindIndex(dashboardItems, parent);
                            dashboardItems.Insert(insertIndex, parent);
                        }
                        insertIndex = FindIndex(parent.Childs, dashboardItem);
                        parent.Childs.Insert(insertIndex, dashboardItem);
                    }
                    else
                    {
                        insertIndex = FindIndex(dashboardItems, dashboardItem);
                        dashboardItems.Insert(insertIndex, dashboardItem);
                    }
                }
            }
            return Json(dashboardItems);
        }

        private static int FindIndex(List<DashboardItem> dashboardItems, DashboardItem itemToBeFound)
        {
            int insertIndex = dashboardItems.BinarySearch(itemToBeFound, new DashboardItemComparer());
            if (insertIndex < 0)
            {
                insertIndex = ~insertIndex; // Handles cases where the item doesn't exist exactly
            }

            return insertIndex;
        }

        private class DashboardItemComparer : IComparer<DashboardItem>
        {
            public int Compare(DashboardItem? x, DashboardItem? y)
            {
                if (x == null || y == null) return 0;
                return x.Order.CompareTo(y.Order);
            }
        }

        [HttpGet]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var exception = context.Error;

            // Log the exception here if needed

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = exception.Message
            };

            if (exception is UnauthorizedAccessException)
            {
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized access";
            }

            return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);

            //TODO: Log Error
            //return StatusCode(500);
        }
    }
}
