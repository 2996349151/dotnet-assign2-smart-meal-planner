using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Services;

public sealed class PantryService : IPantryService
{
    private readonly IPantryRepository _repo;

    public PantryService(IPantryRepository repo) => _repo = repo;

    public Task<Pantry> GetAsync() => _repo.GetAsync();

    public Task SaveAsync(Pantry pantry) => _repo.SaveAsync(pantry);

    public async Task ApplyCookingAsync(Recipe recipe)
    {
        var pantry = await _repo.GetAsync();
        var items = pantry.Items;

        // 1) Validate
        foreach (var (name, qtyStr) in recipe.Ingredients)
        {
            var key = Normalize(name);
            var need = ParseQty(qtyStr);
            var have = items.TryGetValue(key, out var v) ? v : 0.0;
            if (have + 1e-9 < need)
                throw new InvalidOperationException($"Not enough '{name}': need {need}, have {have}");
        }

        // 2) Deduct
        foreach (var (name, qtyStr) in recipe.Ingredients)
        {
            var key = Normalize(name);
            var need = ParseQty(qtyStr);
            items[key] = Math.Max(0, items[key] - need);
        }

        await _repo.SaveAsync(pantry);
    }

    private static string Normalize(string s) => (s ?? "").Trim().ToLowerInvariant();

    private static double ParseQty(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return 0;
        if (double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var v)) return v;
        var filtered = new string((s.Where(ch => char.IsDigit(ch) || ch == '.' || ch == '-')).ToArray());
        return double.TryParse(filtered, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out v) ? v : 0;
    }
}
