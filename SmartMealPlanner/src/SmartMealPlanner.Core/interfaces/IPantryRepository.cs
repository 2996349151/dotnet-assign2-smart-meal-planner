using System.Threading.Tasks;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Interfaces
{
    public interface IPantryRepository
    {
        Task<Pantry> GetAsync();
        Task SaveAsync(Pantry pantry);
    }
}
