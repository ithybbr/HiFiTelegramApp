using System.Text.Json.Nodes;
using HiFiTelegramApp.Models;
using Newtonsoft.Json.Linq;
using Python.Runtime;

namespace HiFiTelegramApp.Services;

public class ArtistsService
{
    //I think it will be better to drop artistspath cuz songs looks better and less files to keep
    // and more consistent
    private const string ArtistsPath = "/Resources/performers.txt";
    private const string SongsPath = "/Resources/performers_title_id.json";
    private const string DownloadedIdsPath = "/Resources/downloaded_id.txt";
    private readonly string _downloadsPath = "/Downloads/";
    private readonly string _scriptPath = "/Utilities/download_script.py";
    // will have to make it env variable probably
    private readonly string _pythonDdlPath = @"C:\Users\xxx\AppData\Local\Programs\Python\Python310\python.exe";
    public ArtistsService()
    {
        var result = GetArtists();
        this.Artists = result.ToList();
    }
    private IReadOnlyCollection<string> GetArtists()
    {
        using var reader = new StreamReader(ArtistsPath);
        var performer = reader.ReadLine()!;
        List<string> result = [];
        while (string.IsNullOrWhiteSpace(performer))
        {
            result.Add(performer);
            performer = reader.ReadLine()!;
        }
        
        return result;
    }

    public IReadOnlyCollection<SongModel> GetSongs(string artist)
    {
        var json = File.ReadAllText(SongsPath);

        var obj = JObject.Parse(json);
        var songs = obj[artist];
        if (songs is null)
            throw new InvalidOperationException();
        var i = 0;
        var songsModel = songs.Select(s => new SongModel
        {
            Id = ++i,
            Artist = artist,
            Name = s.Value<string>() ?? string.Empty,
            SongId = s.Values<int>().First(),
        });
        return songsModel.ToList();
    }

    public Task AddToFavoriteMarkArtists(string artist)
    {
        try
        {
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

    public static Task RemoveFromFavoriteMarkArtists(string artist)
    {
        try
        {
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
    
    public static Task AddToFavoriteSongs(string artist, string song, int songId)
    {
        try
        {
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
        using (Py.GIL())
        {
            var pythonScript = Py.Import(this._scriptPath);
            var message = new PyString(songId.ToString());
            var result = await pythonScript.InvokeMethod("start_up", new PyObject[] { message });
            await AddToDownloads(songId);
        }
    }
    private static Task AddToDownloads(int songId)
    {
        try
        {
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
            Directory.EnumerateFiles(this._downloadsPath, $"*{songId}*").ToList().ForEach(File.Delete);
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