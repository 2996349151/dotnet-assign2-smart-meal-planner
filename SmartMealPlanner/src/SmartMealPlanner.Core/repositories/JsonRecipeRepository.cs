using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Data.Repositories
{
    public sealed class JsonRecipeRepository : IRecipeRepository
    {
        private readonly string _jsonPath;
        // NOTE: System.Text.Json is not available in .NET Framework 4.8.1. You may need to use Newtonsoft.Json instead.
        // private readonly JsonSerializerOptions _opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

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
