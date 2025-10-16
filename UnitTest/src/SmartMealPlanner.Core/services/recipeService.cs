using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Services
{
    public sealed class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _repo;

        public RecipeService(IRecipeRepository repo)
        {
            _repo = repo;
        }

        // Search recipes by keyword in title, tags, or ingredients.
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

        public Task<Recipe> GetByIdAsync(string id)
        {
            return _repo.GetByIdAsync(id);
        }

        // Generate recipe recommendations based on pantry contents and user preferences.
        public async Task<IReadOnlyList<Recipe>> GetRecommendationsAsync(Pantry pantry, UserPreference pref, int tolerance)
        {
            var all = await _repo.GetAllAsync();

            // Remove recipes containing disliked ingredients.
            if (pref.Disliked != null && pref.Disliked.Any())
            {
                all = all
                    .Where(r => !r.Ingredients.Keys.Any(ing =>
                        pref.Disliked.Contains(Normalize(ing))))
                    .ToList();
            }

            // Convert pantry to dictionary
            var pmap = new Dictionary<string, double>();
            foreach (var kv in pantry.Items)
                pmap[Normalize(kv.Key)] = kv.Value;

            var tol = Math.Max(0, tolerance);

            // Calculate cuisine weight score
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

            // Check diet tag match
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

                // Count missing ingredients
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

                // Compute recommendation score
                double score = 1.0;
                score += CuisineWeight(r);                     // weight by cuisine preference
                if (DietOk(r)) score += 0.3;                   // bonus for matching diet tag
                score -= missing * 0.2;                        // penalty for missing ingredients
                score -= Math.Min(Math.Max(r.CookTimeMins / 120.0, 0), 1) * 0.3;  // penalty for long cooking time
                score -= Math.Min(missingDeficit / 10.0, 1.0) * 0.2;              // penalty for ingredient shortage

                accepted.Add(Tuple.Create(r, missing, score));
            }

            // Sort by descending score, then ascending cooking time
            return accepted
                .OrderByDescending(x => x.Item3)
                .ThenBy(x => x.Item1.CookTimeMins)
                .Select(x => x.Item1)
                .ToList();
        }

        private static string Normalize(string s)
        {
            return (s ?? "").Trim().ToLowerInvariant();
        }

        // Convert quantity strings to numeric values.
        private static double ParseQty(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            double v;
            if (double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v))
                return v;

            var filtered = new string(s.ToCharArray()
                .Where(ch => char.IsDigit(ch) || ch == '.' || ch == '-')
                .ToArray());

            return double.TryParse(filtered, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v)
                ? v
                : 0;
        }
    }
}
