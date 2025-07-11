using System.Diagnostics;
using HiFiTelegramApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace HiFiTelegramApp.Controllers
{
    [Route("[controller]")]
    public class DownloadController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public DownloadController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
