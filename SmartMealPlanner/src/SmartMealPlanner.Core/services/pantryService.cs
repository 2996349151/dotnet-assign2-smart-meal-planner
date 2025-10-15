using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Services
{
    public sealed class PantryService : IPantryService
    {
        private readonly IPantryRepository _repo;

        public PantryService(IPantryRepository repo) { _repo = repo; }

        public Task<Pantry> GetAsync() { return _repo.GetAsync(); }

        public Task SaveAsync(Pantry pantry) { return _repo.SaveAsync(pantry); }

        // Apply a recipe to the pantry, reducing ingredient quantities accordingly.
        public async Task ApplyCookingAsync(Recipe recipe)
        {
            var pantry = await _repo.GetAsync();
            var items = pantry.Items;

            foreach (var kvp in recipe.Ingredients)
            {
                var name = kvp.Key;
                var qtyStr = kvp.Value;
                var key = Normalize(name);
                var need = ParseQty(qtyStr);
                var have = items.ContainsKey(key) ? items[key] : 0.0;

                if (have + 1e-9 < need)
                    throw new InvalidOperationException($"Not enough '{name}': need {need}, have {have}");
            }

            // Deduct the used ingredients
            foreach (var kvp in recipe.Ingredients)
            {
                var name = kvp.Key;
                var qtyStr = kvp.Value;
                var key = Normalize(name);
                var need = ParseQty(qtyStr);
                items[key] = Math.Max(0, items[key] - need);
            }

            await _repo.SaveAsync(pantry);
        }

        private static string Normalize(string s) { return (s ?? "").Trim().ToLowerInvariant(); }

        // Parse a quantity string to a double, ignoring non-numeric characters.
        private static double ParseQty(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            double v;
            if (double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v)) return v;
            var filtered = new string(s.ToCharArray().Where(ch => char.IsDigit(ch) || ch == '.' || ch == '-').ToArray());
            return double.TryParse(filtered, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v) ? v : 0;
        }
    }
}
