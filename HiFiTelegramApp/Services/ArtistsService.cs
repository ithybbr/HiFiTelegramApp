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
    public List<string> Artists { get; init; }
}