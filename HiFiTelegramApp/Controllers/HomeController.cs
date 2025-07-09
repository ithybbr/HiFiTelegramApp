using System.Diagnostics;
using HiFiTelegramApp.Models;
using HiFiTelegramApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HiFiTelegramApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ArtistsService _artistsService;

        public HomeController(ILogger<HomeController> logger,  ArtistsService artistsService)
        {
            this._artistsService = artistsService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(this._artistsService.Artists);
        }
        [HttpGet("{artist}")]
        public IActionResult Artist(string artist)
        {
            return View(this._artistsService.GetSongs(artist));
        }
    }
}
