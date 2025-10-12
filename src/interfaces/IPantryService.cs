using System.Collections.Generic;

namespace SmartMealPlanner
{
    public interface IPantryService
    {
        IEnumerable<Ingredient> GetAll();
        Ingredient GetById(int id);
        Ingredient GetByTitle(string title);
        void Add(Ingredient ingredient);
        void Update(Ingredient ingredient);
        void Delete(int id);

        // Checks if the specified ingredient (or required quantity) is available in the pantry.
        // Returns true if available, false otherwise.
        bool HasIngredient(Ingredient ingredient);
    }
}