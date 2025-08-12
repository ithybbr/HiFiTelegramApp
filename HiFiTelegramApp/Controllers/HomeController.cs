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
        public IActionResult ArtistsCollection()
        {
            return PartialView();
        }
        [HttpGet("list")]
        public IActionResult ArtistsList([FromQuery]int offset = 0, int count = 400)
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
            Console.WriteLine($"Returning {artists.Count} artists from offset {offset} with count {count}");
            return PartialView(artists);
        }
        [HttpGet("search")]
        public IActionResult Search([FromQuery]string query)
        {
            var artists = this._artistsService.Artists;
            if (string.IsNullOrWhiteSpace(query) || artists is null || artists.Count == 0)
            {
                _logger.LogError("Search query is empty or artists list is empty.");
                return PartialView("ArtistsList", artists);
            }
            var filteredArtists = artists
                .Where(a => a.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return PartialView("ArtistsList", filteredArtists);
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
