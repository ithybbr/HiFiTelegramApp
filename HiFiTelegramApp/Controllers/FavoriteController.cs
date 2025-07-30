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
            var favoriteArtists = this._favoriteService.GetFavorites();
            return PartialView(favoriteArtists);
        }
        [HttpPost("add")]
        public async Task AddToFavoriteArtists(string artist)
        {
            try
            {
                Console.WriteLine($"Adding {artist} to favorites");
                await this._favoriteService.AddToFavoriteArtists(artist);
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
                await this._favoriteService.RemoveFromFavoriteArtists(artist);
                Console.WriteLine(StatusCode(201));
            }
            catch (Exception ex)
            {
                Console.WriteLine(StatusCode(500, ex));
            }
        }
        
        [HttpPost("add/{artist}/{name}")]
        public async Task AddToFavoriteSongs(string artist, string name, int songId)
        {
            try
            {
                await this._favoriteService.AddToFavoriteSongs(artist, name, songId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(StatusCode(500, ex));
            }
        }

        [HttpPost("remove/{artist}/{songId:int}")]
        public async Task RemoveFromFavoriteSongs(string artist, string name, int songId)
        {
            try
            {
                await this._favoriteService.RemoveFromFavoriteSongs(artist, name, songId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(StatusCode(500, ex));
            }
        }
    }
}
