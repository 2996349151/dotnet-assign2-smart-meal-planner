namespace SmartMealPlanner.Core.Models
{
    public sealed class Recipe
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public Dictionary<string, string> Ingredients { get; set; } = new Dictionary<string, string>();
        public List<string> Steps { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();
        public int CookTimeMins { get; set; }
    }
}
