using System.Diagnostics;
using Fucktor.Models;
using Microsoft.AspNetCore.Mvc;
using Fucktor.Attributes;

namespace Fucktor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
