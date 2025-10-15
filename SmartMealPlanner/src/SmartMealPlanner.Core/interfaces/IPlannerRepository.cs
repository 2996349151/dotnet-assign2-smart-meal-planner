using System.Collections.Generic;
using System.Threading.Tasks;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Interfaces
{
    public interface IPlannerRepository
    {
        Task<IReadOnlyList<MealPlanEntry>> GetAllAsync();
        Task SaveAsync(MealPlanEntry entry);
        Task ClearAsync();
    }
}
