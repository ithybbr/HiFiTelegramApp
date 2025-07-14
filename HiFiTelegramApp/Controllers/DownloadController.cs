using System.Diagnostics;
using HiFiTelegramApp.Models;
using HiFiTelegramApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HiFiTelegramApp.Controllers
{
    [Route("[controller]")]
    public class DownloadController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DownloadService _downloadService;
        public DownloadController(ILogger<HomeController> logger, DownloadService downloadService)
        {
            _logger = logger;
            _downloadService = downloadService;
        }

        public IActionResult Index()
        {
            var list = _downloadService.GetDownloads();
            foreach (var song in list)
            {
                Console.WriteLine($"Song: {song}");
            }
            var audioModels = list.Select(song => new AudioModel
            {
                SongId = int.Parse(song.Split('~')[1]),
                Path = song.Split('~')[2],
                Artist = "Unknown Artist" // Placeholder artist, replace with actual artist if available
            }).ToList();

            return View(audioModels);
        }
        [HttpPost]
        public async Task<IActionResult> DownloadSong(string artist, int songId)
        {
            Console.WriteLine($"Download request for songId: {songId} by artist: {artist}");
            await _downloadService.Download(songId);

            return RedirectToAction("Artist", "Home", new { artist });
        }
    }
}
