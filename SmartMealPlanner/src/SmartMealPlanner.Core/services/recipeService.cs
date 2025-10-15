using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Services
{
    public sealed class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _repo;

        public RecipeService(IRecipeRepository repo) { _repo = repo; }

        public async Task<IReadOnlyList<Recipe>> SearchAsync(string keyword)
        {
            var k = (keyword ?? "").Trim().ToLowerInvariant();
            var all = await _repo.GetAllAsync();
            if (string.IsNullOrEmpty(k)) return all.ToList();
            return all
                .Where(r =>
                    r.Title.ToLowerInvariant().Contains(k) ||
                    r.Tags.Any(t => t.ToLowerInvariant().Contains(k)) ||
                    r.Ingredients.Keys.Any(n => Normalize(n).Contains(k)))
                .ToList();
        }

        public Task<Recipe> GetByIdAsync(string id) { return _repo.GetByIdAsync(id); }

        public async Task<IReadOnlyList<Recipe>> GetRecommendationsAsync(Pantry pantry, UserPreference pref, int tolerance)
        {
            var all = await _repo.GetAllAsync();
            var pmap = new Dictionary<string, double>();
            foreach (var kv in pantry.Items)
                pmap[Normalize(kv.Key)] = kv.Value;
            var tol = Math.Max(0, tolerance);

            double CuisineWeight(Recipe r)
            {
                double sum = 0;
                foreach (var t in r.Tags)
                {
                    double w = 0;
                    pref.CuisineWeights.TryGetValue(t.ToLowerInvariant(), out w);
                    sum += w;
                }
                return sum;
            }

            bool IsDisliked(Recipe r)
            {
                foreach (var n in r.Ingredients.Keys)
                    if (pref.Disliked.Contains(Normalize(n))) return true;
                return false;
            }

            bool DietOk(Recipe r)
            {
                if (pref.DietTags.Count == 0) return true;
                foreach (var t in r.Tags)
                    if (pref.DietTags.Contains(t.ToLowerInvariant())) return true;
                return false;
            }

            var accepted = new List<Tuple<Recipe, int, double>>();

            foreach (var r in all)
            {
                int missing = 0;
                double missingDeficit = 0;

                foreach (var kvp in r.Ingredients)
                {
                    var name = kvp.Key;
                    var qtyStr = kvp.Value;
                    var key = Normalize(name);
                    var need = ParseQty(qtyStr);
                    var have = pmap.ContainsKey(key) ? pmap[key] : 0.0;

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
                score -= Math.Min(Math.Max(r.CookTimeMins / 120.0, 0), 1) * 0.3; // prefer quicker

                // small tie-breaker by deficit
                score -= Math.Min(missingDeficit / 10.0, 1.0) * 0.2;

                accepted.Add(Tuple.Create(r, missing, score));
            }

            return accepted
                .OrderByDescending(x => x.Item3)
                .ThenBy(x => x.Item1.CookTimeMins)
                .Select(x => x.Item1)
                .ToList();
        }

        private static string Normalize(string s) { return (s ?? "").Trim().ToLowerInvariant(); }

        private static double ParseQty(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            double v;
            if (double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v)) return v;
            // fallback: strip non-digits except dot
            var filtered = new string(s.ToCharArray().Where(ch => char.IsDigit(ch) || ch == '.' || ch == '-').ToArray());
            return double.TryParse(filtered, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v) ? v : 0;
        }
    }
}
