using System.Text.Json;
using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Data.Repositories;

public sealed class JsonPantryRepository : IPantryRepository
{
    private readonly string _jsonPath;
    private readonly JsonSerializerOptions _opt = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

    public JsonPantryRepository(string jsonPath) => _jsonPath = jsonPath;

    public async Task<Pantry> GetAsync()
    {
        if (!File.Exists(_jsonPath)) return new Pantry();
        await using var fs = File.OpenRead(_jsonPath);
        return await JsonSerializer.DeserializeAsync<Pantry>(fs, _opt) ?? new Pantry();
    }

    public async Task SaveAsync(Pantry pantry)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
        await using var fs = File.Create(_jsonPath);
        await JsonSerializer.SerializeAsync(fs, pantry, _opt);
    }
}
