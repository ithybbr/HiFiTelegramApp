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
            return View();
        }
        [HttpGet("artists")]
        public IActionResult ArtistsCollection(int offset = 0, int count = 0)
        {
            var artists = this._artistsService.Artists;
            if (artists is null || artists.Count == 0)
            {
                _logger.LogError("Artists list is empty or null.");
                return PartialView();
            }
            if (count <= 0)
            {
                count = artists.Count;
            }
            artists = artists.Skip(offset).Take(count).ToList();
            return PartialView(artists);
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
            return PartialView(this._artistsService.GetSongs(artist));
        }
    }
}
