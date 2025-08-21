using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using ElectronNET.API;
using HiFiTelegramApp.Models;
using Python.Runtime;

namespace HiFiTelegramApp.Services;

public class DownloadService
{
    private readonly IWebHostEnvironment _env;
    private readonly FavoriteService favoriteService;

    public DownloadService(IWebHostEnvironment env, FavoriteService favoriteService)
    {
        this.favoriteService = favoriteService;
        this._env = env;
        var downloadedIdsPath = Path.Combine(this._env.ContentRootPath, "Resources", "downloaded_songs.json");
        var jsonContent = string.Empty;

        try
        {
            jsonContent = File.ReadAllText(downloadedIdsPath); // Read the file content as a single string
            this.Downloads = JsonSerializer.Deserialize<List<AudioModel>>(jsonContent) ?? []; // Deserialize the string content
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading downloaded_songs.json: {ex.Message}");
            this.Downloads = [];
        }

        this.IdsToSong = this.Downloads.ToDictionary(x => x.SongId, x => x);
    }
    public List<AudioModel> GetDownloads()
    {
        return this.Downloads ?? [];
    }
    public AudioModel GetDownloadById(int songId)
    {
        return this.IdsToSong[songId];
    }
    public AudioModel GetDownloadByIndex(int id)
    {
        return this.Downloads[id];
    }
    public Task AddToDownloads(string artist, string songName, int songId, string result)
    {
        try
        {
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_songs.json");
            var trimmed = result.Replace($"{_env.ContentRootPath}\\wwwroot", "").Replace('\\','/');
            Console.WriteLine($"Adding songId {songId} with path {trimmed} to downloads.");
            Console.WriteLine($"Result: {result}");
            var text = File.ReadAllTextAsync(DownloadedIdsPath).Result;
            List<AudioModel> list = [];
            try
            {
                list = JsonSerializer.Deserialize<List<AudioModel>>(text) ?? [];
            }
            catch (Exception)
            {
                list = [];
            }
            AudioModel audioModel = new()
            {
                Id = list!.Count + 1,
                SongId = songId,
                Path = trimmed,
                Artist = artist,
                Name = songName,
                IsFavorite = favoriteService.IsFavoriteSong(artist, songId)
            };
            // 2) Append
            list.Add(audioModel);
            this.Downloads.Add(audioModel);
            this.IdsToSong[songId] = audioModel;
            // 3) Save
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var newJson = JsonSerializer.Serialize(list, opts);
            File.WriteAllTextAsync(DownloadedIdsPath, newJson);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding songId {songId} to downloads: {ex.Message}");
            return Task.FromException(ex);
        }
    }

    public Task DeleteFromDownloads(int songId, string path)
    {
        try
        {
            var downloadsPath = Path.Combine(_env.ContentRootPath, "wwwroot", path.TrimStart('/').Replace('/', '\\'));
            Console.WriteLine($"Deleting songId {songId} with path {downloadsPath} from downloads.");
            File.Delete(downloadsPath);
            RemoveFromDownloads(songId).Wait();
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
            var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_songs.json");
            var list = JsonSerializer.Deserialize<List<AudioModel>>(File.ReadAllText(DownloadedIdsPath));
            list!.RemoveAll(x => x.SongId == songId);
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var newJson = JsonSerializer.Serialize(list, opts);
            Downloads.RemoveAll(x => x.SongId == songId);
            File.WriteAllTextAsync(DownloadedIdsPath, newJson);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    public bool IsDownloaded(int songId)
    {
        return this.IdsToSong.ContainsKey(songId);
    }
    public Task ToggleFavorite(int songId)
    {
        try
        {
            if (this.IdsToSong.TryGetValue(songId, out AudioModel? song))
            {
                song.IsFavorite = !song.IsFavorite;
                var DownloadedIdsPath = Path.Combine(_env.ContentRootPath, "Resources", "downloaded_songs.json");
                var list = JsonSerializer.Deserialize<List<AudioModel>>(File.ReadAllText(DownloadedIdsPath)) ?? [];
                var index = list.FindIndex(x => x.SongId == songId);
                if (index != -1)
                {
                    list[index] = song; // Update the song in the list
                    var opts = new JsonSerializerOptions { WriteIndented = true };
                    var newJson = JsonSerializer.Serialize(list, opts);
                    File.WriteAllTextAsync(DownloadedIdsPath, newJson);
                }
                return Task.CompletedTask;
            }
            return Task.FromException(new Exception("Song not found in downloads"));
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    private List<AudioModel> Downloads { get; set; } = [];
    private Dictionary<int, AudioModel> IdsToSong { get; set; } = [];
}