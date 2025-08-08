using System.ComponentModel;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using ElectronNET.API;
using HiFiTelegramApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Python.Runtime;

namespace HiFiTelegramApp.Services;

public class DownloadSongService
{
    private readonly IWebHostEnvironment _env;
    private readonly DownloadService downloadService;
    public DownloadSongService(IWebHostEnvironment env, DownloadService downloadService)
    {
        this._env = env;
        this.downloadService = downloadService;
        InitializePythonBot();
    }
    private void InitializePythonBot()
    {
        if (!PythonEngine.IsInitialized)
        {
            var pythonHomeDir = Path.Combine(_env.ContentRootPath, "python");
            Runtime.PythonDLL = Path.Combine(pythonHomeDir, "python313.dll");
            var libFolder = Path.Combine(pythonHomeDir, "Lib");
            var sitePackages = Path.Combine(libFolder, "site-packages");
            var DllsFolder = Path.Combine(pythonHomeDir, "DLLs");
            // Build PythonPath so that these come first:
            PythonEngine.PythonPath = string.Join(
                Path.PathSeparator.ToString(),
                [sitePackages, libFolder, DllsFolder, PythonEngine.PythonPath]
            );
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
            using (Py.GIL())
            {
                var scriptFolder = Path.Combine(_env.ContentRootPath, "Utilities");
                dynamic os = Py.Import("os");
                os.environ["API_ID"] = new PyString(Environment.GetEnvironmentVariable("API_ID")!);
                os.environ["API_HASH"] = new PyString(Environment.GetEnvironmentVariable("API_HASH")!);
                os.environ["BOT_TOKEN"] = new PyString(Environment.GetEnvironmentVariable("BOT_TOKEN")!);
                os.environ["CHANNEL_ID"] = new PyString(Environment.GetEnvironmentVariable("CHANNEL_ID")!);
                dynamic sys = Py.Import("sys");
                sys.path.append(Path.Combine(scriptFolder));
                sys.path.append(Path.Combine(_env.ContentRootPath, "Utilities"));
            }
        }
    }
    public Task Download(string artist, string songName, int songId)
    {
        try
        {
            if (downloadService.GetDownloadById(songId) != null)
            {
                Console.WriteLine($"Song with ID {songId} is already downloaded.");
            }
        }
        catch (KeyNotFoundException)
        {
            // If the songId is not found, we proceed to download it.
            Console.WriteLine($"Song with ID {songId} not found in downloads, proceeding to download.");
            using (Py.GIL())
            {
                dynamic pythonBot = Py.Import("downloadScript");
                string path = pythonBot.start_bot(new PyInt(songId));
                Console.WriteLine($"Download result for songId {songId}: {path}");
                downloadService.AddToDownloads(artist, songName, songId, path);
            }
        }
        return Task.CompletedTask;
    }
}