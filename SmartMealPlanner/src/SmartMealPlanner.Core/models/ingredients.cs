namespace SmartMealPlanner.Core.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Quantity { get; set; } // available quantity at home
        public string Unit { get; set; }

        public Ingredient(int id, string title, double quantity, string unit)
        {
            Id = id;
            Title = title;
            Quantity = quantity;
            Unit = unit;
        }
    }
}
