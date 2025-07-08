using System.Diagnostics;
using HiFiTelegramApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace HiFiTelegramApp.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public FavoritesController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
