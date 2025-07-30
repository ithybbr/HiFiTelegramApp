using System.Text.Json;
using HiFiTelegramApp.Models;
using Newtonsoft.Json.Linq;

namespace HiFiTelegramApp.Services;

public class FavoriteService
{
    private readonly IWebHostEnvironment _env;
    
    public FavoriteService(IWebHostEnvironment env)
    {
        this._env = env;
        var favoritesPath = Path.Combine(_env.ContentRootPath, "Resources", "favorites.json");
        this.Favorites = [.. JsonSerializer.Deserialize<List<FavoriteModel>>(File.ReadAllText(favoritesPath))!];
    }
    public List<FavoriteModel> GetFavorites()
    {
        try
        {
            return this.Favorites;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading favorites: {ex.Message}");
            return [];
        }
    }
     public Task AddToFavoriteArtists(string artist)
    {
        Console.WriteLine($"Adding {artist} to favorites");
        try
        {
            var favoritesPath = Path.Combine(_env.ContentRootPath, "Resources", "favorites.json");
            if(this.Favorites.Exists(x => x.Artist == artist))
            {
                Console.WriteLine($"Artist {artist} is already in favorites");
                return Task.CompletedTask;
            }
            var jsonContent = File.ReadAllText(favoritesPath);
            var json = JsonSerializer.Deserialize<List<FavoriteModel>>(jsonContent) ?? [];
            var model = new FavoriteModel
            {
                Artist = artist,
                Songs = []
            };
            json.Add(model);
            File.WriteAllText(favoritesPath, JsonSerializer.Serialize(json));
            this.Favorites.Add(model);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task RemoveFromFavoriteArtists(string artist)
    {
        try
        {
            var favoritesPath = Path.Combine(_env.ContentRootPath, "Resources", "favorites.json");
            var jsonContent = File.ReadAllText(favoritesPath);
            var json = JsonSerializer.Deserialize<List<FavoriteModel>>(jsonContent) ?? [];
            json.RemoveAll(x => x.Artist == artist);
            File.WriteAllText(favoritesPath, JsonSerializer.Serialize(json));
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
            var favoritesPath = Path.Combine(_env.ContentRootPath, "Resources", "favorites.json");
            var jsonContent = File.ReadAllText(favoritesPath);
            var json = JsonSerializer.Deserialize<List<FavoriteModel>>(jsonContent) ?? [];
            var entry = json.FirstOrDefault(x => x.Artist == artist);
            if (entry == null)
            {
                AddToFavoriteArtists(artist);
            }
            entry!.Songs.Add(new FavoriteSongModel
            {
                Artist = artist,
                SongId = songId,
                Name = song
            });
            File.WriteAllText(favoritesPath, JsonSerializer.Serialize(json));
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
            var favoritesPath = Path.Combine(_env.ContentRootPath, "Resources", "favorites.json");
            var jsonContent = File.ReadAllText(favoritesPath);
            var json = JsonSerializer.Deserialize<List<FavoriteModel>>(jsonContent) ?? [];
            var entry = json.FirstOrDefault(x => x.Artist == artist);
            if (entry is null)
            {
                return Task.FromCanceled(new CancellationToken(true));
            }
            entry!.Songs.RemoveAll(x => x.SongId == songId);
            File.WriteAllText(favoritesPath, JsonSerializer.Serialize(json));
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    private List<FavoriteModel> Favorites { get; set; }
}