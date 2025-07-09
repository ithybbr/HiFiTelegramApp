using System.Text.Json.Nodes;
using HiFiTelegramApp.Models;
using Newtonsoft.Json.Linq;

namespace HiFiTelegramApp.Services;

public class ArtistsService
{
    //I think it will be better to drop artistspath cuz songs looks better and less files to keep
    // and more consistent
    private const string ArtistsPath = "/Resources/performers.txt";
    private const string SongsPath = "/Resources/performers_title_id.json";
    private const string DownloadedIdsPath = "/Resources/downloaded_id.txt";
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

    public Task AddToFavoriteArtists(string artist)
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

    public static Task AddToFavoriteSongs(string artist, string song)
    {
        throw new NotImplementedException();
    }
    public static Task RemoveFromFavoriteArtists(string artist)
    {
        throw  new NotImplementedException();
    }
    //Run a download script and when it finishes return download finished notification maybe
    public async Task Download(int songId)
    {
        throw new NotImplementedException();
    }
    public Task AddToDownloads(int songId)
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

    public Task RemoveFromDownloads(int songId)
    {
        throw new NotImplementedException();
    }
    public List<string> Artists { get; init; }
}