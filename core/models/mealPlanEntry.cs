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
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public MealType MealType { get; init; }
        public Recipe? AssignedRecipe { get; set; }

        public MealPlanEntry(int id, DateTime date, MealType mealType, Recipe? assignedRecipe = null)
        {
            Id = id;
            Date = date;
            MealType = mealType;
            AssignedRecipe = assignedRecipe;
        }
    }
}