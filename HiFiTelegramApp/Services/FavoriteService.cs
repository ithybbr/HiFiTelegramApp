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
        var favorites = new List<FavoriteModel>();
        try
        {
            favorites = JsonSerializer.Deserialize<List<FavoriteModel>>(File.ReadAllText(favoritesPath));
        }
        catch {
            Console.WriteLine($"Error reading favorites from {favoritesPath}, initializing empty list.");
        }
        this.Favorites = [.. favorites];
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
            var model = new FavoriteModel
            {
                Artist = artist,
                Songs = []
            };
            var opts = new JsonSerializerOptions { WriteIndented = true };
            this.Favorites.Add(model);
            File.WriteAllText(favoritesPath, JsonSerializer.Serialize(this.Favorites, opts));
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
            Favorites.RemoveAll(x => x.Artist == artist);
            File.WriteAllText(favoritesPath, JsonSerializer.Serialize(json));
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    
    public Task AddToFavoriteSongs(string artist, string song, int songId, bool fromDownloads)
    {
        try
        {
            var favoritesPath = Path.Combine(_env.ContentRootPath, "Resources", "favorites.json");
            var entry = this.Favorites.FirstOrDefault(x => x.Artist == artist);
            if (entry == null)
            {
                AddToFavoriteArtists(artist);
                entry = this.Favorites.FirstOrDefault(x => x.Artist == artist);
            }
            entry!.Songs.Add(new FavoriteSongModel
            {
                Artist = artist,
                SongId = songId,
                Name = song
            });
            this.Favorites.RemoveAll(x => x.Artist == artist);
            this.Favorites.Add(entry);
            File.WriteAllText(favoritesPath, JsonSerializer.Serialize(this.Favorites));
            if (!fromDownloads)
            {
                CheckDownloads(songId, false).Wait();
            }
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
            CheckDownloads(songId, false).Wait(); // Remove from downloads if it was a favorite
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task CheckDownloads(int songId, bool favorite)
    {
        try
        {
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_songs.json");
            var list = JsonSerializer.Deserialize<List<AudioModel>>(File.ReadAllText(DownloadedIdsPath)) ?? [];
            var index = list.FindIndex(x => x.SongId == songId);
            if (index != -1)
            {
                list[index].IsFavorite = favorite;
                var opts = new JsonSerializerOptions { WriteIndented = true };
                var newJson = JsonSerializer.Serialize(list, opts);
                File.WriteAllTextAsync(DownloadedIdsPath, newJson);
            }
            return Task.CompletedTask;
    }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public bool IsFavoriteArtist(string artist)
    {
        return this.Favorites.Any(x => x.Artist == artist);
    }

    public bool IsFavoriteSong(string artist, int songId)
    {
        return this.Favorites.Any(x => x.Artist == artist && x.Songs.Any(s => s.SongId == songId));
    }

    private List<FavoriteModel> Favorites { get; set; }
}