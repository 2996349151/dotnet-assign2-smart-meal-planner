using NUnit.Framework;
using SmartMealPlanner.Core.Data.Repositories;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Tests;

[TestFixture]
public class JsonRoundTripTests
{
    [Test]
    public async Task Pantry_Save_Then_Load_Equals()
    {
        var tmp = Path.GetTempFileName();
        try
        {
            var repo = new JsonPantryRepository(tmp);
            var p1 = new Pantry { Items = new Dictionary<string, double> { ["egg"] = 12, ["rice"] = 999.5 } };
            await repo.SaveAsync(p1);

            var p2 = await repo.GetAsync();
            Assert.That(p2.Items["egg"], Is.EqualTo(12));
            Assert.That(p2.Items["rice"], Is.EqualTo(999.5).Within(1e-9));
        }
        finally
        {
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch { }
        }
    }
}
