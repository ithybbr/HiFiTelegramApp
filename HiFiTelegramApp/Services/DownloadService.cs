using System.Net;
using ElectronNET.API;
using Python.Runtime;

namespace HiFiTelegramApp.Services;

public class DownloadService
{
    private readonly IWebHostEnvironment _env;
    
    public DownloadService(IWebHostEnvironment env)
    {
        this._env = env;
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
        if (!PythonEngine.IsInitialized)
        {
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
        }
    }
    public List<string> GetDownloads()
    {
        var downloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_songs.json");
        var list = File.ReadAllLines(downloadedIdsPath).ToList();
        return list;
    }
        //Run a download script and when it finishes return download finished notification maybe
    public async Task Download(int songId)
    {
        Console.WriteLine($"Download request for songId: {songId}");
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
            var pythonScript = Py.Import("downloadScript");
            var message = new PyInt(songId);
            var result = pythonScript.InvokeMethod("start_up", [ message ]);
            Console.WriteLine($"Download result for songId {songId}: {result}");
            await AddToDownloads(songId, result);
        }
    }
    private Task AddToDownloads(int songId, PyObject result)
    {
        try
        {
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_songs.json");
            File.AppendAllText(DownloadedIdsPath, $"~{songId}~{result}");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task DeleteFromDownloads(int songId)
    {
        try
        {
            var downloadsPath = Path.Combine(_env.ContentRootPath, "Downloads");
            Directory.EnumerateFiles(downloadsPath, $"*{songId}*").ToList().ForEach(File.Delete);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    public Task RemoveFromDownloads(int songId)
    {
        try
        {
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_id.txt");
            var content = File.ReadAllText(DownloadedIdsPath);
            content = content.Replace(songId.ToString(), string.Empty);
            File.WriteAllText(DownloadedIdsPath, content);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
}