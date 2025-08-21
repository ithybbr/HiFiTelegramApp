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
        [HttpGet("download")]
        public IActionResult Index()
        {
            var list = _downloadService.GetDownloads();
            foreach (var song in list)
            {
                Console.WriteLine($"Song: {song.Path}");
            }

            return PartialView(list);
        }
        [HttpGet("songby")]
        public AudioModel Song(int id)
        {
            var nextSong = _downloadService.GetDownloadByIndex(id);
            if (nextSong == null)
            {
                return new AudioModel
                {
                    Path = "No more songs available."
                };
            }
            Console.WriteLine($"Next song path: {nextSong.Path}");
            return nextSong;
        }
        [HttpPost("remove")]
        public async Task DeleteSong(int songId, string path)
        {
            Console.WriteLine($"Delete request for songId: {songId} {path}");
            await _downloadService.DeleteFromDownloads(songId, path);
        }
        [HttpPost("favorite")]
        public async Task<IActionResult> ToggleFavorite(int songId)
        {
            Console.WriteLine($"Toggle favorite for songId: {songId}");
            await _downloadService.ToggleFavorite(songId);
            return Ok();
        }
    }
}
