using System;

namespace SmartMealPlanner.Core.Models
{
    public sealed class MealPlanEntry
    {
        public DateTime Date { get; set; }
        public MealType Meal { get; set; }
        public Recipe Assigned { get; set; }
    }
}
