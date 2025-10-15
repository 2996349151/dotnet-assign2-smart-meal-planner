using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;

namespace SmartMealPlanner.Core.Services
{
    public sealed class PlannerService : IPlannerService
    {
        private readonly Dictionary<DateTime, Dictionary<MealType, Recipe>> _plan = new Dictionary<DateTime, Dictionary<MealType, Recipe>>();

        // Generates a meal plan for the specified number of days starting from today.
        public Task<IDictionary<DateTime, Dictionary<MealType, Recipe>>> GenerateAsync(int days)
        {
            _plan.Clear();
            var start = DateTime.Today;
            for (int i = 0; i < Math.Max(1, days); i++)
            {
                var d = start.AddDays(i);
                _plan[d] = new Dictionary<MealType, Recipe>
                {
                    [MealType.Breakfast] = null,
                    [MealType.Lunch] = null,
                    [MealType.Dinner] = null,
                    [MealType.Snack] = null
                };
            }
            return Task.FromResult((IDictionary<DateTime, Dictionary<MealType, Recipe>>)_plan);
        }

        // Assigns a specific recipe to a meal on a given date in the meal plan.
        public Task AssignAsync(DateTime date, MealType meal, Recipe recipe)
        {
            if (!_plan.ContainsKey(date))
            {
                _plan[date] = new Dictionary<MealType, Recipe>
                {
                    [MealType.Breakfast] = null,
                    [MealType.Lunch] = null,
                    [MealType.Dinner] = null,
                    [MealType.Snack] = null
                };
            }
            _plan[date][meal] = recipe;
            return Task.CompletedTask;
        }

        public IDictionary<DateTime, Dictionary<MealType, Recipe>> GetCurrentPlan()
        {
            return _plan;
        }
    }
}
