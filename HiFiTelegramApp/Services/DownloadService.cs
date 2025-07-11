using Python.Runtime;

namespace HiFiTelegramApp.Services;

public class DownloadService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _pythonDdlPath = @"C:\Users\xxx\AppData\Local\Programs\Python\Python310\python.exe";
    
    public DownloadService(IWebHostEnvironment env)
    {
        this._env = env;
    }
        //Run a download script and when it finishes return download finished notification maybe
    public async Task Download(int songId)
    {
        // something that might be needed
        // dynamic sys = Py.Import("sys");
        // sys.path.append(Path.Combine(this.scriptPath));
        Runtime.PythonDLL = this._pythonDdlPath;
        PythonEngine.Initialize();
        var scriptPath = Path.Combine(_env.ContentRootPath, "Utilities", "download_script.py");
        using (Py.GIL())
        {
            var pythonScript = Py.Import(scriptPath);
            var message = new PyString(songId.ToString());
            var result = pythonScript.InvokeMethod("start_up", [message]);
            await AddToDownloads(songId, result);
        }
    }
    private Task AddToDownloads(int songId, PyObject result)
    {
        try
        {
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_id.txt");
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