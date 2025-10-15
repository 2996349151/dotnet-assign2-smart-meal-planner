using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Interfaces
{
    public interface IPlannerService
    {
        Task<IDictionary<DateTime, Dictionary<MealType, Recipe>>> GenerateAsync(int days);
        Task AssignAsync(DateTime date, MealType meal, Recipe recipe);
    }
}
