using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Data.Repositories
{
    public sealed class JsonPantryRepository : IPantryRepository
    {
        private readonly string _jsonPath;
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public JsonPantryRepository(string jsonPath) => _jsonPath = jsonPath;

        public async Task<Pantry> GetAsync()
        {
            if (!File.Exists(_jsonPath)) return new Pantry();
            using (var reader = new FileStream(_jsonPath, FileMode.Open, FileAccess.Read))
            {
                var pantry = await JsonSerializer.DeserializeAsync<Pantry>(reader, _options);
                return pantry ?? new Pantry();
            }
        }

        public async Task SaveAsync(Pantry pantry)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath));
            using (var stream = new FileStream(_jsonPath, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(stream, pantry, _options);
            }
        }
    }
}
