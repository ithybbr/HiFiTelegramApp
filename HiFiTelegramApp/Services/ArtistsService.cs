using System.Text.Json.Nodes;
using HiFiTelegramApp.Models;
using Newtonsoft.Json.Linq;
using Python.Runtime;

namespace HiFiTelegramApp.Services;

public class ArtistsService
{
    //I think it will be better to drop artistspath cuz songs looks better and less files to keep
    // and more consistent
    private readonly IWebHostEnvironment _env;
    // will have to make it env variable probably
    private readonly string _pythonDdlPath = @"C:\Users\xxx\AppData\Local\Programs\Python\Python310\python.exe";
    public ArtistsService(IWebHostEnvironment env)
    {
        _env = env;
        var result = GetArtists();
        this.Artists = [.. result];
    }
    private IReadOnlyCollection<string> GetArtists()
    {
        var ArtistsPath = Path.Combine(_env.ContentRootPath, "Resources", "performers.txt");
        using var reader = new StreamReader(ArtistsPath);
        var performer = reader.ReadLine()!;
        Console.WriteLine($"First performer: {performer}");
        List<string> result = [];
        while (!string.IsNullOrWhiteSpace(performer))
        {
            result.Add(performer);
            performer = reader.ReadLine()!;
        }
        return result;
    }

    public IReadOnlyCollection<SongModel> GetSongs(string artist)
    {
        Console.WriteLine("I am here");
        var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
        // 2) read & parse
        var raw = File.ReadAllText(SongsPath);
        var root = JObject.Parse(raw);
        var block = root[artist] as JObject
                 ?? throw new InvalidOperationException($"Artist '{artist}' not found.");

        // 3) enumerate each JProperty (key = song name, value = id)
        var list = block.Properties()
            .Select((prop, idx) => new SongModel
            {
                Id = idx + 1,
                Artist = artist,
                Name = prop.Name,
                SongId = prop.Value.Value<int>()
            })
            .ToList();
        Console.WriteLine($"Found {list.Count} songs for artist: {artist}");
        return list;
    }

    public Task AddToFavoriteMarkArtists(string artist)
    {
        try
        {
            var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
            var json = File.ReadAllText(SongsPath);

            var obj = JObject.Parse(json);
            obj[artist]!["~Favorite"] = true;
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task RemoveFromFavoriteMarkArtists(string artist)
    {
        try
        {
            var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
            var json = File.ReadAllText(SongsPath);

            var obj = JObject.Parse(json);
            obj[artist]!["~Favorite"] = false;
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    
    public Task AddToFavoriteSongs(string artist, string song, int songId)
    {
        try
        {
            var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
            var json = File.ReadAllText(SongsPath);

            var obj = JObject.Parse(json);
            _ = obj[artist]!["Favorites"]?.Append(songId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    
    public Task RemoveFromFavoriteSongs(string artist, string song, int songId)
    {
        try
        {
            var SongsPath = Path.Combine(_env.ContentRootPath, "Resources", "performer_title_id.json");
            var json = File.ReadAllText(SongsPath);

            var obj = JObject.Parse(json);
            var favs = obj[artist]?["Favorites"];
            for (var i = 0; i < favs?.Count(); i++)
            {
                if (favs[i]?.Value<int>() == songId)
                {
                    favs[i]?.Remove();
                }
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
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
            var result = pythonScript.InvokeMethod("start_up", new PyObject[] { message });
            await AddToDownloads(songId);
        }
    }
    private Task AddToDownloads(int songId)
    {
        try
        {
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_id.txt");
            File.AppendAllText(DownloadedIdsPath, songId.ToString());
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
    public List<string> Artists { get; init; }
}