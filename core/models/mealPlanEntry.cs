// meal plan entry class
// date, mealType (breakfast/lunch/dinner), Recipe assigned (or null)

namespace SmartMealPlanner
{
    using System;

    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner
    }

    public class MealPlanEntry
    {
        public DateTime Date { get; init; }
        public MealType MealType { get; init; }
        public Recipe? AssignedRecipe { get; set; }

        public MealPlanEntry(DateTime date, MealType mealType, Recipe? assignedRecipe = null)
        {
            Date = date;
            MealType = mealType;
            AssignedRecipe = assignedRecipe;
        }
    }
}