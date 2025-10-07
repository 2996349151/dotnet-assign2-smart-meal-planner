using System;
using System.Collections.Generic;

namespace SmartMealPlanner
{
    public interface IPlannerService
    {

        // Generates a (possibly empty) plan for the next numberOfDays starting from today.
        // Each entry has a Date, MealType, and possibly a Recipe (or null).
        List<MealPlanEntry> GeneratePlan(int numberOfDays);


        // Assigns a recipe to a specific date and meal (breakfast, lunch, dinner).
        void AssignRecipe(DateTime date, MealType mealType, Recipe recipe);

        // Given a list of meal plan entries, aggregate a shopping list of ingredients
        // (ingredient → total quantity needed).
        Dictionary<Ingredient, double> GetShoppingList(IEnumerable<MealPlanEntry> planEntries);

        // Export the plan (and shopping list) to a file (CSV / JSON / other formats).
        void ExportPlan(IEnumerable<MealPlanEntry> planEntries, string filePath);
    }
}