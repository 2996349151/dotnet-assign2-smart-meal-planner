using System.Collections.Generic;

namespace SmartMealPlanner
{
    public interface IRecipeService
    {
        Recipe GetById(int id);
        IEnumerable<Recipe> GetAllRecipes();
        IEnumerable<Recipe> SearchByTitle(string title);
        IEnumerable<Recipe> SearchByIngredient(string ingredientTitle);
        IEnumerable<Recipe> SearchByWordsOfInstructions(string words);
        void SaveFavorite(Recipe recipe);
        IEnumerable<Recipe> GetFavorites();
    }
}