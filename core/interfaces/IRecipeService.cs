using System.Collections.Generic;

namespace SmartMealPlanner
{
    public interface IRecipeService
    {
        Recipe GetById(int id);
        IEnumerable<Recipe> GetAllRecipes();
        IEnumerable<Recipe> SearchByName(string name);
        IEnumerable<Recipe> SearchByIngredient(string ingredient);
        void SaveFavorite(Recipe recipe);
        IEnumerable<Recipe> GetFavorites();
    }
}