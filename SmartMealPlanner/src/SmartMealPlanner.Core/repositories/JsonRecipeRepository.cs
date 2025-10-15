using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Data.Repositories
{
    public sealed class JsonRecipeRepository : IRecipeRepository
    {
        private readonly string _jsonPath;

        public JsonRecipeRepository(string jsonPath) => _jsonPath = jsonPath;

        public async Task<IReadOnlyList<Recipe>> GetAllAsync()
        {
            if (!File.Exists(_jsonPath)) return Array.Empty<Recipe>();
            try
            {
                using var stream = File.OpenRead(_jsonPath);
                var recipes = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Recipe>>(stream, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return recipes ?? new List<Recipe>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JsonRecipeRepository] Error reading recipes: {ex}");
                return new List<Recipe>();
            }
        }

        public async Task<Recipe> GetByIdAsync(string id)
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault(r => string.Equals(r.Id, id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
