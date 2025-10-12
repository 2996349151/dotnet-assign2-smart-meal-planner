// ingredients class
// id, title, quantity, unit

namespace SmartMealPlanner
{
    public class Ingredient
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public double Quantity { get; init; } // available quantity at home
        public string Unit { get; init; }

        public Ingredient(int id, string title, double quantity, string unit)
        {
            Id = id;
            Title = title;
            Quantity = quantity;
            Unit = unit;
        }
    }
}