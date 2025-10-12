namespace SmartMealPlanner
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        IEnumerable<Recipe> SearchByTitle(string title);
        IEnumerable<Recipe> SearchByIngredient(string ingredientTitle);
        IEnumerable<Recipe> SearchByWordsOfInstructions(string words);
        IEnumerable<Recipe> GetFavorites();
    }
}