using System.Threading.Tasks;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Interfaces
{
    public interface IPantryService
    {
        Task<Pantry> GetAsync();
        Task SaveAsync(Pantry pantry);
        Task ApplyCookingAsync(Recipe recipe); // throws if insufficient
    }
}
