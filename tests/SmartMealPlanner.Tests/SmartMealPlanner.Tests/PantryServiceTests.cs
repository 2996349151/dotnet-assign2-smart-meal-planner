using NUnit.Framework;
using SmartMealPlanner.Core.Data.Repositories;
using SmartMealPlanner.Core.Models;
using SmartMealPlanner.Core.Services;

namespace SmartMealPlanner.Tests;

[TestFixture]
public class PantryServiceTests
{
    private string _tmpFile = null!;
    private PantryService _svc = null!;

    [SetUp]
    public async Task Setup()
    {
        // Create a temp pantry json for round-trip and cooking tests
        _tmpFile = Path.GetTempFileName();
        var repo = new JsonPantryRepository(_tmpFile);
        _svc = new PantryService(repo);

        var p = new Pantry { Items = new Dictionary<string, double> { ["pasta"] = 500, ["tomato"] = 3, ["garlic"] = 5 } };
        await _svc.SaveAsync(p);
    }

    [TearDown]
    public void Cleanup()
    {
        try { if (File.Exists(_tmpFile)) File.Delete(_tmpFile); } catch { }
    }

    [Test]
    public async Task ApplyCooking_Deducts_Quantities()
    {
        var before = await _svc.GetAsync();
        Assert.That(before.Items["pasta"], Is.EqualTo(500));

        var recipe = new Recipe
        {
            Id = "test",
            Title = "Test Pasta",
            Ingredients = new Dictionary<string,string> { ["pasta"] = "200", ["tomato"] = "2", ["garlic"] = "2" },
            Steps = new(), Tags = new(), CookTimeMins = 10
        };

        await _svc.ApplyCookingAsync(recipe);

        var after = await _svc.GetAsync();
        Assert.That(after.Items["pasta"], Is.EqualTo(300));
        Assert.That(after.Items["tomato"], Is.EqualTo(1));
        Assert.That(after.Items["garlic"], Is.EqualTo(3));
    }

    [Test]
    public void ApplyCooking_Throws_When_Insufficient()
    {
        var recipe = new Recipe
        {
            Id = "test2",
            Title = "Too Much",
            Ingredients = new Dictionary<string,string> { ["pasta"] = "600" }
        };

        Assert.ThrowsAsync<InvalidOperationException>(async () => await _svc.ApplyCookingAsync(recipe));
    }
}
