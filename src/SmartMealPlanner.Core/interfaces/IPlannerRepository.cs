using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Interfaces;

public interface IPlannerRepository
{
    Task<IReadOnlyList<MealPlanEntry>> GetAllAsync();
    Task SaveAsync(MealPlanEntry entry);
    Task ClearAsync();
}
