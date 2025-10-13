using System.Linq;
using NUnit.Framework;
using SmartMealPlanner.Core.Data.Repositories;
using SmartMealPlanner.Core.Models;
using SmartMealPlanner.Core.Services;

namespace SmartMealPlanner.Tests;

[TestFixture]
public class RecipeServiceTests
{
    private string _assetsDir = null!;
    private RecipeService _svc = null!;

    [SetUp]
    public void Setup()
    {
        // Locate repo-root/assets from test bin directory
        var baseDir = AppContext.BaseDirectory;
        _assetsDir = FindDirUpwards(baseDir, "assets");
        var recipesPath = Path.Combine(_assetsDir, "recipes.json");

        var repo = new JsonRecipeRepository(recipesPath);
        _svc = new RecipeService(repo);
    }

    [Test]
    public async Task Search_Finds_By_Title_And_Tags_And_Ingredients()
    {
        var byTitle = await _svc.SearchAsync("pasta");
        Assert.That(byTitle.Any(r => r.Title.Contains("Pasta", StringComparison.OrdinalIgnoreCase)));

        var byTag = await _svc.SearchAsync("vegetarian");
        Assert.That(byTag.Any(r => r.Tags.Contains("vegetarian")));

        var byIngredient = await _svc.SearchAsync("garlic");
        Assert.That(byIngredient.Any(r => r.Ingredients.Keys.Any(k => k.ToLower().Contains("garlic"))));
    }

    [Test]
    public async Task Recommendations_Respects_Missing_Tolerance_Le2()
    {
        var pantry = new Pantry
        {
            Items = new Dictionary<string, double> {
                ["pasta"] = 500, ["tomato"] = 1, ["garlic"] = 1, ["rice"] = 1000, ["egg"] = 12
            }
        };
        var pref = new UserPreference { MissingTolerance = 2 };

        var recs = await _svc.GetRecommendationsAsync(pantry, pref, pref.MissingTolerance);
        // Should include items with <= 2 missing; our seed set should produce at least one
        Assert.That(recs.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task Preference_Scoring_Considers_Disliked_And_Cuisine_Weights()
    {
        var pantry = new Pantry
        {
            Items = new Dictionary<string, double>
            {
                ["egg"] = 12, ["garlic"] = 10, ["rice"] = 1000, ["pasta"] = 500, ["tomato"] = 3
            }
        };

        var prefWeighted = new UserPreference
        {
            CuisineWeights = new Dictionary<string, double> { ["italian"] = 1.0 }, // boosts Tomato Pasta
            Disliked = new List<string>() // no dislike
        };
        var prefDislike = new UserPreference
        {
            CuisineWeights = new Dictionary<string, double>(),
            Disliked = new List<string> { "garlic" } // penalizes garlic-heavy options
        };

        var a = await _svc.GetRecommendationsAsync(pantry, prefWeighted, 2);
        var b = await _svc.GetRecommendationsAsync(pantry, prefDislike, 2);

        // Convert to List to use FindIndex
        var la = a.ToList();
        var lb = b.ToList();

        var idxA = la.FindIndex(r => r.Title.Contains("Tomato Pasta", StringComparison.OrdinalIgnoreCase));
        var idxB = lb.FindIndex(r => r.Title.Contains("Tomato Pasta", StringComparison.OrdinalIgnoreCase));

        Assert.That(idxA, Is.GreaterThanOrEqualTo(0));
        Assert.That(idxB, Is.GreaterThanOrEqualTo(0));
        Assert.That(idxA, Is.LessThanOrEqualTo(idxB));
    }

    private static string FindDirUpwards(string start, string targetName)
    {
        var dir = new DirectoryInfo(start);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, targetName);
            if (Directory.Exists(candidate)) return candidate;
            dir = dir.Parent!;
        }
        throw new DirectoryNotFoundException($"Could not find '{targetName}' upwards from {start}");
    }
}
