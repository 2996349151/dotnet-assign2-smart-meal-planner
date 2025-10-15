using SmartMealPlanner.Core.Data.Repositories;
using SmartMealPlanner.Core.Models;
using SmartMealPlanner.Core.Services;

static string FindDirUpwards(string start, string targetName)
{
    var dir = new DirectoryInfo(start);
    while (dir != null)
    {
        var candidate = Path.Combine(dir.FullName, targetName);
        if (Directory.Exists(candidate)) return candidate;
        dir = dir.Parent;
    }
    throw new DirectoryNotFoundException($"Could not find '{targetName}' upwards from {start}");
}

try
{
    // 1) Locate repo root /assets at runtime robustly
    var baseDir = AppContext.BaseDirectory; // .../src/SmartMealPlanner.Cli/bin/Debug/net9.0/
    var assetsDir = FindDirUpwards(baseDir, "assets");
    var recipesPath = Path.Combine(assetsDir, "recipes.json");
    var pantryPath  = Path.Combine(assetsDir, "pantry.json");

    // 2) Wire repositories and services
    var recipeRepo = new JsonRecipeRepository(recipesPath);
    var pantryRepo = new JsonPantryRepository(pantryPath);
    var recipeSvc  = new RecipeService(recipeRepo);
    var pantrySvc  = new PantryService(pantryRepo);

    // 3) Load current pantry
    var pantry = await pantrySvc.GetAsync();
    Console.WriteLine($"Loaded pantry items: {pantry.Items.Count}");

    // 4) Build a simple preference and fetch recommendations (tolerance = 2)
    var pref = new UserPreference
    {
        DietTags = new List<string> { }, // e.g., "vegetarian"
        Disliked = new List<string> { "cilantro" }, // example disliked
        CuisineWeights = new Dictionary<string, double> { ["italian"] = 0.6, ["asian"] = 0.4 },
        MissingTolerance = 2
    };

    var recs = await recipeSvc.GetRecommendationsAsync(pantry, pref, pref.MissingTolerance);
    Console.WriteLine($"Recommendations (≤{pref.MissingTolerance} missing): {recs.Count}");
    foreach (var r in recs.Take(3))
    {
        Console.WriteLine($" - {r.Title} (time {r.CookTimeMins}m)");
    }

    // 5) If any recommendation exists, "cook" the first one and save pantry
    var first = recs.FirstOrDefault();
    if (first != null)
    {
        Console.WriteLine($"\nCooking: {first.Title} ...");
        await pantrySvc.ApplyCookingAsync(first);
        Console.WriteLine("Pantry updated after cooking.");
    }
    else
    {
        Console.WriteLine("No recipe within missing tolerance.");
    }

    Console.WriteLine("\nSmoke test finished. ✅");
}
catch (Exception ex)
{
    Console.Error.WriteLine("ERROR: " + ex.Message);
    Environment.ExitCode = 1;
}
