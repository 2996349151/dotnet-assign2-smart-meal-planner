using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Interfaces;

public interface IPlannerService
{
    Task<IDictionary<DateOnly, Dictionary<MealType, Recipe?>>> GenerateAsync(int days);
    Task AssignAsync(DateOnly date, MealType meal, Recipe recipe);
}
