using System.Diagnostics;
using HiFiTelegramApp.Models;
using HiFiTelegramApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HiFiTelegramApp.Controllers
{
    [Route("[controller]")]
    public class DownloadSongController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DownloadSongService _downloadSongService;
        public DownloadSongController(ILogger<HomeController> logger, DownloadSongService downloadSongService)
        {
            _logger = logger;
            _downloadSongService = downloadSongService;
        }

        [HttpPost]
        public async Task DownloadSong(string artist, int songId)
        {
            Console.WriteLine($"Download request for songId: {songId} by artist: {artist}");
            await _downloadSongService.Download(artist, songId);
        }
    }
}
