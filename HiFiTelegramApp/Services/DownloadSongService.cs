using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using ElectronNET.API;
using HiFiTelegramApp.Models;
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
                dynamic pythonScript = Py.Import("downloadScript");
                sys.path.append(Path.Combine(_env.ContentRootPath, "Utilities"));
                pythonScript.start_up();
            }
        }
    }
    public async Task Download(string artist, int songId)
    {
        Console.WriteLine($"Download request for songId: {songId}");
        using (Py.GIL())
        {
            dynamic pythonScript = Py.Import("downloadScript");
            var message = new PyInt(songId);
            dynamic result = pythonScript.download_song(new PyInt(songId));
            string path = (string)result;
            Console.WriteLine($"Download result for songId {songId}: {path}");
            await downloadService.AddToDownloads(artist, songId, path);
        }
    }
}