using System.Diagnostics;
using HiFiTelegramApp.Models;
using HiFiTelegramApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HiFiTelegramApp.Controllers
{
    [Route("[controller]")]
    public class FavoriteController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FavoriteService _favoriteService;

        public FavoriteController(ILogger<HomeController> logger, FavoriteService favoriteService)
        {
            _logger = logger;
            _favoriteService = favoriteService;
        }

        public IActionResult Index()
        {
            var favoriteArtists = this._favoriteService.GetFavoriteArtists();
            return PartialView(favoriteArtists);
        }
        [HttpPost("add")]
        public IActionResult AddToFavoriteArtists(string artist)
        {
            try
            {
                Console.WriteLine($"Adding {artist} to favorites");
                this._favoriteService.AddToFavoriteMarkArtists(artist);
                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpPost("remove")]
        public IActionResult RemoveFromFavoriteArtists(string artist)
        {
            try
            {
                this._favoriteService.RemoveFromFavoriteMarkArtists(artist);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
