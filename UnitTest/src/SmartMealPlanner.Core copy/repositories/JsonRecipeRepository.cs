using System.Text.Json;
using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Data.Repositories;

public sealed class JsonRecipeRepository : IRecipeRepository
{
    private readonly string _jsonPath;
    private readonly JsonSerializerOptions _opt = new() { PropertyNameCaseInsensitive = true };

    public JsonRecipeRepository(string jsonPath) => _jsonPath = jsonPath;

    public async Task<IReadOnlyList<Recipe>> GetAllAsync()
    {
        if (!File.Exists(_jsonPath)) return Array.Empty<Recipe>();
        await using var fs = File.OpenRead(_jsonPath);
        var list = await JsonSerializer.DeserializeAsync<List<Recipe>>(fs, _opt) ?? new();
        return list;
    }

    public async Task<Recipe?> GetByIdAsync(string id)
    {
        var all = await GetAllAsync();
        return all.FirstOrDefault(r => string.Equals(r.Id, id, StringComparison.OrdinalIgnoreCase));
    }
}
