// recipe class
// id, title, instructions, dictionary of ingredients

namespace SmartMealPlanner
{
    using System.Collections.Generic;

    public class Recipe
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Instructions { get; init; }

        public List<Ingredient> IngredientsList { get; init; } // ingredient.Quantity is the required quantity for the recipe

        public bool IsFavorite { get; set; } = false;
        public Recipe(int id, string title, string instructions, List<Ingredient> ingredients, bool isFavorite)
        {
            Id = id;
            Title = title;
            Instructions = instructions;
            IngredientsList = ingredients;
            IsFavorite = isFavorite;
        }
    }
}