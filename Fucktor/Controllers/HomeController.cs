using System.Diagnostics;
using Fucktor.Models;
using Microsoft.AspNetCore.Mvc;
using Fucktor.Attributes;
using Business.Attributes;

namespace Fucktor.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [Permission("ViewHome", true)]
        [Dashboard("home", Order = 1)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
