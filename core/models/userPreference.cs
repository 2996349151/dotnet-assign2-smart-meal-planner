// user preference class
// stores user settings
// dietary requirements, allergies, preferred units, default planner days, favorite recipes

namespace core.models
{
    public class UserPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string DietaryRequirements { get; set; } // e.g., vegetarian, vegan, gluten-free
        public List<Ingredient> Allergies { get; set; } // e.g., peanuts, shellfish
        public string PreferredUnits { get; set; } // e.g., metric, imperial
        public int DefaultPlannerDays { get; set; } // e.g., 7 for a week
        public List<Recipe> FavoriteRecipes { get; set; } // list of favorite recipes

        public UserPreference()
        {
            FavoriteRecipes = new List<Recipe>();
        }
    }
}