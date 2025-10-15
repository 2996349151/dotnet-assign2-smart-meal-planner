using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Interfaces
{
    public interface IRecipeService
    {
        Task<IReadOnlyList<Recipe>> SearchAsync(string keyword);
        Task<Recipe> GetByIdAsync(string id);
        Task<IReadOnlyList<Recipe>> GetRecommendationsAsync(Pantry pantry, UserPreference pref, int tolerance);
    }
}