using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Services;

public sealed class PlannerService : IPlannerService
{
    private readonly Dictionary<DateOnly, Dictionary<MealType, Recipe?>> _plan = new();

    public Task<IDictionary<DateOnly, Dictionary<MealType, Recipe?>>> GenerateAsync(int days)
    {
        var start = DateOnly.FromDateTime(DateTime.Today);
        for (int i = 0; i < Math.Max(1, days); i++)
        {
            var d = start.AddDays(i);
            if (!_plan.ContainsKey(d))
            {
                _plan[d] = new()
                {
                    [MealType.Breakfast] = null,
                    [MealType.Lunch] = null,
                    [MealType.Dinner] = null,
                    [MealType.Snack] = null
                };
            }
        }
        return Task.FromResult((IDictionary<DateOnly, Dictionary<MealType, Recipe?>>) _plan);
    }

    public Task AssignAsync(DateOnly date, MealType meal, Recipe recipe)
    {
        if (!_plan.ContainsKey(date)) _plan[date] = new()
        {
            [MealType.Breakfast] = null,
            [MealType.Lunch] = null,
            [MealType.Dinner] = null,
            [MealType.Snack] = null
        };
        _plan[date][meal] = recipe;
        return Task.CompletedTask;
    }
}
