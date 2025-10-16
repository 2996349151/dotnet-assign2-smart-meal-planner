namespace SmartMealPlanner.Core.Models;

public sealed class Recipe
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    // name -> quantity string (e.g., "200", "2", "1.5")
    public Dictionary<string, string> Ingredients { get; set; } = new();
    public List<string> Steps { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public int CookTimeMins { get; set; }
}
