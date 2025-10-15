using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Interfaces
{
    public interface IRecipeRepository
    {
        Task<IReadOnlyList<Recipe>> GetAllAsync();
        Task<Recipe> GetByIdAsync(string id);
    }
}