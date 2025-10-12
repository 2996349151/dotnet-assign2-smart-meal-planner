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
        
        public Dictionary<Ingredient, double> Ingredients { get; init; }

        public Recipe(int id, string title, string instructions, Dictionary<Ingredient, double> ingredients)
        {
            Id = id;
            Title = title;
            Instructions = instructions;
            Ingredients = ingredients;
        }
    }
}