using System.Diagnostics;
using HiFiTelegramApp.Models;
using HiFiTelegramApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HiFiTelegramApp.Controllers
{
    [Route("[controller]")]
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
            var artists = this._artistsService.Artists;
            if (artists is null || !artists.Any())
            {
                _logger.LogError("Artists list is empty or null.");
                return View();
            }
            return View(artists);
        }
        [HttpGet("{artist}")]
        public IActionResult Artist(string artist)
        {
            var songs = this._artistsService.GetSongs(artist);
            if (songs is null || songs.Count == 0)
            {
                _logger.LogError($"No songs found for artist: {artist}");
                return NotFound($"No songs found for artist: {artist}");
            }
            ViewData["Artist"] = artist;
            return View(this._artistsService.GetSongs(artist));
        }
    }
}
