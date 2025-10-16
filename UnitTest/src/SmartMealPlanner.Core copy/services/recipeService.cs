using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Services;

public sealed class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repo;

    public RecipeService(IRecipeRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Recipe>> SearchAsync(string keyword)
    {
        var k = (keyword ?? "").Trim().ToLowerInvariant();
        var all = await _repo.GetAllAsync();
        if (string.IsNullOrEmpty(k)) return all;
        return all
            .Where(r =>
                r.Title.ToLowerInvariant().Contains(k) ||
                r.Tags.Any(t => t.ToLowerInvariant().Contains(k)) ||
                r.Ingredients.Keys.Any(n => Normalize(n).Contains(k)))
            .ToList();
    }

    public Task<Recipe?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);

    public async Task<IReadOnlyList<Recipe>> GetRecommendationsAsync(Pantry pantry, UserPreference pref, int tolerance)
    {
        var all = await _repo.GetAllAsync();
        var pmap = pantry.Items.ToDictionary(kv => Normalize(kv.Key), kv => kv.Value);
        var tol = Math.Max(0, tolerance);

        double CuisineWeight(Recipe r)
            => r.Tags.Sum(t => pref.CuisineWeights.TryGetValue(t.ToLowerInvariant(), out var w) ? w : 0);

        bool IsDisliked(Recipe r)
            => r.Ingredients.Keys.Any(n => pref.Disliked.Contains(Normalize(n)));

        bool DietOk(Recipe r)
            => pref.DietTags.Count == 0 || r.Tags.Any(t => pref.DietTags.Contains(t.ToLowerInvariant()));

        var accepted = new List<(Recipe recipe, int missing, double score)>();

        foreach (var r in all)
        {
            int missing = 0;
            double missingDeficit = 0;

            foreach (var (name, qtyStr) in r.Ingredients)
            {
                var key = Normalize(name);
                var need = ParseQty(qtyStr);
                var have = pmap.TryGetValue(key, out var v) ? v : 0.0;

                if (have + 1e-9 < need)
                {
                    missing++;
                    missingDeficit += (need - have);
                }
            }

            if (missing > tol) continue;

            double score = 1.0;
            score += CuisineWeight(r);
            if (DietOk(r)) score += 0.3;
            if (IsDisliked(r)) score -= 0.8;
            score -= missing * 0.2;
            score -= Math.Clamp(r.CookTimeMins / 120.0, 0, 1) * 0.3; // prefer quicker

            // small tie-breaker by deficit
            score -= Math.Min(missingDeficit / 10.0, 1.0) * 0.2;

            accepted.Add((r, missing, score));
        }

        return accepted
            .OrderByDescending(x => x.score)
            .ThenBy(x => x.recipe.CookTimeMins)
            .Select(x => x.recipe)
            .ToList();
    }

    private static string Normalize(string s) => (s ?? "").Trim().ToLowerInvariant();

    private static double ParseQty(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return 0;
        if (double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var v)) return v;
        // fallback: strip non-digits except dot
        var filtered = new string((s.Where(ch => char.IsDigit(ch) || ch == '.' || ch == '-')).ToArray());
        return double.TryParse(filtered, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out v) ? v : 0;
    }
}
