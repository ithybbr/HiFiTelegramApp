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
                Console.WriteLine($"Song: {song.Path}");
            }

            return PartialView(list);
        }

        [HttpPost("remove/{songId:int}")]
        public async Task DeleteSong(int songId)
        {
            Console.WriteLine($"Delete request for songId: {songId}");
            await _downloadService.DeleteFromDownloads(songId);
        }
    }
}
