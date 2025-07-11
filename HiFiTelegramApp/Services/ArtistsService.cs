using System.Text.Json.Nodes;
using HiFiTelegramApp.Models;
using Newtonsoft.Json.Linq;
using Python.Runtime;

namespace HiFiTelegramApp.Services;

//Will have to separate it grows monolithic. 
public class ArtistsService
{
    //I think it will be better to drop artistspath cuz songs looks better and less files to keep
    // and more consistent
    private readonly IWebHostEnvironment _env;
    // will have to make it env variable probably
    public ArtistsService(IWebHostEnvironment env)
    {
        _env = env;
        var result = GetArtists();
        this.Artists = [.. result];
    }
    private List<string> GetArtists()
    {
        var ArtistsPath = Path.Combine(_env.ContentRootPath, "Resources", "performers.txt");
        using var reader = new StreamReader(ArtistsPath);
        var performer = reader.ReadLine()!;
        List<string> result = [];
        while (!string.IsNullOrWhiteSpace(performer))
        {
            result.Add(performer);
            performer = reader.ReadLine()!;
        }
        reader.Close();
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
    public List<string> Artists { get; init; }
}