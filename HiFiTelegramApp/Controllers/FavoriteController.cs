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
        public async Task AddToFavoriteArtists(string artist)
        {
            try
            {
                Console.WriteLine($"Adding {artist} to favorites");
                await this._favoriteService.AddToFavoriteMarkArtists(artist);
            }
            catch (Exception ex)
            {
                Console.WriteLine(StatusCode(500, ex));
            }
        }
        [HttpPost("remove")]
        public async Task RemoveFromFavoriteArtists(string artist)
        {
            try
            {
                await this._favoriteService.RemoveFromFavoriteMarkArtists(artist);
                Console.WriteLine(StatusCode(201));
            }
            catch (Exception ex)
            {
                Console.WriteLine(StatusCode(500, ex));
            }
        }
    }
}
